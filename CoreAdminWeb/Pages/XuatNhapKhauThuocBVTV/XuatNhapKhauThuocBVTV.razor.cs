using CoreAdminWeb.Enums;
using CoreAdminWeb.Model;
using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model.ThuocBaoVeThucVat;
using CoreAdminWeb.Model.XuatNhapKhauPhanBon;
using CoreAdminWeb.Model.XuatNhapKhauThuocBaoVeThucVat;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CoreAdminWeb.Pages.XuatNhapKhauThuocBVTV
{
    public partial class XuatNhapKhauThuocBVTV(IBaseService<XuatNhapKhauThuocBVTVChiTietModel> MainService,
                                             IBaseService<XuatNhapKhauThuocBVTVModel> XuatNhapKhauThuocBVTVService,
                                             IBaseService<QuanLyCoSoSanXuatThuocBVTVModel> CoSoSanXuatPhanBonService,
                                             IBaseService<QuanLyCoSoKinhDoanhThuocBVTVModel> CoSoBuonBanPhanBonService,
                                             IBaseService<ThuocBaoVeThucVatModel> PhanBonService) : BlazorCoreBase
    {
        private static List<string> _hinhThucList = new List<string>() {
            HinhThucXuatNhapKhau.NhapKhau.ToString(),
            HinhThucXuatNhapKhau.XuatKhau.ToString()
        };

        private List<XuatNhapKhauThuocBVTVChiTietModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private XuatNhapKhauThuocBVTVAddOrUpdateModel SelectedItem { get; set; } = new XuatNhapKhauThuocBVTVAddOrUpdateModel();
        private string _searchString = "";
        private string _titleAddOrUpdate = "Thêm mới";
      
        private readonly List<LoaiToChuc> _loaiToChucList = new List<LoaiToChuc>() { LoaiToChuc.CoSoKinhDoanh, LoaiToChuc.CoSoSanXuat };
        private LoaiToChuc _selectedLoaiDonVi { get; set; } = LoaiToChuc.CoSoKinhDoanh;

        private string? _selectedHinhThucFilter { get; set; }
        private string? _noiXuatNhapString { get; set; }

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
            BuildPaginationQuery(Page, PageSize);
            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&filter[_and][][name][_contains]={_searchString}";
            }
            if (!string.IsNullOrEmpty(_noiXuatNhapString))
            {
                BuilderQuery += $"&filter[_and][][nuoc_xuat_nhap][_contains]={_noiXuatNhapString}";
            }
            if (!string.IsNullOrEmpty(_selectedHinhThucFilter))
            {
                BuilderQuery += $"&filter[_and][][xnk_thuoc_bvtv][hinh_thuc][_eq]={_selectedHinhThucFilter}";
            }
            BuilderQuery += $"&filter[_and][][deleted][_eq]=false";

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<XuatNhapKhauThuocBVTVChiTietModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<XuatNhapKhauThuocBVTVChiTietModel>();
            }
        }

        private async Task<IEnumerable<QuanLyCoSoSanXuatThuocBVTVModel>> LoadCoSoSanXuatPhanBonData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, CoSoSanXuatPhanBonService);
        }

        private async Task<IEnumerable<QuanLyCoSoKinhDoanhThuocBVTVModel>> LoadCoSoBuonBanPhanBonData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, CoSoBuonBanPhanBonService);
        }

        private async Task<IEnumerable<ThuocBaoVeThucVatModel>> LoadPhanBonData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, PhanBonService);
        }

        private async Task OnPageSizeChanged()
        {
            Page = 1;
            await LoadData();
        }

        private async Task PreviousPage()
        {
            if (Page > 1)
            {
                Page--;
                await LoadData();
            }
        }

        private async Task SelectedPage(int page)
        {
            Page = page;
            await LoadData();
        }

        private async Task NextPage()
        {
            if (Page < TotalPages)
            {
                Page++;
                await LoadData();
            }
        }


        private void OpenDeleteModal(XuatNhapKhauThuocBVTVChiTietModel item)
        {
            SelectedItem = new XuatNhapKhauThuocBVTVAddOrUpdateModel()
            {
                parrent_id = item.xnk_thuoc_bvtv?.id,
                id = item.id,
                thuoc_bvtv = item.thuoc_bvtv,
                co_so_san_xuat_thuoc_bvtv = item.xnk_thuoc_bvtv?.co_so_san_xuat_thuoc_bvtv,
                co_so_kinh_doanh_thuoc_bvtv = item.xnk_thuoc_bvtv?.co_so_kinh_doanh_thuoc_bvtv,
                nuoc_xuat_nhap = item.nuoc_xuat_nhap,
                dvt = item.dvt,
                so_luong = item.so_luong,
                thanh_phan_ty_le = item.thanh_phan_ty_le,
                description = item.description,
                status = item.status.ToString(),
                ngay_chung_tu = item.xnk_thuoc_bvtv?.ngay_chung_tu ?? DateTime.Now,
                hinh_thuc = item.xnk_thuoc_bvtv?.hinh_thuc,
                giay_phep_xnk = item.xnk_thuoc_bvtv?.giay_phep_xnk,
                ngay_cap = item.xnk_thuoc_bvtv?.ngay_cap,
                co_quan_cap = item.xnk_thuoc_bvtv?.co_quan_cap,
                sort = item.sort ?? 0,
                so_chung_tu = item.xnk_thuoc_bvtv?.so_chung_tu
            };

            openDeleteModal = true;
        }

        private async Task OnDelete()
        {
            if (SelectedItem.parrent_id is null || SelectedItem.parrent_id == 0)
            {
                AlertService.ShowAlert("Không có dữ liệu để xóa", "warning");
                return;
            }

            var result = await XuatNhapKhauThuocBVTVService.DeleteAsync(new XuatNhapKhauThuocBVTVModel() { id = SelectedItem.parrent_id ?? 0 });
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
            SelectedItem = new XuatNhapKhauThuocBVTVAddOrUpdateModel();

            openDeleteModal = false;
        }

        private async Task OpenAddOrUpdateModal(XuatNhapKhauThuocBVTVChiTietModel? item)
        {
            _selectedLoaiDonVi = LoaiToChuc.CoSoKinhDoanh;
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ?
                new XuatNhapKhauThuocBVTVAddOrUpdateModel()
                {
                    parrent_id = item.xnk_thuoc_bvtv?.id,
                    id = item.id,
                    thuoc_bvtv = item.thuoc_bvtv,
                    co_so_san_xuat_thuoc_bvtv = item.xnk_thuoc_bvtv?.co_so_san_xuat_thuoc_bvtv,
                    co_so_kinh_doanh_thuoc_bvtv = item.xnk_thuoc_bvtv?.co_so_kinh_doanh_thuoc_bvtv,
                    nuoc_xuat_nhap = item.nuoc_xuat_nhap,
                    dvt = item.dvt ?? "Tấn",
                    so_luong = item.so_luong,
                    thanh_phan_ty_le = item.thanh_phan_ty_le,
                    description = item.xnk_thuoc_bvtv?.description,
                    status = item.xnk_thuoc_bvtv?.status.ToString() ?? Status.active.ToString(),
                    ngay_chung_tu = item.xnk_thuoc_bvtv?.ngay_chung_tu ?? DateTime.Now,
                    hinh_thuc = item.xnk_thuoc_bvtv?.hinh_thuc,
                    giay_phep_xnk = item.xnk_thuoc_bvtv?.giay_phep_xnk,
                    ngay_cap = item.xnk_thuoc_bvtv?.ngay_cap,
                    co_quan_cap = item.xnk_thuoc_bvtv?.co_quan_cap,
                    so_chung_tu = item.xnk_thuoc_bvtv?.so_chung_tu
                }
                : new XuatNhapKhauThuocBVTVAddOrUpdateModel();

            if (SelectedItem.co_so_kinh_doanh_thuoc_bvtv != null)
                _selectedLoaiDonVi = LoaiToChuc.CoSoKinhDoanh;
            else if (SelectedItem.co_so_san_xuat_thuoc_bvtv != null)
                _selectedLoaiDonVi = LoaiToChuc.CoSoSanXuat;

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
            var xnkPhanBon = new XuatNhapKhauThuocBVTVModel()
            {
                id = SelectedItem.parrent_id ?? 0,
                so_chung_tu = SelectedItem.so_chung_tu,
                description = SelectedItem.description,
                co_quan_cap = SelectedItem.co_quan_cap,
                co_so_kinh_doanh_thuoc_bvtv = SelectedItem.co_so_kinh_doanh_thuoc_bvtv,
                co_so_san_xuat_thuoc_bvtv = SelectedItem.co_so_san_xuat_thuoc_bvtv,
                giay_phep_xnk = SelectedItem.giay_phep_xnk,
                hinh_thuc = SelectedItem.hinh_thuc,
                ngay_cap = SelectedItem.ngay_cap,
                ngay_chung_tu = SelectedItem.ngay_chung_tu ?? DateTime.Now,
                status = Enum.Parse<Status>(SelectedItem.status),
                sort = SelectedItem.sort // Default sort value
            };

            var xnkPhanBonChiTiet = new XuatNhapKhauThuocBVTVChiTietModel()
            {
                id = SelectedItem.id,
                xnk_thuoc_bvtv = xnkPhanBon,
                thuoc_bvtv = SelectedItem.thuoc_bvtv,
                nuoc_xuat_nhap = SelectedItem.nuoc_xuat_nhap,
                dvt = SelectedItem.dvt ?? "Tấn",
                so_luong = SelectedItem.so_luong,
                thanh_phan_ty_le = SelectedItem.thanh_phan_ty_le,
                description = SelectedItem.description,
                status = Enum.Parse<Status>(SelectedItem.status)
            };


            if (xnkPhanBon.id == 0)
            {
                var result = await XuatNhapKhauThuocBVTVService.CreateAsync(xnkPhanBon);
                if (result.IsSuccess)
                {
                    xnkPhanBonChiTiet.xnk_thuoc_bvtv = result.Data;
                    var detailResult = await MainService.CreateAsync(xnkPhanBonChiTiet);
                    if (!detailResult.IsSuccess)
                    {
                        AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
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
                var result = await XuatNhapKhauThuocBVTVService.UpdateAsync(xnkPhanBon);
                if (result.IsSuccess)
                {
                    var detailResult = await MainService.UpdateAsync(xnkPhanBonChiTiet);
                    if (!detailResult.IsSuccess)
                    {
                        AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                        return;
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
            SelectedItem = new XuatNhapKhauThuocBVTVAddOrUpdateModel();
            openAddOrUpdateModal = false;
        }

        private void OnPhanBonChanged(ThuocBaoVeThucVatModel? selected)
        {
            SelectedItem.thuoc_bvtv = selected;
            SelectedItem.dvt = selected?.don_vi_tinh?.name ?? "Tấn";
            SelectedItem.thanh_phan_ty_le = selected?.hoat_chat_ky_thuat;
        }

        private void OnCoSoSanXuatChanged(QuanLyCoSoSanXuatThuocBVTVModel? selected)
        {
            SelectedItem.co_so_san_xuat_thuoc_bvtv = selected;
        }

        private void OnCoSoBuonBanChanged(QuanLyCoSoKinhDoanhThuocBVTVModel? selected)
        {
            SelectedItem.co_so_kinh_doanh_thuoc_bvtv = selected;
        }

        private void OnLoaiDonViChanged(ChangeEventArgs args)
        {
            SelectedItem.co_so_kinh_doanh_thuoc_bvtv = default;
            SelectedItem.co_so_san_xuat_thuoc_bvtv = default;

            string selectedValue = args.Value?.ToString() ?? string.Empty;
            if (Enum.TryParse<LoaiToChuc>(selectedValue, out var loaiToChuc))
            {
                _selectedLoaiDonVi = loaiToChuc;
            }
            else
            {
                _selectedLoaiDonVi = LoaiToChuc.CoSoKinhDoanh;
            }
        }

        private async Task OnHinhThucFilterChanged(ChangeEventArgs args)
        {
            _selectedHinhThucFilter = args.Value?.ToString();

            await LoadData();
        }
        private void OnDateChanged(ChangeEventArgs e, string fieldName)
        {
            try
            {
                var dateStr = e.Value?.ToString();
                if (string.IsNullOrEmpty(dateStr))
                {
                    switch (fieldName)
                    {
                        case nameof(SelectedItem.ngay_chung_tu):
                            SelectedItem.ngay_chung_tu = null;
                            break;
                        case nameof(SelectedItem.ngay_cap):
                            SelectedItem.ngay_cap = null;
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
                        case nameof(SelectedItem.ngay_chung_tu):
                            SelectedItem.ngay_chung_tu = date;
                            break;
                        case nameof(SelectedItem.ngay_cap):
                            SelectedItem.ngay_cap = date;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi khi xử lý ngày: {ex.Message}", "danger");
            }
        }
    }
}
