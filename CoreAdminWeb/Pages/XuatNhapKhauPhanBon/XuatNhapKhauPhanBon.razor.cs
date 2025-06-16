using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model.CoSoBuonBan;
using CoreAdminWeb.Model.PhanBon;
using CoreAdminWeb.Model.QuanLyCoSoSanXuatPhanBon;
using CoreAdminWeb.Model.XuatNhapKhauPhanBon;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CoreAdminWeb.Pages.XuatNhapKhauPhanBon
{
    public partial class XuatNhapKhauPhanBon(IBaseService<XuatNhapKhauPhanBonChiTietModel> MainService,
                                             IBaseService<XuatNhapKhauPhanBonModel> XuatNhapKhauPhanBonService,
                                             IBaseService<QuanLyCoSoSanXuatPhanBonModel> CoSoSanXuatPhanBonService,
                                             IBaseService<CoSoDuDieuKienBuonBanPhanBonModel> CoSoBuonBanPhanBonService,
                                             IBaseService<PhanBonModel> PhanBonService) : BlazorCoreBase
    {
        private static List<string> _hinhThucList = new List<string>() {
            HinhThucXuatNhapKhau.NhapKhau.ToString(),
            HinhThucXuatNhapKhau.XuatKhau.ToString()
        };

        private List<XuatNhapKhauPhanBonChiTietModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private XuatNhapKhauPhanBonAddOrUpdateModel SelectedItem { get; set; } = new XuatNhapKhauPhanBonAddOrUpdateModel();
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
                BuilderQuery += $"&filter[_and][][xnk_phan_bon][hinh_thuc][_eq]={_selectedHinhThucFilter}";
            }
            BuilderQuery += $"&filter[_and][][deleted][_eq]=false";

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<XuatNhapKhauPhanBonChiTietModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<XuatNhapKhauPhanBonChiTietModel>();
            }
        }

        private async Task<IEnumerable<QuanLyCoSoSanXuatPhanBonModel>> LoadCoSoSanXuatPhanBonData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, CoSoSanXuatPhanBonService);
        }

        private async Task<IEnumerable<CoSoDuDieuKienBuonBanPhanBonModel>> LoadCoSoBuonBanPhanBonData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, CoSoBuonBanPhanBonService);
        }

        private async Task<IEnumerable<PhanBonModel>> LoadPhanBonData(string searchText)
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


        private void OpenDeleteModal(XuatNhapKhauPhanBonChiTietModel item)
        {
            SelectedItem = new XuatNhapKhauPhanBonAddOrUpdateModel()
            {
                parrent_id = item.xnk_phan_bon?.id,
                id = item.id,
                phan_bon = item.phan_bon,
                co_so_san_xuat_phan_bon = item.xnk_phan_bon?.co_so_san_xuat_phan_bon,
                co_so_du_dieu_kien_buon_ban_phan_bon = item.xnk_phan_bon?.co_so_du_dieu_kien_buon_ban_phan_bon,
                nuoc_xuat_nhap = item.nuoc_xuat_nhap,
                dvt = item.dvt,
                so_luong = item.so_luong,
                thanh_phan_ty_le = item.thanh_phan_ty_le,
                description = item.description,
                status = item.status.ToString(),
                ngay_chung_tu = item.xnk_phan_bon?.ngay_chung_tu ?? DateTime.Now,
                hinh_thuc = item.xnk_phan_bon?.hinh_thuc,
                giay_phep_xnk = item.xnk_phan_bon?.giay_phep_xnk,
                ngay_cap = item.xnk_phan_bon?.ngay_cap,
                co_quan_cap = item.xnk_phan_bon?.co_quan_cap,
                sort = item.sort ?? 0,
                so_chung_tu = item.xnk_phan_bon?.so_chung_tu
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

            var result = await XuatNhapKhauPhanBonService.DeleteAsync(new XuatNhapKhauPhanBonModel() { id = SelectedItem.parrent_id ?? 0 });
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
            SelectedItem = new XuatNhapKhauPhanBonAddOrUpdateModel();

            openDeleteModal = false;
        }

        private async Task OpenAddOrUpdateModal(XuatNhapKhauPhanBonChiTietModel? item)
        {
            _selectedLoaiDonVi = LoaiToChuc.CoSoKinhDoanh;
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ?
                new XuatNhapKhauPhanBonAddOrUpdateModel()
                {
                    parrent_id = item.xnk_phan_bon?.id,
                    id = item.id,
                    phan_bon = item.phan_bon,
                    co_so_san_xuat_phan_bon = item.xnk_phan_bon?.co_so_san_xuat_phan_bon,
                    co_so_du_dieu_kien_buon_ban_phan_bon = item.xnk_phan_bon?.co_so_du_dieu_kien_buon_ban_phan_bon,
                    nuoc_xuat_nhap = item.nuoc_xuat_nhap,
                    dvt = item.dvt ?? "Tấn",
                    so_luong = item.so_luong,
                    thanh_phan_ty_le = item.thanh_phan_ty_le,
                    description = item.xnk_phan_bon?.description,
                    status = item.xnk_phan_bon?.status.ToString() ?? Status.active.ToString(),
                    ngay_chung_tu = item.xnk_phan_bon?.ngay_chung_tu ?? DateTime.Now,
                    hinh_thuc = item.xnk_phan_bon?.hinh_thuc,
                    giay_phep_xnk = item.xnk_phan_bon?.giay_phep_xnk,
                    ngay_cap = item.xnk_phan_bon?.ngay_cap,
                    co_quan_cap = item.xnk_phan_bon?.co_quan_cap,
                    so_chung_tu = item.xnk_phan_bon?.so_chung_tu
                }
                : new XuatNhapKhauPhanBonAddOrUpdateModel();

            if (SelectedItem.co_so_du_dieu_kien_buon_ban_phan_bon != null)
                _selectedLoaiDonVi = LoaiToChuc.CoSoKinhDoanh;
            else if (SelectedItem.co_so_san_xuat_phan_bon != null)
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
            var xnkPhanBon = new XuatNhapKhauPhanBonModel()
            {
                id = SelectedItem.parrent_id ?? 0,
                so_chung_tu = SelectedItem.so_chung_tu,
                description = SelectedItem.description,
                co_quan_cap = SelectedItem.co_quan_cap,
                co_so_du_dieu_kien_buon_ban_phan_bon = SelectedItem.co_so_du_dieu_kien_buon_ban_phan_bon,
                co_so_san_xuat_phan_bon = SelectedItem.co_so_san_xuat_phan_bon,
                giay_phep_xnk = SelectedItem.giay_phep_xnk,
                hinh_thuc = SelectedItem.hinh_thuc,
                ngay_cap = SelectedItem.ngay_cap,
                ngay_chung_tu = SelectedItem.ngay_chung_tu ?? DateTime.Now,
                status = Enum.Parse<Status>(SelectedItem.status),
                sort = SelectedItem.sort // Default sort value
            };

            var xnkPhanBonChiTiet = new XuatNhapKhauPhanBonChiTietModel()
            {
                id = SelectedItem.id,
                xnk_phan_bon = xnkPhanBon,
                phan_bon = SelectedItem.phan_bon,
                nuoc_xuat_nhap = SelectedItem.nuoc_xuat_nhap,
                dvt = SelectedItem.dvt ?? "Tấn",
                so_luong = SelectedItem.so_luong,
                thanh_phan_ty_le = SelectedItem.thanh_phan_ty_le,
                description = SelectedItem.description,
                status = Enum.Parse<Status>(SelectedItem.status)
            };


            if (xnkPhanBon.id == 0)
            {
                var result = await XuatNhapKhauPhanBonService.CreateAsync(xnkPhanBon);
                if (result.IsSuccess)
                {
                    xnkPhanBonChiTiet.xnk_phan_bon = result.Data;
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
                var result = await XuatNhapKhauPhanBonService.UpdateAsync(xnkPhanBon);
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
            SelectedItem = new XuatNhapKhauPhanBonAddOrUpdateModel();
            openAddOrUpdateModal = false;
        }

        private void OnPhanBonChanged(PhanBonModel? selected)
        {
            SelectedItem.phan_bon = selected;
            SelectedItem.dvt = selected?.don_vi_tinh?.name ?? "Tấn";
            SelectedItem.thanh_phan_ty_le = selected?.thanh_phan_ham_luong;
        }

        private void OnCoSoSanXuatChanged(QuanLyCoSoSanXuatPhanBonModel? selected)
        {
            SelectedItem.co_so_san_xuat_phan_bon = selected;
        }

        private void OnCoSoBuonBanChanged(CoSoDuDieuKienBuonBanPhanBonModel? selected)
        {
            SelectedItem.co_so_du_dieu_kien_buon_ban_phan_bon = selected;
        }

        private void OnLoaiDonViChanged(ChangeEventArgs args)
        {
            SelectedItem.co_so_du_dieu_kien_buon_ban_phan_bon = default;
            SelectedItem.co_so_san_xuat_phan_bon = default;

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
