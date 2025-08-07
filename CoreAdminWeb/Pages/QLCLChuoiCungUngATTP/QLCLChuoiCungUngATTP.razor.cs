using CoreAdminWeb.Enums;
using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CoreAdminWeb.Pages.QLCLChuoiCungUngATTP
{
    public partial class QLCLChuoiCungUngATTP(IBaseService<QLCLChuoiCungUngATTPModel> MainService,
                                              IQLCLChuoiCungUngATTPCoSoService QLCLChuoiCungUngATTPCoSoService,
                                              IBaseService<TinhModel> TinhService,
                                              IBaseService<XaPhuongModel> XaPhuongService,
                                              IBaseService<QLCLCoSoCheBienNLTSModel> QLCLCoSoCheBienNLTSService,
                                              IBaseService<QLCLCoSoNLTSDuDieuKienATTPModel> QLCLCoSoNLTSDuDieuKienATTPService) : BlazorCoreBase
    {
        private List<QLCLChuoiCungUngATTPModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openCoSoDetailDeleteModal = false;
        private bool openAddOrUpdateModal = false;

        private List<Enums.LoaiCoSoChuoiCungUng> LoaiCoSoChuoiCungUngItems { get; set; } = new List<Enums.LoaiCoSoChuoiCungUng>() {
            Enums.LoaiCoSoChuoiCungUng.CoSoCheBien,
            Enums.LoaiCoSoChuoiCungUng.CoSoXSKD,
        };

        private QLCLChuoiCungUngATTPModel SelectedItem { get; set; } = new QLCLChuoiCungUngATTPModel();
        private List<QLCLChuoiCungUngATTPCoSoModel> SelectedCoSoItemsDetail { get; set; } = new List<QLCLChuoiCungUngATTPCoSoModel>();
        private QLCLChuoiCungUngATTPCoSoModel? SelectedCoSoItemDetail { get; set; } = default;

        private string _searchString = "";
        private TinhModel? _selectedTinhFilter { get; set; }
        private XaPhuongModel? _selectedXaFilter { get; set; }
        private string _titleAddOrUpdate = "Thêm mới";
        private string activeDefTab { get; set; } = "tab1";
        private DateTime? _fromDate { get; set; } = null;
        private DateTime? _toDate { get; set; } = null;

        // Select2 define
        private Dictionary<int, List<XaPhuongModel>> SelectedXaPhuongItems { get; set; } = new();
        private List<XaPhuongModel> XaPhuongItems { get; set; } = new();
        private Dictionary<int, List<XaPhuongModel>> SelectedXaPhuongKinhDoanhItems { get; set; } = new();
        private List<XaPhuongModel> XaPhuongKinhDoanhItems { get; set; } = new();
        private Dictionary<int, List<XaPhuongModel>> SelectedXaPhuongSanXuatItems { get; set; } = new();
        private List<XaPhuongModel> XaPhuongSanXuatItems { get; set; } = new();

        private List<QLCLCoSoCheBienNLTSModel> QLCLCoSoCheBienNLTSItems = new List<QLCLCoSoCheBienNLTSModel>();
        private Dictionary<int, List<QLCLCoSoCheBienNLTSModel>> SelectedQLCLCoSoCheBienNLTSItems = new Dictionary<int, List<QLCLCoSoCheBienNLTSModel>>();

        private List<QLCLCoSoNLTSDuDieuKienATTPModel> QLCLCoSoNLTSDuDieuKienATTPItems = new List<QLCLCoSoNLTSDuDieuKienATTPModel>();
        private Dictionary<int, List<QLCLCoSoNLTSDuDieuKienATTPModel>> SelectedQLCLCoSoNLTSDuDieuKienATTPItems = new Dictionary<int, List<QLCLCoSoNLTSDuDieuKienATTPModel>>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadData();
                _selectedTinhFilter = await LoadDefaultData(TinhService);
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
            int index = 2;

            BuilderQuery += "&filter[_and][0][deleted][_eq]=false";
            if (!string.IsNullOrEmpty(_searchString))
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][_or][0][code][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][2][dia_chi_san_xuat][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][3][dia_chi_kinh_doanh][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][4][so_xac_nhan][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][5][co_quan_xac_nhan][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][6][san_pham][_contains]={_searchString}";
            }
            if (_selectedTinhFilter != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][_or][0][province_san_xuat][_eq]={_selectedTinhFilter.id}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][province_kinh_doanh][_eq]={_selectedTinhFilter.id}";
            }

            if (_selectedXaFilter != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][_or][0][ward_san_xuat][_eq]={_selectedXaFilter.id}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][ward_kinh_doanh][_eq]={_selectedXaFilter.id}";
            }
            else
            {
                index++;
                XaPhuongItems = await LoadDataInTable(new List<XaPhuongModel>(), "", CancellationToken.None, XaPhuongService);
                string xaFilterIds = string.Join(",", XaPhuongItems.Select(x => x.id).ToList());
                BuilderQuery += $"&filter[_and][{index}][_or][0][ward_san_xuat][_in]={xaFilterIds}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][ward_kinh_doanh][_in]={xaFilterIds}";
            }

            if (_fromDate != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][ngay_chung_nhan][_gte]={_fromDate.Value:yyyy-MM-dd}";
            }

            if (_toDate != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][ngay_chung_nhan][_lte]={_toDate.Value:yyyy-MM-dd}";
            }

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<QLCLChuoiCungUngATTPModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<QLCLChuoiCungUngATTPModel>();
            }
            IsLoading = false;
        }

        private async Task LoadCoSoDetailData()
        {
            var buildQuery = $"sort=-id";
            buildQuery += $"&filter[_and][][chuoi_cung_ung][_eq]={SelectedItem.id}";
            var result = await QLCLChuoiCungUngATTPCoSoService.GetAllAsync(buildQuery);
            SelectedCoSoItemsDetail = result.Data ?? new List<QLCLChuoiCungUngATTPCoSoModel>();
        }

        private async Task<IEnumerable<TinhModel>> LoadTinhData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, TinhService);
        }

        private async Task<List<XaPhuongModel>> FilterFunctionXaPhuongData(IEnumerable<XaPhuongModel> allItems, string filter,
            CancellationToken token)
        {
            string query = $"sort=-id";
            query += $"&filter[_and][][ProvinceId][_eq]={(_selectedTinhFilter == null ? 0 : _selectedTinhFilter?.id)}";
            XaPhuongItems = await LoadDataInTable(allItems, filter, token, XaPhuongService, query);
            StateHasChanged();
            return XaPhuongItems;
        }

        private async Task<List<XaPhuongModel>> FilterFunctionXaPhuongKinhDoanhData(IEnumerable<XaPhuongModel> allItems, string filter,
            CancellationToken token)
        {
            string query = $"sort=-id";
            query += $"&filter[_and][][ProvinceId][_eq]={(SelectedItem.province_kinh_doanh == null ? 0 : SelectedItem.province_kinh_doanh?.id)}";
            XaPhuongKinhDoanhItems = await LoadDataInTable(allItems, filter, token, XaPhuongService, query);
            StateHasChanged();
            return XaPhuongKinhDoanhItems;
        }

        private async Task<List<XaPhuongModel>> FilterFunctionXaPhuongSanXuatData(IEnumerable<XaPhuongModel> allItems, string filter,
            CancellationToken token)
        {
            string query = $"sort=-id";
            query += $"&filter[_and][][ProvinceId][_eq]={(SelectedItem.province_san_xuat == null ? 0 : SelectedItem.province_san_xuat?.id)}";
            XaPhuongSanXuatItems = await LoadDataInTable(allItems, filter, token, XaPhuongService, query);
            StateHasChanged();
            return XaPhuongSanXuatItems;
        }

        private async Task<List<QLCLCoSoCheBienNLTSModel>> FilterFunctionQLCLCoSoCheBienNLTSMData(IEnumerable<QLCLCoSoCheBienNLTSModel> allItems, string filter,
            CancellationToken token)
        {
            QLCLCoSoCheBienNLTSItems = await LoadDataInTable(allItems, filter, token, QLCLCoSoCheBienNLTSService);
            StateHasChanged();
            return QLCLCoSoCheBienNLTSItems;
        }

        private async Task<List<QLCLCoSoNLTSDuDieuKienATTPModel>> FilterFunctionQLCLCoSoNLTSDuDieuKienATTPData(IEnumerable<QLCLCoSoNLTSDuDieuKienATTPModel> allItems, string filter,
            CancellationToken token)
        {
            string query = $"&filter[_and][][loai][_eq]={1}";
            QLCLCoSoNLTSDuDieuKienATTPItems = await LoadDataInTable(allItems, filter, token, QLCLCoSoNLTSDuDieuKienATTPService, query);
            StateHasChanged();
            return QLCLCoSoNLTSDuDieuKienATTPItems;
        }

        private void OpenDeleteModal(QLCLChuoiCungUngATTPModel item)
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
            SelectedItem = new QLCLChuoiCungUngATTPModel();
            openDeleteModal = false;
        }

        private void OpenCoSoDetailDeleteModal(QLCLChuoiCungUngATTPCoSoModel item)
        {
            SelectedCoSoItemDetail = item;
            openCoSoDetailDeleteModal = true;
        }

        private void OnCoSoDetailDelete()
        {
            if (SelectedCoSoItemDetail == null)
            {
                AlertService.ShowAlert("Không có dữ liệu để xóa", "warning");
                return;
            }

            foreach (var item in SelectedCoSoItemsDetail)
            {
                if (item.id > 0 && item.id == SelectedCoSoItemDetail.id || item.sort > 0 && item.sort == SelectedCoSoItemDetail.sort)
                {
                    item.deleted = true;
                }
            }

            SelectedCoSoItemDetail = default;

            openCoSoDetailDeleteModal = false;

            if (!SelectedCoSoItemsDetail.Any(c => c.deleted == null || c.deleted == false))
            {
                SelectedCoSoItemsDetail.Add(new QLCLChuoiCungUngATTPCoSoModel()
                {
                    chuoi_cung_ung = SelectedItem,
                    sort = (SelectedCoSoItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    loai_co_so = LoaiCoSoChuoiCungUng.CoSoCheBien,
                    co_so_che_bien_nlts = null,
                    co_so_nlts_du_dieu_kien_attp = null,
                    deleted = false,
                });
            }

            StateHasChanged();
        }

        private void CloseCoSoDetailDeleteModal()
        {
            SelectedCoSoItemDetail = default;
            openCoSoDetailDeleteModal = false;
        }

        private void OnAddCoSo()
        {
            if (SelectedCoSoItemsDetail == null)
            {
                SelectedCoSoItemsDetail = new List<QLCLChuoiCungUngATTPCoSoModel>();
            }

            SelectedCoSoItemsDetail.Add(new QLCLChuoiCungUngATTPCoSoModel
            {
                chuoi_cung_ung = SelectedItem,
                sort = (SelectedCoSoItemsDetail.Max(c => c.sort) ?? 0) + 1,
                loai_co_so = LoaiCoSoChuoiCungUng.CoSoCheBien,
                co_so_che_bien_nlts = null,
                co_so_nlts_du_dieu_kien_attp = null,
                deleted = false,
            });
        }

        private async Task OpenAddOrUpdateModal(QLCLChuoiCungUngATTPModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedCoSoItemsDetail = new List<QLCLChuoiCungUngATTPCoSoModel>();
            SelectedItem = item != null ? item.DeepClone() : new QLCLChuoiCungUngATTPModel()
            {
                province_san_xuat = await LoadDefaultData(TinhService),
                province_kinh_doanh = await LoadDefaultData(TinhService),
            };

            if (SelectedItem.id > 0)
            {
                await LoadCoSoDetailData();
            }

            if (!SelectedCoSoItemsDetail.Any())
            {
                SelectedCoSoItemsDetail.Add(new QLCLChuoiCungUngATTPCoSoModel()
                {
                    chuoi_cung_ung = SelectedItem,
                    sort = (SelectedCoSoItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    loai_co_so = LoaiCoSoChuoiCungUng.CoSoCheBien,
                    co_so_che_bien_nlts = null,
                    co_so_nlts_du_dieu_kien_attp = null,
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
            if (SelectedItem.id <= 0)
            {
                var result = await MainService.CreateAsync(SelectedItem);
                if (result.IsSuccess)
                {
                    var CoSoChiTietList = SelectedCoSoItemsDetail
                        .Where(c => c.deleted == false || c.deleted == null)
                        .Select(c =>
                        {
                            c.chuoi_cung_ung = result.Data;
                            return c;
                        })
                        .ToList();

                    var CoSoDetailResult = await QLCLChuoiCungUngATTPCoSoService.CreateAsync(CoSoChiTietList);
                    if (!CoSoDetailResult.IsSuccess)
                    {
                        AlertService.ShowAlert(CoSoDetailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
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


                    var addNewCoSoChiTietList = SelectedCoSoItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id == 0)
                        .Select(c =>
                        {
                            c.chuoi_cung_ung = SelectedItem;
                            return c;
                        }).ToList();
                    var removeCoSoChiTietList = SelectedCoSoItemsDetail
                        .Where(c => c.deleted == true && c.id > 0)
                        .Select(c =>
                        {
                            c.chuoi_cung_ung = SelectedItem;
                            c.deleted = true;
                            return c;
                        }).ToList();
                    var updateCoSoChiTietList = SelectedCoSoItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id > 0)
                        .Select(c =>
                        {
                            c.chuoi_cung_ung = SelectedItem;
                            return c;
                        }).ToList();

                    if (addNewCoSoChiTietList.Any())
                    {
                        var detailResult = await QLCLChuoiCungUngATTPCoSoService.CreateAsync(addNewCoSoChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (removeCoSoChiTietList.Any())
                    {
                        var detailResult = await QLCLChuoiCungUngATTPCoSoService.DeleteAsync(removeCoSoChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi xóa chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (updateCoSoChiTietList.Any())
                    {
                        var detailResult = await QLCLChuoiCungUngATTPCoSoService.UpdateAsync(updateCoSoChiTietList);
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
            SelectedItem = new QLCLChuoiCungUngATTPModel();
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

        public async Task OnTinhFilterChanged(TinhModel? tinh)
        {
            _selectedTinhFilter = tinh;
            _selectedXaFilter = null;
            await LoadData();
        }
        public async Task OnXaFilterChanged(XaPhuongModel? xa)
        {
            _selectedXaFilter = xa;
            await LoadData();
        }
        public void OnLoaiCoSoChanged(ChangeEventArgs e, QLCLChuoiCungUngATTPCoSoModel item)
        {
            var value = e.Value?.ToString();
            SelectedCoSoItemDetail = item;
            SelectedCoSoItemDetail.loai_co_so = !string.IsNullOrEmpty(value) ? (LoaiCoSoChuoiCungUng)Enum.Parse(typeof(LoaiCoSoChuoiCungUng), value) : LoaiCoSoChuoiCungUng.CoSoCheBien;
            SelectedCoSoItemDetail.co_so_che_bien_nlts = null;
            SelectedCoSoItemDetail.co_so_nlts_du_dieu_kien_attp = null;
        }

        private async Task OnExportExcel()
        {
            // Get all data for export
            BuildPaginationQuery(Page, int.MaxValue);
            int index = 2;

            BuilderQuery += "&filter[_and][0][deleted][_eq]=false";
            if (!string.IsNullOrEmpty(_searchString))
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][_or][0][code][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][2][dia_chi_san_xuat][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][3][dia_chi_kinh_doanh][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][4][so_xac_nhan][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][5][co_quan_xac_nhan][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][6][san_pham][_contains]={_searchString}";
            }
            if (_selectedTinhFilter != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][_or][0][province_san_xuat][_eq]={_selectedTinhFilter.id}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][province_kinh_doanh][_eq]={_selectedTinhFilter.id}";
            }

            if (_selectedXaFilter != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][_or][0][ward_san_xuat][_eq]={_selectedXaFilter.id}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][ward_kinh_doanh][_eq]={_selectedXaFilter.id}";
            }
            else
            {
                index++;
                XaPhuongItems = await LoadDataInTable(new List<XaPhuongModel>(), "", CancellationToken.None, XaPhuongService);
                string xaFilterIds = string.Join(",", XaPhuongItems.Select(x => x.id).ToList());
                BuilderQuery += $"&filter[_and][{index}][_or][0][ward_san_xuat][_in]={xaFilterIds}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][ward_kinh_doanh][_in]={xaFilterIds}";
            }

            if (_fromDate != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][ngay_chung_nhan][_gte]={_fromDate.Value:yyyy-MM-dd}";
            }

            if (_toDate != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][ngay_chung_nhan][_lte]={_toDate.Value:yyyy-MM-dd}";
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
            ws.Cells[1, 2].Value = "Mã số chuỗi";
            ws.Cells[1, 3].Value = "Tên chuỗi cung ứng";
            ws.Cells[1, 4].Value = "Địa chỉ sản xuất";
            ws.Cells[1, 5].Value = "Tỉnh thành";
            ws.Cells[1, 6].Value = "Xã phường";
            ws.Cells[1, 7].Value = "Địa chỉ kinh doanh";
            ws.Cells[1, 8].Value = "Tỉnh thành";
            ws.Cells[1, 9].Value = "Xã phường";
            ws.Cells[1, 10].Value = "Số xác nhận";
            ws.Cells[1, 11].Value = "Ngày chứng nhận";
            ws.Cells[1, 12].Value = "Cơ quan xác nhận";
            ws.Cells[1, 13].Value = "Sản phẩm tham gia chuỗi cung ứng";

            // Style header
            using (var range = ws.Cells[1, 1, 1, 13])
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
                ws.Cells[row, 2].Value = item.code;
                ws.Cells[row, 3].Value = item.name;
                ws.Cells[row, 4].Value = item.dia_chi_san_xuat;
                ws.Cells[row, 5].Value = item.province_san_xuat?.name;
                ws.Cells[row, 6].Value = item.ward_san_xuat?.name;
                ws.Cells[row, 7].Value = item.dia_chi_kinh_doanh;
                ws.Cells[row, 8].Value = item.province_kinh_doanh?.name;
                ws.Cells[row, 9].Value = item.ward_kinh_doanh?.name;
                ws.Cells[row, 10].Value = item.so_xac_nhan;
                ws.Cells[row, 11].Value = item.ngay_chung_nhan?.ToString("dd/MM/yyyy");
                ws.Cells[row, 12].Value = item.co_quan_xac_nhan;
                ws.Cells[row, 13].Value = item.san_pham;
                row++;
                stt++;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            // Export to browser
            var fileName = $"DuLieuCacChuoiCungUngThucPhamAnToan_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var fileBytes = await package.GetAsByteArrayAsync();
            // Nếu chưa có hàm saveAsFile trong wwwroot/js, hãy thêm hàm này để hỗ trợ download file từ base64
            await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(fileBytes));
        }
    }
}
