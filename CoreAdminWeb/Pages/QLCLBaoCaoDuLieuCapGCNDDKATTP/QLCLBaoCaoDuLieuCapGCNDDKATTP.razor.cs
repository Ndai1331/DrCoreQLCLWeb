using CoreAdminWeb.Model;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using CoreAdminWeb.Services.Reports;
using CoreAdminWeb.Model.Reports;
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
      
        private string _searchString = "";
        private TinhModel? _selectedTinhFilter { get; set; }
        private XaPhuongModel? _selectedXaFilter { get; set; }
        private DateTime? _fromDate { get; set; }
        private DateTime? _toDate { get; set; }

        private bool openDetailModal { get; set; } = false;

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
            BuilderQuery = $"QLCLBaoCaoThamDinhCapGCN?limit={PageSize}&offset={(Page - 1) * PageSize}";
           
            if(_selectedTinhFilter != null)
            {
                BuilderQuery += $"&province={_selectedTinhFilter.id}";
            }
            if(_selectedXaFilter != null)
            {
                BuilderQuery += $"&ward={_selectedXaFilter.id}";
            }
            if(_fromDate != null)
            {
                BuilderQuery += $"&fromDate={_fromDate.Value.ToString("yyyy-MM-dd")}";
            }

            if(_toDate != null)
            {
                BuilderQuery += $"&toDate={_toDate.Value.ToString("yyyy-MM-dd")}";
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
        }


        private async Task<IEnumerable<TinhModel>> LoadTinhData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, TinhService);
        }


        private async Task<IEnumerable<XaPhuongModel>> LoadXaFilterData(string searchText)
        {
            string query = $"sort=-id";
            query += $"&filter[_and][][ProvinceId][_eq]={(_selectedTinhFilter == null ? 0 : _selectedTinhFilter?.id)}";
            return await LoadBlazorTypeaheadData(searchText, XaPhuongService,query);
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

        private async Task OnTinhFilterChanged(TinhModel? item)
        {
            _selectedTinhFilter = item;
            await LoadData();
        }

        private async Task OnXaFilterChanged(XaPhuongModel? item)
        {
            _selectedXaFilter = item;
            await LoadData();
        }

        private void CloseDetailModal()
        {
            openDetailModal = false;
        }

        private async Task OnRowClick(string thang)
        {
            string query = $"QLCLBaoCaoThamDinhCapGCN/coSoKhongCapGCN?thangNam={thang}";
            if(_selectedTinhFilter != null)
            {
                query += $"&province={_selectedTinhFilter.id}";
            }
            if(_selectedXaFilter != null)
            {
                query += $"&ward={_selectedXaFilter.id}";
            }
            var result = await DetailService.GetAllAsync(query);
            if (result.IsSuccess)
            {
                DetailModels = result.Data;
            }
            openDetailModal = true;
        }

        private async Task OnExportExcel()
        {
            // Get all data for export
            BuilderQuery = $"QLCLBaoCaoThamDinhCapGCN?";
           
            if(_selectedTinhFilter != null)
            {
                BuilderQuery += $"&province={_selectedTinhFilter.id}";
            }
            if(_selectedXaFilter != null)
            {
                BuilderQuery += $"&ward={_selectedXaFilter.id}";
            }
            if(_fromDate != null)
            {
                BuilderQuery += $"&fromDate={_fromDate.Value.ToString("yyyy-MM-dd")}";
            }

            if(_toDate != null)
            {
                BuilderQuery += $"&toDate={_toDate.Value.ToString("yyyy-MM-dd")}";
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
            var fileBytes = package.GetAsByteArray();
            // Nếu chưa có hàm saveAsFile trong wwwroot/js, hãy thêm hàm này để hỗ trợ download file từ base64
            await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(fileBytes));
        }
    }
}
