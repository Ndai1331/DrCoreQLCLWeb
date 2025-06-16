using CoreAdminWeb.Enums;
using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model.NhomThuocBaoVeThucVat;
using CoreAdminWeb.Model.LoaiThuocBaoVeThucVat;
using CoreAdminWeb.Model.ThuocBaoVeThucVat;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CoreAdminWeb.Pages.ThuocBaoVeThucVat
{
    public partial class ThuocBaoVeThucVat(AlertService AlertService,
                                           IBaseService<NhomThuocBaoVeThucVatModel> NhomThuocBaoVeThucVatService,
                                           IBaseService<LoaiThuocBaoVeThucVatModel> LoaiThuocBaoVeThucVatService,
                                           IBaseService<DonViTinhModel> DonViTinhService,
                                           IBaseService<ThuocBaoVeThucVatModel> ThuocBaoVeThucVatService) : BlazorCoreBase
    {
        private List<ThuocBaoVeThucVatModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private ThuocBaoVeThucVatModel? SelectedItem { get; set; }
        private string _searchString = "";
        private string _titleAddOrUpdate = "Thêm mới";
        private NhomThuocBaoVeThucVatModel? _selectedFilterNhomThuocBaoVeThucVat { get; set;  }
        private NhomThuocBaoVeThucVatModel? _selectedCRUDNhomThuocBaoVeThucVat { get; set;  }
        private LoaiThuocBaoVeThucVatModel? _selectedFilterLoaiThuocBaoVeThucVat { get; set;  }
        private LoaiThuocBaoVeThucVatModel? _selectedCRUDLoaiThuocBaoVeThucVat { get; set;  }
        private DonViTinhModel? _selectedCRUDDonViTinh { get; set;  }
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
            var query = "filter[_and][0][deleted][_eq]=false";
            if (!string.IsNullOrEmpty(searchText))
            {
                query += $"&filter[_and][0][_or][0][code][_contains]={searchText}&filter[_and][0][_or][1][name][_contains]={searchText}&filter[_and][0][_or][2][ten_thuong_pham][_contains]={searchText}&filter[_and][0][_or][3][doi_tuong_phong_tru][_contains]={searchText}";
            }

            if(_selectedFilterLoaiThuocBaoVeThucVat != null && _selectedFilterLoaiThuocBaoVeThucVat.id > 0)
            {
                query += $"&filter[_and][1][loai_thuoc_bvtv][_eq]={_selectedFilterLoaiThuocBaoVeThucVat.id}";
            }
            if(_selectedFilterNhomThuocBaoVeThucVat != null && _selectedFilterNhomThuocBaoVeThucVat.id > 0)
            {
                query += $"&filter[_and][2][nhom_thuoc_bvtv][_eq]={_selectedFilterNhomThuocBaoVeThucVat.id}";       
            }

            query += "&sort=sort";
            return query;
        }
        private async Task<IEnumerable<NhomThuocBaoVeThucVatModel>> LoadNhomThuocBaoVeThucVat(string searchText)
        {
            return await LoadBlazorTypeaheadData<NhomThuocBaoVeThucVatModel>(searchText, NhomThuocBaoVeThucVatService);
        }

        private async Task<IEnumerable<LoaiThuocBaoVeThucVatModel>> LoadLoaiThuocBaoVeThucVat(string searchText)
        {
            return await LoadBlazorTypeaheadData<LoaiThuocBaoVeThucVatModel>(searchText, LoaiThuocBaoVeThucVatService);
        }
        private async Task<IEnumerable<DonViTinhModel>> LoadDonViTinh(string searchText)
        {
            return await LoadBlazorTypeaheadData<DonViTinhModel>(searchText, DonViTinhService);
        }

        private async Task LoadData()
        {
            try
            {
                IsLoading = true;
                BuildPaginationQuery(Page, PageSize);
                BuilderQuery += BuildBaseQuery(_searchString);

                var result = await MainService.GetAllAsync(BuilderQuery);
                if (result.IsSuccess)
                {
                    MainModels = result.Data;
                    if (result.Meta != null)
                    {
                        TotalItems = result.Meta.filter_count ?? 0;
                        TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                    }
                }
                else
                {
                    MainModels = new List<ThuocBaoVeThucVatModel>();
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

        private async Task OpenAddOrUpdateModal(ThuocBaoVeThucVatModel? item)
        {
            try
            {
                _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
                SelectedItem = item?.DeepClone() ?? new ThuocBaoVeThucVatModel();
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

        private void OpenDeleteModal(ThuocBaoVeThucVatModel item)
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
                SelectedItem = null;
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
                SelectedItem = null;
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

                var resultCreate = SelectedItem.id == 0 ? await MainService.CreateAsync(SelectedItem) : new RequestHttpResponse<ThuocBaoVeThucVatModel>();
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


        private async Task OnSelectedCRUDNhomThuocBaoVeThucVatChanged(NhomThuocBaoVeThucVatModel? nhomThuocBaoVeThucVat)   
        {
            _selectedCRUDNhomThuocBaoVeThucVat = nhomThuocBaoVeThucVat;
            SelectedItem.nhom_thuoc_bvtv = nhomThuocBaoVeThucVat;
        }

        private async Task OnSelectedCRUDLoaiThuocBaoVeThucVatChanged(LoaiThuocBaoVeThucVatModel? loaiThuocBaoVeThucVat)   
        {
            _selectedCRUDLoaiThuocBaoVeThucVat = loaiThuocBaoVeThucVat;
            SelectedItem.loai_thuoc_bvtv = loaiThuocBaoVeThucVat;
        }
        private async Task OnSelectedCRUDDonViTinhChanged(DonViTinhModel? donViTinh)   
        {
            _selectedCRUDDonViTinh = donViTinh;
            SelectedItem.don_vi_tinh = donViTinh;
        }
        private async Task OnSelectedFilterNhomThuocBaoVeThucVatChanged(NhomThuocBaoVeThucVatModel? nhomThuocBaoVeThucVat)   
        {
            _selectedFilterNhomThuocBaoVeThucVat = nhomThuocBaoVeThucVat;
            await LoadData();   
        }

        private async Task OnSelectedFilterLoaiThuocBaoVeThucVatChanged(LoaiThuocBaoVeThucVatModel? loaiThuocBaoVeThucVat)   
        {
            _selectedFilterLoaiThuocBaoVeThucVat = loaiThuocBaoVeThucVat;
            await LoadData();
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
    }
}
