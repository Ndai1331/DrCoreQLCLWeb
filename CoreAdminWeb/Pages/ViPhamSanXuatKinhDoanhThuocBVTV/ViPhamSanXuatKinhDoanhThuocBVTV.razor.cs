using CoreAdminWeb.Enums;
using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CoreAdminWeb.Pages.ViPhamSanXuatKinhDoanhThuocBVTV
{
    public partial class ViPhamSanXuatKinhDoanhThuocBVTV(IBaseService<QuanLyCoSoSanXuatThuocBVTVModel> CoSoSanXuatThuocBVTVService,
                                                         IBaseService<QuanLyCoSoKinhDoanhThuocBVTVModel> CoSoDuDieuKienBuonBanThuocBVTVService) : BlazorCoreBase
    {
        private List<ViPhamSanXuatKinhDoanhThuocBVTVModel> MainModels { get; set; } = new();
        private List<TrangThaiXuLy> TrangThaiXuLyList = new() { TrangThaiXuLy.ChuaXuLy, TrangThaiXuLy.DangXuLy, TrangThaiXuLy.DaXuLy };
        private List<LoaiToChuc> LoaiToChucList = new() { LoaiToChuc.CoSoSanXuat, LoaiToChuc.CoSoKinhDoanh };
        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private ViPhamSanXuatKinhDoanhThuocBVTVModel SelectedItem { get; set; } = new ViPhamSanXuatKinhDoanhThuocBVTVModel();
        private LoaiToChuc? _selectedFilterLoaiToChuc { get; set; }
        private TrangThaiXuLy? _selectedFilterTrangThaiXuLy { get; set; }
        private QuanLyCoSoSanXuatThuocBVTVModel? _selectedFilterCoSoSanXuatThuocBVTV { get; set; }
        private QuanLyCoSoKinhDoanhThuocBVTVModel? _selectedFilterCoSoDuDieuKienBuonBanThuocBVTV { get; set; }
        private string _searchString = "";
        private string _titleAddOrUpdate = "Thêm mới";

        private QuanLyCoSoSanXuatThuocBVTVModel? _selectedCRUDCoSoSanXuatThuocBVTV { get; set; }
        private QuanLyCoSoKinhDoanhThuocBVTVModel? _selectedCRUDCoSoDuDieuKienBuonBanThuocBVTV { get; set; }

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

        private string BuildBaseQuery(string searchText = "")
        {
            var query = "filter[_and][][deleted][_eq]=false&sort=sort";
            if (!string.IsNullOrEmpty(searchText))
            {
                query += $"&filter[_and][][name][_contains]={searchText}";
            }
            return query;
        }

        private async Task<IEnumerable<QuanLyCoSoSanXuatThuocBVTVModel>> LoadCoSoSanXuatThuocBVTV(string searchText)
        {
            return await LoadBlazorTypeaheadData<QuanLyCoSoSanXuatThuocBVTVModel>(searchText, CoSoSanXuatThuocBVTVService);
        }
        private async Task<IEnumerable<QuanLyCoSoKinhDoanhThuocBVTVModel>> LoadCoSoDuDieuKienBuonBanThuocBVTV(string searchText)
        {
            return await LoadBlazorTypeaheadData<QuanLyCoSoKinhDoanhThuocBVTVModel>(searchText, CoSoDuDieuKienBuonBanThuocBVTVService);
        }
        private async Task<IEnumerable<QuanLyCoSoSanXuatThuocBVTVModel>> LoadCRUDCoSoSanXuatThuocBVTV(string searchText)
        {
            return await LoadBlazorTypeaheadData<QuanLyCoSoSanXuatThuocBVTVModel>(searchText, CoSoSanXuatThuocBVTVService);
        }
        private async Task<IEnumerable<QuanLyCoSoKinhDoanhThuocBVTVModel>> LoadCRUDCoSoDuDieuKienBuonBanThuocBVTV(string searchText)
        {
            return await LoadBlazorTypeaheadData<QuanLyCoSoKinhDoanhThuocBVTVModel>(searchText, CoSoDuDieuKienBuonBanThuocBVTVService);
        }


        private string BuildFilterQuery()
        {
            var query = BuildBaseQuery(_searchString);

            if (_selectedFilterLoaiToChuc != null)
            {
                query += $"&filter[_and][][loai_to_chuc][_eq]={_selectedFilterLoaiToChuc.GetHashCode()}";

                if (_selectedFilterLoaiToChuc == LoaiToChuc.CoSoSanXuat && _selectedFilterCoSoSanXuatThuocBVTV?.id > 0)
                {
                    query += $"&filter[_and][][co_so_san_xuat_thuoc_bvtv][_eq]={_selectedFilterCoSoSanXuatThuocBVTV.id}";
                }
                else if (_selectedFilterLoaiToChuc == LoaiToChuc.CoSoKinhDoanh && _selectedFilterCoSoDuDieuKienBuonBanThuocBVTV?.id > 0)
                {
                    query += $"&filter[_and][][co_so_kinh_doanh_thuoc_bvtv][_eq]={_selectedFilterCoSoDuDieuKienBuonBanThuocBVTV.id}";
                }
            }

            if (_selectedFilterTrangThaiXuLy != null)
            {
                query += $"&filter[_and][][status][_eq]={_selectedFilterTrangThaiXuLy}";
            }

            return query;
        }

        private async Task LoadData()
        {
            try
            {
                IsLoading = true;
                BuildPaginationQuery(Page, PageSize);
                BuilderQuery += BuildFilterQuery();

                var result = await MainService.GetAllAsync(BuilderQuery);
                if (result.IsSuccess)
                {
                    MainModels = result.Data ?? new List<ViPhamSanXuatKinhDoanhThuocBVTVModel>();
                    if (result.Meta != null)
                    {
                        TotalItems = result.Meta.filter_count ?? 0;
                        TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                    }
                }
                else
                {
                    MainModels = new List<ViPhamSanXuatKinhDoanhThuocBVTVModel>();
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

        private void OpenAddOrUpdateModal(ViPhamSanXuatKinhDoanhThuocBVTVModel? item)
        {
            try
            {
                _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
                SelectedItem = item?.DeepClone() ?? new ViPhamSanXuatKinhDoanhThuocBVTVModel();
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

        private void OpenDeleteModal(ViPhamSanXuatKinhDoanhThuocBVTVModel item)
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
                SelectedItem = new ViPhamSanXuatKinhDoanhThuocBVTVModel();
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
                SelectedItem = new ViPhamSanXuatKinhDoanhThuocBVTVModel();
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
                if (SelectedItem == null) return;

                var resultCreate = SelectedItem.id == 0 ? await MainService.CreateAsync(SelectedItem) : new RequestHttpResponse<ViPhamSanXuatKinhDoanhThuocBVTVModel>();
                var resultUpdate = SelectedItem.id > 0 ? await MainService.UpdateAsync(SelectedItem) : new RequestHttpResponse<bool>();
                string message = resultCreate.Message ?? resultUpdate.Message;
                if (resultCreate.IsSuccess || resultUpdate.IsSuccess)
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

        private async Task OnLoaiToChucFilterChanged(ChangeEventArgs e)
        {
            try
            {
                var value = e.Value?.ToString();
                if (string.IsNullOrEmpty(value))
                {
                    _selectedFilterLoaiToChuc = null;
                    _selectedFilterCoSoSanXuatThuocBVTV = null;
                    _selectedFilterCoSoDuDieuKienBuonBanThuocBVTV = null;
                }
                else
                {
                    _selectedFilterLoaiToChuc = (LoaiToChuc)Enum.Parse(typeof(LoaiToChuc), value);
                }

                await LoadData();
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi: {ex.Message}", "danger");
            }
        }

        private async Task OnSelectedFilterCoSoDuDieuKienBuonBanThuocBVTVChanged(QuanLyCoSoKinhDoanhThuocBVTVModel? coSoDuDieuKienBuonBanThuocBVTV)
        {
            _selectedFilterCoSoDuDieuKienBuonBanThuocBVTV = coSoDuDieuKienBuonBanThuocBVTV;
            _selectedFilterCoSoSanXuatThuocBVTV = null;
            await LoadData();
        }
        private async Task OnSelectedFilterCoSoSanXuatThuocBVTVChanged(QuanLyCoSoSanXuatThuocBVTVModel? coSoSanXuatThuocBVTV)
        {
            _selectedFilterCoSoSanXuatThuocBVTV = coSoSanXuatThuocBVTV;
            _selectedFilterCoSoDuDieuKienBuonBanThuocBVTV = null;
            await LoadData();
        }

        private void OnSelectedCRUDCoSoSanXuatThuocBVTVChanged(QuanLyCoSoSanXuatThuocBVTVModel? coSoSanXuatThuocBVTV)
        {
            _selectedCRUDCoSoSanXuatThuocBVTV = coSoSanXuatThuocBVTV;
            _selectedCRUDCoSoDuDieuKienBuonBanThuocBVTV = null;
            SelectedItem.co_so_san_xuat_thuoc_bvtv = coSoSanXuatThuocBVTV;
            SelectedItem.co_so_kinh_doanh_thuoc_bvtv = null;
        }

        private void OnSelectedCRUDCoSoDuDieuKienBuonBanThuocBVTVChanged(QuanLyCoSoKinhDoanhThuocBVTVModel? coSoDuDieuKienBuonBanThuocBVTV)
        {
            _selectedCRUDCoSoDuDieuKienBuonBanThuocBVTV = coSoDuDieuKienBuonBanThuocBVTV;
            _selectedCRUDCoSoSanXuatThuocBVTV = null;
            SelectedItem.co_so_kinh_doanh_thuoc_bvtv = coSoDuDieuKienBuonBanThuocBVTV;
            SelectedItem.co_so_san_xuat_thuoc_bvtv = null;
        }

        private async Task OnTrangThaiXuLyFilterChanged(ChangeEventArgs e)
        {
            try
            {
                var value = e.Value?.ToString();
                _selectedFilterTrangThaiXuLy = !string.IsNullOrEmpty(value)
                    ? (TrangThaiXuLy)Enum.Parse(typeof(TrangThaiXuLy), value)
                    : null;
                await LoadData();
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

        private void OnDateChanged(ChangeEventArgs e, string fieldName)
        {
            try
            {
                var dateStr = e.Value?.ToString();
                if (string.IsNullOrEmpty(dateStr))
                {
                    if (fieldName == "ngay_phat_hien")
                        SelectedItem.ngay_phat_hien = null;
                    else if (fieldName == "ngay_xu_ly")
                        SelectedItem.ngay_xu_ly = null;
                    return;
                }

                var parts = dateStr.Split('/');
                if (parts.Length == 3 &&
                    int.TryParse(parts[0], out int day) &&
                    int.TryParse(parts[1], out int month) &&
                    int.TryParse(parts[2], out int year))
                {
                    var date = new DateTime(year, month, day);

                    if (fieldName == "ngay_phat_hien")
                        SelectedItem.ngay_phat_hien = date;
                    else if (fieldName == "ngay_xu_ly")
                        SelectedItem.ngay_xu_ly = date;
                }
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi khi xử lý ngày: {ex.Message}", "danger");
            }
        }
    }
}
