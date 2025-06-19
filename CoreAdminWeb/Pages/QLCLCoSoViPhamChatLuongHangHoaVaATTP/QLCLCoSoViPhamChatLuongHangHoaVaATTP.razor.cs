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

namespace CoreAdminWeb.Pages.QLCLCoSoViPhamChatLuongHangHoaVaATTP
{
    public partial class QLCLCoSoViPhamChatLuongHangHoaVaATTP(IBaseService<QLCLCoSoViPhamATTPModel> MainService,
                                              IBaseService<TinhModel> TinhService,
                                              IBaseService<XaPhuongModel> XaPhuongService) : BlazorCoreBase
    {
        private List<QLCLCoSoViPhamATTPModel> MainModels { get; set; } = new();
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
                await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/assets/js/pages/flatpickr.js");
                StateHasChanged();
            }
        }

        private async Task LoadData()
        {
            BuildPaginationQuery(Page, PageSize, "id", false);
            int index = 3;

            BuilderQuery += "&filter[_and][0][deleted][_eq]=false";
            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&filter[_and][{index}][_or][0][co_so_che_bien_nlts][code][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][co_so_nlts_du_dieu_kien_attp][code][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][2][co_so_che_bien_nlts][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][3][co_so_nlts_du_dieu_kien_attp][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][4][co_so_che_bien_nlts][dia_chi][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][5][co_so_nlts_du_dieu_kien_attp][dia_chi][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][6][san_pham_vi_pham][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][7][hanh_vi_vi_pham][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][8][xu_ly_khac][_contains]={_searchString}";
                index++;
            }


            if(_selectedTinhFilter != null)
            {
                BuilderQuery += $"&filter[_and][{index}][_or][0][co_so_che_bien_nlts][province][_eq]={_selectedTinhFilter.id}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][co_so_nlts_du_dieu_kien_attp][province][_eq]={_selectedTinhFilter.id}";
                index++;
            }

            if(_selectedXaFilter != null)
            {
                BuilderQuery += $"&filter[_and][{index}][_or][0][co_so_che_bien_nlts][ward][_eq]={_selectedXaFilter.id}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][co_so_nlts_du_dieu_kien_attp][ward][_eq]={_selectedXaFilter.id}";
                index++;
            }

            if(_fromDate != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ngay_ghi_nhan][_gte]={_fromDate.Value.ToString("yyyy-MM-dd")}";
                index++;
            }

            if(_toDate != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ngay_ghi_nhan][_lte]={_toDate.Value.ToString("yyyy-MM-dd")}";
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
        }
        private async Task<IEnumerable<TinhModel>> LoadTinhData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, TinhService);
        }
        private async Task<IEnumerable<XaPhuongModel>> LoadXaFilterData(string searchText)
        {
            string query = $"sort=-id";
            query += $"&filter[_and][][ProvinceId][_eq]={(_selectedTinhFilter == null ? 0 : _selectedTinhFilter?.id)}";
            query += $"&filter[_and][][deleted][_eq]=false";
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


        private async Task OnExportExcel()
        {
            // Get all data for export
            string  query  = $"sort=-id";
            
            int index = 3;

            query += "&filter[_and][0][deleted][_eq]=false";

            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&filter[_and][{index}][_or][0][co_so_che_bien_nlts][code][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][co_so_nlts_du_dieu_kien_attp][code][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][2][co_so_che_bien_nlts][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][3][co_so_nlts_du_dieu_kien_attp][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][4][co_so_che_bien_nlts][dia_chi][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][5][co_so_nlts_du_dieu_kien_attp][dia_chi][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][6][san_pham_vi_pham][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][7][hanh_vi_vi_pham][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][8][xu_ly_khac][_contains]={_searchString}";
                index++;
            }

            if(_selectedTinhFilter != null)
            {
                BuilderQuery += $"&filter[_and][{index}][co_so_che_bien_nlts][province][_eq]={_selectedTinhFilter.id}";
                BuilderQuery += $"&filter[_and][{index}][co_so_nlts_du_dieu_kien_attp][province][_eq]={_selectedTinhFilter.id}";
                index++;
            }

            if(_selectedXaFilter != null)
            {
                BuilderQuery += $"&filter[_and][{index}][co_so_che_bien_nlts][ward][_eq]={_selectedXaFilter.id}";
                BuilderQuery += $"&filter[_and][{index}][co_so_nlts_du_dieu_kien_attp][ward][_eq]={_selectedXaFilter.id}";
                index++;
            }

            if(_fromDate != null)
            {
                query += $"&filter[_and][{index}][ngay_ghi_nhan][_gte]={_fromDate.Value.ToString("yyyy-MM-dd")}";
                index++;
            }

            if(_toDate != null)
            {
                query += $"&filter[_and][{index}][ngay_ghi_nhan][_lte]={_toDate.Value.ToString("yyyy-MM-dd")}";
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
            ws.Cells[1, 2].Value = "Mã cơ sở";
            ws.Cells[1, 3].Value = "Tên cơ sở";
            ws.Cells[1, 4].Value = "Ngày vi phạm";
            ws.Cells[1, 5].Value = "Địa chỉ";
            ws.Cells[1, 6].Value = "Sản phẩm";
            ws.Cells[1, 7].Value = "Hành vi vi phạm";
            ws.Cells[1, 8].Value = "Hình thức xử lý";

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
                var maCoSo = (item.loai_co_so == Enums.LoaiCoSoNLTS.CoSoCheBien) ? item.co_so_che_bien_nlts?.code : item.co_so_nlts_du_dieu_kien_attp?.code;
                var tenCoSo = (item.loai_co_so == Enums.LoaiCoSoNLTS.CoSoCheBien) ? item.co_so_che_bien_nlts?.name : item.co_so_nlts_du_dieu_kien_attp?.name;
                var diaChi = (item.loai_co_so == Enums.LoaiCoSoNLTS.CoSoCheBien) ? item.co_so_che_bien_nlts?.dia_chi : item.co_so_nlts_du_dieu_kien_attp?.dia_chi;
                var hinhThucXuLy = item.hinh_thuc_xu_phat != null ? item.hinh_thuc_xu_phat?.name : item.xu_ly_khac?.GetDescription();
                
                ws.Cells[row, 1].Value = stt;
                ws.Cells[row, 2].Value = maCoSo;
                ws.Cells[row, 3].Value = tenCoSo;
                ws.Cells[row, 4].Value = item.ngay_ghi_nhan?.ToString("dd/MM/yyyy");
                ws.Cells[row, 5].Value = diaChi;
                ws.Cells[row, 6].Value = item.san_pham_vi_pham;
                ws.Cells[row, 7].Value = item.hanh_vi_vi_pham?.name;
                ws.Cells[row, 8].Value = hinhThucXuLy;
                row++;
                stt++;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            // Export to browser
            var fileName = $"DanhSachCoSoViPhamChatLuongHangHoaVaATTP_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var fileBytes = package.GetAsByteArray();
            // Nếu chưa có hàm saveAsFile trong wwwroot/js, hãy thêm hàm này để hỗ trợ download file từ base64
            await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(fileBytes));
        }
    }
}
