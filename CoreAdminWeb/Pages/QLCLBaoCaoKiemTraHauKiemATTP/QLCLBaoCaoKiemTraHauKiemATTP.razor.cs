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


namespace CoreAdminWeb.Pages.QLCLBaoCaoKiemTraHauKiemATTP
{
    public partial class QLCLBaoCaoKiemTraHauKiemATTP(IReportService<ReportBaoCaoKiemTraHauKiemATTPModel> MainService,
                                                IReportService<QLCLCoSoNLTSDuDieuKienATTPModel> DetailService,
                                              IBaseService<TinhModel> TinhService,
                                              IBaseService<XaPhuongModel> XaPhuongService) : BlazorCoreBase
    {
        private List<ReportBaoCaoKiemTraHauKiemATTPModel> MainModels { get; set; } = new();
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
            IsLoading = true;
            BuilderQuery = $"QLCLBaoCaoKiemTraHauKiemATTP?limit={PageSize}&offset={(Page - 1) * PageSize}";
           
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
                MainModels = result.Data ?? new List<ReportBaoCaoKiemTraHauKiemATTPModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<ReportBaoCaoKiemTraHauKiemATTPModel>();
            }
            IsLoading = false;
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
            string query = $"QLCLBaoCaoKiemTraHauKiemATTP/detail?thangNam={thang}";
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
            BuilderQuery = $"QLCLBaoCaoKiemTraHauKiemATTP?";
           
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
            ws.Cells[1, 3].Value = "Tổng số đợt kiểm tra";
            ws.Cells[1, 4].Value = "Tổng số cơ sở kiểm tra";
            ws.Cells[1, 5].Value = "Số cơ sở vi phạm";
            ws.Cells[1, 6].Value = "Số cơ sở chấp hành";
            ws.Cells[1, 7].Value = "Số cơ sở đạt";
            ws.Cells[1, 8].Value = "Số cơ sở không đạt";

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
                ws.Cells[row, 3].Value = item.tong_dot_kiem_tra;
                ws.Cells[row, 4].Value = item.tong_co_so_kiem_tra;
                ws.Cells[row, 5].Value = item.so_vi_pham;
                ws.Cells[row, 6].Value = item.so_chap_hanh;
                ws.Cells[row, 7].Value = item.so_dat;
                ws.Cells[row, 8].Value = item.so_khong_dat;
                row++;
                stt++;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            // Export to browser
            var fileName = $"BaoCaoKiemTraHauKiemATTP_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var fileBytes = package.GetAsByteArray();
            // Nếu chưa có hàm saveAsFile trong wwwroot/js, hãy thêm hàm này để hỗ trợ download file từ base64
            await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(fileBytes));
        }
    }
}
