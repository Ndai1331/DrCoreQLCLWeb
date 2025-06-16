using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model.LoaiPhanBon;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;

namespace CoreAdminWeb.Pages.LoaiPhanBon
{
    public partial class LoaiPhanBon(IBaseService<LoaiPhanBonModel> MainService) : BlazorCoreBase
    {
        private List<LoaiPhanBonModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private LoaiPhanBonModel SelectedItem { get; set; } = new LoaiPhanBonModel();
        private string _searchString = "";
        private string _searchStatusString = "";
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
            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&filter[_and][][name][_contains]={_searchString}";
            }
            if (!string.IsNullOrEmpty(_searchStatusString))
            {
                BuilderQuery += $"&filter[_and][][status][_eq]={_searchStatusString}";
            }
            BuilderQuery += $"&filter[_and][][deleted][_eq]=false";
            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<LoaiPhanBonModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<LoaiPhanBonModel>();
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


        private void OpenDeleteModal(LoaiPhanBonModel item)
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
            SelectedItem = new LoaiPhanBonModel();
            openDeleteModal = false;
        }

        private void OpenAddOrUpdateModal(LoaiPhanBonModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item.DeepClone() : new LoaiPhanBonModel();
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
            SelectedItem = new LoaiPhanBonModel();
            openAddOrUpdateModal = false;
        }
        private async Task OnStatusFilterChanged(ChangeEventArgs? selected)
        {
            _searchStatusString = selected?.Value?.ToString() ?? string.Empty;

            await LoadData();
        }
    }
}
