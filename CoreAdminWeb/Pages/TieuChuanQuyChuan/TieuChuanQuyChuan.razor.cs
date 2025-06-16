using CoreAdminWeb.Enums;
using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CoreAdminWeb.Pages.TieuChuanQuyChuan
{
    public partial class TieuChuanQuyChuan(
        IBaseService<TieuChuanQuyChuanModel> MainService) : BlazorCoreBase
    {
        private List<TieuChuanQuyChuanModel> MainModels { get; set; } = new();
        private List<Enums.TieuChuanQuyChuan> PhanLoaiTieuChuanQuyChuanList = new() { Enums.TieuChuanQuyChuan.TieuChuan, Enums.TieuChuanQuyChuan.QuyChuan };
        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private string _titleAddOrUpdate = "Thêm mới";
        private string _searchString = "";
        private DateTime? _fromDate = null;
        private DateTime? _toDate = null;

        private TieuChuanQuyChuanModel SelectedItem { get; set; } = new TieuChuanQuyChuanModel();

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
            try
            {
                IsLoading = true;
                BuildPaginationQuery(Page, PageSize);
                int intdex =1;

                BuilderQuery += "filter[_and][0][deleted][_eq]=false&sort=sort";
                if (!string.IsNullOrEmpty(_searchString))
                {
                    BuilderQuery += $"&filter[_and][{intdex}][_or][0][code][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{intdex}][_or][1][name][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{intdex}][_or][2][co_quan_cap][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{intdex}][_or][3][pham_vi_ap_dung][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{intdex}][_or][4][noi_dung_ky_thuat_chinh][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{intdex}][_or][5][huong_dan_ap_dung][_contains]={_searchString}";
                    intdex++;
                }

                if(_fromDate != null)
                {
                    BuilderQuery += $"&filter[_and][{intdex}][ngay_ban_hanh][_gte]={_fromDate.Value.ToString("yyyy-MM-dd")}";
                    intdex++;
                }

                if(_toDate != null)
                {
                    BuilderQuery += $"&filter[_and][{intdex}][ngay_ban_hanh][_lte]={_toDate.Value.ToString("yyyy-MM-dd")}";
                    intdex++;
                }
                

                var result = await MainService.GetAllAsync(BuilderQuery);
                if (result.IsSuccess)
                {
                    MainModels = result.Data ?? new List<TieuChuanQuyChuanModel>();
                    if (result.Meta != null)
                    {
                        TotalItems = result.Meta.filter_count ?? 0;
                        TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                    }
                }
                else
                {
                    MainModels = new List<TieuChuanQuyChuanModel>();
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

        private void OpenAddOrUpdateModal(TieuChuanQuyChuanModel? item)
        {
            try
            {
                _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
                SelectedItem = item?.DeepClone() ?? new TieuChuanQuyChuanModel();
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

        private void OpenDeleteModal(TieuChuanQuyChuanModel item)
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
                SelectedItem = new TieuChuanQuyChuanModel();
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
                SelectedItem = new TieuChuanQuyChuanModel();
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

        private async Task OnDateChanged(ChangeEventArgs e, string fieldName)
        {
            try
            {
                var dateStr = e.Value?.ToString();
                if (string.IsNullOrEmpty(dateStr))
                {
                    if (fieldName == "ngay_ban_hanh")
                        SelectedItem.ngay_ban_hanh = null;
                    else if (fieldName == "ngay_hieu_luc")
                        SelectedItem.ngay_hieu_luc = null;
                    else if (fieldName == "ngay_het_han")
                        SelectedItem.ngay_het_han = null;
                    else if (fieldName == "fromDate"){
                        _fromDate = null;
                        await LoadData();
                    }
                    else if (fieldName == "toDate"){
                        _toDate = null;
                        await LoadData();
                    }
                    return;
                }

                var parts = dateStr.Split('/');
                if (parts.Length == 3 && 
                    int.TryParse(parts[0], out int day) && 
                    int.TryParse(parts[1], out int month) && 
                    int.TryParse(parts[2], out int year))
                {
                    var date = new DateTime(year, month, day);
                    
                    if (fieldName == "ngay_ban_hanh")
                        SelectedItem.ngay_ban_hanh = date;
                    else if (fieldName == "ngay_hieu_luc")
                        SelectedItem.ngay_hieu_luc = date;
                    else if (fieldName == "ngay_het_han")
                        SelectedItem.ngay_het_han = date;
                    else if (fieldName == "fromDate"){
                        _fromDate = date;
                        await LoadData();
                    }
                    else if (fieldName == "toDate")
                    {
                        _toDate = date;
                        await LoadData();
                    }
                    
                }
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi khi xử lý ngày: {ex.Message}", "danger");
            }
        }
    }
}
