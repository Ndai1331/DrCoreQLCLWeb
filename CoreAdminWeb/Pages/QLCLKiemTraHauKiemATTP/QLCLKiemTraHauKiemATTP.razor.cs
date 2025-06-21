using CoreAdminWeb.Model;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using CoreAdminWeb.Extensions;

namespace CoreAdminWeb.Pages.QLCLKiemTraHauKiemATTP
{
    public partial class QLCLKiemTraHauKiemATTP(IBaseService<QLCLKiemTraHauKiemATTPModel> MainService,
                                              IQLCLKiemTraHauKiemATTPChiTietService QLCLKiemTraHauKiemATTPChiTietService,
                                              IBaseService<TinhModel> TinhService,
                                              IBaseService<XaPhuongModel> XaPhuongService,
                                              IBaseService<QLCLLoaiHinhKinhDoanhModel> LoaiHinhKinhDoanhService,
                                              IBaseService<QLCLSanPhamSanXuatModel> SanPhamService,
                                              IBaseService<QLCLCoSoNLTSDuDieuKienATTPModel> CoSoService,
                                              IBaseService<QLCLDotKiemTraHauKiemATTPModel> DotKiemTraService) : BlazorCoreBase
    {
        private List<QLCLKiemTraHauKiemATTPModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openSanPhamDetailDeleteModal = false;
        private bool openAddOrUpdateModal = false;

        private List<Enums.LoaiCoSo> LoaiCoSoItems { get; set; } = new List<Enums.LoaiCoSo>() {
            Enums.LoaiCoSo.DuDieuKien,
            Enums.LoaiCoSo.KhongDuDieuKien,
        };
        private List<Enums.LoaiHinhKiemTra> LoaiHinhKiemTraItems { get; set; } = new List<Enums.LoaiHinhKiemTra>() {
            Enums.LoaiHinhKiemTra.KiemTraBanDau,
            Enums.LoaiHinhKiemTra.KiemTraDinhKy,
            Enums.LoaiHinhKiemTra.KiemTraDotXuat,
            Enums.LoaiHinhKiemTra.KiemTraTheoDonYeuCau,
            Enums.LoaiHinhKiemTra.KiemTraDoCoDauHieuViPham,
        };
        private List<Enums.HinhThucXetNghiem> HinhThucXetNghiemItems { get; set; } = new List<Enums.HinhThucXetNghiem>() {
            Enums.HinhThucXetNghiem.XetNghiemTaiPhongKiemNghiem,
            Enums.HinhThucXetNghiem.XetNghiemNhanh,
        };
        private List<Enums.KetQuaKiemTraDinhKy> KetQuaKiemTraItems { get; set; } = new List<Enums.KetQuaKiemTraDinhKy>() {
            Enums.KetQuaKiemTraDinhKy.Dat,
            Enums.KetQuaKiemTraDinhKy.KhongDat,
        };
        private List<Enums.TinhHinhViPham> TinhHinhViPhamItems { get; set; } = new List<Enums.TinhHinhViPham>() {
            Enums.TinhHinhViPham.Co,
            Enums.TinhHinhViPham.Khong,
        };
        private List<Enums.LoaiXetNghiem> LoaiXetNghiemItems { get; set; } = new List<Enums.LoaiXetNghiem>() {
            Enums.LoaiXetNghiem.HoaSinh,
            Enums.LoaiXetNghiem.ViSinh,
        };
        private List<Enums.ChiTieu> ChiTieuItems { get; set; } = new List<Enums.ChiTieu>() {
            Enums.ChiTieu.ChiTieuViSinh,
            Enums.ChiTieu.ChiTieuThuocBVTV,
            Enums.ChiTieu.ChiTieuHoaChat
        };
        private List<Enums.MauGoc> MauGocItems { get; set; } = new List<Enums.MauGoc>() {
            Enums.MauGoc.MauGoc,
            Enums.MauGoc.MauKhac,
        };
        private QLCLKiemTraHauKiemATTPModel SelectedItem { get; set; } = new QLCLKiemTraHauKiemATTPModel();
        private List<QLCLKiemTraHauKiemATTPChiTietModel> SelectedSanPhamItemsDetail { get; set; } = new List<QLCLKiemTraHauKiemATTPChiTietModel>();
        private QLCLKiemTraHauKiemATTPChiTietModel? SelectedSanPhamItemDetail { get; set; } = default;
        private string _searchString = "";
        private TinhModel? _selectedTinhFilter { get; set; }
        private XaPhuongModel? _selectedXaFilter { get; set; }
        private string _titleAddOrUpdate = "Thêm mới";
        private string activeDefTab { get; set; } = "tab1";
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
            BuildPaginationQuery(Page, PageSize, "id", false);
            int index = 2;

            BuilderQuery += "&filter[_and][0][deleted][_eq]=false";
            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&filter[_and][{index}][_or][0][code][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][2][dia_chi_san_xuat_kinh_doanh][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][3][co_quan_kiem_tra][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][4][noi_dung_kiem_tra][_contains]={_searchString}";
                index++;
            }
            if (_fromDate != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ngay_kiem_tra][_gte]={_fromDate.Value.ToString("yyyy-MM-dd")}";
                index++;
            }

            if (_toDate != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ngay_kiem_tra][_lte]={_toDate.Value.ToString("yyyy-MM-dd")}";
            }

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<QLCLKiemTraHauKiemATTPModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<QLCLKiemTraHauKiemATTPModel>();
            }
            IsLoading = false;
        }

        private async Task LoadSanPhamDetailData()
        {
            var buildQuery = $"sort=id";
            buildQuery += $"&filter[_and][][kiem_tra_hau_kiem_attp][_eq]={SelectedItem.id}";
            // buildQuery += $"&filter[_and][][deleted][_eq]=false";
            var result = await QLCLKiemTraHauKiemATTPChiTietService.GetAllAsync(buildQuery);
            SelectedSanPhamItemsDetail = result.Data ?? new List<QLCLKiemTraHauKiemATTPChiTietModel>();
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


        private async Task<IEnumerable<XaPhuongModel>> LoadXaFilterData(string searchText)
        {
            string query = $"sort=-id";
            query += $"&filter[_and][][ProvinceId][_eq]={(_selectedTinhFilter == null ? 0 : _selectedTinhFilter?.id)}";
            return await LoadBlazorTypeaheadData(searchText, XaPhuongService, query);
        }
        private async Task<IEnumerable<QLCLLoaiHinhKinhDoanhModel>> LoadLoaiHinhKinhDoanhData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, LoaiHinhKinhDoanhService);
        }
        private async Task<IEnumerable<QLCLSanPhamSanXuatModel>> LoadSanPhamData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, SanPhamService);
        }

        private async Task<IEnumerable<QLCLCoSoNLTSDuDieuKienATTPModel>> LoadCoSoData(string searchText)
        {
            string query = $"&filter[_and][][loai][_eq]={SelectedItem.loai_co_so?.GetHashCode()}";
            return await LoadBlazorTypeaheadData(searchText, CoSoService, query);
        }

        private async Task<IEnumerable<QLCLDotKiemTraHauKiemATTPModel>> LoadDotKiemTraData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, DotKiemTraService);
        }

        private void OpenDeleteModal(QLCLKiemTraHauKiemATTPModel item)
        {
            SelectedItem = item;
            openDeleteModal = true;
        }

        private async Task OnDelete()
        {
            if (SelectedItem.id == 0)
            {
                AlertService.ShowAlert("Không có dữ liệu để xóa", "warning");
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

        private void CloseDeleteModal()
        {
            SelectedItem = new QLCLKiemTraHauKiemATTPModel();
            openDeleteModal = false;
        }

        private void OpenSanPhamDetailDeleteModal(QLCLKiemTraHauKiemATTPChiTietModel item)
        {
            SelectedSanPhamItemDetail = item;
            openSanPhamDetailDeleteModal = true;
        }

        private void OnSanPhamDetailDelete()
        {
            if (SelectedSanPhamItemDetail == null)
            {
                AlertService.ShowAlert("Không có dữ liệu để xóa", "warning");
                return;
            }

            foreach (var item in SelectedSanPhamItemsDetail)
            {
                if (item.id > 0 && item.id == SelectedSanPhamItemDetail.id || item.sort > 0 && item.sort == SelectedSanPhamItemDetail.sort)
                {
                    item.deleted = true;
                }
            }

            SelectedSanPhamItemDetail = default;

            openSanPhamDetailDeleteModal = false;

            if (!SelectedSanPhamItemsDetail.Any(c => c.deleted == null || c.deleted == false))
                SelectedSanPhamItemsDetail.Add(new QLCLKiemTraHauKiemATTPChiTietModel()
                {
                    kiem_tra_hau_kiem_attp = SelectedItem,
                    sort = (SelectedSanPhamItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    san_pham = null,
                    deleted = false,
                });
            StateHasChanged();
        }

        private void CloseSanPhamDetailDeleteModal()
        {
            SelectedSanPhamItemDetail = default;
            openSanPhamDetailDeleteModal = false;
        }

        private void OnAddSanPham()
        {
            if (SelectedSanPhamItemsDetail == null)
                SelectedSanPhamItemsDetail = new List<QLCLKiemTraHauKiemATTPChiTietModel>();

            SelectedSanPhamItemsDetail.Add(new QLCLKiemTraHauKiemATTPChiTietModel
            {
                kiem_tra_hau_kiem_attp = SelectedItem,
                sort = (SelectedSanPhamItemsDetail.Max(c => c.sort) ?? 0) + 1,
                san_pham = null,
                so_luong_mau = 0,
                loai_xet_nghiem = Enums.LoaiXetNghiem.HoaSinh,
                mau_goc = Enums.MauGoc.MauGoc,
                chi_tieu = Enums.ChiTieu.ChiTieuViSinh,
                so_mau_khong_dat = 0,
                chi_tieu_vi_pham = "",
                muc_phat_hien = "",
                deleted = false,
            });
        }

        private async Task OpenAddOrUpdateModal(QLCLKiemTraHauKiemATTPModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item : new QLCLKiemTraHauKiemATTPModel();

            if (SelectedItem.id > 0)
            {
                await LoadSanPhamDetailData();
                SelectedItem.loai_co_so = SelectedItem.co_so.loai;
            }

            if (!SelectedSanPhamItemsDetail.Any())
                SelectedSanPhamItemsDetail.Add(new QLCLKiemTraHauKiemATTPChiTietModel()
                {
                    kiem_tra_hau_kiem_attp = SelectedItem,
                    sort = (SelectedSanPhamItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    san_pham = null,
                    deleted = false,
                });

            openAddOrUpdateModal = true;

            // Wait for modal to render
            _ = Task.Run(async () =>
            {
                await Task.Delay(500);
                await JsRuntime.InvokeVoidAsync("initializeDatePicker");
            });
        }

        private async Task OnValidSubmit()
        {
            if (SelectedItem.id <= 0)
            {
                var result = await MainService.CreateAsync(SelectedItem);
                if (result.IsSuccess)
                {
                    var sanPhamChiTietList = SelectedSanPhamItemsDetail
                        .Where(c => c.deleted == false || c.deleted == null)
                        .Select(c =>
                        {
                            c.kiem_tra_hau_kiem_attp = result.Data;
                            return c;
                        })
                        .ToList();

                    var sanPhamDetailResult = await QLCLKiemTraHauKiemATTPChiTietService.CreateAsync(sanPhamChiTietList);
                    if (!sanPhamDetailResult.IsSuccess)
                    {
                        AlertService.ShowAlert(sanPhamDetailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                        return;
                    }
                    await LoadData();
                    openAddOrUpdateModal = false;
                    AlertService.ShowAlert("Thêm mới thành công!", "success");
                }
                else
                {
                    AlertService.ShowAlert(result.Message ?? "Lỗi khi thêm mới dữ liệu", "danger");
                }
            }
            else
            {
                var result = await MainService.UpdateAsync(SelectedItem);
                if (result.IsSuccess)
                {


                    var addNewSanPhamChiTietList = SelectedSanPhamItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id == 0)
                        .Select(c =>
                        {
                            c.kiem_tra_hau_kiem_attp = SelectedItem;
                            return c;
                        }).ToList();
                    var removeSanPhamChiTietList = SelectedSanPhamItemsDetail
                        .Where(c => c.deleted == true && c.id > 0)
                        .Select(c =>
                        {
                            c.kiem_tra_hau_kiem_attp = SelectedItem;
                            c.deleted = true;
                            return c;
                        }).ToList();
                    var updateSanPhamChiTietList = SelectedSanPhamItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id > 0)
                        .Select(c =>
                        {
                            c.kiem_tra_hau_kiem_attp = SelectedItem;
                            return c;
                        }).ToList();

                    if (addNewSanPhamChiTietList.Any())
                    {
                        var detailResult = await QLCLKiemTraHauKiemATTPChiTietService.CreateAsync(addNewSanPhamChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (removeSanPhamChiTietList.Any())
                    {
                        var detailResult = await QLCLKiemTraHauKiemATTPChiTietService.DeleteAsync(removeSanPhamChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi xóa chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (updateSanPhamChiTietList.Any())
                    {
                        var detailResult = await QLCLKiemTraHauKiemATTPChiTietService.UpdateAsync(updateSanPhamChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi cập nhật chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    await LoadData();
                    openAddOrUpdateModal = false;
                    AlertService.ShowAlert("Cập nhật thành công!", "success");
                }
                else
                {
                    AlertService.ShowAlert(result.Message ?? "Lỗi khi cập nhật dữ liệu", "danger");
                }
            }
        }

        private void CloseAddOrUpdateModal()
        {
            SelectedItem = new QLCLKiemTraHauKiemATTPModel();
            openAddOrUpdateModal = false;
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
                        case nameof(SelectedItem.ngay_kiem_tra):
                            SelectedItem.ngay_kiem_tra = null;
                            break;

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
                        case nameof(SelectedItem.ngay_kiem_tra):
                            SelectedItem.ngay_kiem_tra = date;
                            break;
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
        private void OnTabChanged(string tab)
        {
            activeDefTab = tab;
        }
        private void OnMauGocChanged(ChangeEventArgs e, QLCLKiemTraHauKiemATTPChiTietModel item)
        {
            item.mau_goc = e.Value?.ToString() == "true" ? Enums.MauGoc.MauGoc : Enums.MauGoc.MauKhac;
        }
        
        private async Task OnExportExcel()
        {
            // Get all data for export
            BuildPaginationQuery(Page, int.MaxValue, "id", false);
            int index = 2;

            BuilderQuery += "&filter[_and][0][deleted][_eq]=false";
            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&filter[_and][{index}][_or][0][code][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][2][dia_chi_san_xuat_kinh_doanh][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][3][co_quan_kiem_tra][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][4][noi_dung_kiem_tra][_contains]={_searchString}";
                index++;
            }
            if (_fromDate != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ngay_kiem_tra][_gte]={_fromDate.Value.ToString("yyyy-MM-dd")}";
                index++;
            }

            if (_toDate != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ngay_kiem_tra][_lte]={_toDate.Value.ToString("yyyy-MM-dd")}";
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
            ws.Cells[1, 2].Value = "Tên cơ sở";
            ws.Cells[1, 3].Value = "Địa chỉ";
            ws.Cells[1, 4].Value = "Sản phẩm kiểm tra";
            ws.Cells[1, 5].Value = "Loại hình kiểm tra";
            ws.Cells[1, 6].Value = "Hình thức xét nghiệm";
            ws.Cells[1, 7].Value = "Ngày kiểm tra";
            ws.Cells[1, 8].Value = "Kết quả kiểm tra";
            ws.Cells[1, 9].Value = "Tình hình vi phạm";


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
                ws.Cells[row, 2].Value = item.co_so?.name; // Tên cơ sở
                ws.Cells[row, 3].Value = item.dia_chi_san_xuat_kinh_doanh; // Địa chỉ
                ws.Cells[row, 4].Value = ""; // Sản phẩm kiểm tra
                ws.Cells[row, 5].Value = item.loai_hinh_kiem_tra; // Loại hình kiểm tra
                ws.Cells[row, 6].Value = item.hinh_thuc_xet_nghiem; // Hình thức xét nghiệm
                ws.Cells[row, 7].Value = item.ngay_kiem_tra?.ToString("dd/MM/yyyy"); // Ngày kiểm tra
                ws.Cells[row, 8].Value = item.ket_qua_kiem_tra; // Kết quả kiểm tra
                ws.Cells[row, 9].Value = item.tinh_hinh_vi_pham; // Tình hình vi phạm
                row++;
                stt++;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            // Export to browser
            var fileName = $"DanhSachKiemTraHauKiemATTP_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var fileBytes = package.GetAsByteArray();
            // Nếu chưa có hàm saveAsFile trong wwwroot/js, hãy thêm hàm này để hỗ trợ download file từ base64
            await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(fileBytes));
        }

    }
    
}
