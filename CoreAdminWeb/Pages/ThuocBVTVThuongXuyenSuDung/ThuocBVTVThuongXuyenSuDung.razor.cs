using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Model.ThuocBaoVeThucVat;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CoreAdminWeb.Pages.ThuocBVTVThuongXuyenSuDung
{
    public partial class ThuocBVTVThuongXuyenSuDung(IBaseService<ThuocBaoVeThucVatModel> ThuocBVTVService,
                                           IBaseService<TinhModel> TinhThanhService,
                                           IBaseService<XaPhuongModel> XaPhuongService) : BlazorCoreBase
    {
        private List<ThuocBVTVThuongXuyenSuDungModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private ThuocBVTVThuongXuyenSuDungModel SelectedItem { get; set; } = new ThuocBVTVThuongXuyenSuDungModel();
        private string _searchString = "";
        private string _titleAddOrUpdate = "Thêm mới";
        private TinhModel? _selectedTinhFilter { get; set; }
        private XaPhuongModel? _selectedXaFilter { get; set; }
        private Status? _selectedFilterStatus { get; set; }
        private ThuocBaoVeThucVatModel? _selectedCRUDThuocBaoVeThucVat { get; set; }
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
                query += $"&filter[_and][][phan_bon.name][_contains]={searchText}";
            }
            return query;
        }
        private async Task<IEnumerable<ThuocBaoVeThucVatModel>> LoadCRUDThuocBaoVeThucVat(string searchText)
        {
            return await LoadBlazorTypeaheadData<ThuocBaoVeThucVatModel>(searchText, ThuocBVTVService);
        }

        private string BuildFilterQuery()
        {
            var query = BuildBaseQuery(_searchString);

            if (_selectedCRUDThuocBaoVeThucVat?.id > 0)
            {
                query += $"&filter[_and][][thuoc_bvtv][_eq]={_selectedCRUDThuocBaoVeThucVat.id}";
            }

            if (_selectedTinhFilter?.id > 0)
            {
                BuilderQuery += $"&filter[_and][][province][_eq]={_selectedTinhFilter?.id}";
            }
            if (_selectedXaFilter?.id > 0)
            {
                BuilderQuery += $"&filter[_and][][ward][_eq]={_selectedXaFilter?.id}";
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
                    MainModels = result.Data ?? new List<ThuocBVTVThuongXuyenSuDungModel>();
                    if (result.Meta != null)
                    {
                        TotalItems = result.Meta.filter_count ?? 0;
                        TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                    }
                }
                else
                {
                    MainModels = new List<ThuocBVTVThuongXuyenSuDungModel>();
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

        private void OpenAddOrUpdateModal(ThuocBVTVThuongXuyenSuDungModel? item)
        {
            try
            {
                _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
                SelectedItem = item?.DeepClone() ?? new ThuocBVTVThuongXuyenSuDungModel();
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

        private void OpenDeleteModal(ThuocBVTVThuongXuyenSuDungModel item)
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
                SelectedItem = new ThuocBVTVThuongXuyenSuDungModel();
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
                SelectedItem = new ThuocBVTVThuongXuyenSuDungModel();
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

                var resultCreate = SelectedItem.id == 0 ? await MainService.CreateAsync(SelectedItem) : new RequestHttpResponse<ThuocBVTVThuongXuyenSuDungModel>();
                var resultUpdate = SelectedItem.id > 0 ? await MainService.UpdateAsync(SelectedItem) : new RequestHttpResponse<bool>();
                string message = resultCreate.Message ?? resultUpdate.Message;
                if ((SelectedItem.id == 0 && resultCreate.IsSuccess) || (SelectedItem.id > 0 && resultUpdate.IsSuccess))
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


        private async Task OnTrangThaiFilterChanged(ChangeEventArgs e)
        {
            try
            {
                var value = e.Value?.ToString();
                _selectedFilterStatus = !string.IsNullOrEmpty(value)
                    ? (Status)Enum.Parse(typeof(Status), value)
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

        #region DVHC Filter
        private async Task<IEnumerable<TinhModel>> LoadTinhData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, TinhThanhService, isIgnoreCheck: true);
        }
        private async Task<IEnumerable<XaPhuongModel>> LoadFilterXaData(string searchText)
        {
            var queryBuilder = $"&filter[_and][][ProvinceId][_eq]={_selectedTinhFilter?.id}";
            return await LoadBlazorTypeaheadData(searchText, XaPhuongService, isIgnoreCheck: true);
        }
        public async Task OnTinhFilterChanged(TinhModel? value)
        {
            _selectedTinhFilter = value;
            _selectedXaFilter = null;
            await LoadData();
        }

        public async Task OnXaFilterChanged(XaPhuongModel? value)
        {
            _selectedXaFilter = value;
            await LoadData();
        }
        #endregion

        #region CRUD Filter
        private async Task<IEnumerable<XaPhuongModel>> LoadXaData(string searchText)
        {
            var queryBuilder = $"&filter[_and][][ProvinceId][_eq]={SelectedItem.province?.id}";
            return await LoadBlazorTypeaheadData(searchText, XaPhuongService, isIgnoreCheck: true);
        }

        public void OnTinhChanged(TinhModel? e)
        {
            SelectedItem.province = e;
            SelectedItem.ward = null;
        }

        public void OnXaChanged(XaPhuongModel? e)
        {
            SelectedItem.ward = e;
        }
        #endregion
    }
}
