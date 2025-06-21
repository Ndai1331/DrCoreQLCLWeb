using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using CoreAdminWeb.Enums;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using CoreAdminWeb.Extensions;

namespace CoreAdminWeb.Pages.QLCLKenhQuangBaXucTienThuongMai
{
    public partial class QLCLKenhQuangBaXucTienThuongMai(
        IBaseService<QLCLKenhQuangBaXucTienThuongMaiModel> MainService,
        IBaseService<TinhModel> TinhService,
        IBaseService<XaPhuongModel> XaPhuongService) : BlazorCoreBase
    {
        private List<QLCLKenhQuangBaXucTienThuongMaiModel> MainModels { get; set; } = new();
        private List<KenhQuangBa> KenhQuangBaList = new() {
            KenhQuangBa.HoiCho,
            KenhQuangBa.TrienLam,
            KenhQuangBa.SanTMDT,
            KenhQuangBa.Website,
            KenhQuangBa.MangXaHoi,
            KenhQuangBa.BaoChi,
            KenhQuangBa.TruyenHinh,
            KenhQuangBa.Khac
        };

        private List<HinhThucQuangBa> HinhThucQuangBaList = new() {
            HinhThucQuangBa.TrucTiep,
            HinhThucQuangBa.TrucTuyen,
            HinhThucQuangBa.KetHop
        };

        private List<PhamViTiepCan> PhamViTiepCanList = new() {
            PhamViTiepCan.DiaPhuong,
            PhamViTiepCan.TrongNuoc,
            PhamViTiepCan.QuocTe
        };

        private List<DoiTuongTiepCan> DoiTuongTiepCanList = new() {
            DoiTuongTiepCan.DoanhNghiep,
            DoiTuongTiepCan.NhaPhanPhoi,
            DoiTuongTiepCan.NguoiTieuDung,
            DoiTuongTiepCan.Khac
        };

        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private string _titleAddOrUpdate = "Thêm mới";
        private string _searchString = "";
        private TinhModel? _selectedTinhFilter { get; set; }
        private XaPhuongModel? _selectedXaFilter { get; set; }
        private DateTime? _fromDate = null;
        private DateTime? _toDate = null;

        private QLCLKenhQuangBaXucTienThuongMaiModel SelectedItem { get; set; } = new QLCLKenhQuangBaXucTienThuongMaiModel();

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
            try
            {
                IsLoading = true;
                BuildPaginationQuery(Page, PageSize);
                int index = 1;

                BuilderQuery += "filter[_and][0][deleted][_eq]=false&sort=sort";
                if (!string.IsNullOrEmpty(_searchString))
                {
                    BuilderQuery += $"&filter[_and][{index}][_or][0][code][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{index}][_or][1][name][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{index}][_or][2][dia_diem_to_chuc][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{index}][_or][3][description][_contains]={_searchString}";
                    index++;
                }

                if (_selectedTinhFilter != null)
                {
                    BuilderQuery += $"&filter[_and][{index}][province][_eq]={_selectedTinhFilter.id}";
                    index++;
                }

                if (_selectedXaFilter != null)
                {
                    BuilderQuery += $"&filter[_and][{index}][ward][_eq]={_selectedXaFilter.id}";
                    index++;
                }

                if (_fromDate != null)
                {
                    BuilderQuery += $"&filter[_and][{index}][ngay_to_chuc][_gte]={_fromDate.Value.ToString("yyyy-MM-dd")}";
                    index++;
                }

                if (_toDate != null)
                {
                    BuilderQuery += $"&filter[_and][{index}][ngay_to_chuc][_lte]={_toDate.Value.ToString("yyyy-MM-dd")}";
                    index++;
                }


                var result = await MainService.GetAllAsync(BuilderQuery);
                if (result.IsSuccess)
                {
                    MainModels = result.Data ?? new List<QLCLKenhQuangBaXucTienThuongMaiModel>();
                    if (result.Meta != null)
                    {
                        TotalItems = result.Meta.filter_count ?? 0;
                        TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                    }
                }
                else
                {
                    MainModels = new List<QLCLKenhQuangBaXucTienThuongMaiModel>();
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

        private void OpenAddOrUpdateModal(QLCLKenhQuangBaXucTienThuongMaiModel? item)
        {
            try
            {
                _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
                SelectedItem = item?.DeepClone() ?? new QLCLKenhQuangBaXucTienThuongMaiModel();
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

        private void OpenDeleteModal(QLCLKenhQuangBaXucTienThuongMaiModel item)
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
                SelectedItem = new QLCLKenhQuangBaXucTienThuongMaiModel();
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
                SelectedItem = new QLCLKenhQuangBaXucTienThuongMaiModel();
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

        private async Task OnDateChanged(ChangeEventArgs e, string fieldName)
        {
            try
            {
                var dateStr = e.Value?.ToString();
                if (string.IsNullOrEmpty(dateStr))
                {
                    if (fieldName == "ngay_to_chuc")
                        SelectedItem.ngay_to_chuc = null;
                    else if (fieldName == "fromDate")
                    {
                        _fromDate = null;
                        await LoadData();
                    }
                    else if (fieldName == "toDate")
                    {
                        _toDate = null;
                        await LoadData();
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

                    if (fieldName == "ngay_to_chuc")
                        SelectedItem.ngay_to_chuc = date;
                    else if (fieldName == "fromDate")
                    {
                        _fromDate = date;
                        await LoadData();
                    }
                    else if (fieldName == "toDate")
                    {
                        _toDate = date;
                        await LoadData();
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
            BuildPaginationQuery(Page, int.MaxValue);
            int index = 1;

            BuilderQuery += "filter[_and][0][deleted][_eq]=false&sort=sort";
            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&filter[_and][{index}][_or][0][code][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][2][dia_diem_to_chuc][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][3][description][_contains]={_searchString}";
                index++;
            }

            if (_selectedTinhFilter != null)
            {
                BuilderQuery += $"&filter[_and][{index}][province][_eq]={_selectedTinhFilter.id}";
                index++;
            }

            if (_selectedXaFilter != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ward][_eq]={_selectedXaFilter.id}";
                index++;
            }

            if (_fromDate != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ngay_to_chuc][_gte]={_fromDate.Value.ToString("yyyy-MM-dd")}";
                index++;
            }

            if (_toDate != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ngay_to_chuc][_lte]={_toDate.Value.ToString("yyyy-MM-dd")}";
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
            ws.Cells[1, 2].Value = "Mã số chương trình";
            ws.Cells[1, 3].Value = "Tên chương trình";
            ws.Cells[1, 4].Value = "Ngày tổ chức";
            ws.Cells[1, 5].Value = "Địa điểm";
            ws.Cells[1, 6].Value = "Kênh quảng bá";
            ws.Cells[1, 7].Value = "Sản phẩm";
            ws.Cells[1, 8].Value = "Hình thức quảng bá";
            ws.Cells[1, 9].Value = "Phạm vi tiếp cận";
            ws.Cells[1, 10].Value = "Đối tượng tiếp cận";
            ws.Cells[1, 11].Value = "Số lượng chủ thể";
            ws.Cells[1, 12].Value = "Lượt khách tham quan";
            ws.Cells[1, 13].Value = "Số hợp đồng ký kết";
            ws.Cells[1, 14].Value = "Giá trị giao dịch (VNĐ)";
            ws.Cells[1, 15].Value = "Ghi chú";
            ws.Cells[1, 16].Value = "Trạng thái";

            // Style header
            using (var range = ws.Cells[1, 1, 1, 16])
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
                ws.Cells[row, 4].Value = item.ngay_to_chuc?.ToString("dd/MM/yyyy");
                ws.Cells[row, 5].Value = item.dia_diem_to_chuc;
                ws.Cells[row, 6].Value = item.kenh_quang_ba?.GetDescription();
                ws.Cells[row, 7].Value = item.san_pham_tham_gia;
                ws.Cells[row, 8].Value = item.hinh_thuc_quang_ba?.GetDescription();
                ws.Cells[row, 9].Value = item.pham_vi_tiep_can?.GetDescription();
                ws.Cells[row, 10].Value = item.doi_tuong_tiep_can?.GetDescription();
                ws.Cells[row, 11].Value = item.so_luong_chu_the_tham_gia;
                ws.Cells[row, 12].Value = item.luot_khach_tham_quan;
                ws.Cells[row, 13].Value = item.so_hop_dong_ky_ket;
                ws.Cells[row, 14].Value = item.gia_tri_giao_dich;
                ws.Cells[row, 15].Value = item.description;
                ws.Cells[row, 16].Value = item.status.GetDescription();
                row++;
                stt++;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            // Export to browser
            var fileName = $"DanhSachChuongTrinhQuangBaXucTienThuongMai_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var fileBytes = package.GetAsByteArray();
            // Nếu chưa có hàm saveAsFile trong wwwroot/js, hãy thêm hàm này để hỗ trợ download file từ base64
            await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(fileBytes));
        }
    }

}
