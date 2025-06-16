using CoreAdminWeb.Enums;
using CoreAdminWeb.Model;
using CoreAdminWeb.Model.CayTrongChinh;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CoreAdminWeb.Pages.CayTrongChinh
{
    public partial class CayTrongChinh(IBaseService<CayTrongChinhModel> MainService,
                                              IBaseService<CayGiongCayTrongModel> CayGiongCayTrongService,
                                              IBaseService<LoaiCayTrongModel> LoaiCayTrongService) : BlazorCoreBase
    {
        private static List<NguonGoc> _nguonGoc = new List<NguonGoc>() { 
            NguonGoc.NoiDia, 
            NguonGoc.NhapKhau,
            NguonGoc.Khac 
        };

        private List<CayTrongChinhModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;

        private CayTrongChinhModel SelectedItem { get; set; } = new CayTrongChinhModel();

        private string _searchString = "";
        private LoaiCayTrongModel? _selectedLoaiCayTrongFilter { get; set; }
        private int _searchNguonGoc = 0;
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
                BuilderQuery += $"&filter[_and][0][_or][0][cay_giong_cay_trong][code][_contains]={_searchString}&filter[_and][0][_or][1][cay_giong_cay_trong][name][_contains]={_searchString}";
            }
            if (_selectedLoaiCayTrongFilter?.id > 0)
            {
                BuilderQuery += $"&filter[cay_giong_cay_trong][loai_cay_trong][_eq]={_selectedLoaiCayTrongFilter?.id}";
            }
            if (_searchNguonGoc > 0)
            {
                BuilderQuery += $"&filter[cay_giong_cay_trong][nguon_goc][_eq]={_searchNguonGoc}";
            }

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<CayTrongChinhModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<CayTrongChinhModel>();
            }
        }

        private async Task<IEnumerable<CayGiongCayTrongModel>> LoadCayGiongCayTrongData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, CayGiongCayTrongService);
        }

        private async Task<IEnumerable<LoaiCayTrongModel>> LoadLoaiCayTrongData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, LoaiCayTrongService);
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

        private void OpenDeleteModal(CayTrongChinhModel item)
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
            SelectedItem = new CayTrongChinhModel();

            openDeleteModal = false;
        }

        private async Task OpenAddOrUpdateModal(CayTrongChinhModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item : new CayTrongChinhModel();

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
            SelectedItem = new CayTrongChinhModel();
            openAddOrUpdateModal = false;
        }
        private void OnCayGiongCayTrongChanged(CayGiongCayTrongModel? selected)
        {
            SelectedItem.cay_giong_cay_trong = selected;
        }

        private async Task OnLoaiCayTrongFilterChanged(LoaiCayTrongModel? selected)
        {
            _selectedLoaiCayTrongFilter = selected;

            await LoadData();
        }

        private async Task OnNguonGocChanged(ChangeEventArgs? selected)
        {
            if (int.TryParse(selected?.Value?.ToString(), out int intValue))
            {
                _searchNguonGoc = intValue;
            }

            await LoadData();
        }
    }
}
