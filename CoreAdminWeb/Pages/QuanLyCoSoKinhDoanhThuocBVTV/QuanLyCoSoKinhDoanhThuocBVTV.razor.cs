using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;

namespace CoreAdminWeb.Pages.QuanLyCoSoKinhDoanhThuocBVTV
{
    public partial class QuanLyCoSoKinhDoanhThuocBVTV(
                                           IBaseService<TinhModel> TinhThanhService,
                                           IBaseService<XaPhuongModel> XaPhuongService,
                                           IBaseService<QLCLLoaiHinhKinhDoanhModel> QLCLLoaiHinhKinhDoanhService) : BlazorCoreBase
    {

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

        private async Task<IEnumerable<TinhModel>> LoadTinhData(string stringSearch)
        {
            return await LoadBlazorTypeaheadData(stringSearch, TinhThanhService, isIgnoreCheck: true);
        }
        private async Task<IEnumerable<XaPhuongModel>> LoadXaFilterData(string stringSearch)
        {
            string buildQuery = $"filter[_and][][ProvinceId][_eq]={_selectedTinhFilter?.id ?? 0}";
            return await LoadBlazorTypeaheadData(stringSearch, XaPhuongService, otherQuery: buildQuery, isIgnoreCheck: true);
        }
        private async Task<IEnumerable<XaPhuongModel>> LoadXaData(string stringSearch)
        {
            string buildQuery = $"filter[_and][][ProvinceId][_eq]={SelectedItem.province?.id ?? 0}";
            return await LoadBlazorTypeaheadData(stringSearch, XaPhuongService, otherQuery: buildQuery, isIgnoreCheck: true);
        }
        private async Task<IEnumerable<QLCLLoaiHinhKinhDoanhModel>> LoadQLCLLoaiHinhKinhDoanhData(string stringSearch)
        {
            return await LoadBlazorTypeaheadData(stringSearch, QLCLLoaiHinhKinhDoanhService);
        }

        public async Task OnTinhFilterChanged(TinhModel? e)
        {
            _selectedTinhFilter = e;
            _selectedXaFilter = null;

            await LoadData();
        }

        public async Task OnXaFilterChanged(XaPhuongModel e)
        {
            _selectedXaFilter = e;

            await LoadData();
        }
        
        public void OnTinhChanged(TinhModel? e)
        {
            SelectedItem.province = e;
            SelectedItem.ward = null;
        }

        public void OnXaChanged(XaPhuongModel e)
        {
            SelectedItem.ward = e;
        }

        private void OpenAddOrUpdateModal(QuanLyCoSoKinhDoanhThuocBVTVModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item.DeepClone() : new QuanLyCoSoKinhDoanhThuocBVTVModel();

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

    }
}
