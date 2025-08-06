using CoreAdminWeb.Extensions;
using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CoreAdminWeb.Pages.QLCLDataTuyenTruyenATTP
{
    public partial class QLCLDataTuyenTruyenATTP(
        IBaseService<QLCLDataTuyenTruyenATTPModel> MainService,
        IBaseService<TinhModel> TinhService,
        IBaseService<XaPhuongModel> XaPhuongService) : BlazorCoreBase
    {
        private List<QLCLDataTuyenTruyenATTPModel> MainModels { get; set; } = new();

        private List<Enums.HinhThucTuyenTruyen> HinhThucTuyenTruyenList { get; set; } = new List<Enums.HinhThucTuyenTruyen>() {
            Enums.HinhThucTuyenTruyen.HoiNghi,
            Enums.HinhThucTuyenTruyen.TapHuan,
            Enums.HinhThucTuyenTruyen.TruyenThong,
            Enums.HinhThucTuyenTruyen.ToRoi,
            Enums.HinhThucTuyenTruyen.ApPhich,
            Enums.HinhThucTuyenTruyen.Khac,
        };

        private List<Enums.DonViTinhTuyenTruyen> DonViTinhTuyenTruyenList { get; set; } = new List<Enums.DonViTinhTuyenTruyen>() {
            Enums.DonViTinhTuyenTruyen.Buoi,
            Enums.DonViTinhTuyenTruyen.Lop,
            Enums.DonViTinhTuyenTruyen.Bai,
            Enums.DonViTinhTuyenTruyen.Cai,
            Enums.DonViTinhTuyenTruyen.To,
        };

        private List<Enums.DoiTuongThamGiaTuyenTruyen> DoiTuongThamGiaTuyenTruyenList { get; set; } = new List<Enums.DoiTuongThamGiaTuyenTruyen>()
        {
            Enums.DoiTuongThamGiaTuyenTruyen.HoNongDan,
            Enums.DoiTuongThamGiaTuyenTruyen.DoanhNghiep,
            Enums.DoiTuongThamGiaTuyenTruyen.NguoiTieuDung,
            Enums.DoiTuongThamGiaTuyenTruyen.Khac,
        };

        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private string _titleAddOrUpdate = "Thêm mới";
        private string _searchString = "";
        private TinhModel? _selectedTinhFilter { get; set; }
        private XaPhuongModel? _selectedXaFilter { get; set; }
        private DateTime? _fromDate = null;
        private DateTime? _toDate = null;

        private QLCLDataTuyenTruyenATTPModel SelectedItem { get; set; } = new QLCLDataTuyenTruyenATTPModel();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _selectedTinhFilter = await LoadDefaultData(TinhService);

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
                    BuilderQuery += $"&filter[_and][{index}][_or][0][code][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{index}][_or][1][dia_diem][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{index}][_or][2][co_quan_thuc_hien][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{index}][_or][3][hinh_thuc][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{index}][_or][4][don_vi_tinh][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{index}][_or][5][doi_tuong_tham_gia][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{index}][_or][6][noi_dung][_contains]={_searchString}";
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

                if (_fromDate != null)
                {
                    index++;
                    BuilderQuery += $"&filter[_and][{index}][ngay_thuc_hien][_gte]={_fromDate.Value:yyyy-MM-dd}";
                }

                if (_toDate != null)
                {
                    index++;
                    BuilderQuery += $"&filter[_and][{index}][ngay_thuc_hien][_lte]={_toDate.Value:yyyy-MM-dd}";
                }

                var result = await MainService.GetAllAsync(BuilderQuery);
                if (result.IsSuccess)
                {
                    MainModels = result.Data ?? new List<QLCLDataTuyenTruyenATTPModel>();
                    if (result.Meta != null)
                    {
                        TotalItems = result.Meta.filter_count ?? 0;
                        TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                    }
                }
                else
                {
                    MainModels = new List<QLCLDataTuyenTruyenATTPModel>();
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

        private async Task<IEnumerable<TinhModel>> LoadTinhData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, TinhService);
        }

        private async Task<IEnumerable<XaPhuongModel>> LoadXaFilterData(string searchText)
        {
            string query = $"sort=-id";
            query += $"&filter[_and][][ProvinceId][_eq]={(_selectedTinhFilter == null ? 0 : _selectedTinhFilter?.id)}";
            return await LoadBlazorTypeaheadData(searchText, XaPhuongService, query);
        }

        private async Task<IEnumerable<XaPhuongModel>> LoadXaCRUDData(string searchText)
        {
            string query = $"sort=-id";
            query += $"&filter[_and][][ProvinceId][_eq]={(SelectedItem.province == null ? 0 : SelectedItem.province?.id)}";
            return await LoadBlazorTypeaheadData(searchText, XaPhuongService, query);
        }

        private async Task OpenAddOrUpdateModal(QLCLDataTuyenTruyenATTPModel? item)
        {
            try
            {
                _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
                SelectedItem = item?.DeepClone() ?? new QLCLDataTuyenTruyenATTPModel()
                {
                    province = await LoadDefaultData(TinhService),
                };
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

        private void OpenDeleteModal(QLCLDataTuyenTruyenATTPModel item)
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
                SelectedItem = new QLCLDataTuyenTruyenATTPModel()
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
                SelectedItem = new QLCLDataTuyenTruyenATTPModel()
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

        private async Task OnTinhFilterChanged(TinhModel? tinh)
        {
            _selectedTinhFilter = tinh;
            _selectedXaFilter = null;
            await LoadData();
        }

        private async Task OnXaFilterChanged(XaPhuongModel? xa)
        {
            _selectedXaFilter = xa;
            await LoadData();
        }

        private async Task OnExportExcel()
        {
            // Get all data for export
            BuildPaginationQuery(Page, int.MaxValue);
            int index = 1;

            BuilderQuery += "&filter[_and][0][deleted][_eq]=false&sort=-date_created";
            if (!string.IsNullOrEmpty(_searchString))
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][_or][0][code][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][dia_diem][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][2][co_quan_thuc_hien][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][3][hinh_thuc][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][4][don_vi_tinh][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][5][doi_tuong_tham_gia][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][6][noi_dung][_contains]={_searchString}";
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

            if (_fromDate != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][ngay_thuc_hien][_gte]={_fromDate.Value:yyyy-MM-dd}";
            }

            if (_toDate != null)
            {
                index++;
                BuilderQuery += $"&filter[_and][{index}][ngay_thuc_hien][_lte]={_toDate.Value:yyyy-MM-dd}";
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
            ws.Cells[1, 2].Value = "Mã chứng từ";
            ws.Cells[1, 3].Value = "Ngày thực hiện";
            ws.Cells[1, 4].Value = "Địa điểm diễn ra";
            ws.Cells[1, 5].Value = "Tỉnh thành";
            ws.Cells[1, 6].Value = "Xã phường";
            ws.Cells[1, 7].Value = "Cơ quan thực hiện";
            ws.Cells[1, 8].Value = "Hình thức tuyên truyền";
            ws.Cells[1, 9].Value = "Số lượng";
            ws.Cells[1, 10].Value = "Đơn vị tính";
            ws.Cells[1, 11].Value = "Đối tượng tham gia";
            ws.Cells[1, 12].Value = "Số lượng người tham gia";
            ws.Cells[1, 13].Value = "Nội dung/chủ đề";

            // Style header
            using (var range = ws.Cells[1, 1, 1, 13])
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
                ws.Cells[row, 3].Value = item.ngay_thuc_hien?.ToString("dd/MM/yyyy");
                ws.Cells[row, 4].Value = item.dia_diem;
                ws.Cells[row, 5].Value = item.province?.name;
                ws.Cells[row, 6].Value = item.ward?.name;
                ws.Cells[row, 7].Value = item.co_quan_thuc_hien;
                ws.Cells[row, 8].Value = item.hinh_thuc?.GetDescription();
                ws.Cells[row, 9].Value = item.so_luong;
                ws.Cells[row, 10].Value = item.don_vi_tinh?.GetDescription();
                ws.Cells[row, 11].Value = item.doi_tuong_tham_gia?.GetDescription();
                ws.Cells[row, 12].Value = item.so_luong_nguoi_tham_gia;
                ws.Cells[row, 13].Value = item.noi_dung;
                row++;
                stt++;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            // Export to browser
            var fileName = $"DanhSachTuyenTruyenATTP_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var fileBytes = await package.GetAsByteArrayAsync();
            // Nếu chưa có hàm saveAsFile trong wwwroot/js, hãy thêm hàm này để hỗ trợ download file từ base64
            await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(fileBytes));
        }
    }

}
