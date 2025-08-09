using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Model.Reports;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Services.Reports;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OfficeOpenXml;
using OfficeOpenXml.Style;


namespace CoreAdminWeb.Pages.QLCLBaoCaoDuLieuCapGCNDDKATTP
{
    public partial class QLCLBaoCaoDuLieuCapGCNDDKATTP(IReportService<ReportBaoCaoThamDinhCapGCNModel> MainService,
                                                IReportService<QLCLCoSoNLTSDuDieuKienATTPModel> DetailService,
                                              IBaseService<TinhModel> TinhService,
                                              IBaseService<XaPhuongModel> XaPhuongService) : BlazorCoreBase
    {
        private List<ReportBaoCaoThamDinhCapGCNModel> MainModels { get; set; } = new();
        private List<QLCLCoSoNLTSDuDieuKienATTPModel> DetailModels { get; set; } = new();

        private TinhModel? _selectedTinhFilter { get; set; }
        private XaPhuongModel? _selectedXaFilter { get; set; }
        private DateTime? _fromDate { get; set; } = null;
        private DateTime? _toDate { get; set; } = null;

        private bool openDetailModal { get; set; } = false;

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
            IsLoading = true;
            if (_selectedXaFilter == null)
            {
                XaPhuongItems = await LoadDataInTable(new List<XaPhuongModel>(), "", CancellationToken.None, XaPhuongService);
            }
            BuilderQuery = $"QLCLBaoCaoThamDinhCapGCN?limit={PageSize}&offset={(Page - 1) * PageSize}";

            if (_selectedTinhFilter != null)
            {
                BuilderQuery += $"&province={_selectedTinhFilter.id}";
            }
            if (_selectedXaFilter != null)
            {
                BuilderQuery += $"&wards={_selectedXaFilter.id}";
            }
            else
            {
                string xaFilterIds = string.Join(",", XaPhuongItems.Select(x => x.id).ToList());
                BuilderQuery += $"&wards={xaFilterIds}";
            }
            if (_fromDate != null)
            {
                BuilderQuery += $"&fromDate={_fromDate?.ToString("yyyy-MM-dd")}";
            }

            if (_toDate != null)
            {
                BuilderQuery += $"&toDate={_toDate?.ToString("yyyy-MM-dd")}";
            }

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<ReportBaoCaoThamDinhCapGCNModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<ReportBaoCaoThamDinhCapGCNModel>();
            }
            IsLoading = false;
            StateHasChanged();
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

        private async Task OnTinhFilterChanged(TinhModel? item)
        {
            _selectedTinhFilter = item;
            await LoadData();
        }

        private void CloseDetailModal()
        {
            openDetailModal = false;
        }

        private async Task OnRowClick(string thang)
        {
            if (_selectedXaFilter == null)
            {
                XaPhuongItems = await LoadDataInTable(new List<XaPhuongModel>(), "", CancellationToken.None, XaPhuongService);
            }
            string query = $"QLCLBaoCaoThamDinhCapGCN/coSoKhongCapGCN?thangNam={thang}";
            if (_selectedTinhFilter != null)
            {
                query += $"&province={_selectedTinhFilter.id}";
            }
            if (_selectedXaFilter != null)
            {
                query += $"&wards={_selectedXaFilter.id}";
            }
            else
            {
                string xaFilterIds = string.Join(",", XaPhuongItems.Select(x => x.id).ToList());
                BuilderQuery += $"&wards={xaFilterIds}";
            }
            var result = await DetailService.GetAllAsync(query);
            if (result.IsSuccess)
            {
                DetailModels = result.Data ?? new List<QLCLCoSoNLTSDuDieuKienATTPModel>();
            }
            openDetailModal = true;
        }

        private async Task OnExportExcel()
        {
            // Get all data for export
            if (_selectedXaFilter == null)
            {
                XaPhuongItems = await LoadDataInTable(new List<XaPhuongModel>(), "", CancellationToken.None, XaPhuongService);
            }
            BuilderQuery = $"QLCLBaoCaoThamDinhCapGCN?";

            if (_selectedTinhFilter != null)
            {
                BuilderQuery += $"&province={_selectedTinhFilter.id}";
            }
            if (_selectedXaFilter != null)
            {
                BuilderQuery += $"&wards={_selectedXaFilter.id}";
            }
            else
            {
                string xaFilterIds = string.Join(",", XaPhuongItems.Select(x => x.id).ToList());
                BuilderQuery += $"&wards={xaFilterIds}";
            }
            if (_fromDate != null)
            {
                BuilderQuery += $"&fromDate={_fromDate?.ToString("yyyy-MM-dd")}";
            }

            if (_toDate != null)
            {
                BuilderQuery += $"&toDate={_toDate?.ToString("yyyy-MM-dd")}";
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
            ws.Cells[1, 2].Value = "Tháng";
            ws.Cells[1, 3].Value = "Tổng số cơ sở được thẩm định";
            ws.Cells[1, 4].Value = "Số cơ sở đạt";
            ws.Cells[1, 5].Value = "Số cơ sở không đạt";
            ws.Cells[1, 6].Value = "Số cơ sở được cấp GCN";
            ws.Cells[1, 7].Value = "Tỷ lệ cơ sở được cấp GCN";

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
                ws.Cells[row, 2].Value = item.thang;
                ws.Cells[row, 3].Value = item.tong_co_so_tham_dinh;
                ws.Cells[row, 4].Value = item.so_dat;
                ws.Cells[row, 5].Value = item.so_khong_dat;
                ws.Cells[row, 6].Value = item.so_co_so_duoc_cap_gcn;
                ws.Cells[row, 7].Value = item.ty_le_co_so_duoc_cap_gcn;
                row++;
                stt++;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            // Export to browser
            var fileName = $"BaoCaoThamDinhCapGCN_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var fileBytes = await package.GetAsByteArrayAsync();
            // Nếu chưa có hàm saveAsFile trong wwwroot/js, hãy thêm hàm này để hỗ trợ download file từ base64
            await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(fileBytes));
        }
    }
}
