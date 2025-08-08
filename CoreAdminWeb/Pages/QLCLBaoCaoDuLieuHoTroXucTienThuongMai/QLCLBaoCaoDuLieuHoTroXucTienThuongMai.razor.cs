using CoreAdminWeb.Helpers;
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

        private Dictionary<int, List<XaPhuongModel>> SelectedXaPhuongItems { get; set; } = new();
        private List<XaPhuongModel> XaPhuongItems { get; set; } = new();

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
            try
            {
                IsLoading = true;
                if (_selectedXaFilter == null)
                {
                    XaPhuongItems = await LoadDataInTable(new List<XaPhuongModel>(), "", CancellationToken.None, XaPhuongService);
                }

                BuildPaginationQuery(Page, PageSize);
                int index = 0;

                BuilderQuery += "&filter[_and][0][deleted][_eq]=false&sort=sort";
                if (!string.IsNullOrEmpty(_searchString))
                {
                    index++;
                    BuilderQuery += $"&filter[_and][{index}][_or][0][name][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{index}][_or][1][dia_diem_to_chuc][_contains]={_searchString}";
                }

                if (_selectedTinhFilter != null)
                {
                    index++;
                    BuilderQuery += $"&filter[_and][{index}][province][_eq]={_selectedTinhFilter.id}";
                }

                if (_selectedXaFilter != null)
                {
                    index++;
                    BuilderQuery += $"&filter[_and][{index}][ward][_eq]={_selectedXaFilter.id}";
                }
                else
                {
                    index++;
                    string xaFilterIds = string.Join(",", XaPhuongItems.Select(x => x.id).ToList());
                    BuilderQuery += $"&filter[_and][{index}][ward][_in]={xaFilterIds}";
                }

                if (_fromDate != null)
                {
                    index++;
                    BuilderQuery += $"&filter[_and][{index}][ngay_to_chuc][_gte]={_fromDate?.ToString("yyyy-MM-dd")}";
                }

                if (_toDate != null)
                {
                    index++;
                    BuilderQuery += $"&filter[_and][{index}][ngay_to_chuc][_lte]={_toDate?.ToString("yyyy-MM-dd")}";
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
                StateHasChanged();
            }
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

        private async Task OnDateChanged(ChangeEventArgs e, string fieldName)
        {
            try
            {
                var dateStr = e.Value?.ToString();
                if (string.IsNullOrEmpty(dateStr))
                {
                    ReflectionHelper.SetDateFieldValue(this, fieldName, null);
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
                        ReflectionHelper.SetDateFieldValue(this, fieldName, date);
                    }
                }
                await LoadData();
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi khi xử lý ngày: {ex.Message}", "danger");
            }
        }

        public async Task OnTinhFilterChanged(TinhModel? tinh)
        {
            _selectedTinhFilter = tinh;
            _selectedXaFilter = null;
            await LoadData();
        }

        private async Task OnExportExcel()
        {
            if (_selectedXaFilter == null)
            {
                XaPhuongItems = await LoadDataInTable(new List<XaPhuongModel>(), "", CancellationToken.None, XaPhuongService);
            }

            // Get all data for export
            BuildPaginationQuery(Page, int.MaxValue);
            int index = 0;

            BuilderQuery += "&filter[_and][0][deleted][_eq]=false&sort=sort";
            if (!string.IsNullOrEmpty(_searchString))
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][_or][0][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][dia_diem_to_chuc][_contains]={_searchString}";
            }

            if (_selectedTinhFilter != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][province][_eq]={_selectedTinhFilter.id}";
            }

            if (_selectedXaFilter != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][ward][_eq]={_selectedXaFilter.id}";
            }
            else
            {
                index++;
                string xaFilterIds = string.Join(",", XaPhuongItems.Select(x => x.id).ToList());
                BuilderQuery += $"&filter[_and][{index}][ward][_in]={xaFilterIds}";
            }

            if (_fromDate != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][ngay_to_chuc][_gte]={_fromDate?.ToString("yyyy-MM-dd")}";
            }

            if (_toDate != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][ngay_to_chuc][_lte]={_toDate?.ToString("yyyy-MM-dd")}";
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
            var fileBytes = await package.GetAsByteArrayAsync();
            // Nếu chưa có hàm saveAsFile trong wwwroot/js, hãy thêm hàm này để hỗ trợ download file từ base64
            await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(fileBytes));
        }
    }
}
