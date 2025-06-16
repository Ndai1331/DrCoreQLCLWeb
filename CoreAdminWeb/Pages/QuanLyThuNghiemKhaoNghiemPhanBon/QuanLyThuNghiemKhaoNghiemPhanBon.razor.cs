using CoreAdminWeb.Enums;
using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Model.KhaoNghiemPhanBon;
using CoreAdminWeb.Model.PhanBon;
using CoreAdminWeb.Model.QuanLyCoSoSanXuatPhanBon;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CoreAdminWeb.Pages.QuanLyThuNghiemKhaoNghiemPhanBon
{
    public partial class QuanLyThuNghiemKhaoNghiemPhanBon(IBaseService<KhaoNghiemPhanBonModel> MainService,
                                                          IBaseService<QuanLyCoSoSanXuatPhanBonModel> CoSoSanXuatPhanBonService,
                                                          IBaseService<PhanBonModel> PhanBonService,
                                                          IBaseService<TinhModel> TinhThanhService,
                                                          IBaseService<XaPhuongModel> XaPhuongService) : BlazorCoreBase
    {
        private static List<string> _loaihinhKhaoNghiemList = new List<string>() {
            LoaiHinhKhaoNghiem.NhaNuoc.ToString(),
            LoaiHinhKhaoNghiem.DoanhNghiep.ToString(),
            LoaiHinhKhaoNghiem.VienNghienCuu.ToString(),
            LoaiHinhKhaoNghiem.Khac.ToString()
        };

        private List<KhaoNghiemPhanBonModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private KhaoNghiemPhanBonModel SelectedItem { get; set; } = new KhaoNghiemPhanBonModel();
        private string _searchString = "";
        private TinhModel? _selectedTinhFilter { get; set; } = null;
        private XaPhuongModel? _selectedXaFilter { get; set; } = null;
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
            if (_selectedTinhFilter?.id > 0)
            {
                BuilderQuery += $"&filter[_and][][co_so_san_xuat_phan_bon][province][_eq]={_selectedTinhFilter.id}";
            }
            if (_selectedXaFilter?.id > 0)
            {
                BuilderQuery += $"&filter[_and][][co_so_san_xuat_phan_bon][ward][_eq]={_selectedXaFilter.id}";
            }
            BuilderQuery += $"&filter[_and][][deleted][_eq]=false";

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<KhaoNghiemPhanBonModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<KhaoNghiemPhanBonModel>();
            }
        }

        private async Task<IEnumerable<QuanLyCoSoSanXuatPhanBonModel>> LoadCoSoSanXuatPhanBonData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, CoSoSanXuatPhanBonService);
        }

        private async Task<IEnumerable<PhanBonModel>> LoadPhanBonData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, PhanBonService);
        }

        private async Task<IEnumerable<TinhModel>> LoadTinhData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, TinhThanhService, isIgnoreCheck: true);
        }
        private async Task<IEnumerable<XaPhuongModel>> LoadXaFilterData(string searchText)
        {
            string query = $"&filter[_and][][ProvinceId][_eq]={_selectedTinhFilter?.id ?? 0}";
            return await LoadBlazorTypeaheadData(searchText, XaPhuongService, otherQuery: query, isIgnoreCheck: true);
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


        private void OpenDeleteModal(KhaoNghiemPhanBonModel item)
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
            SelectedItem = new KhaoNghiemPhanBonModel();
            openDeleteModal = false;
        }

        private async Task OpenAddOrUpdateModal(KhaoNghiemPhanBonModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item.DeepClone() : new KhaoNghiemPhanBonModel();

            if (SelectedItem.phan_bon != null)
                await LoadPhanBonInfoAsync(SelectedItem.phan_bon, false);

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
            SelectedItem = new KhaoNghiemPhanBonModel();
            openAddOrUpdateModal = false;
        }

        private async Task OnPhanBonChanged(PhanBonModel selected)
        {
            SelectedItem.phan_bon = selected;
            if (selected != null)
                await LoadPhanBonInfoAsync(selected);

            StateHasChanged();
        }

        private void OnCoSoSanXuatPhanBonChanged(QuanLyCoSoSanXuatPhanBonModel selected)
        {
            SelectedItem.co_so_san_xuat_phan_bon = selected;
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
        private async Task LoadPhanBonInfoAsync(PhanBonModel selected, bool isForceUpdate = true)
        {
            SelectedItem.phan_bon = (await PhanBonService.GetByIdAsync(selected.id.ToString())).Data;
            if (SelectedItem.phan_bon != null)
            {
                if (isForceUpdate || SelectedItem.co_so_san_xuat_phan_bon == null)
                    SelectedItem.co_so_san_xuat_phan_bon = SelectedItem.phan_bon.co_so_san_xuat_phan_bon;

                if (isForceUpdate || string.IsNullOrEmpty(SelectedItem.thanh_phan_cong_thuc))
                    SelectedItem.thanh_phan_cong_thuc = SelectedItem.phan_bon.thanh_phan_ham_luong;
            }
        }
        private void OnDateChanged(ChangeEventArgs e, string fieldName)
        {
            try
            {
                var dateStr = e.Value?.ToString();
                if (string.IsNullOrEmpty(dateStr))
                {
                    switch (fieldName)
                    {
                        case nameof(SelectedItem.ngay_bat_dau):
                            SelectedItem.ngay_bat_dau = null;
                            break;
                        case nameof(SelectedItem.ngay_ket_thuc):
                            SelectedItem.ngay_ket_thuc = null;
                            break;
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

                    switch (fieldName)
                    {
                        case nameof(SelectedItem.ngay_bat_dau):
                            SelectedItem.ngay_bat_dau = date;
                            break;
                        case nameof(SelectedItem.ngay_ket_thuc):
                            SelectedItem.ngay_ket_thuc = date;
                            break;
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
