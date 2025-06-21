using CoreAdminWeb.Model;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CoreAdminWeb.Pages.QLCLCoSoNLTSDuDieuKienATTPHetHan
{
    public partial class QLCLCoSoNLTSDuDieuKienATTPHetHan(IBaseService<QLCLCoSoNLTSDuDieuKienATTPModel> MainService,
                                              IQLCLCoSoNLTSDuDieuKienATTPSanPhamService QLCLCoSoNLTSDuDieuKienATTPSanPhamService,
                                              IBaseService<TinhModel> TinhService,
                                              IBaseService<XaPhuongModel> XaPhuongService,
                                              IBaseService<QLCLLoaiHinhKinhDoanhModel> LoaiHinhKinhDoanhService,
                                              IBaseService<QLCLSanPhamSanXuatModel> SanPhamService) : BlazorCoreBase
    {
        private List<QLCLCoSoNLTSDuDieuKienATTPModel> MainModels { get; set; } = new();
        private string _searchString = "";
        private TinhModel? _selectedTinhFilter { get; set; }
        private XaPhuongModel? _selectedXaFilter { get; set; }
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
            BuildPaginationQuery(Page, PageSize, "id", false);
            int index = 3;

            BuilderQuery += "&filter[_and][0][deleted][_eq]=false";
            BuilderQuery += "&filter[_and][1][loai][_eq]=1";
            BuilderQuery += "&filter[_and][2][ngay_het_hieu_luc][_lte]=" + DateTime.Now.ToString("yyyy-MM-dd");
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


            if (_selectedTinhFilter != null)
            {
                BuilderQuery += $"&filter[_and][{index}][province][_eq]={_selectedTinhFilter.id}";
                index++;
            }

            if (_selectedXaFilter != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ward][_eq]={_selectedXaFilter.id}";
                index++;
            }

            if (_fromDate != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ngay_het_hieu_luc][_gte]={_fromDate.Value.ToString("yyyy-MM-dd")}";
                index++;
            }

            if (_toDate != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ngay_het_hieu_luc][_lte]={_toDate.Value.ToString("yyyy-MM-dd")}";
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

        private async Task OnDateChanged(ChangeEventArgs e, string fieldName)
        {
            try
            {
                var dateStr = e.Value?.ToString();
                if (string.IsNullOrEmpty(dateStr))
                {
                    switch (fieldName)
                    {
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


        private async Task OnExportExcel()
        {
            // Get all data for export
            string query = $"sort=-id";

            int index = 3;

            query += "&filter[_and][0][deleted][_eq]=false";
            query += "&filter[_and][1][loai][_eq]=1";
            query += "&filter[_and][2][ngay_het_hieu_luc][_lte]=" + DateTime.Now.ToString("yyyy-MM-dd");

            if (!string.IsNullOrEmpty(_searchString))
            {
                query += $"&filter[_and][{index}][_or][0][code][_contains]={_searchString}";
                query += $"&filter[_and][{index}][_or][1][name][_contains]={_searchString}";
                query += $"&filter[_and][{index}][_or][2][dia_chi][_contains]={_searchString}";
                query += $"&filter[_and][{index}][_or][3][loai_hinh_kinh_doanh][name][_contains]={_searchString}";
                query += $"&filter[_and][{index}][_or][4][so_giay_chung_nhan][_contains]={_searchString}";
                query += $"&filter[_and][{index}][_or][5][co_quan_cap][_contains]={_searchString}";
                query += $"&filter[_and][{index}][_or][6][xu_ly_ket_qua][_contains]={_searchString}";
                query += $"&filter[_and][{index}][_or][7][he_thong_quan_ly_chat_luong][_contains]={_searchString}";
                index++;
            }


            if (_selectedTinhFilter != null)
            {
                query += $"&filter[_and][{index}][province][_eq]={_selectedTinhFilter.id}";
                index++;
            }

            if (_selectedXaFilter != null)
            {
                query += $"&filter[_and][{index}][ward][_eq]={_selectedXaFilter.id}";
                index++;
            }

            if (_fromDate != null)
            {
                query += $"&filter[_and][{index}][ngay_het_hieu_luc][_gte]={_fromDate.Value.ToString("yyyy-MM-dd")}";
                index++;
            }

            if (_toDate != null)
            {
                query += $"&filter[_and][{index}][ngay_het_hieu_luc][_lte]={_toDate.Value.ToString("yyyy-MM-dd")}";
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
            ws.Cells[1, 2].Value = "Tên cơ sở";
            ws.Cells[1, 3].Value = "Loại hình kinh doanh";
            ws.Cells[1, 4].Value = "Số GCN";
            ws.Cells[1, 5].Value = "Ngày cấp";
            ws.Cells[1, 6].Value = "Ngày hết hạn";
            ws.Cells[1, 7].Value = "Cơ quan cấp";
            ws.Cells[1, 8].Value = "Sản phẩm";

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
                ws.Cells[row, 2].Value = item.name;
                ws.Cells[row, 3].Value = item.loai_hinh_kinh_doanh?.name;
                ws.Cells[row, 4].Value = item.so_giay_chung_nhan;
                ws.Cells[row, 5].Value = item.ngay_cap?.ToString("dd/MM/yyyy");
                ws.Cells[row, 6].Value = item.ngay_het_hieu_luc?.ToString("dd/MM/yyyy");
                ws.Cells[row, 7].Value = item.co_quan_cap;
                ws.Cells[row, 8].Value = item.chi_tiets?.Count > 0 ? string.Join(", ", item.chi_tiets?.Select(c => c.san_pham?.name)) : "";
                row++;
                stt++;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            // Export to browser
            var fileName = $"DanhSachCoSoNLTSDuDieuKienATTPHetHan_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var fileBytes = package.GetAsByteArray();
            // Nếu chưa có hàm saveAsFile trong wwwroot/js, hãy thêm hàm này để hỗ trợ download file từ base64
            await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(fileBytes));
        }
    }
}
