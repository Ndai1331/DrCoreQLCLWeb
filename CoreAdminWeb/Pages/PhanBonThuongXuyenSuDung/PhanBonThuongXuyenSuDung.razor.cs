using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model.PhanBon;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.JSInterop;

namespace CoreAdminWeb.Pages.PhanBonThuongXuyenSuDung
{
    public partial class PhanBonThuongXuyenSuDung(IBaseService<PhanBonModel> PhanBonService,
                                                  IBaseService<TinhModel> TinhThanhService,
                                                  IBaseService<XaPhuongModel> XaPhuongService) : BlazorCoreBase
    {
        private List<PhanBonThuongXuyenSuDungModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private PhanBonThuongXuyenSuDungModel SelectedItem { get; set; } = new PhanBonThuongXuyenSuDungModel();
        private string _searchString = "";
        private string _loaiCayTrongString = "";
        private string _titleAddOrUpdate = "Thêm mới";

        private Status? _selectedFilterStatus { get; set; }
        private TinhModel? _selectedTinhFilter { get; set; }

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
        private async Task<IEnumerable<PhanBonModel>> LoadCRUDPhanBon(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, PhanBonService);
        }

        private async Task<IEnumerable<TinhModel>> LoadTinhData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, TinhThanhService, isIgnoreCheck: true);
        }

        private async Task<IEnumerable<XaPhuongModel>> LoadXaData(string searchText)
        {
            string query = $"&filter[_and][][ProvinceId][_eq]={SelectedItem.province?.id ?? 0}";
            return await LoadBlazorTypeaheadData(searchText, XaPhuongService, isIgnoreCheck: true);
        }

        private string BuildFilterQuery()
        {
            var query = BuildBaseQuery(_searchString);

            if(_selectedTinhFilter != null && _selectedTinhFilter.id > 0)
            {
                query += $"&filter[_and][][province][_eq]={_selectedTinhFilter.id}";
            }
            
            if(!string.IsNullOrEmpty(_loaiCayTrongString))
            {
                query += $"&filter[_and][][loai_cay_trong_ap_dung][_contains]={_loaiCayTrongString}";
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
                    MainModels = result.Data ?? new List<PhanBonThuongXuyenSuDungModel>();
                    if (result.Meta != null)
                    {
                        TotalItems = result.Meta.filter_count ?? 0;
                        TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                    }
                }
                else
                {
                    MainModels = new List<PhanBonThuongXuyenSuDungModel>();
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

        private void OpenAddOrUpdateModal(PhanBonThuongXuyenSuDungModel? item)
        {
            try
            {
                _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
                SelectedItem = item?.DeepClone() ?? new PhanBonThuongXuyenSuDungModel();
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

        private void OpenDeleteModal(PhanBonThuongXuyenSuDungModel item)
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
                SelectedItem = new PhanBonThuongXuyenSuDungModel();
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
                SelectedItem = new PhanBonThuongXuyenSuDungModel();
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

                var resultCreate = SelectedItem.id == 0 ? await MainService.CreateAsync(SelectedItem) : new RequestHttpResponse<PhanBonThuongXuyenSuDungModel>();
                var resultUpdate = SelectedItem.id > 0 ? await MainService.UpdateAsync(SelectedItem) : new RequestHttpResponse<bool>();
                string message =resultCreate.Message ?? resultUpdate.Message;
                if ((SelectedItem.id == 0 && resultCreate.IsSuccess) || (SelectedItem.id > 0 &&resultUpdate.IsSuccess))
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


        private async Task OnSelectedCRUDPhanBonChanged(PhanBonModel? phanBon)   
        {
            SelectedItem.phan_bon = (await PhanBonService.GetByIdAsync($"{phanBon?.id ?? 0}")).Data;
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

        private async Task OnTinhFilterChanged(TinhModel? selected)
        {
            _selectedTinhFilter = selected;

            await LoadData();
        }

        private void OnTinhChanged(TinhModel? selected)
        {
            SelectedItem.province = selected;
            SelectedItem.ward = null;
        }
    }
}
