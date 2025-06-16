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

namespace CoreAdminWeb.Pages.CoSoViPhamTrongLinhVucCheBien
{
    public partial class CoSoViPhamTrongLinhVucCheBien(
        IBaseService<CoSoViPhamTrongLinhVucCheBienModel> MainService,
        IBaseService<CoSoSanXuatCheBienModel> CoSoSanXuatCheBienService) : BlazorCoreBase
    {
        private List<CoSoViPhamTrongLinhVucCheBienModel> MainModels { get; set; } = new();
        private List<TrangThaiXuLy> TrangThaiXuLyList = new() { TrangThaiXuLy.ChuaXuLy, TrangThaiXuLy.DangXuLy, TrangThaiXuLy.DaXuLy };
        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private CoSoViPhamTrongLinhVucCheBienModel SelectedItem { get; set; } = new CoSoViPhamTrongLinhVucCheBienModel();
        private TrangThaiXuLy? _selectedFilterTrangThaiXuLy { get; set;  }
        private CoSoSanXuatCheBienModel? _selectedFilterCoSoSanXuatCheBien { get; set;  }
        private string _searchString = "";
        private string _titleAddOrUpdate = "Thêm mới";
        
        private CoSoSanXuatCheBienModel? _selectedCRUDCoSoSanXuatCheBien { get; set;  }

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

        private async Task<IEnumerable<CoSoSanXuatCheBienModel>> LoadCoSoSanXuatCheBien(string searchText)
        {
            return await LoadBlazorTypeaheadData<CoSoSanXuatCheBienModel>(searchText, CoSoSanXuatCheBienService);
        }
        private async Task<IEnumerable<CoSoSanXuatCheBienModel>> LoadCRUDCoSoSanXuatCheBien(string searchText)
        {
            return await LoadBlazorTypeaheadData<CoSoSanXuatCheBienModel>(searchText, CoSoSanXuatCheBienService);
        }

        private async Task LoadData()
        {
            try
            {
                IsLoading = true;
                BuildPaginationQuery(Page, PageSize);
                int intdex =1;

                BuilderQuery += "filter[_and][0][deleted][_eq]=false&sort=sort";
                if (!string.IsNullOrEmpty(_searchString))
                {
                    BuilderQuery += $"&filter[_and][{intdex}][_or][0][noi_dung_vi_pham][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{intdex}][_or][1][doi_tuong_vi_pham][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{intdex}][_or][2][hinh_thuc_xu_ly][_contains]={_searchString}";
                    intdex++;
                }

                if(_selectedFilterCoSoSanXuatCheBien != null)
                {
                    BuilderQuery += $"&filter[_and][{intdex}][co_so_vi_pham][_eq]={_selectedFilterCoSoSanXuatCheBien.id}";
                    intdex++;
                }
                
                if(_selectedFilterTrangThaiXuLy != null)
                {
                    BuilderQuery += $"&filter[_and][{intdex}][status][_eq]={_selectedFilterTrangThaiXuLy.GetHashCode()}";
                }

                var result = await MainService.GetAllAsync(BuilderQuery);
                if (result.IsSuccess)
                {
                    MainModels = result.Data ?? new List<CoSoViPhamTrongLinhVucCheBienModel>();
                    if (result.Meta != null)
                    {
                        TotalItems = result.Meta.filter_count ?? 0;
                        TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                    }
                }
                else
                {
                    MainModels = new List<CoSoViPhamTrongLinhVucCheBienModel>();
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

        private void OpenAddOrUpdateModal(CoSoViPhamTrongLinhVucCheBienModel? item)
        {
            try
            {
                _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
                SelectedItem = item?.DeepClone() ?? new CoSoViPhamTrongLinhVucCheBienModel();
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

        private void OpenDeleteModal(CoSoViPhamTrongLinhVucCheBienModel item)
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
                SelectedItem = new CoSoViPhamTrongLinhVucCheBienModel();
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
                SelectedItem = new CoSoViPhamTrongLinhVucCheBienModel();
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
                var resultCreate = SelectedItem.id == 0 ? await MainService.CreateAsync(SelectedItem) : new();
                var resultUpdate = SelectedItem.id > 0 ? await MainService.UpdateAsync(SelectedItem) : new();
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

        private async Task OnSelectedFilterCoSoSanXuatCheBienChanged(CoSoSanXuatCheBienModel? coSoSanXuatCheBien)
        {
            _selectedFilterCoSoSanXuatCheBien = coSoSanXuatCheBien;
            await LoadData();
        }

        private void OnSelectedCRUDCoSoSanXuatCheBienChanged(CoSoSanXuatCheBienModel? coSoSanXuatCheBien)   
        {
            _selectedCRUDCoSoSanXuatCheBien = coSoSanXuatCheBien;
            SelectedItem.co_so_vi_pham= coSoSanXuatCheBien;
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
