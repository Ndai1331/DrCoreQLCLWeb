using CoreAdminWeb.Enums;
using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Model.CoSoBuonBan;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CoreAdminWeb.Pages.QuanLyCoSoBuonBan
{
    public partial class QuanLyCoSoBuonBan(IBaseService<CoSoDuDieuKienBuonBanPhanBonModel> MainService,
                                           IBaseService<TinhModel> TinhThanhService,
                                           IBaseService<XaPhuongModel> XaPhuongService,
                                           IBaseService<QLCLLoaiHinhKinhDoanhModel> QLCLLoaiHinhKinhDoanhService) : BlazorCoreBase
    {
        private static List<int> NhomCoSoList = new List<int>() { (int)NhomCoSo.toChuc, (int)NhomCoSo.caNhan, (int)NhomCoSo.khac };

        private List<CoSoDuDieuKienBuonBanPhanBonModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private CoSoDuDieuKienBuonBanPhanBonModel SelectedItem { get; set; } = new CoSoDuDieuKienBuonBanPhanBonModel();
        private string _searchString = "";
        private string _titleAddOrUpdate = "Thêm mới";

        private TinhModel? _selectedTinhFilter { get; set; }
        private XaPhuongModel? _selectedXaFilter { get; set; }

        private string activeDefTab = "tab1";
        private bool isFistChangeTab2 = false;
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
            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&filter[_and][][name][_contains]={_searchString}";
            }
            if (_selectedTinhFilter != null && _selectedTinhFilter.id > 0)
            {
                BuilderQuery += $"&filter[_and][][tinh_thanh][_eq]={_selectedTinhFilter.id}";
            }

            if (_selectedXaFilter != null && _selectedXaFilter.id > 0)
            {
                BuilderQuery += $"&filter[_and][][xa_phuong][_eq]={_selectedXaFilter.id}";
            }
            BuilderQuery += $"&filter[_and][][deleted][_eq]=false";

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<CoSoDuDieuKienBuonBanPhanBonModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<CoSoDuDieuKienBuonBanPhanBonModel>();
            }
        }

        private async Task<IEnumerable<TinhModel>> LoadTinhFilterData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, TinhThanhService, isIgnoreCheck: true);
        }

        private async Task<IEnumerable<XaPhuongModel>> LoadXaFilterData(string searchText)
        {
            string query = $"&filter[_and][][ProvinceId][_eq]={_selectedTinhFilter?.id ?? 0}";
            return await LoadBlazorTypeaheadData(searchText, XaPhuongService, query, isIgnoreCheck: true);
        }
        
        private async Task<IEnumerable<TinhModel>> LoadTinhData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, TinhThanhService, isIgnoreCheck: true);
        }

        private async Task<IEnumerable<XaPhuongModel>> LoadXaData(string searchText)
        {
            string query = $"&filter[_and][][ProvinceId][_eq]={SelectedItem.province?.id ?? 0}";
            return await LoadBlazorTypeaheadData(searchText, XaPhuongService, query, isIgnoreCheck: true);
        }

        private async Task<IEnumerable<QLCLLoaiHinhKinhDoanhModel>> LoadQLCLLoaiHinhKinhDoanhData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, QLCLLoaiHinhKinhDoanhService);
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


        private void OpenDeleteModal(CoSoDuDieuKienBuonBanPhanBonModel item)
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
            SelectedItem = new CoSoDuDieuKienBuonBanPhanBonModel();
            openDeleteModal = false;
        }

        private async Task OpenAddOrUpdateModal(CoSoDuDieuKienBuonBanPhanBonModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item.DeepClone() : new CoSoDuDieuKienBuonBanPhanBonModel();

            if (SelectedItem.province == null)
                SelectedItem.province = (await TinhThanhService.GetByIdAsync("1")).Data;

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
            SelectedItem = new CoSoDuDieuKienBuonBanPhanBonModel();
            openAddOrUpdateModal = false;
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

        private void OnTinhChanged(TinhModel? selected)
        {
            SelectedItem.province = selected;
            SelectedItem.ward = null;
        }

        private void OnXaChanged(XaPhuongModel? selected)
        {
            SelectedItem.ward = selected;
        }

        private void OnLoaihinhKinhDoanhChanged(QLCLLoaiHinhKinhDoanhModel? selected)
        {
            SelectedItem.loai_hinh_kinh_doanh = selected;
        }

        private void OnDateChanged(ChangeEventArgs e, string fieldName)
        {
            try
            {
                var dateStr = e.Value?.ToString();
                if (string.IsNullOrEmpty(dateStr))
                {
                    if (fieldName == "ngay_cap")
                        SelectedItem.ngay_cap = null;
                    return;
                }

                var parts = dateStr.Split('/');
                if (parts.Length == 3 &&
                    int.TryParse(parts[0], out int day) &&
                    int.TryParse(parts[1], out int month) &&
                    int.TryParse(parts[2], out int year))
                {
                    var date = new DateTime(year, month, day);

                    if (fieldName == "ngay_cap")
                        SelectedItem.ngay_cap = date;
                }
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi khi xử lý ngày: {ex.Message}", "danger");
            }
        }

        private void OnTabChanged(string tab)
        {
            activeDefTab = tab;

            if (activeDefTab == "tab2")
            {
                // Wait for modal to render
                _ = Task.Run(async () =>
                {
                    await Task.Delay(500);
                    await JsRuntime.InvokeVoidAsync("initializeDatePicker");
                });
            }
        }
    }
}
