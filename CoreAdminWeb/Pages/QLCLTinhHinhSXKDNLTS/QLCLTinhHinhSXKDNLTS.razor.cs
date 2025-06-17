using CoreAdminWeb.Enums;
using CoreAdminWeb.Model;
using CoreAdminWeb.Model.DienTichGieoTrongCayHangNam;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Services.DienTichGieoTrongCayHangNam;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CoreAdminWeb.Pages.QLCLTinhHinhSXKDNLTS
{
    public partial class QLCLTinhHinhSXKDNLTS(IBaseService<QLCLTinhHinhSXKDNLTSModel> MainService,
                                              IQLCLTinhHinhSXKDNLTSNguyenLieuService NguyenLieuService,
                                              IQLCLTinhHinhSXKDNLTSSanPhamService SanPhamService,
                                              IBaseService<QLCLSanPhamSanXuatModel> SanPhamSanXuatService,
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
        private TinhModel? _selectedTinhFilter { get; set; }
        private string _titleAddOrUpdate = "Thêm mới";
        private string activeDefTab { get; set; } = "tab1";
        private DateTime? _fromDate { get; set; }
        private DateTime? _toDate { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadData();
                await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/assets/js/pages/flatpickr.js");
                StateHasChanged();
            }
        }

        private async Task LoadData()
        {
            BuildPaginationQuery(Page, PageSize);
            int index =1;

            BuilderQuery += "filter[_and][0][deleted][_eq]=false&sort=sort";
            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&filter[_and][{index}][_or][0][su_co_an_toan][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][bien_phap_xu_ly_chat_thai][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][2][qlcl_co_so_che_bien_nlts][name][_contains]={_searchString}";
                index++;
            }
            if(_fromDate != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ngay_ghi_nhan][_gte]={_fromDate.Value.ToString("yyyy-MM-dd")}";
                index++;
            }

            if(_toDate != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ngay_ghi_nhan][_lte]={_toDate.Value.ToString("yyyy-MM-dd")}";
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

        private async Task<IEnumerable<QLCLNguyenLieuCheBienModel>> LoadNguyenLieuData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, NguyenLieuCheBienService);
        }
        private async Task<IEnumerable<QLCLCoSoCheBienNLTSModel>> LoadCoSoCheBienNLTSData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, QLCLCoSoCheBienNLTSService);
        }
        private async Task<IEnumerable<QLCLSanPhamSanXuatModel>> LoadSanPhamData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, SanPhamSanXuatService);
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
                SelectedNguyenLieuItemsDetail.Add(new QLCLTinhHinhSXKDNLTSNguyenLieuModel()
                {
                    tinh_hinh_san_xuat_kinh_doanh_nlts = SelectedItem,
                    sort = (SelectedNguyenLieuItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    nguyen_lieu = null,
                    khoi_luong_tan_suat = 0,
                    deleted = false,
                });
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
                SelectedNguyenLieuItemsDetail = new List<QLCLTinhHinhSXKDNLTSNguyenLieuModel>();

            SelectedNguyenLieuItemsDetail.Add(new QLCLTinhHinhSXKDNLTSNguyenLieuModel
            {
                tinh_hinh_san_xuat_kinh_doanh_nlts = SelectedItem,
                sort = (SelectedNguyenLieuItemsDetail.Max(c => c.sort) ?? 0) + 1,
                nguyen_lieu = null,
                khoi_luong_tan_suat = 0,
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
                SelectedSanPhamItemsDetail.Add(new QLCLTinhHinhSXKDNLTSSanPhamModel()
                {
                    tinh_hinh_san_xuat_kinh_doanh_nlts = SelectedItem,
                    sort = (SelectedSanPhamItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    san_pham = null,
                    san_luong_tan = 0,
                    deleted = false,
                });
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
                SelectedSanPhamItemsDetail = new List<QLCLTinhHinhSXKDNLTSSanPhamModel>();

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
            SelectedItem = item != null ? item : new QLCLTinhHinhSXKDNLTSModel();
            SelectedNguyenLieuItemsDetail = new List<QLCLTinhHinhSXKDNLTSNguyenLieuModel>();

            if (SelectedItem.id > 0)
            {
                await LoadNguyenLieuDetailData();
            }

            if (!SelectedNguyenLieuItemsDetail.Any())
                SelectedNguyenLieuItemsDetail.Add(new QLCLTinhHinhSXKDNLTSNguyenLieuModel()
                {
                    tinh_hinh_san_xuat_kinh_doanh_nlts = SelectedItem,
                    sort = (SelectedNguyenLieuItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    nguyen_lieu = null,
                    khoi_luong_tan_suat = 0,
                    deleted = false,
                });
            
            
            if (!SelectedSanPhamItemsDetail.Any())
                SelectedSanPhamItemsDetail.Add(new QLCLTinhHinhSXKDNLTSSanPhamModel()
                {
                    tinh_hinh_san_xuat_kinh_doanh_nlts = SelectedItem,
                    sort = (SelectedSanPhamItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    san_pham = null,
                    san_luong_tan = 0,
                    deleted = false,
                });

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
            if (SelectedItem.id == 0)
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
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id == 0)
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
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id == 0)
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

        private void CloseAddOrUpdateModal()
        {
            SelectedItem = new QLCLTinhHinhSXKDNLTSModel();
            openAddOrUpdateModal = false;
        }
        private async Task OnDateChanged(ChangeEventArgs e, string fieldName)
        {
            try
            {
                var dateStr = e.Value?.ToString();
                if (string.IsNullOrEmpty(dateStr))
                {
                    switch (fieldName)
                    {
                        case nameof(SelectedItem.ngay_ghi_nhan):
                            SelectedItem.ngay_ghi_nhan = null;
                            break;


                        case nameof(SelectedItem.thoi_gian_bat_dau):
                            SelectedItem.thoi_gian_bat_dau = null;
                            break;

                        case nameof(SelectedItem.thoi_gian_ket_thuc):
                            SelectedItem.thoi_gian_ket_thuc = null;
                            break;


                        case "fromDate":
                            _fromDate = null;
                            await LoadData();
                            break;

                        case "toDate":
                            _toDate = null;
                            await LoadData();
                            break;
                    }
                    return;
                }

                var parts = dateStr.Split('/');
                if (parts.Length == 3 &&
                    int.TryParse(parts[0], out int day) &&
                    int.TryParse(parts[1], out int month) &&
                    int.TryParse(parts[2], out int year))
                {
                    var date = new DateTime(year, month, day);

                    switch (fieldName)
                    {
                        case nameof(SelectedItem.ngay_ghi_nhan):
                            SelectedItem.ngay_ghi_nhan = date;
                            break;

                        case nameof(SelectedItem.thoi_gian_bat_dau):
                            SelectedItem.thoi_gian_bat_dau = date;
                            break;

                        case nameof(SelectedItem.thoi_gian_ket_thuc):
                            SelectedItem.thoi_gian_ket_thuc = date;
                            break;

                        case "fromDate":
                            _fromDate = date;
                            await LoadData();
                            break;

                        case "toDate":
                            _toDate = date;
                            await LoadData();
                            break;
                    }
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
    }
}
