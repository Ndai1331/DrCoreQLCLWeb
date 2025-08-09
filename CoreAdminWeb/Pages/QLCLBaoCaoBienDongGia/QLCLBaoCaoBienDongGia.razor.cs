using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Services.Reports;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OfficeOpenXml;
using OfficeOpenXml.Style;


namespace CoreAdminWeb.Pages.QLCLBaoCaoBienDongGia
{
    public partial class QLCLBaoCaoBienDongGia(IReportService<QLCLBienDongGiaModel> MainService, IBaseService<XaPhuongModel> XaPhuongService) : BlazorCoreBase
    {
        private List<QLCLBienDongGiaModel> MainModels { get; set; } = new();

        private string _searchString = "";
        private DateTime? _fromDate { get; set; } = default;
        private DateTime? _toDate { get; set; } = default;


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
            XaPhuongItems = await LoadDataInTable(new List<XaPhuongModel>(), "", CancellationToken.None, XaPhuongService);
            string xaFilterIds = string.Join(",", XaPhuongItems.Select(x => x.id).ToList());

            BuilderQuery = $"QLCLBaoCaoBienDongGia?limit={PageSize}&offset={(Page - 1) * PageSize}";
            BuilderQuery += $"&wards={xaFilterIds}";

            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&tenSanPham={_searchString}";
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
                MainModels = result.Data ?? new List<QLCLBienDongGiaModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<QLCLBienDongGiaModel>();
            }
            IsLoading = false;
            StateHasChanged();
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

        private async Task OnExportExcel()
        {
            XaPhuongItems = await LoadDataInTable(new List<XaPhuongModel>(), "", CancellationToken.None, XaPhuongService);
            string xaFilterIds = string.Join(",", XaPhuongItems.Select(x => x.id).ToList());

            // Get all data for export
            BuilderQuery = $"QLCLBaoCaoBienDongGia?limit={int.MaxValue}&offset=0";
            BuilderQuery += $"&wards={xaFilterIds}";

            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&stringSearch={_searchString}";
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
            ws.Cells[1, 2].Value = "Ngày ghi nhận";
            ws.Cells[1, 3].Value = "Loại";
            ws.Cells[1, 4].Value = "Tên sản phẩm";
            ws.Cells[1, 5].Value = "Nhà cung cấp";
            ws.Cells[1, 6].Value = "Địa điểm";
            ws.Cells[1, 7].Value = "Đơn vị tính";
            ws.Cells[1, 8].Value = "Giá mua vào (VNĐ)";
            ws.Cells[1, 9].Value = "Giá bán ra (VNĐ)";
            ws.Cells[1, 10].Value = "Biến động giá (%)";

            // Style header
            using (var range = ws.Cells[1, 1, 1, 10])
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
                ws.Cells[row, 3].Value = item.loai;
                ws.Cells[row, 4].Value = item.ten_san_pham;
                ws.Cells[row, 5].Value = item.nha_cung_cap;
                ws.Cells[row, 6].Value = item.dia_diem;
                ws.Cells[row, 7].Value = item.don_vi_tinh;
                ws.Cells[row, 8].Value = item.gia_mua_vao;
                ws.Cells[row, 9].Value = item.gia_ban_ra;
                ws.Cells[row, 10].Value = item.bien_dong;
                row++;
                stt++;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            // Export to browser
            var fileName = $"BaoCaoBienDongGia_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var fileBytes = await package.GetAsByteArrayAsync();
            // Nếu chưa có hàm saveAsFile trong wwwroot/js, hãy thêm hàm này để hỗ trợ download file từ base64
            await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(fileBytes));
        }
    }
}
