using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using CoreAdminWeb.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using CoreAdminWeb.Enums;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace CoreAdminWeb.Pages.QLCLCoSoViPhamATTP
{
    public partial class QLCLCoSoViPhamATTP(
        IBaseService<QLCLCoSoViPhamATTPModel> MainService,
        IBaseService<QLCLCoSoCheBienNLTSModel> QLCLCoSoCheBienNLTSService,
        IBaseService<QLCLCoSoNLTSDuDieuKienATTPModel> QLCLCoSoNLTSDuDieuKienATTPService,
        IBaseService<QLCLHanhViViPhamModel> QLCLHanhViViPhamService,
        IBaseService<QLCLHinhThucXuPhatModel> QLCLHinhThucXuPhatService,
        IBaseService<DonViTinhModel> DonViTinhService,
        IBaseService<TinhModel> TinhService,
        IBaseService<XaPhuongModel> XaPhuongService) : BlazorCoreBase
    {
        private List<QLCLCoSoViPhamATTPModel> MainModels { get; set; } = new();
        private List<LoaiCoSoNLTS> LoaiCoSoList = new() { LoaiCoSoNLTS.CoSoCheBien, LoaiCoSoNLTS.CoSoXSKDDuDieuKien, LoaiCoSoNLTS.CoSoXSKDKhongDuDieuKien };
        private List<KhacPhuc> KhacPhucList = new() { KhacPhuc.BuocThuHoi, KhacPhuc.BuocTieuHuy, KhacPhuc.Khac };
        private List<TrangThaiXuLyKhac> TrangThaiXuLyKhacList = new() { TrangThaiXuLyKhac.DinhChiLuuHanh, TrangThaiXuLyKhac.ChuyenCoQuanDieuTra };
        private List<TrangThaiXuLy> TrangThaiXuLyList = new() { TrangThaiXuLy.ChuaXuLy, TrangThaiXuLy.DangXuLy, TrangThaiXuLy.DaXuLy };

        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private string _titleAddOrUpdate = "Thêm mới";
        private string _searchString = "";
        private TinhModel? _selectedTinhFilter { get; set; }
        private XaPhuongModel? _selectedXaFilter { get; set; }
        private DateTime? _fromDate = null;
        private DateTime? _toDate = null;

        private List<QLCLCoSoCheBienNLTSModel> QLCLCoSoCheBienNLTSList { get; set; } = new();
        private List<QLCLCoSoNLTSDuDieuKienATTPModel> QLCLCoSoNLTSDuDieuKienATTPList { get; set; } = new();
        private List<QLCLHanhViViPhamModel> QLCLHanhViViPhamList { get; set; } = new();
        private List<QLCLHinhThucXuPhatModel> QLCLHinhThucXuPhatList { get; set; } = new();
        private List<DonViTinhModel> DonViTinhList { get; set; } = new();

        private QLCLCoSoViPhamATTPModel SelectedItem { get; set; } = new QLCLCoSoViPhamATTPModel();

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
            try
            {
                IsLoading = true;
                BuildPaginationQuery(Page, PageSize);
                int intdex = 1;

                BuilderQuery += "filter[_and][0][deleted][_eq]=false&sort=sort";
                if (!string.IsNullOrEmpty(_searchString))
                {
                    BuilderQuery += $"&filter[_and][{intdex}][_or][0][co_so_che_bien_nlts][name][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{intdex}][_or][1][co_so_nlts_du_dieu_kien_attp][name][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{intdex}][_or][2][hanh_vi_vi_pham][name][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{intdex}][_or][3][huong_xu_ly][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{intdex}][_or][4][description][_contains]={_searchString}";
                    intdex++;
                }

                if (_selectedTinhFilter != null)
                {
                    BuilderQuery += $"&filter[_and][{intdex}][_or][0][co_so_che_bien_nlts][province][_eq]={_selectedTinhFilter.id}";
                    BuilderQuery += $"&filter[_and][{intdex}][_or][1][co_so_nlts_du_dieu_kien_attp][province][_eq]={_selectedTinhFilter.id}";
                    intdex++;
                }

                if (_selectedXaFilter != null)
                {
                    BuilderQuery += $"&filter[_and][{intdex}][_or][0][co_so_che_bien_nlts][ward][_eq]={_selectedXaFilter.id}";
                    BuilderQuery += $"&filter[_and][{intdex}][_or][1][co_so_nlts_du_dieu_kien_attp][ward][_eq]={_selectedXaFilter.id}";
                    intdex++;
                }

                if (_fromDate != null)
                {
                    BuilderQuery += $"&filter[_and][{intdex}][ngay_ghi_nhan][_gte]={_fromDate.Value.ToString("yyyy-MM-dd")}";
                    intdex++;
                }

                if (_toDate != null)
                {
                    BuilderQuery += $"&filter[_and][{intdex}][ngay_ghi_nhan][_lte]={_toDate.Value.ToString("yyyy-MM-dd")}";
                    intdex++;
                }


                var result = await MainService.GetAllAsync(BuilderQuery);
                if (result.IsSuccess)
                {
                    MainModels = result.Data ?? new List<QLCLCoSoViPhamATTPModel>();
                    if (result.Meta != null)
                    {
                        TotalItems = result.Meta.filter_count ?? 0;
                        TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                    }
                }
                else
                {
                    MainModels = new List<QLCLCoSoViPhamATTPModel>();
                    AlertService.ShowAlert(result.Message ?? "Lỗi khi lấy dữ liệu", "danger");
                }
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi: {ex.Message}", "danger");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task<IEnumerable<TinhModel>> LoadTinhData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, TinhService);
        }

        private async Task<IEnumerable<XaPhuongModel>> LoadXaFilterData(string searchText)
        {
            string query = $"sort=-id";
            query += $"&filter[_and][][ProvinceId][_eq]={(_selectedTinhFilter == null ? 0 : _selectedTinhFilter?.id)}";
            return await LoadBlazorTypeaheadData(searchText, XaPhuongService, query);
        }

        private void OpenAddOrUpdateModal(QLCLCoSoViPhamATTPModel? item)
        {
            try
            {
                _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
                SelectedItem = item?.DeepClone() ?? new QLCLCoSoViPhamATTPModel();
                openAddOrUpdateModal = true;

                // Wait for modal to render
                _ = Task.Run(async () =>
                {
                    await Task.Delay(500);
                    await JsRuntime.InvokeVoidAsync("initializeDatePicker");
                });
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi: {ex.Message}", "danger");
            }
        }

        private void OpenDeleteModal(QLCLCoSoViPhamATTPModel item)
        {
            try
            {
                SelectedItem = item;
                openDeleteModal = true;
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi: {ex.Message}", "danger");
            }
        }

        private void CloseDeleteModal()
        {
            try
            {
                SelectedItem = new QLCLCoSoViPhamATTPModel();
                openDeleteModal = false;
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi: {ex.Message}", "danger");
            }
        }

        private void CloseAddOrUpdateModal()
        {
            try
            {
                SelectedItem = new QLCLCoSoViPhamATTPModel();
                openAddOrUpdateModal = false;
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi: {ex.Message}", "danger");
            }
        }

        private async Task OnValidSubmit()
        {
            try
            {
                var resultCreate = SelectedItem.id == 0 ? await MainService.CreateAsync(SelectedItem) : new();
                var resultUpdate = SelectedItem.id > 0 ? await MainService.UpdateAsync(SelectedItem) : new();
                string message = resultCreate.Message ?? resultUpdate.Message;
                if ((SelectedItem.id == 0 && resultCreate.IsSuccess) || (SelectedItem.id > 0 && resultUpdate.IsSuccess))
                {
                    await LoadData();
                    openAddOrUpdateModal = false;
                    AlertService.ShowAlert(SelectedItem.id == 0 ? "Thêm mới thành công!" : "Cập nhật thành công!", "success");
                }
                else
                {
                    AlertService.ShowAlert($"Lỗi khi {(SelectedItem.id == 0 ? "thêm mới" : "cập nhật")} dữ liệu :" + message, "danger");
                }
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi: {ex.Message}", "danger");
            }
        }


        private async Task OnDelete()
        {
            try
            {
                if (SelectedItem == null) return;

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
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi: {ex.Message}", "danger");
            }
        }

        private async Task OnDateChanged(ChangeEventArgs e, string fieldName)
        {
            try
            {
                var dateStr = e.Value?.ToString();
                if (string.IsNullOrEmpty(dateStr))
                {
                    if (fieldName == "ngay_ghi_nhan")
                        SelectedItem.ngay_ghi_nhan = null;
                    else if (fieldName == "ngay_xu_ly")
                        SelectedItem.ngay_xu_ly = null;
                    else if (fieldName == "fromDate")
                    {
                        _fromDate = null;
                        await LoadData();
                    }
                    else if (fieldName == "toDate")
                    {
                        _toDate = null;
                        await LoadData();
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

                    if (fieldName == "ngay_ghi_nhan")
                        SelectedItem.ngay_ghi_nhan = date;
                    else if (fieldName == "ngay_xu_ly")
                        SelectedItem.ngay_xu_ly = date;
                    else if (fieldName == "fromDate")
                    {
                        _fromDate = date;
                        await LoadData();
                    }
                    else if (fieldName == "toDate")
                    {
                        _toDate = date;
                        await LoadData();
                    }

                }
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi khi xử lý ngày: {ex.Message}", "danger");
            }
        }

        private async Task<IEnumerable<QLCLCoSoCheBienNLTSModel>> LoadQLCLCoSoCheBienNLTSData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, QLCLCoSoCheBienNLTSService, isIgnoreCheck: true);
        }
        private async Task<IEnumerable<QLCLCoSoNLTSDuDieuKienATTPModel>> LoadQLCLCoSoNLTSDuDieuKienATTPData(string searchText)
        {
            string query = $"&filter[_and][][loai][_eq]={1}";
            return await LoadBlazorTypeaheadData(searchText, QLCLCoSoNLTSDuDieuKienATTPService, query, isIgnoreCheck: true);
        }
        private async Task<IEnumerable<QLCLCoSoNLTSDuDieuKienATTPModel>> LoadQLCLCoSoNLTSKhongDuDieuKienATTPData(string searchText)
        {
            string query = $"&filter[_and][][loai][_eq]={2}";
            return await LoadBlazorTypeaheadData(searchText, QLCLCoSoNLTSDuDieuKienATTPService, query, isIgnoreCheck: true);
        }
        private async Task<IEnumerable<QLCLHanhViViPhamModel>> LoadQLCLHanhViViPhamData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, QLCLHanhViViPhamService, isIgnoreCheck: true);
        }
        private async Task<IEnumerable<QLCLHinhThucXuPhatModel>> LoadQLCLHinhThucXuPhatData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, QLCLHinhThucXuPhatService, isIgnoreCheck: true);
        }
        private async Task<IEnumerable<DonViTinhModel>> LoadDonViTinhData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, DonViTinhService, isIgnoreCheck: true);
        }

        public void OnLoaiCoSoChanged(ChangeEventArgs e)
        {
            var value = e.Value?.ToString();
            SelectedItem.loai_co_so = !string.IsNullOrEmpty(value) ? (LoaiCoSoNLTS)Enum.Parse(typeof(LoaiCoSoNLTS), value) : LoaiCoSoNLTS.CoSoCheBien;
            SelectedItem.co_so_che_bien_nlts = null;
            SelectedItem.co_so_nlts_du_dieu_kien_attp = null;
        }
        
        private async Task OnExportExcel()
        {
            // Get all data for export
            BuildPaginationQuery(Page, int.MaxValue);
            int intdex = 1;

            BuilderQuery += "filter[_and][0][deleted][_eq]=false&sort=sort";
            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&filter[_and][{intdex}][_or][0][co_so_che_bien_nlts][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{intdex}][_or][1][co_so_nlts_du_dieu_kien_attp][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{intdex}][_or][2][hanh_vi_vi_pham][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{intdex}][_or][3][huong_xu_ly][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{intdex}][_or][4][description][_contains]={_searchString}";
                intdex++;
            }

            if (_selectedTinhFilter != null)
            {
                BuilderQuery += $"&filter[_and][{intdex}][_or][0][co_so_che_bien_nlts][province][_eq]={_selectedTinhFilter.id}";
                BuilderQuery += $"&filter[_and][{intdex}][_or][1][co_so_nlts_du_dieu_kien_attp][province][_eq]={_selectedTinhFilter.id}";
                intdex++;
            }

            if (_selectedXaFilter != null)
            {
                BuilderQuery += $"&filter[_and][{intdex}][_or][0][co_so_che_bien_nlts][ward][_eq]={_selectedXaFilter.id}";
                BuilderQuery += $"&filter[_and][{intdex}][_or][1][co_so_nlts_du_dieu_kien_attp][ward][_eq]={_selectedXaFilter.id}";
                intdex++;
            }

            if (_fromDate != null)
            {
                BuilderQuery += $"&filter[_and][{intdex}][ngay_ghi_nhan][_gte]={_fromDate.Value.ToString("yyyy-MM-dd")}";
                intdex++;
            }

            if (_toDate != null)
            {
                BuilderQuery += $"&filter[_and][{intdex}][ngay_ghi_nhan][_lte]={_toDate.Value.ToString("yyyy-MM-dd")}";
                intdex++;
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
            ws.Cells[1, 4].Value = "Hành vi vi phạm";
            ws.Cells[1, 5].Value = "Hướng xử lý";
            ws.Cells[1, 6].Value = "Phạt tiền (VNĐ)";
            ws.Cells[1, 7].Value = "Giá trị tang vật (VNĐ)";
            ws.Cells[1, 8].Value = "Ghi chú";
            ws.Cells[1, 9].Value = "Trạng thái";

            // Style header
            using (var range = ws.Cells[1, 1, 1, 9])
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
                ws.Cells[row, 3].Value = (item.loai_co_so == Enums.LoaiCoSoNLTS.CoSoCheBien) ? item.co_so_che_bien_nlts?.name : item.co_so_nlts_du_dieu_kien_attp?.name;
                ws.Cells[row, 4].Value = item.hanh_vi_vi_pham?.name;
                ws.Cells[row, 5].Value = item.huong_xu_ly;
                ws.Cells[row, 6].Value = item.phat_tien;
                ws.Cells[row, 7].Value = item.gia_tri_tang_vat;
                ws.Cells[row, 8].Value = item.description;
                ws.Cells[row, 9].Value = item.status?.GetDescription();

                row++;
                stt++;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            // Export to browser
            var fileName = $"DanhSachCoSoViPhamATTP_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var fileBytes = package.GetAsByteArray();
            // Nếu chưa có hàm saveAsFile trong wwwroot/js, hãy thêm hàm này để hỗ trợ download file từ base64
            await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(fileBytes));
        }
    }

}
