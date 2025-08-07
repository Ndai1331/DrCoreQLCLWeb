using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OfficeOpenXml;
using OfficeOpenXml.Style;


namespace CoreAdminWeb.Pages.QLCLTinhHinhSXKDNLTS
{
    public partial class QLCLTinhHinhSXKDNLTS(IBaseService<QLCLTinhHinhSXKDNLTSModel> MainService,
                                              IQLCLTinhHinhSXKDNLTSNguyenLieuService NguyenLieuService,
                                              IQLCLTinhHinhSXKDNLTSSanPhamService SanPhamService,
                                              IBaseService<QLCLSanPhamSanXuatModel> QLCLSanPhamSanXuatService,
                                              IBaseService<QLCLCoSoCheBienNLTSModel> QLCLCoSoCheBienNLTSService,
                                              IBaseService<QLCLNguyenLieuCheBienModel> NguyenLieuCheBienService) : BlazorCoreBase
    {
        private List<QLCLTinhHinhSXKDNLTSModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openNguyenLieuDetailDeleteModal = false;
        private bool openSanPhamDetailDeleteModal = false;
        private bool openAddOrUpdateModal = false;

        private QLCLTinhHinhSXKDNLTSModel SelectedItem { get; set; } = new QLCLTinhHinhSXKDNLTSModel();
        private List<QLCLTinhHinhSXKDNLTSNguyenLieuModel> SelectedNguyenLieuItemsDetail { get; set; } = new List<QLCLTinhHinhSXKDNLTSNguyenLieuModel>();
        private QLCLTinhHinhSXKDNLTSNguyenLieuModel? SelectedNguyenLieuItemDetail { get; set; } = default;
        private List<QLCLTinhHinhSXKDNLTSSanPhamModel> SelectedSanPhamItemsDetail { get; set; } = new List<QLCLTinhHinhSXKDNLTSSanPhamModel>();
        private QLCLTinhHinhSXKDNLTSSanPhamModel? SelectedSanPhamItemDetail { get; set; } = default;

        private string _searchString = "";
        private string _titleAddOrUpdate = "Thêm mới";
        private string activeDefTab { get; set; } = "tab1";
        private DateTime? _fromDate { get; set; } = null;
        private DateTime? _toDate { get; set; } = null;

        // Select2 define
        private List<QLCLNguyenLieuCheBienModel> QLCLNguyenLieuCheBienItems = new List<QLCLNguyenLieuCheBienModel>();
        private Dictionary<int, List<QLCLNguyenLieuCheBienModel>> SelectedQLCLNguyenLieuCheBienItems = new Dictionary<int, List<QLCLNguyenLieuCheBienModel>>();

        private List<QLCLSanPhamSanXuatModel> QLCLSanPhamSanXuatItems = new List<QLCLSanPhamSanXuatModel>();
        private Dictionary<int, List<QLCLSanPhamSanXuatModel>> SelectedQLCLSanPhamSanXuatItems = new Dictionary<int, List<QLCLSanPhamSanXuatModel>>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadData();
                _ = Task.Run(async () =>
                {
                    await Task.Delay(500);
                    await JsRuntime.InvokeVoidAsync("initializeDatePicker");
                });
                await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/assets/js/pages/flatpickr.js");
                StateHasChanged();
            }
        }

        private async Task LoadData()
        {
            IsLoading = true;
            BuildPaginationQuery(Page, PageSize, "id", false);
            int index = 1;

            BuilderQuery += "&filter[_and][0][deleted][_eq]=false";
            if (!string.IsNullOrEmpty(_searchString))
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][_or][0][su_co_an_toan][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][bien_phap_xu_ly_chat_thai][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][2][qlcl_co_so_che_bien_nlts][name][_contains]={_searchString}";
            }
            if (_fromDate != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][ngay_ghi_nhan][_gte]={_fromDate.Value:yyyy-MM-dd}";
            }

            if (_toDate != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][ngay_ghi_nhan][_lte]={_toDate.Value:yyyy-MM-dd}";
            }

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<QLCLTinhHinhSXKDNLTSModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<QLCLTinhHinhSXKDNLTSModel>();
            }
            IsLoading = false;
        }

        private async Task LoadNguyenLieuDetailData()
        {
            var buildQuery = $"sort=-id";
            buildQuery += $"&filter[_and][][tinh_hinh_san_xuat_kinh_doanh_nlts][_eq]={SelectedItem.id}";
            buildQuery += $"&filter[_and][][deleted][_eq]=false";
            var result = await NguyenLieuService.GetAllAsync(buildQuery);
            SelectedNguyenLieuItemsDetail = result.Data ?? new List<QLCLTinhHinhSXKDNLTSNguyenLieuModel>();
        }
        private async Task LoadSanPhamDetailData()
        {
            var buildQuery = $"sort=-id";
            buildQuery += $"&filter[_and][][tinh_hinh_san_xuat_kinh_doanh_nlts][_eq]={SelectedItem.id}";
            buildQuery += $"&filter[_and][][deleted][_eq]=false";
            var result = await SanPhamService.GetAllAsync(buildQuery);
            SelectedSanPhamItemsDetail = result.Data ?? new List<QLCLTinhHinhSXKDNLTSSanPhamModel>();
        }

        private async Task<List<QLCLNguyenLieuCheBienModel>> FilterFunctionQLCLNguyenLieuCheBienData(IEnumerable<QLCLNguyenLieuCheBienModel> allItems, string filter,
            CancellationToken token)
        {
            QLCLNguyenLieuCheBienItems = await LoadDataInTable(allItems, filter, token, NguyenLieuCheBienService);
            StateHasChanged();
            return QLCLNguyenLieuCheBienItems;
        }

        private async Task<IEnumerable<QLCLCoSoCheBienNLTSModel>> LoadCoSoCheBienNLTSData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, QLCLCoSoCheBienNLTSService);
        }

        private async Task<List<QLCLSanPhamSanXuatModel>> FilterFunctionQLCLSanPhamSanXuatData(IEnumerable<QLCLSanPhamSanXuatModel> allItems, string filter,
            CancellationToken token)
        {
            QLCLSanPhamSanXuatItems = await LoadDataInTable(allItems, filter, token, QLCLSanPhamSanXuatService);
            StateHasChanged();
            return QLCLSanPhamSanXuatItems;
        }

        private void OpenDeleteModal(QLCLTinhHinhSXKDNLTSModel item)
        {
            SelectedItem = item;
            openDeleteModal = true;
        }

        private async Task OnDelete()
        {
            if (SelectedItem.id == 0)
            {
                AlertService.ShowAlert("Không có dữ liệu để xóa", "warning");
                return;
            }

            var result = await MainService.DeleteAsync(SelectedItem);
            if (result.IsSuccess && result.Data)
            {
                await LoadData();
                AlertService.ShowAlert("Xoá thành công!", "success");
                openDeleteModal = false;
            }
            else
            {
                AlertService.ShowAlert(result.Message ?? "Lỗi khi xóa dữ liệu", "danger");
            }
        }

        private void CloseDeleteModal()
        {
            SelectedItem = new QLCLTinhHinhSXKDNLTSModel();
            openDeleteModal = false;
        }

        private void OpenNguyenLieuDetailDeleteModal(QLCLTinhHinhSXKDNLTSNguyenLieuModel item)
        {
            SelectedNguyenLieuItemDetail = item;
            openNguyenLieuDetailDeleteModal = true;
        }
        private void OnNguyenLieuDetailDelete()
        {
            if (SelectedNguyenLieuItemDetail == null)
            {
                AlertService.ShowAlert("Không có dữ liệu để xóa", "warning");
                return;
            }

            foreach (var item in SelectedNguyenLieuItemsDetail)
            {
                if (item.id > 0 && item.id == SelectedNguyenLieuItemDetail.id || item.sort > 0 && item.sort == SelectedNguyenLieuItemDetail.sort)
                {
                    item.deleted = true;
                }
            }

            SelectedNguyenLieuItemDetail = default;

            openNguyenLieuDetailDeleteModal = false;

            if (!SelectedNguyenLieuItemsDetail.Any(c => c.deleted == null || c.deleted == false))
            {
                SelectedNguyenLieuItemsDetail.Add(new QLCLTinhHinhSXKDNLTSNguyenLieuModel()
                {
                    tinh_hinh_san_xuat_kinh_doanh_nlts = SelectedItem,
                    sort = (SelectedNguyenLieuItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    nguyen_lieu = null,
                    khoi_luong_tan = 0,
                    deleted = false,
                });
            }

            StateHasChanged();
        }
        private void CloseNguyenLieuDetailDeleteModal()
        {
            SelectedNguyenLieuItemDetail = default;

            openNguyenLieuDetailDeleteModal = false;
        }

        private void OnAddNguyenLieu()
        {
            if (SelectedNguyenLieuItemsDetail == null)
            {
                SelectedNguyenLieuItemsDetail = new List<QLCLTinhHinhSXKDNLTSNguyenLieuModel>();
            }

            SelectedNguyenLieuItemsDetail.Add(new QLCLTinhHinhSXKDNLTSNguyenLieuModel
            {
                tinh_hinh_san_xuat_kinh_doanh_nlts = SelectedItem,
                sort = (SelectedNguyenLieuItemsDetail.Max(c => c.sort) ?? 0) + 1,
                nguyen_lieu = null,
                khoi_luong_tan = 0,
                deleted = false,
            });
        }



        private void OpenSanPhamDetailDeleteModal(QLCLTinhHinhSXKDNLTSSanPhamModel item)
        {
            SelectedSanPhamItemDetail = item;
            openSanPhamDetailDeleteModal = true;
        }

        private void OnSanPhamDetailDelete()
        {
            if (SelectedSanPhamItemDetail == null)
            {
                AlertService.ShowAlert("Không có dữ liệu để xóa", "warning");
                return;
            }

            foreach (var item in SelectedSanPhamItemsDetail)
            {
                if (item.id > 0 && item.id == SelectedSanPhamItemDetail.id || item.sort > 0 && item.sort == SelectedSanPhamItemDetail.sort)
                {
                    item.deleted = true;
                }
            }

            SelectedSanPhamItemDetail = default;

            openSanPhamDetailDeleteModal = false;

            if (!SelectedSanPhamItemsDetail.Any(c => c.deleted == null || c.deleted == false))
            {
                SelectedSanPhamItemsDetail.Add(new QLCLTinhHinhSXKDNLTSSanPhamModel()
                {
                    tinh_hinh_san_xuat_kinh_doanh_nlts = SelectedItem,
                    sort = (SelectedSanPhamItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    san_pham = null,
                    san_luong_tan = 0,
                    deleted = false,
                });
            }

            StateHasChanged();
        }

        private void CloseSanPhamDetailDeleteModal()
        {
            SelectedSanPhamItemDetail = default;
            openSanPhamDetailDeleteModal = false;
        }

        private void OnAddSanPham()
        {
            if (SelectedSanPhamItemsDetail == null)
            {
                SelectedSanPhamItemsDetail = new List<QLCLTinhHinhSXKDNLTSSanPhamModel>();
            }

            SelectedSanPhamItemsDetail.Add(new QLCLTinhHinhSXKDNLTSSanPhamModel
            {
                tinh_hinh_san_xuat_kinh_doanh_nlts = SelectedItem,
                sort = (SelectedSanPhamItemsDetail.Max(c => c.sort) ?? 0) + 1,
                san_pham = null,
                san_luong_tan = 0,
                deleted = false,
            });
        }

        private async Task OpenAddOrUpdateModal(QLCLTinhHinhSXKDNLTSModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item.DeepClone() : new QLCLTinhHinhSXKDNLTSModel();
            SelectedNguyenLieuItemsDetail = new List<QLCLTinhHinhSXKDNLTSNguyenLieuModel>();
            SelectedSanPhamItemsDetail = new List<QLCLTinhHinhSXKDNLTSSanPhamModel>();

            if (SelectedItem.id is not null && SelectedItem.id > 0)
            {
                await LoadNguyenLieuDetailData();
                await LoadSanPhamDetailData();
            }

            if (!SelectedNguyenLieuItemsDetail.Any())
            {
                SelectedNguyenLieuItemsDetail.Add(new QLCLTinhHinhSXKDNLTSNguyenLieuModel()
                {
                    tinh_hinh_san_xuat_kinh_doanh_nlts = SelectedItem,
                    sort = (SelectedNguyenLieuItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    nguyen_lieu = null,
                    khoi_luong_tan = 0,
                    deleted = false,
                });
            }

            if (!SelectedSanPhamItemsDetail.Any())
            {
                SelectedSanPhamItemsDetail.Add(new QLCLTinhHinhSXKDNLTSSanPhamModel()
                {
                    tinh_hinh_san_xuat_kinh_doanh_nlts = SelectedItem,
                    sort = (SelectedSanPhamItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    san_pham = null,
                    san_luong_tan = 0,
                    deleted = false,
                });
            }

            openAddOrUpdateModal = true;

            // Wait for modal to render
            _ = Task.Run(async () =>
            {
                await Task.Delay(500);
                await JsRuntime.InvokeVoidAsync("initializeDatePicker");
            });
        }

        private async Task OnValidSubmit()
        {
            var isValid = true;
            if (SelectedNguyenLieuItemsDetail.Count == 0 || SelectedNguyenLieuItemsDetail.Any(c => c.nguyen_lieu == null))
            {
                AlertService.ShowAlert("Vui lòng nhập nguyên liệu", "warning");
                isValid = false;
            }

            if (SelectedSanPhamItemsDetail.Count == 0 || SelectedSanPhamItemsDetail.Any(c => c.san_pham == null))
            {
                AlertService.ShowAlert("Vui lòng nhập sản phẩm", "warning");
                isValid = false;
            }

            if (isValid)
            {
                if (SelectedItem.id is null || SelectedItem.id <= 0)
                {
                    var result = await MainService.CreateAsync(SelectedItem);
                    if (result.IsSuccess)
                    {
                        var nguyenLieuChiTietList = SelectedNguyenLieuItemsDetail
                            .Where(c => c.deleted == false || c.deleted == null)
                            .Select(c =>
                            {
                                c.tinh_hinh_san_xuat_kinh_doanh_nlts = result.Data;
                                return c;
                            })
                            .ToList();

                        var sanPhamChiTietList = SelectedSanPhamItemsDetail
                            .Where(c => c.deleted == false || c.deleted == null)
                            .Select(c =>
                            {
                                c.tinh_hinh_san_xuat_kinh_doanh_nlts = result.Data;
                                return c;
                            })
                            .ToList();

                        var nguyenLieuDetailResult = await NguyenLieuService.CreateAsync(nguyenLieuChiTietList);
                        if (!nguyenLieuDetailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(nguyenLieuDetailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                            return;
                        }
                        var sanPhamDetailResult = await SanPhamService.CreateAsync(sanPhamChiTietList);
                        if (!sanPhamDetailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(sanPhamDetailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                            return;
                        }
                        await LoadData();
                        openAddOrUpdateModal = false;
                        AlertService.ShowAlert("Thêm mới thành công!", "success");
                    }
                    else
                    {
                        AlertService.ShowAlert(result.Message ?? "Lỗi khi thêm mới dữ liệu", "danger");
                    }
                }
                else
                {
                    var result = await MainService.UpdateAsync(SelectedItem);
                    if (result.IsSuccess)
                    {
                        var addNewNguyenLieuChiTietList = SelectedNguyenLieuItemsDetail
                            .Where(c => (c.deleted == false || c.deleted == null) && (c.id ?? 0) == 0)
                            .Select(c =>
                            {
                                c.tinh_hinh_san_xuat_kinh_doanh_nlts = SelectedItem;
                                return c;
                            }).ToList();
                        var removeNguyenLieuChiTietList = SelectedNguyenLieuItemsDetail
                            .Where(c => c.deleted == true && c.id > 0)
                            .Select(c =>
                            {
                                c.tinh_hinh_san_xuat_kinh_doanh_nlts = SelectedItem;
                                c.deleted = true;
                                return c;
                            }).ToList();
                        var updateNguyenLieuChiTietList = SelectedNguyenLieuItemsDetail
                            .Where(c => (c.deleted == false || c.deleted == null) && c.id > 0)
                            .Select(c =>
                            {
                                c.tinh_hinh_san_xuat_kinh_doanh_nlts = SelectedItem;
                                return c;
                            }).ToList();

                        if (addNewNguyenLieuChiTietList.Any())
                        {
                            var detailResult = await NguyenLieuService.CreateAsync(addNewNguyenLieuChiTietList);
                            if (!detailResult.IsSuccess)
                            {
                                AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                                return;
                            }
                        }

                        if (removeNguyenLieuChiTietList.Any())
                        {
                            var detailResult = await NguyenLieuService.DeleteAsync(removeNguyenLieuChiTietList);
                            if (!detailResult.IsSuccess)
                            {
                                AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi xóa chi tiết dữ liệu", "danger");
                                return;
                            }
                        }

                        if (updateNguyenLieuChiTietList.Any())
                        {
                            var detailResult = await NguyenLieuService.UpdateAsync(updateNguyenLieuChiTietList);
                            if (!detailResult.IsSuccess)
                            {
                                AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi cập nhật chi tiết dữ liệu", "danger");
                                return;
                            }
                        }


                        var addNewSanPhamChiTietList = SelectedSanPhamItemsDetail
                            .Where(c => (c.deleted == false || c.deleted == null) && (c.id ?? 0) == 0)
                            .Select(c =>
                            {
                                c.tinh_hinh_san_xuat_kinh_doanh_nlts = SelectedItem;
                                return c;
                            }).ToList();
                        var removeSanPhamChiTietList = SelectedSanPhamItemsDetail
                            .Where(c => c.deleted == true && c.id > 0)
                            .Select(c =>
                            {
                                c.tinh_hinh_san_xuat_kinh_doanh_nlts = SelectedItem;
                                c.deleted = true;
                                return c;
                            }).ToList();
                        var updateSanPhamChiTietList = SelectedSanPhamItemsDetail
                            .Where(c => (c.deleted == false || c.deleted == null) && c.id > 0)
                            .Select(c =>
                            {
                                c.tinh_hinh_san_xuat_kinh_doanh_nlts = SelectedItem;
                                return c;
                            }).ToList();

                        if (addNewSanPhamChiTietList.Any())
                        {
                            var detailResult = await SanPhamService.CreateAsync(addNewSanPhamChiTietList);
                            if (!detailResult.IsSuccess)
                            {
                                AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                                return;
                            }
                        }

                        if (removeSanPhamChiTietList.Any())
                        {
                            var detailResult = await SanPhamService.DeleteAsync(removeSanPhamChiTietList);
                            if (!detailResult.IsSuccess)
                            {
                                AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi xóa chi tiết dữ liệu", "danger");
                                return;
                            }
                        }

                        if (updateSanPhamChiTietList.Any())
                        {
                            var detailResult = await SanPhamService.UpdateAsync(updateSanPhamChiTietList);
                            if (!detailResult.IsSuccess)
                            {
                                AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi cập nhật chi tiết dữ liệu", "danger");
                                return;
                            }
                        }

                        await LoadData();
                        openAddOrUpdateModal = false;
                        AlertService.ShowAlert("Cập nhật thành công!", "success");
                    }
                    else
                    {
                        AlertService.ShowAlert(result.Message ?? "Lỗi khi cập nhật dữ liệu", "danger");
                    }
                }
            }
        }

        private void CloseAddOrUpdateModal()
        {
            SelectedItem = new QLCLTinhHinhSXKDNLTSModel();
            openAddOrUpdateModal = false;
        }
        private async Task OnDateChanged(ChangeEventArgs e, string fieldName, bool isFilter = false)
        {
            try
            {
                var dateStr = e.Value?.ToString();
                if (string.IsNullOrEmpty(dateStr))
                {
                    ReflectionHelper.SetDateFieldValue(this, SelectedItem, fieldName, null);
                }
                else
                {
                    var parts = dateStr.Split('/');
                    if (parts.Length == 3 &&
                        int.TryParse(parts[0], out int day) &&
                        int.TryParse(parts[1], out int month) &&
                        int.TryParse(parts[2], out int year))
                    {
                        var date = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Local);
                        ReflectionHelper.SetDateFieldValue(this, SelectedItem, fieldName, date);
                    }
                }

                if (isFilter)
                {
                    await LoadData();
                }
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi khi xử lý ngày: {ex.Message}", "danger");
            }
        }
        private void OnTabChanged(string tab)
        {
            activeDefTab = tab;
        }


        private async Task OnExportExcel()
        {
            // Get all data for export
            BuildPaginationQuery(Page, int.MaxValue);
            int index = 1;

            BuilderQuery += "&filter[_and][0][deleted][_eq]=false";
            if (!string.IsNullOrEmpty(_searchString))
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][_or][0][su_co_an_toan][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][bien_phap_xu_ly_chat_thai][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][2][qlcl_co_so_che_bien_nlts][name][_contains]={_searchString}";
            }
            if (_fromDate != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][ngay_ghi_nhan][_gte]={_fromDate.Value:yyyy-MM-dd}";
            }

            if (_toDate != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][ngay_ghi_nhan][_lte]={_toDate.Value:yyyy-MM-dd}";
            }

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (!result.IsSuccess || result.Data == null)
            {
                AlertService.ShowAlert("Không có dữ liệu để xuất Excel", "warning");
                return;
            }
            var data = result.Data;

            ExcelPackage.License.SetNonCommercialPersonal("Ndai1331");
            // Create Excel package
            using var package = new ExcelPackage(new FileInfo("MyWorkbook.xlsx"));
            var ws = package.Workbook.Worksheets.Add("Data");

            // Header
            ws.Cells[1, 1].Value = "STT";
            ws.Cells[1, 2].Value = "Ngày ghi nhận";
            ws.Cells[1, 3].Value = "Tên cơ sở";
            ws.Cells[1, 4].Value = "Thời gian bắt đầu";
            ws.Cells[1, 5].Value = "Thời gian kết thúc";
            ws.Cells[1, 6].Value = "Sản phẩm";
            ws.Cells[1, 7].Value = "Sự cố an toàn";
            ws.Cells[1, 8].Value = "Biện pháp xử lý chất thải";

            // Style header
            using (var range = ws.Cells[1, 1, 1, 8])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Fill data
            int row = 2;
            int stt = 1;
            foreach (var item in data)
            {
                ws.Cells[row, 1].Value = stt;
                ws.Cells[row, 2].Value = item.ngay_ghi_nhan?.ToString("dd/MM/yyyy");
                ws.Cells[row, 3].Value = item.qlcl_co_so_che_bien_nlts?.name;
                ws.Cells[row, 4].Value = item.thoi_gian_bat_dau?.ToString("dd/MM/yyyy");
                ws.Cells[row, 5].Value = item.thoi_gian_ket_thuc?.ToString("dd/MM/yyyy");
                //ws.Cells[row, 6].Value = item.qlcl_san_pham_san_xuat_nlts?.name;
                ws.Cells[row, 7].Value = item.su_co_an_toan;
                ws.Cells[row, 8].Value = item.bien_phap_xu_ly_chat_thai;
                row++;
                stt++;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            // Export to browser
            var fileName = $"DanhSachTinhHinhSXKDNLTS_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var fileBytes = await package.GetAsByteArrayAsync();
            // Nếu chưa có hàm saveAsFile trong wwwroot/js, hãy thêm hàm này để hỗ trợ download file từ base64
            await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(fileBytes));
        }

    }
}
