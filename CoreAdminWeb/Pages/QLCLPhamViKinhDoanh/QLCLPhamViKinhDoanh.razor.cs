using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OfficeOpenXml;
using CoreAdminWeb.Extensions;
using OfficeOpenXml.Style;

namespace CoreAdminWeb.Pages.QLCLPhamViKinhDoanh
{
    public partial class QLCLPhamViKinhDoanh(
        IBaseService<QLCLPhamViHoatDongModel> MainService,
        IBaseService<TinhModel> TinhService,
        IBaseService<XaPhuongModel> XaService) : BlazorCoreBase
    {
        private List<QLCLPhamViHoatDongModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private string _titleAddOrUpdate = "Thêm mới";
        private string _searchString = "";
        private string _searchStatusString = "";
        private List<TinhModel> TinhList { get; set; } = new();
        private List<XaPhuongModel> XaList { get; set; } = new();

        private TinhModel? _selectedTinhFilter = null;
        private XaPhuongModel? _selectedXaFilter = null;

        private QLCLPhamViHoatDongModel SelectedItem { get; set; } = new QLCLPhamViHoatDongModel();

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
                int intdex =1;

                BuilderQuery += "filter[_and][0][deleted][_eq]=false&sort=sort";
                if (!string.IsNullOrEmpty(_searchString))
                {
                    BuilderQuery += $"&filter[_and][{intdex}][_or][0][name][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{intdex}][_or][1][description][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{intdex}][_or][2][code][_contains]={_searchString}";
                    intdex++;
                }

                if(_selectedTinhFilter != null)
                {
                    BuilderQuery += $"&filter[_and][{intdex}][province][_eq]={_selectedTinhFilter.id}";
                    intdex++;
                }

                if(_selectedXaFilter != null)
                {
                    BuilderQuery += $"&filter[_and][{intdex}][ward][_eq]={_selectedXaFilter.id}";
                    intdex++;
                }

                if(!string.IsNullOrEmpty(_searchStatusString))
                {
                    BuilderQuery += $"&filter[_and][{intdex}][status][_eq]={_searchStatusString}";
                    intdex++;
                }
                

                var result = await MainService.GetAllAsync(BuilderQuery);
                if (result.IsSuccess)
                {
                    MainModels = result.Data ?? new List<QLCLPhamViHoatDongModel>();
                    if (result.Meta != null)
                    {
                        TotalItems = result.Meta.filter_count ?? 0;
                        TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                    }
                }
                else
                {
                    MainModels = new List<QLCLPhamViHoatDongModel>();
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

        private void OpenAddOrUpdateModal(QLCLPhamViHoatDongModel? item)
        {
            try
            {
                _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
                SelectedItem = item?.DeepClone() ?? new QLCLPhamViHoatDongModel();
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

        private void OpenDeleteModal(QLCLPhamViHoatDongModel item)
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

        private void CloseDeleteModal()
        {
            try
            {
                SelectedItem = new QLCLPhamViHoatDongModel();
                openDeleteModal = false;
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi: {ex.Message}", "danger");
            }
        }

        private void CloseAddOrUpdateModal()
        {
            try
            {
                SelectedItem = new QLCLPhamViHoatDongModel();
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
                string message =resultCreate.Message ?? resultUpdate.Message;
                if (resultCreate.IsSuccess || resultUpdate.IsSuccess)
                {
                    await LoadData();
                    openAddOrUpdateModal = false;
                    AlertService.ShowAlert(SelectedItem.id == 0 ? "Thêm mới thành công!" : "Cập nhật thành công!", "success");
                }
                else
                {
                    AlertService.ShowAlert($"Lỗi khi {(SelectedItem.id == 0 ? "thêm mới" : "cập nhật")} dữ liệu :" + message , "danger");
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
                if (SelectedItem == null) return;

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

        private async Task<IEnumerable<TinhModel>> LoadTinhData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, TinhService, isIgnoreCheck: true);
        }

        private async Task<IEnumerable<XaPhuongModel>> LoadXaCRUDData(string searchText)
        {
            string query = $"&filter[_and][][ProvinceId][_eq]={SelectedItem.province?.id ?? 0}";
            return await LoadBlazorTypeaheadData(searchText, XaService,query, isIgnoreCheck: true);
        }

        private async Task<IEnumerable<XaPhuongModel>> LoadXaFilterData(string searchText)
        {
            string query = $"&filter[_and][][ProvinceId][_eq]={_selectedTinhFilter?.id ?? 0}";
            return await LoadBlazorTypeaheadData(searchText, XaService, query, isIgnoreCheck: true);
        }

        public async Task OnTinhFilterChanged(TinhModel? item)
        {
            _selectedTinhFilter = item;
            await LoadData();
        }

        public async Task OnStatusFilterChanged(ChangeEventArgs e)
        {
            _searchStatusString = e.Value?.ToString() ?? "";
            await LoadData();
        }

        public async Task OnXaFilterChanged(XaPhuongModel? item)
        {
            _selectedXaFilter = item;
            await LoadData();
        }


        private async Task OnExportExcel()
        {
            // Get all data for export
            BuildPaginationQuery(Page, int.MaxValue);
            int intdex =1;

            BuilderQuery += "filter[_and][0][deleted][_eq]=false&sort=sort";
            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&filter[_and][{intdex}][_or][0][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{intdex}][_or][1][description][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{intdex}][_or][2][code][_contains]={_searchString}";
                intdex++;
            }

            if(_selectedTinhFilter != null)
            {
                BuilderQuery += $"&filter[_and][{intdex}][province][_eq]={_selectedTinhFilter.id}";
                intdex++;
            }

            if(_selectedXaFilter != null)
            {
                BuilderQuery += $"&filter[_and][{intdex}][ward][_eq]={_selectedXaFilter.id}";
                intdex++;
            }

            if(!string.IsNullOrEmpty(_searchStatusString))
            {
                BuilderQuery += $"&filter[_and][{intdex}][status][_eq]={_searchStatusString}";
                intdex++;
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
            ws.Cells[1, 2].Value = "MS khu vực";
            ws.Cells[1, 3].Value = "Tên khu vực";
            ws.Cells[1, 4].Value = "Khu vực hoạt động";
            ws.Cells[1, 5].Value = "Địa chỉ";
            ws.Cells[1, 6].Value = "Đối tác tiêu thụ";
            ws.Cells[1, 7].Value = "Phạm vi cung cấp";
            ws.Cells[1, 8].Value = "Trạng thái";
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
                ws.Cells[row, 2].Value = item.code;
                ws.Cells[row, 3].Value = item.name;
                ws.Cells[row, 4].Value = item.khu_vuc_hoat_dong;
                ws.Cells[row, 5].Value = item.province?.name + ", " + item.ward?.name;
                ws.Cells[row, 6].Value = item.doi_tac_tieu_thu;
                ws.Cells[row, 7].Value = item.pham_vi_noi_dia == true ? "Nội địa," : "" + (item.pham_vi_xuat_khau == true ? "Xuất khẩu" : "");
                ws.Cells[row, 8].Value = item.status.GetDescription();
                row++;
                stt++;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            // Export to browser
            var fileName = $"DanhSachPhamViKinhDoanh_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var fileBytes = package.GetAsByteArray();
            // Nếu chưa có hàm saveAsFile trong wwwroot/js, hãy thêm hàm này để hỗ trợ download file từ base64
            await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(fileBytes));
        }

    }

}
