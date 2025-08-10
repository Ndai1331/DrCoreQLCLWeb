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

namespace CoreAdminWeb.Pages.QLCLCoSoCheBienNLTS
{
    public partial class QLCLCoSoCheBienNLTS(
        IBaseService<QLCLCoSoCheBienNLTSModel> MainService,
        IBaseService<TinhModel> TinhService,
        IBaseService<XaPhuongModel> XaService,
        IBaseService<QLCLLoaiHinhCoSoModel> LoaiHinhCoSoService,
        IBaseService<QLCLNguyenLieuCheBienModel> NguyenLieuCheBienService) : BlazorCoreBase
    {
        private List<QLCLCoSoCheBienNLTSModel> MainModels { get; set; } = new();
        private List<Enums.QuyMoEnum> QuyMoList = new() { Enums.QuyMoEnum.QuyMoNho, Enums.QuyMoEnum.QuyMoVua, Enums.QuyMoEnum.QuyMoLon };
        private List<Enums.PhamViHoatDong> PhamViList = new() { Enums.PhamViHoatDong.ToanQuoc,
         Enums.PhamViHoatDong.ToanTinh, Enums.PhamViHoatDong.KhuVucMienTrung, Enums.PhamViHoatDong.XuatKhau};
        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private string _titleAddOrUpdate = "Thêm mới";
        private string _searchString = "";
        private DateTime? _fromDate = null;
        private DateTime? _toDate = null;

        private List<TinhModel> TinhList { get; set; } = new();
        private List<XaPhuongModel> XaList { get; set; } = new();
        private List<QLCLLoaiHinhCoSoModel> QLCLLoaiHinhCoSoList { get; set; } = new();
        private List<QLCLNguyenLieuCheBienModel> QLCLNguyenLieuCheBienList { get; set; } = new();
        private TinhModel? _selectedTinhFilter = null;
        private XaPhuongModel? _selectedXaFilter = null;


        private QLCLCoSoCheBienNLTSModel SelectedItem { get; set; } = new QLCLCoSoCheBienNLTSModel();

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
                SelectedItem.province = await LoadDefaultData(TinhService);
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
                    XaPhuongItems = await LoadDataInTable(new List<XaPhuongModel>(), "", CancellationToken.None, XaService);
                }
                BuildPaginationQuery(Page, PageSize, "id", false);
                int intdex = 0;

                BuilderQuery += "&filter[_and][0][deleted][_eq]=false";
                if (!string.IsNullOrEmpty(_searchString))
                {
                    intdex++;
                    BuilderQuery += $"&filter[_and][{intdex}][_or][0][so_giay_phep][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{intdex}][_or][1][co_quan_cap_phep][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{intdex}][_or][2][dai_dien][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{intdex}][_or][3][dien_thoai][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{intdex}][_or][4][dia_chi][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{intdex}][_or][5][code][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{intdex}][_or][6][name][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{intdex}][_or][7][chung_nhan_attp][_contains]={_searchString}";
                }
                if (_selectedTinhFilter != null)
                {
                    intdex++;
                    BuilderQuery += $"&filter[_and][{intdex}][province][_eq]={_selectedTinhFilter.id}";
                }
                if (_selectedXaFilter != null)
                {
                    intdex++;
                    BuilderQuery += $"&filter[_and][{intdex}][ward][_eq]={_selectedXaFilter.id}";
                }
                else
                {
                    intdex++;
                    string xaFilterIds = string.Join(",", XaPhuongItems.Select(x => x.id).ToList());
                    BuilderQuery += $"&filter[_and][{intdex}][ward][_in]={xaFilterIds}";
                }
                if (_fromDate != null)
                {
                    intdex++;
                    BuilderQuery += $"&filter[_and][{intdex}][ngay_cap][_gte]={_fromDate?.ToString("yyyy-MM-dd")}";
                }

                if (_toDate != null)
                {
                    intdex++;
                    BuilderQuery += $"&filter[_and][{intdex}][ngay_cap][_lte]={_toDate?.ToString("yyyy-MM-dd")}";
                }

                var result = await MainService.GetAllAsync(BuilderQuery);
                if (result.IsSuccess)
                {
                    MainModels = result.Data ?? new List<QLCLCoSoCheBienNLTSModel>();
                    if (result.Meta != null)
                    {
                        TotalItems = result.Meta.filter_count ?? 0;
                        TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                    }
                }
                else
                {
                    MainModels = new List<QLCLCoSoCheBienNLTSModel>();
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

        private async Task OpenAddOrUpdateModal(QLCLCoSoCheBienNLTSModel? item)
        {
            try
            {
                _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
                SelectedItem = item?.DeepClone() ?? new QLCLCoSoCheBienNLTSModel();

                if (SelectedItem.province == null)
                {
                    SelectedItem.province = await LoadDefaultData(TinhService);
                }

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

        private void OpenDeleteModal(QLCLCoSoCheBienNLTSModel item)
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

        private async Task CloseDeleteModal()
        {
            try
            {
                SelectedItem = new QLCLCoSoCheBienNLTSModel()
                {
                    province = await LoadDefaultData(TinhService),
                };
                openDeleteModal = false;
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi: {ex.Message}", "danger");
            }
        }

        private async Task CloseAddOrUpdateModal()
        {
            try
            {
                SelectedItem = new QLCLCoSoCheBienNLTSModel()
                {
                    province = await LoadDefaultData(TinhService),
                };
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
                if ((resultCreate.IsSuccess && SelectedItem.id == 0) || (resultUpdate.IsSuccess && SelectedItem.id > 0))
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
                if (SelectedItem == null)
                {
                    return;
                }

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

        private async Task OnDateChanged(ChangeEventArgs e, string fieldName, bool isFilter = false)
        {
            try
            {
                var dateStr = e.Value?.ToString();
                if (string.IsNullOrEmpty(dateStr))
                {
                    ReflectionHelper.SetDateFieldValue(this, SelectedItem, fieldName, null);
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
                        ReflectionHelper.SetDateFieldValue(this, SelectedItem, fieldName, date);
                    }
                }

                if (isFilter)
                {
                    await LoadData();
                }
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi khi xử lý ngày: {ex.Message}", "danger");
            }
        }

        private async Task<IEnumerable<TinhModel>> LoadTinhData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, TinhService);
        }

        private async Task<IEnumerable<XaPhuongModel>> LoadXaCRUDData(string searchText)
        {
            string query = $"&filter[_and][][ProvinceId][_eq]={SelectedItem.province?.id ?? 0}";
            return await LoadBlazorTypeaheadData(searchText, XaService, query);
        }

        private async Task<List<XaPhuongModel>> FilterFunctionXaPhuongData(IEnumerable<XaPhuongModel> allItems, string filter,
            CancellationToken token)
        {
            string query = $"sort=-id";
            query += $"&filter[_and][][ProvinceId][_eq]={(_selectedTinhFilter == null ? 0 : _selectedTinhFilter?.id)}";
            XaPhuongItems = await LoadDataInTable(allItems, filter, token, XaService, query);
            StateHasChanged();
            return XaPhuongItems;
        }

        private async Task<IEnumerable<QLCLLoaiHinhCoSoModel>> LoadQLCLLoaiHinhCoSoData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, LoaiHinhCoSoService);
        }


        private async Task<IEnumerable<QLCLNguyenLieuCheBienModel>> LoadQLCLNguyenLieuCheBienData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, NguyenLieuCheBienService);
        }

        public async Task OnTinhFilterChanged(TinhModel? item)
        {
            _selectedTinhFilter = item;
            await LoadData();
        }

        private async Task OnExportExcel()
        {
            if (_selectedXaFilter == null)
            {
                XaPhuongItems = await LoadDataInTable(new List<XaPhuongModel>(), "", CancellationToken.None, XaService);
            }

            // Get all data for export
            BuildPaginationQuery(1, int.MaxValue);
            int intdex = 0;

            BuilderQuery += "&filter[_and][0][deleted][_eq]=false";
            if (!string.IsNullOrEmpty(_searchString))
            {
                intdex++;
                BuilderQuery += $"&filter[_and][{intdex}][_or][0][so_giay_phep][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{intdex}][_or][1][co_quan_cap_phep][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{intdex}][_or][2][dai_dien][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{intdex}][_or][3][dien_thoai][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{intdex}][_or][4][dia_chi][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{intdex}][_or][5][code][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{intdex}][_or][6][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{intdex}][_or][7][chung_nhan_attp][_contains]={_searchString}";
            }
            if (_selectedTinhFilter != null)
            {
                intdex++;
                BuilderQuery += $"&filter[_and][{intdex}][province][_eq]={_selectedTinhFilter.id}";
            }
            if (_selectedXaFilter != null)
            {
                intdex++;
                BuilderQuery += $"&filter[_and][{intdex}][ward][_eq]={_selectedXaFilter.id}";
            }
            else
            {
                intdex++;
                string xaFilterIds = string.Join(",", XaPhuongItems.Select(x => x.id).ToList());
                BuilderQuery += $"&filter[_and][{intdex}][ward][_in]={xaFilterIds}";
            }
            if (_fromDate != null)
            {
                intdex++;
                BuilderQuery += $"&filter[_and][{intdex}][ngay_cap][_gte]={_fromDate?.ToString("yyyy-MM-dd")}";
            }

            if (_toDate != null)
            {
                intdex++;
                BuilderQuery += $"&filter[_and][{intdex}][ngay_cap][_lte]={_toDate?.ToString("yyyy-MM-dd")}";
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
            ws.Cells[1, 2].Value = "MS doanh nghiệp";
            ws.Cells[1, 3].Value = "Tên cơ sở";
            ws.Cells[1, 4].Value = "Loại hình cơ sở";
            ws.Cells[1, 5].Value = "Tên sản phẩm";
            ws.Cells[1, 6].Value = "Sản lượng";
            ws.Cells[1, 7].Value = "Địa chỉ";
            ws.Cells[1, 8].Value = "GCN đủ điều kiện";
            ws.Cells[1, 9].Value = "Ngày cấp";
            ws.Cells[1, 10].Value = "Chứng nhận về ATTP";
            ws.Cells[1, 11].Value = "Trạng thái";
            // Style header
            using (var range = ws.Cells[1, 1, 1, 11])
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
                ws.Cells[row, 2].Value = item.code;
                ws.Cells[row, 3].Value = item.name;
                ws.Cells[row, 4].Value = item.loai_hinh_co_so?.name;
                ws.Cells[row, 5].Value = item.nguyen_lieu_che_bien?.name;
                ws.Cells[row, 6].Value = item.san_luong_du_kien;
                ws.Cells[row, 7].Value = item.dia_chi;
                ws.Cells[row, 8].Value = item.so_giay_phep;
                ws.Cells[row, 9].Value = item.ngay_cap?.ToString("dd/MM/yyyy");
                ws.Cells[row, 10].Value = item.chung_nhan_attp;
                ws.Cells[row, 11].Value = item.status.GetDescription();
                row++;
                stt++;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            // Export to browser
            var fileName = $"DanhSachCoSoCheBienNLTS_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var fileBytes = await package.GetAsByteArrayAsync();
            // Nếu chưa có hàm saveAsFile trong wwwroot/js, hãy thêm hàm này để hỗ trợ download file từ base64
            await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(fileBytes));
        }
    }

}
