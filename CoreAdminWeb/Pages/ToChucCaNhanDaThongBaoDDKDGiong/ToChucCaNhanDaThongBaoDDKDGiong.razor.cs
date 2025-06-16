using CoreAdminWeb.Model;
using CoreAdminWeb.Model.ToChucCaNhanDaThongBaoDDKDGiong;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;

namespace CoreAdminWeb.Pages.ToChucCaNhanDaThongBaoDDKDGiong
{
    public partial class ToChucCaNhanDaThongBaoDDKDGiong(IBaseService<ToChucCaNhanDaThongBaoDDKDGiongModel> MainService,
                                              IBaseService<CoSoSanXuatGiongModel> CoSoSanXuatGiongService,
                                              IBaseService<TinhModel> TinhThanhService,
                                              IBaseService<XaPhuongModel> XaPhuongService) : BlazorCoreBase
    {
        private List<ToChucCaNhanDaThongBaoDDKDGiongModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;

        private ToChucCaNhanDaThongBaoDDKDGiongModel SelectedItem { get; set; } = new ToChucCaNhanDaThongBaoDDKDGiongModel();

        private string _searchString = "";
        private TinhModel? _selectedTinhFilter { get; set; }
        private XaPhuongModel? _selectedXaFilter { get; set; }
        private string _titleAddOrUpdate = "Thêm mới";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadData();
                StateHasChanged();
            }
        }

        private async Task LoadData()
        {
            BuildPaginationQuery(Page, PageSize);

            BuilderQuery += $"&filter[deleted][_eq]=false";

            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&filter[_and][0][_or][0][co_so_san_xuat_giong][code][_contains]={_searchString}&filter[_and][0][_or][1][co_so_san_xuat_giong][name][_contains]={_searchString}";
            }
            if (_selectedTinhFilter?.id > 0)
            {
                BuilderQuery += $"&filter[co_so_san_xuat_giong][province][_eq]={_selectedTinhFilter?.id}";
            }
            if (_selectedXaFilter?.id > 0)
            {
                BuilderQuery += $"&filter[co_so_san_xuat_giong][ward][_eq]={_selectedXaFilter?.id}";
            }

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<ToChucCaNhanDaThongBaoDDKDGiongModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<ToChucCaNhanDaThongBaoDDKDGiongModel>();
            }
        }

        private async Task<IEnumerable<CoSoSanXuatGiongModel>> LoadCoSoSanXuatGiongData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, CoSoSanXuatGiongService);
        }

        private async Task<IEnumerable<TinhModel>> LoadTinhFilterData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, TinhThanhService, isIgnoreCheck: true);
        }
        
        private async Task<IEnumerable<XaPhuongModel>> LoadXaFilterData(string searchText)
        {
            string query = $"&filter[_and][][ProvinceId][_eq]={_selectedTinhFilter?.id ?? 0}";
            return await LoadBlazorTypeaheadData(searchText, XaPhuongService, isIgnoreCheck: true);
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

        private void OpenDeleteModal(ToChucCaNhanDaThongBaoDDKDGiongModel item)
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
            SelectedItem = new ToChucCaNhanDaThongBaoDDKDGiongModel();

            openDeleteModal = false;
        }

        private async Task OpenAddOrUpdateModal(ToChucCaNhanDaThongBaoDDKDGiongModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item : new ToChucCaNhanDaThongBaoDDKDGiongModel();

            openAddOrUpdateModal = true;
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
            SelectedItem = new ToChucCaNhanDaThongBaoDDKDGiongModel();
            openAddOrUpdateModal = false;
        }
        
        private void OnCoSoSanXuatGiongChanged(CoSoSanXuatGiongModel? selected)
        {
            SelectedItem.co_so_san_xuat_giong = selected;
        }
        
        private async Task OnTinhFilterChanged(TinhModel? selected)
        {
            _selectedTinhFilter = selected;
            _selectedXaFilter = null;

            await LoadData();
        }

        private async Task OnXaFilterChanged(XaPhuongModel? selected)
        {
            _selectedXaFilter = selected;
            await LoadData();
        }
    }
}
