using CoreAdminWeb.Enums;
using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CoreAdminWeb.Pages.ToChucCaNhanDuDieuKienSanXuatGiong
{
    public partial class ToChucCaNhanDuDieuKienSanXuatGiong(IBaseService<ToChucCaNhanDuDieuKienSanXuatGiongModel> MainService,
                                                IBaseService<TinhModel> TinhThanhService,
                                                IBaseService<XaPhuongModel> XaPhuongService,
                                                IBaseService<CoSoSanXuatGiongModel> CoSoSanXuatGiongService) : BlazorCoreBase
    {
        private List<ToChucCaNhanDuDieuKienSanXuatGiongModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private ToChucCaNhanDuDieuKienSanXuatGiongModel SelectedItem { get; set; } = new ToChucCaNhanDuDieuKienSanXuatGiongModel();
        private string _searchString = "";
        private string _titleAddOrUpdate = "Thêm mới";
        private CoSoSanXuatGiongModel? _selectedCoSoSanXuatGiong = null;


        private List<TinhModel> _tinhThanhFilterList = new();
        private List<XaPhuongModel> _xaPhuongFilterList = new();
        private int? _selectedTinhFilterId = null;
        private int? _selectedXaFilterId = null;

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
            BuilderQuery += $"&filter[_and][][co_so_san_xuat_giong][deleted][_eq]=false";

            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&filter[_and][][co_so_san_xuat_giong][name][_contains]={_searchString}";
            }
            if (_selectedTinhFilterId != null)
            {
                BuilderQuery += $"&filter[_and][][co_so_san_xuat_giong][province][_eq]={_selectedTinhFilterId}";
            }
            if (_selectedXaFilterId != null)
            {
                BuilderQuery += $"&filter[_and][][co_so_san_xuat_giong][ward][_eq]={_selectedXaFilterId}";
            }
            BuilderQuery += $"&filter[_and][][deleted][_eq]=false";

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<ToChucCaNhanDuDieuKienSanXuatGiongModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<ToChucCaNhanDuDieuKienSanXuatGiongModel>();
            }
        }

        private async Task<IEnumerable<CoSoSanXuatGiongModel>> LoadCoSoSanXuatGiongData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, CoSoSanXuatGiongService);
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


        private void OpenDeleteModal(ToChucCaNhanDuDieuKienSanXuatGiongModel item)
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
            SelectedItem = new ToChucCaNhanDuDieuKienSanXuatGiongModel();
            openDeleteModal = false;
        }

        private async Task OpenAddOrUpdateModal(ToChucCaNhanDuDieuKienSanXuatGiongModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item.DeepClone() : new ToChucCaNhanDuDieuKienSanXuatGiongModel();
            _selectedCoSoSanXuatGiong = SelectedItem.co_so_san_xuat_giong;
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
            if (SelectedItem.id == 0)
            {
                var result = await MainService.CreateAsync(SelectedItem);
                if (result.IsSuccess)
                {
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
            SelectedItem = new ToChucCaNhanDuDieuKienSanXuatGiongModel();
            openAddOrUpdateModal = false;
        }

        private void OnCoSoSanXuatGiongChanged(CoSoSanXuatGiongModel? selected)
        {
            SelectedItem.co_so_san_xuat_giong = selected;
            _selectedCoSoSanXuatGiong = selected;
        }



        private async Task LoadTinhThanh()
        {
            string buildQuery = "sort=id";
            var result = await TinhThanhService.GetAllAsync(buildQuery);
            if (result.IsSuccess)
            {
                _tinhThanhFilterList = result.Data ?? new List<TinhModel>();
            }
            else
            {
                _tinhThanhFilterList = new List<TinhModel>();
                AlertService.ShowAlert("Không tìm thấy dữ liệu theo tỉnh thành", "warning");
            }
        }

        private async Task LoadXa(int tinhId)
        {
            string buildQuery = "sort=id";
            if (tinhId > 0)
            {
                buildQuery += $"&filter[_and][][ProvinceId][_eq]={tinhId}";
            }

            var result = await XaPhuongService.GetAllAsync(buildQuery);
            if (result.IsSuccess)
            {
                _xaPhuongFilterList = result.Data ?? new List<XaPhuongModel>();
            }
            else
            {
                _xaPhuongFilterList = new List<XaPhuongModel>();
                AlertService.ShowAlert("Không tìm thấy dữ liệu theo xã", "warning");
            }

        }

        
        public async Task OnTinhFilterChanged(int? value)
        {
            if (value != null && value > 0)
            {
                _selectedTinhFilterId = value.Value;
                _selectedXaFilterId = null;
                await LoadXa(value.Value);
            }
            else
            {
                _selectedTinhFilterId = null;
                _selectedXaFilterId = null;
            }
            await LoadData();

        }

        public async Task OnXaFilterChanged(int? value)
        {
            if (value != null && value > 0)
            {
                _selectedXaFilterId = value;
            }
            else
            {
                _selectedXaFilterId = null;
            }
            await LoadData();
        }

    }
}
