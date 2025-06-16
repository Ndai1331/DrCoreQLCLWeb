using CoreAdminWeb.Enums;
using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Model.CoSoBuonBan;
using CoreAdminWeb.Model.QuanLyCoSoSanXuatPhanBon;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CoreAdminWeb.Pages.ViPhamSanXuatKinhDoanhPhanBon
{
    public partial class ViPhamSanXuatKinhDoanhPhanBon(IBaseService<QuanLyCoSoSanXuatPhanBonModel> CoSoSanXuatPhanBonService,
                                                       IBaseService<CoSoDuDieuKienBuonBanPhanBonModel> CoSoDuDieuKienBuonBanPhanBonService) : BlazorCoreBase
    {
        private List<ViPhamSanXuatKinhDoanhPhanBonModel> MainModels { get; set; } = new();
        private List<TrangThaiXuLy> TrangThaiXuLyList = new() { TrangThaiXuLy.ChuaXuLy, TrangThaiXuLy.DangXuLy, TrangThaiXuLy.DaXuLy };
        private List<LoaiToChuc> LoaiToChucList = new() { LoaiToChuc.CoSoSanXuat, LoaiToChuc.CoSoKinhDoanh };
        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private ViPhamSanXuatKinhDoanhPhanBonModel SelectedItem { get; set; } = new ViPhamSanXuatKinhDoanhPhanBonModel();
        private LoaiToChuc? _selectedFilterLoaiToChuc { get; set;  }
        private TrangThaiXuLy? _selectedFilterTrangThaiXuLy { get; set;  }
        private QuanLyCoSoSanXuatPhanBonModel? _selectedFilterCoSoSanXuatPhanBon { get; set;  }
        private CoSoDuDieuKienBuonBanPhanBonModel? _selectedFilterCoSoDuDieuKienBuonBanPhanBon { get; set;  }
        private string _searchString = "";
        private string _titleAddOrUpdate = "Thêm mới";
        
        private QuanLyCoSoSanXuatPhanBonModel? _selectedCRUDCoSoSanXuatPhanBon { get; set;  }
        private CoSoDuDieuKienBuonBanPhanBonModel? _selectedCRUDCoSoDuDieuKienBuonBanPhanBon { get; set;  }

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

        private async Task<IEnumerable<QuanLyCoSoSanXuatPhanBonModel>> LoadCoSoSanXuatPhanBon(string searchText)
        {
            return await LoadBlazorTypeaheadData<QuanLyCoSoSanXuatPhanBonModel>(searchText, CoSoSanXuatPhanBonService);
        }
        private async Task<IEnumerable<CoSoDuDieuKienBuonBanPhanBonModel>> LoadCoSoDuDieuKienBuonBanPhanBon(string searchText) 
        {
            return await LoadBlazorTypeaheadData<CoSoDuDieuKienBuonBanPhanBonModel>(searchText, CoSoDuDieuKienBuonBanPhanBonService);
        }
        private async Task<IEnumerable<QuanLyCoSoSanXuatPhanBonModel>> LoadCRUDCoSoSanXuatPhanBon(string searchText)
        {
            return await LoadBlazorTypeaheadData<QuanLyCoSoSanXuatPhanBonModel>(searchText, CoSoSanXuatPhanBonService);
        }
        private async Task<IEnumerable<CoSoDuDieuKienBuonBanPhanBonModel>> LoadCRUDCoSoDuDieuKienBuonBanPhanBon(string searchText)
        {
            return await LoadBlazorTypeaheadData<CoSoDuDieuKienBuonBanPhanBonModel>(searchText, CoSoDuDieuKienBuonBanPhanBonService);
        }


        private string BuildFilterQuery()
        {
            var query = BuildBaseQuery(_searchString);

            if(_selectedFilterLoaiToChuc != null)
            {
                query += $"&filter[_and][][loai_to_chuc][_eq]={_selectedFilterLoaiToChuc.GetHashCode()}";
                
                if(_selectedFilterLoaiToChuc == LoaiToChuc.CoSoSanXuat && _selectedFilterCoSoSanXuatPhanBon?.id > 0)
                {
                    query += $"&filter[_and][][co_so_san_xuat_phan_bon][_eq]={_selectedFilterCoSoSanXuatPhanBon.id}";
                }
                else if(_selectedFilterLoaiToChuc == LoaiToChuc.CoSoKinhDoanh && _selectedFilterCoSoDuDieuKienBuonBanPhanBon?.id > 0)
                {
                    query += $"&filter[_and][][co_so_du_dieu_kien_buon_ban_phan_bon][_eq]={_selectedFilterCoSoDuDieuKienBuonBanPhanBon.id}";
                }
            }

            if(_selectedFilterTrangThaiXuLy != null)
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
                    MainModels = result.Data = new List<ViPhamSanXuatKinhDoanhPhanBonModel>();
                    if (result.Meta != null)
                    {
                        TotalItems = result.Meta.filter_count ?? 0;
                        TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                    }
                }
                else
                {
                    MainModels = new List<ViPhamSanXuatKinhDoanhPhanBonModel>();
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

        private void OpenAddOrUpdateModal(ViPhamSanXuatKinhDoanhPhanBonModel? item)
        {
            try
            {
                _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
                SelectedItem = item?.DeepClone() ?? new ViPhamSanXuatKinhDoanhPhanBonModel();
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

        private void OpenDeleteModal(ViPhamSanXuatKinhDoanhPhanBonModel item)
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
                SelectedItem = new ViPhamSanXuatKinhDoanhPhanBonModel();
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
                SelectedItem = new ViPhamSanXuatKinhDoanhPhanBonModel();
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
                var resultCreate = SelectedItem.id == 0 ? await MainService.CreateAsync(SelectedItem) : new RequestHttpResponse<ViPhamSanXuatKinhDoanhPhanBonModel>();
                var resultUpdate = SelectedItem.id > 0 ? await MainService.UpdateAsync(SelectedItem) : new RequestHttpResponse<bool>();
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
                if(string.IsNullOrEmpty(value))
                {
                    _selectedFilterLoaiToChuc = null;
                    _selectedFilterCoSoSanXuatPhanBon = null;
                    _selectedFilterCoSoDuDieuKienBuonBanPhanBon = null;
                }else{
                    _selectedFilterLoaiToChuc =(LoaiToChuc)Enum.Parse(typeof(LoaiToChuc), value);
                }

                await LoadData();
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi: {ex.Message}", "danger");
            }
        }

        private async Task OnSelectedFilterCoSoDuDieuKienBuonBanPhanBonChanged(CoSoDuDieuKienBuonBanPhanBonModel? coSoDuDieuKienBuonBanPhanBon)
        {
            _selectedFilterCoSoDuDieuKienBuonBanPhanBon = coSoDuDieuKienBuonBanPhanBon;
            _selectedFilterCoSoSanXuatPhanBon = null;
            await LoadData();
        }
        private async Task OnSelectedFilterCoSoSanXuatPhanBonChanged(QuanLyCoSoSanXuatPhanBonModel? coSoSanXuatPhanBon)
        {
            _selectedFilterCoSoSanXuatPhanBon = coSoSanXuatPhanBon;
            _selectedFilterCoSoDuDieuKienBuonBanPhanBon = null;
            await LoadData();
        }

        private void OnSelectedCRUDCoSoSanXuatPhanBonChanged(QuanLyCoSoSanXuatPhanBonModel? coSoSanXuatPhanBon)
        {
            _selectedCRUDCoSoSanXuatPhanBon = coSoSanXuatPhanBon;
            _selectedCRUDCoSoDuDieuKienBuonBanPhanBon = null;
            SelectedItem.co_so_san_xuat_phan_bon = coSoSanXuatPhanBon;
            SelectedItem.co_so_du_dieu_kien_buon_ban_phan_bon = null;
        }

        private void OnSelectedCRUDCoSoDuDieuKienBuonBanPhanBonChanged(CoSoDuDieuKienBuonBanPhanBonModel? coSoDuDieuKienBuonBanPhanBon)   
        {
            _selectedCRUDCoSoDuDieuKienBuonBanPhanBon = coSoDuDieuKienBuonBanPhanBon;
            _selectedCRUDCoSoSanXuatPhanBon = null;
            SelectedItem.co_so_du_dieu_kien_buon_ban_phan_bon = coSoDuDieuKienBuonBanPhanBon;
            SelectedItem.co_so_san_xuat_phan_bon = null;
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
