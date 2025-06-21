using CoreAdminWeb.Model;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CoreAdminWeb.Pages.QLCLBaoCaoDuLieuHoTroXucTienThuongMai
{
    public partial class QLCLBaoCaoDuLieuHoTroXucTienThuongMai(
        IBaseService<QLCLKenhQuangBaXucTienThuongMaiModel> MainService,
        IBaseService<TinhModel> TinhService,
        IBaseService<XaPhuongModel> XaPhuongService) : BlazorCoreBase
    {
        private List<QLCLKenhQuangBaXucTienThuongMaiModel> MainModels { get; set; } = new();
        private string _searchString = "";
        private TinhModel? _selectedTinhFilter { get; set; }
        private XaPhuongModel? _selectedXaFilter { get; set; }
        private DateTime? _fromDate = null;
        private DateTime? _toDate = null;


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
                int index = 1;

                BuilderQuery += "filter[_and][0][deleted][_eq]=false&sort=sort";
                if (!string.IsNullOrEmpty(_searchString))
                {
                    BuilderQuery += $"&filter[_and][{index}][_or][0][name][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{index}][_or][1][dia_diem_to_chuc][_contains]={_searchString}";
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
                    BuilderQuery += $"&filter[_and][{index}][ngay_to_chuc][_gte]={_fromDate.Value.ToString("yyyy-MM-dd")}";
                    index++;
                }

                if (_toDate != null)
                {
                    BuilderQuery += $"&filter[_and][{index}][ngay_to_chuc][_lte]={_toDate.Value.ToString("yyyy-MM-dd")}";
                    index++;
                }

                var result = await MainService.GetAllAsync(BuilderQuery);
                if (result.IsSuccess)
                {
                    MainModels = result.Data ?? new List<QLCLKenhQuangBaXucTienThuongMaiModel>();
                    if (result.Meta != null)
                    {
                        TotalItems = result.Meta.filter_count ?? 0;
                        TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                    }
                }
                else
                {
                    MainModels = new List<QLCLKenhQuangBaXucTienThuongMaiModel>();
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

        private async Task OnDateChanged(ChangeEventArgs e, string fieldName)
        {
            try
            {
                var dateStr = e.Value?.ToString();
                if (string.IsNullOrEmpty(dateStr))
                {
                    if (fieldName == "fromDate")
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

                    if (fieldName == "fromDate")
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
        
        private async Task OnExportExcel()
        {
            // Get all data for export
            string  query  = $"sort=-id";
            
            int index = 1;

            query += "&filter[_and][0][deleted][_eq]=false";

            if (!string.IsNullOrEmpty(_searchString))
            {
                query += $"&filter[_and][{index}][_or][0][name][_contains]={_searchString}";
                query += $"&filter[_and][{index}][_or][1][dia_diem_to_chuc][_contains]={_searchString}";
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
                BuilderQuery += $"&filter[_and][{index}][ngay_to_chuc][_gte]={_fromDate.Value.ToString("yyyy-MM-dd")}";
                index++;
            }

            if (_toDate != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ngay_to_chuc][_lte]={_toDate.Value.ToString("yyyy-MM-dd")}";
                index++;
            }


            var result = await MainService.GetAllAsync(query);
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
            ws.Cells[1, 2].Value = "Tên chương trình";
            ws.Cells[1, 3].Value = "Địa điểm";
            ws.Cells[1, 4].Value = "Số lượng chủ thể tham gia";
            ws.Cells[1, 5].Value = "Lượt khách tham quan";
            ws.Cells[1, 6].Value = "Doanh thu (VNĐ)";
            ws.Cells[1, 7].Value = "Số HĐ ký kết";

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
                ws.Cells[row, 3].Value = item.dia_diem_to_chuc;
                ws.Cells[row, 4].Value = item.so_luong_chu_the_tham_gia;
                ws.Cells[row, 5].Value = item.luot_khach_tham_quan;
                ws.Cells[row, 6].Value = item.gia_tri_giao_dich;
                ws.Cells[row, 7].Value = item.so_hop_dong_ky_ket;
                row++;
                stt++;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            // Export to browser
            var fileName = $"BaoCaoDuLieuHoTroXucTienThuongMai_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var fileBytes = package.GetAsByteArray();
            // Nếu chưa có hàm saveAsFile trong wwwroot/js, hãy thêm hàm này để hỗ trợ download file từ base64
            await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(fileBytes));
        }
    }
}
