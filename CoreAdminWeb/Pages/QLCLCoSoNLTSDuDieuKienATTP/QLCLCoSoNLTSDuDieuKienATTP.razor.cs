using CoreAdminWeb.Model;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using CoreAdminWeb.Extensions;

namespace CoreAdminWeb.Pages.QLCLCoSoNLTSDuDieuKienATTP
{
    public partial class QLCLCoSoNLTSDuDieuKienATTP(IBaseService<QLCLCoSoNLTSDuDieuKienATTPModel> MainService,
                                              IQLCLCoSoNLTSDuDieuKienATTPSanPhamService QLCLCoSoNLTSDuDieuKienATTPSanPhamService,
                                              IBaseService<TinhModel> TinhService,
                                              IBaseService<XaPhuongModel> XaPhuongService,
                                              IBaseService<QLCLLoaiHinhKinhDoanhModel> LoaiHinhKinhDoanhService,
                                              IBaseService<QLCLSanPhamSanXuatModel> SanPhamService) : BlazorCoreBase
    {
        private List<QLCLCoSoNLTSDuDieuKienATTPModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openSanPhamDetailDeleteModal = false;
        private bool openAddOrUpdateModal = false;

        private List<Enums.KetQuaKiemTraDinhKy> KetQuaThamDinhItems { get; set; } = new List<Enums.KetQuaKiemTraDinhKy>() {
            Enums.KetQuaKiemTraDinhKy.Dat,
            Enums.KetQuaKiemTraDinhKy.KhongDat,
        };

        private QLCLCoSoNLTSDuDieuKienATTPModel SelectedItem { get; set; } = new QLCLCoSoNLTSDuDieuKienATTPModel();
        private List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel> SelectedSanPhamItemsDetail { get; set; } = new List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>();
        private QLCLCoSoNLTSDuDieuKienATTPSanPhamModel? SelectedSanPhamItemDetail { get; set; } = default;

        private string _searchString = "";
        private TinhModel? _selectedTinhFilter { get; set; }
        private XaPhuongModel? _selectedXaFilter { get; set; }
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
            IsLoading = true;
            BuildPaginationQuery(Page, PageSize, "id", false);
            int index = 2;

            BuilderQuery += "&filter[_and][0][deleted][_eq]=false";
            BuilderQuery += "&filter[_and][1][loai][_eq]=1";
            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&filter[_and][{index}][_or][0][code][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][2][dia_chi][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][3][loai_hinh_kinh_doanh][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][4][so_giay_chung_nhan][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][5][co_quan_cap][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][6][xu_ly_ket_qua][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][7][he_thong_quan_ly_chat_luong][_contains]={_searchString}";
                index++;
            }
            if(_selectedTinhFilter != null)
            {
                BuilderQuery += $"&filter[_and][{index}][province][_eq]={_selectedTinhFilter.id}";
                index++;
            }

            if(_selectedXaFilter != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ward][_eq]={_selectedXaFilter.id}";
                index++;
            }

            if(_fromDate != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ngay_cap][_gte]={_fromDate.Value.ToString("yyyy-MM-dd")}";
                index++;
            }

            if(_toDate != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ngay_cap][_lte]={_toDate.Value.ToString("yyyy-MM-dd")}";
            }

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<QLCLCoSoNLTSDuDieuKienATTPModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<QLCLCoSoNLTSDuDieuKienATTPModel>();
            }
            IsLoading = false;
        }

        private async Task LoadSanPhamDetailData()
        {
            var buildQuery = $"sort=-id";
            buildQuery += $"&filter[_and][][co_so_nlts_du_dieu_kien_attp][_eq]={SelectedItem.id}";
            // buildQuery += $"&filter[_and][][deleted][_eq]=false";
            var result = await QLCLCoSoNLTSDuDieuKienATTPSanPhamService.GetAllAsync(buildQuery);
            SelectedSanPhamItemsDetail = result.Data ?? new List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>();
        }

        private async Task<IEnumerable<TinhModel>> LoadTinhData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, TinhService);
        }

        private async Task<IEnumerable<XaPhuongModel>> LoadXaCRUDData(string searchText)
        {
            string query = $"sort=-id";
            query += $"&filter[_and][][ProvinceId][_eq]={(SelectedItem.province == null ? 0 : SelectedItem.province?.id)}";
            return await LoadBlazorTypeaheadData(searchText, XaPhuongService,query);
        }


        private async Task<IEnumerable<XaPhuongModel>> LoadXaFilterData(string searchText)
        {
            string query = $"sort=-id";
            query += $"&filter[_and][][ProvinceId][_eq]={(_selectedTinhFilter == null ? 0 : _selectedTinhFilter?.id)}";
            return await LoadBlazorTypeaheadData(searchText, XaPhuongService,query);
        }
        private async Task<IEnumerable<QLCLLoaiHinhKinhDoanhModel>> LoadLoaiHinhKinhDoanhData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, LoaiHinhKinhDoanhService);
        }
        private async Task<IEnumerable<QLCLSanPhamSanXuatModel>> LoadSanPhamData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, SanPhamService);
        }

        private void OpenDeleteModal(QLCLCoSoNLTSDuDieuKienATTPModel item)
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
            SelectedItem = new QLCLCoSoNLTSDuDieuKienATTPModel();
            openDeleteModal = false;
        }

        private void OpenSanPhamDetailDeleteModal(QLCLCoSoNLTSDuDieuKienATTPSanPhamModel item)
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
                SelectedSanPhamItemsDetail.Add(new QLCLCoSoNLTSDuDieuKienATTPSanPhamModel()
                {
                    co_so_nlts_du_dieu_kien_attp = SelectedItem,
                    sort = (SelectedSanPhamItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    san_pham = null,
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
                SelectedSanPhamItemsDetail = new List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>();

            SelectedSanPhamItemsDetail.Add(new QLCLCoSoNLTSDuDieuKienATTPSanPhamModel
            {
                co_so_nlts_du_dieu_kien_attp = SelectedItem,
                sort = (SelectedSanPhamItemsDetail.Max(c => c.sort) ?? 0) + 1,
                san_pham = null,
                san_luong_tan = 0,
                deleted = false,
            });
        }

        private async Task OpenAddOrUpdateModal(QLCLCoSoNLTSDuDieuKienATTPModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item : new QLCLCoSoNLTSDuDieuKienATTPModel();

            if (SelectedItem.id > 0)
            {
                await LoadSanPhamDetailData();
            }

            if (!SelectedSanPhamItemsDetail.Any())
                SelectedSanPhamItemsDetail.Add(new QLCLCoSoNLTSDuDieuKienATTPSanPhamModel()
                {
                    co_so_nlts_du_dieu_kien_attp = SelectedItem,
                    sort = (SelectedSanPhamItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    san_pham = null,
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
            if (SelectedItem.id <= 0)
            {
                var result = await MainService.CreateAsync(SelectedItem);
                if (result.IsSuccess)
                {
                    var sanPhamChiTietList = SelectedSanPhamItemsDetail
                        .Where(c => c.deleted == false || c.deleted == null)
                        .Select(c =>
                        {
                            c.co_so_nlts_du_dieu_kien_attp = result.Data;
                            return c;
                        })
                        .ToList();

                    var sanPhamDetailResult = await QLCLCoSoNLTSDuDieuKienATTPSanPhamService.CreateAsync(sanPhamChiTietList);
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


                    var addNewSanPhamChiTietList = SelectedSanPhamItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id == 0)
                        .Select(c =>
                        {
                            c.co_so_nlts_du_dieu_kien_attp = SelectedItem;
                            return c;
                        }).ToList();
                    var removeSanPhamChiTietList = SelectedSanPhamItemsDetail
                        .Where(c => c.deleted == true && c.id > 0)
                        .Select(c =>
                        {
                            c.co_so_nlts_du_dieu_kien_attp = SelectedItem;
                            c.deleted = true;
                            return c;
                        }).ToList();
                    var updateSanPhamChiTietList = SelectedSanPhamItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id > 0)
                        .Select(c =>
                        {
                            c.co_so_nlts_du_dieu_kien_attp = SelectedItem;
                            return c;
                        }).ToList();

                    if (addNewSanPhamChiTietList.Any())
                    {
                        var detailResult = await QLCLCoSoNLTSDuDieuKienATTPSanPhamService.CreateAsync(addNewSanPhamChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (removeSanPhamChiTietList.Any())
                    {
                        var detailResult = await QLCLCoSoNLTSDuDieuKienATTPSanPhamService.DeleteAsync(removeSanPhamChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi xóa chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (updateSanPhamChiTietList.Any())
                    {
                        var detailResult = await QLCLCoSoNLTSDuDieuKienATTPSanPhamService.UpdateAsync(updateSanPhamChiTietList);
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
            SelectedItem = new QLCLCoSoNLTSDuDieuKienATTPModel();
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
                        case nameof(SelectedItem.ngay_cap):
                            SelectedItem.ngay_cap = null;
                            break;


                        case nameof(SelectedItem.ngay_het_hieu_luc):
                            SelectedItem.ngay_het_hieu_luc = null;
                            break;

                        case nameof(SelectedItem.ngay_tham_dinh):
                            SelectedItem.ngay_tham_dinh = null;
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
                        case nameof(SelectedItem.ngay_cap):
                            SelectedItem.ngay_cap = date;
                            break;

                        case nameof(SelectedItem.ngay_het_hieu_luc):
                            SelectedItem.ngay_het_hieu_luc = date;
                            break;

                        case nameof(SelectedItem.ngay_tham_dinh):
                            SelectedItem.ngay_tham_dinh = date;
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
        
        private async Task OnExportExcel()
        {
            // Get all data for export
            BuildPaginationQuery(Page, int.MaxValue);
            int index = 2;
            BuilderQuery += "&filter[_and][0][deleted][_eq]=false";
            BuilderQuery += "&filter[_and][1][loai][_eq]=1";
            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&filter[_and][{index}][_or][0][code][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][2][dia_chi][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][3][loai_hinh_kinh_doanh][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][4][so_giay_chung_nhan][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][5][co_quan_cap][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][6][xu_ly_ket_qua][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][7][he_thong_quan_ly_chat_luong][_contains]={_searchString}";
                index++;
            }
            if(_selectedTinhFilter != null)
            {
                BuilderQuery += $"&filter[_and][{index}][province][_eq]={_selectedTinhFilter.id}";
                index++;
            }

            if(_selectedXaFilter != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ward][_eq]={_selectedXaFilter.id}";
                index++;
            }

            if(_fromDate != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ngay_cap][_gte]={_fromDate.Value.ToString("yyyy-MM-dd")}";
                index++;
            }

            if(_toDate != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ngay_cap][_lte]={_toDate.Value.ToString("yyyy-MM-dd")}";
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
            ws.Cells[1, 2].Value = "MS doanh nghiệp";
            ws.Cells[1, 3].Value = "Tên cơ sở";
            ws.Cells[1, 4].Value = "Loại hình kinh doanh";
            ws.Cells[1, 5].Value = "Số GCN";
            ws.Cells[1, 6].Value = "Ngày cấp";
            ws.Cells[1, 7].Value = "Ngày hết hiệu lực";
            ws.Cells[1, 8].Value = "Ngày thẩm định";
            ws.Cells[1, 9].Value = "Địa chỉ";
            ws.Cells[1, 10].Value = "Kết quả thẩm định";
            ws.Cells[1, 11].Value = "Trạng thái";

            // Style header
            using (var range = ws.Cells[1, 1, 1, 11])
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
                ws.Cells[row, 4].Value = item.loai_hinh_kinh_doanh?.name;
                ws.Cells[row, 5].Value = item.so_giay_chung_nhan;
                ws.Cells[row, 6].Value = item.ngay_cap?.ToString("dd/MM/yyyy");
                ws.Cells[row, 7].Value = item.ngay_het_hieu_luc?.ToString("dd/MM/yyyy");
                ws.Cells[row, 8].Value = item.ngay_tham_dinh?.ToString("dd/MM/yyyy");
                ws.Cells[row, 9].Value = item.dia_chi;
                ws.Cells[row, 10].Value = item.ket_qua_tham_dinh?.GetDescription();
                ws.Cells[row, 11].Value = item.status.GetDescription();
                row++;
                stt++;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            // Export to browser
            var fileName = $"DanhSachCoSoNLTSDuDieuKienATTP_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var fileBytes = package.GetAsByteArray();
            // Nếu chưa có hàm saveAsFile trong wwwroot/js, hãy thêm hàm này để hỗ trợ download file từ base64
            await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(fileBytes));
        }
    }
}
