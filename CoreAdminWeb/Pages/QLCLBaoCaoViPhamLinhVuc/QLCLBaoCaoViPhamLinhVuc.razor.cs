using CoreAdminWeb.Extensions;
using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CoreAdminWeb.Pages.QLCLBaoCaoViPhamLinhVuc
{
    public partial class QLCLBaoCaoViPhamLinhVuc(IBaseService<QLCLCoSoViPhamATTPModel> MainService,
                                              IBaseService<TinhModel> TinhService,
                                              IBaseService<XaPhuongModel> XaPhuongService) : BlazorCoreBase
    {
        private List<QLCLCoSoViPhamATTPModel> MainModels { get; set; } = new();
        private string _searchString = "";
        private TinhModel? _selectedTinhFilter { get; set; }
        private XaPhuongModel? _selectedXaFilter { get; set; }
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
            BuildPaginationQuery(Page, PageSize, "id", false);
            int index = 0;

            BuilderQuery += "&filter[_and][0][deleted][_eq]=false";
            if (!string.IsNullOrEmpty(_searchString))
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][_or][0][co_so_che_bien_nlts][code][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][co_so_nlts_du_dieu_kien_attp][code][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][2][co_so_che_bien_nlts][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][3][co_so_nlts_du_dieu_kien_attp][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][4][co_so_che_bien_nlts][dia_chi][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][5][co_so_nlts_du_dieu_kien_attp][dia_chi][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][6][san_pham_vi_pham][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][7][hanh_vi_vi_pham][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][8][xu_ly_khac][_contains]={_searchString}";
            }

            if (_selectedTinhFilter != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][_or][0][co_so_che_bien_nlts][province][_eq]={_selectedTinhFilter.id}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][co_so_nlts_du_dieu_kien_attp][province][_eq]={_selectedTinhFilter.id}";
            }

            if (_selectedXaFilter != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][_or][0][co_so_che_bien_nlts][ward][_eq]={_selectedXaFilter.id}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][co_so_nlts_du_dieu_kien_attp][ward][_eq]={_selectedXaFilter.id}";
            }
            else
            {
                index++;
                string xaFilterIds = string.Join(",", XaPhuongItems.Select(x => x.id).ToList());
                BuilderQuery += $"&filter[_and][{index}][_or][0][co_so_che_bien_nlts][ward][_in]={xaFilterIds}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][co_so_nlts_du_dieu_kien_attp][ward][_in]={xaFilterIds}";
            }

            if (_fromDate != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][ngay_ghi_nhan][_gte]={_fromDate?.ToString("yyyy-MM-dd")}";
            }

            if (_toDate != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][ngay_ghi_nhan][_lte]={_toDate?.ToString("yyyy-MM-dd")}";
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

        private async Task OnTinhFilterChanged(TinhModel? item)
        {
            _selectedTinhFilter = item;
            await LoadData();
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
            if (_selectedXaFilter == null)
            {
                XaPhuongItems = await LoadDataInTable(new List<XaPhuongModel>(), "", CancellationToken.None, XaPhuongService);
            }
            // Get all data for export
            string query = $"sort=-id";
            int index = 0;

            query += "&filter[_and][0][deleted][_eq]=false";
            if (!string.IsNullOrEmpty(_searchString))
            {
                index++;
                query += $"&filter[_and][{index}][_or][0][co_so_che_bien_nlts][code][_contains]={_searchString}";
                query += $"&filter[_and][{index}][_or][1][co_so_nlts_du_dieu_kien_attp][code][_contains]={_searchString}";
                query += $"&filter[_and][{index}][_or][2][co_so_che_bien_nlts][name][_contains]={_searchString}";
                query += $"&filter[_and][{index}][_or][3][co_so_nlts_du_dieu_kien_attp][name][_contains]={_searchString}";
                query += $"&filter[_and][{index}][_or][4][co_so_che_bien_nlts][dia_chi][_contains]={_searchString}";
                query += $"&filter[_and][{index}][_or][5][co_so_nlts_du_dieu_kien_attp][dia_chi][_contains]={_searchString}";
                query += $"&filter[_and][{index}][_or][6][san_pham_vi_pham][_contains]={_searchString}";
                query += $"&filter[_and][{index}][_or][7][hanh_vi_vi_pham][name][_contains]={_searchString}";
                query += $"&filter[_and][{index}][_or][8][xu_ly_khac][_contains]={_searchString}";
            }


            if (_selectedTinhFilter != null)
            {
                index++;
                query += $"&filter[_and][{index}][_or][0][co_so_che_bien_nlts][province][_eq]={_selectedTinhFilter.id}";
                query += $"&filter[_and][{index}][_or][1][co_so_nlts_du_dieu_kien_attp][province][_eq]={_selectedTinhFilter.id}";
            }

            if (_selectedXaFilter != null)
            {
                index++;
                query += $"&filter[_and][{index}][_or][0][co_so_che_bien_nlts][ward][_eq]={_selectedXaFilter.id}";
                query += $"&filter[_and][{index}][_or][1][co_so_nlts_du_dieu_kien_attp][ward][_eq]={_selectedXaFilter.id}";
            }
            else
            {
                index++;
                string xaFilterIds = string.Join(",", XaPhuongItems.Select(x => x.id).ToList());
                query += $"&filter[_and][{index}][_or][0][co_so_che_bien_nlts][ward][_in]={xaFilterIds}";
                query += $"&filter[_and][{index}][_or][1][co_so_nlts_du_dieu_kien_attp][ward][_in]={xaFilterIds}";
            }

            if (_fromDate != null)
            {
                index++;
                query += $"&filter[_and][{index}][ngay_ghi_nhan][_gte]={_fromDate?.ToString("yyyy-MM-dd")}";
            }

            if (_toDate != null)
            {
                index++;
                query += $"&filter[_and][{index}][ngay_ghi_nhan][_lte]={_toDate?.ToString("yyyy-MM-dd")}";
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
            ws.Cells[1, 2].Value = "Ngày ghi nhận";
            ws.Cells[1, 3].Value = "Tên cơ sở";
            ws.Cells[1, 4].Value = "Loại cơ sở";
            ws.Cells[1, 5].Value = "Hành vi vi phạm";
            ws.Cells[1, 6].Value = "Hình thức xử phạt";
            ws.Cells[1, 7].Value = "Ngày xử lý";

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
                var tenCoSo = (item.loai_co_so == Enums.LoaiCoSoNLTS.CoSoCheBien) ? item.co_so_che_bien_nlts?.name : item.co_so_nlts_du_dieu_kien_attp?.name;

                ws.Cells[row, 1].Value = stt;
                ws.Cells[row, 2].Value = item.ngay_ghi_nhan?.ToString("dd/MM/yyyy") ?? string.Empty;
                ws.Cells[row, 3].Value = tenCoSo ?? string.Empty;
                ws.Cells[row, 4].Value = item.loai_co_so?.GetDescription() ?? string.Empty;
                ws.Cells[row, 5].Value = item.hanh_vi_vi_pham?.name ?? string.Empty;
                ws.Cells[row, 6].Value = item.hinh_thuc_xu_phat?.name ?? string.Empty;
                ws.Cells[row, 7].Value = item.ngay_xu_ly?.ToString("dd/MM/yyyy") ?? string.Empty;
                row++;
                stt++;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            // Export to browser
            var fileName = $"BaoCaoDuLieuViPham_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var fileBytes = await package.GetAsByteArrayAsync();
            // Nếu chưa có hàm saveAsFile trong wwwroot/js, hãy thêm hàm này để hỗ trợ download file từ base64
            await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(fileBytes));
        }
    }
}
