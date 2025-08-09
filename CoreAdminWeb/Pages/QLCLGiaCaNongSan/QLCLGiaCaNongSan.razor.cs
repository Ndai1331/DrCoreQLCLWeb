using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CoreAdminWeb.Pages.QLCLGiaCaNongSan
{
    public partial class QLCLGiaCaNongSan(
        IBaseService<QLCLGiaCaNongSanModel> MainService,
        IBaseService<TinhModel> TinhService,
        IBaseService<XaPhuongModel> XaPhuongService,
        IBaseService<QLCLSanPhamSanXuatModel> SanPhamSanXuatService,
        IBaseService<DonViTinhModel> DonViTinhService) : BlazorCoreBase
    {
        private List<QLCLGiaCaNongSanModel> MainModels { get; set; } = new();

        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private string _titleAddOrUpdate = "Thêm mới";
        private string _searchString = "";
        private QLCLSanPhamSanXuatModel? _selectedSanPhamSanXuatFilter { get; set; }
        private DateTime? _fromDate = null;
        private DateTime? _toDate = null;

        private QLCLGiaCaNongSanModel SelectedItem { get; set; } = new QLCLGiaCaNongSanModel();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                SelectedItem.province = await LoadDefaultData(TinhService);
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

                BuilderQuery += "&filter[_and][0][deleted][_eq]=false&sort=-date_created";
                if (!string.IsNullOrEmpty(_searchString))
                {
                    index++;
                    BuilderQuery += $"&filter[_and][{index}][_or][0][san_pham_san_xuat][name][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{index}][_or][1][nha_cung_cap][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{index}][_or][2][description][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{index}][_or][3][province][name][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{index}][_or][4][ward][name][_contains]={_searchString}";
                }

                if (_selectedSanPhamSanXuatFilter != null)
                {
                    index++;
                    BuilderQuery += $"&filter[_and][{index}][san_pham_san_xuat][_eq]={_selectedSanPhamSanXuatFilter.id}";
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
                    MainModels = result.Data ?? new List<QLCLGiaCaNongSanModel>();
                    if (result.Meta != null)
                    {
                        TotalItems = result.Meta.filter_count ?? 0;
                        TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                    }
                }
                else
                {
                    MainModels = new List<QLCLGiaCaNongSanModel>();
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

        private async Task<IEnumerable<XaPhuongModel>> LoadXaCRUDData(string searchText)
        {
            string query = $"sort=-id";
            query += $"&filter[_and][][ProvinceId][_eq]={(SelectedItem.province == null ? 0 : SelectedItem.province?.id)}";
            return await LoadBlazorTypeaheadData(searchText, XaPhuongService, query);
        }

        private async Task<IEnumerable<QLCLSanPhamSanXuatModel>> LoadSanPhamSanXuatData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, SanPhamSanXuatService);
        }

        private async Task<IEnumerable<DonViTinhModel>> LoadDonViTinhData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, DonViTinhService);
        }

        private async Task OpenAddOrUpdateModal(QLCLGiaCaNongSanModel? item)
        {
            try
            {
                _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
                SelectedItem = item?.DeepClone() ?? new QLCLGiaCaNongSanModel();

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

        private void OpenDeleteModal(QLCLGiaCaNongSanModel item)
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
                SelectedItem = new QLCLGiaCaNongSanModel()
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
                SelectedItem = new QLCLGiaCaNongSanModel();
                SelectedItem.province = await LoadDefaultData(TinhService);
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
                if ((SelectedItem.id == 0 && resultCreate.IsSuccess) || (SelectedItem.id > 0 && resultUpdate.IsSuccess))
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

        public async Task OnSanPhamSanXuatFilterChanged(QLCLSanPhamSanXuatModel? item)
        {
            _selectedSanPhamSanXuatFilter = item;
            await LoadData();
        }

        private async Task OnExportExcel()
        {
            // Get all data for export
            BuildPaginationQuery(1, int.MaxValue);
            int index = 1;

            BuilderQuery += "&filter[_and][0][deleted][_eq]=false&sort=-date_created";
            if (!string.IsNullOrEmpty(_searchString))
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][_or][0][san_pham_san_xuat][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][nha_cung_cap][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][2][description][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][3][province][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][4][ward][name][_contains]={_searchString}";
            }

            if (_selectedSanPhamSanXuatFilter != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][san_pham_san_xuat][_eq]={_selectedSanPhamSanXuatFilter.id}";
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
            ws.Cells[1, 3].Value = "Tên nông sản";
            ws.Cells[1, 4].Value = "Nhà cung cấp";
            ws.Cells[1, 5].Value = "Địa điểm khảo sát";
            ws.Cells[1, 6].Value = "Đơn vị tính";
            ws.Cells[1, 7].Value = "Giá mua vào (VNĐ)";
            ws.Cells[1, 8].Value = "Giá bán ra (VNĐ)";
            ws.Cells[1, 9].Value = "Ghi chú";

            // Style header
            using (var range = ws.Cells[1, 1, 1, 9])
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
                ws.Cells[row, 3].Value = item.san_pham_san_xuat?.name;
                ws.Cells[row, 4].Value = item.nha_cung_cap;
                ws.Cells[row, 5].Value = item.province?.name + "/" + item.ward?.name;
                ws.Cells[row, 6].Value = item.don_vi_tinh?.name;
                ws.Cells[row, 7].Value = item.gia_mua_vao;
                ws.Cells[row, 8].Value = item.gia_ban_ra;
                ws.Cells[row, 9].Value = item.description;
                row++;
                stt++;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            // Export to browser
            var fileName = $"DanhSachGiaCaNongSan_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var fileBytes = await package.GetAsByteArrayAsync();
            // Nếu chưa có hàm saveAsFile trong wwwroot/js, hãy thêm hàm này để hỗ trợ download file từ base64
            await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(fileBytes));
        }
    }

}
