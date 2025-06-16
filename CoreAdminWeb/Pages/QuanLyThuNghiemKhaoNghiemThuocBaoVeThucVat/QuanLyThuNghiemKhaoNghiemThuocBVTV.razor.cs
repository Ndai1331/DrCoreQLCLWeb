using CoreAdminWeb.Enums;
using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Model.KhaoNghiemThuocBaoVeThucVat;
using CoreAdminWeb.Model.ThuocBaoVeThucVat;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CoreAdminWeb.Pages.QuanLyThuNghiemKhaoNghiemThuocBaoVeThucVat
{
    public partial class QuanLyThuNghiemKhaoNghiemThuocBVTV(IBaseService<KhaoNghiemThuocBaoVeThucVatModel> MainService,
                                                            IBaseService<QuanLyCoSoSanXuatThuocBVTVModel> CoSoSanXuatThuocBVTVService,
                                                            IBaseService<ThuocBaoVeThucVatModel> ThuocBVTVService,
                                                            IBaseService<TinhModel> TinhThanhService,
                                                            IBaseService<XaPhuongModel> XaPhuongService) : BlazorCoreBase
    {
        private static List<string> _loaihinhKhaoNghiemList = new List<string>() {
            LoaiHinhKhaoNghiem.NhaNuoc.ToString(),
            LoaiHinhKhaoNghiem.DoanhNghiep.ToString(),
            LoaiHinhKhaoNghiem.VienNghienCuu.ToString(),
            LoaiHinhKhaoNghiem.Khac.ToString()
        };

        private List<KhaoNghiemThuocBaoVeThucVatModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private KhaoNghiemThuocBaoVeThucVatModel SelectedItem { get; set; } = new KhaoNghiemThuocBaoVeThucVatModel();
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
                BuilderQuery += $"&filter[_and][][co_so_san_xuat_thuoc_bvtv][province][_eq]={_selectedTinhFilter.id}";
            }
            if (_selectedXaFilter?.id > 0)
            {
                BuilderQuery += $"&filter[_and][][co_so_san_xuat_thuoc_bvtv][ward][_eq]={_selectedXaFilter.id}";
            }
            BuilderQuery += $"&filter[_and][][deleted][_eq]=false";

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<KhaoNghiemThuocBaoVeThucVatModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<KhaoNghiemThuocBaoVeThucVatModel>();
            }
        }

        private async Task<IEnumerable<QuanLyCoSoSanXuatThuocBVTVModel>> LoadCoSoSanXuatThuocBVTVData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, CoSoSanXuatThuocBVTVService);
        }

        private async Task<IEnumerable<ThuocBaoVeThucVatModel>> LoadThuocBVTVData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, ThuocBVTVService);
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


        private void OpenDeleteModal(KhaoNghiemThuocBaoVeThucVatModel item)
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
            SelectedItem = new KhaoNghiemThuocBaoVeThucVatModel();
            openDeleteModal = false;
        }

        private async Task OpenAddOrUpdateModal(KhaoNghiemThuocBaoVeThucVatModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item.DeepClone() : new KhaoNghiemThuocBaoVeThucVatModel();

            if (SelectedItem.thuoc_bvtv != null)
                await LoadPhanBonInfoAsync(SelectedItem.thuoc_bvtv, false);

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
            SelectedItem = new KhaoNghiemThuocBaoVeThucVatModel();
            openAddOrUpdateModal = false;
        }

        private async Task OnPhanBonChanged(ThuocBaoVeThucVatModel selected)
        {
            SelectedItem.thuoc_bvtv = selected;
            if (selected != null)
                await LoadPhanBonInfoAsync(selected);

            StateHasChanged();
        }

        private void OnCoSoSanXuatPhanBonChanged(QuanLyCoSoSanXuatThuocBVTVModel selected)
        {
            SelectedItem.co_so_san_xuat_thuoc_bvtv = selected;
        }

        private async Task LoadPhanBonInfoAsync(ThuocBaoVeThucVatModel selected, bool isForceUpdate = true)
        {
            SelectedItem.thuoc_bvtv = (await ThuocBVTVService.GetByIdAsync(selected.id.ToString())).Data;
            if (SelectedItem.thuoc_bvtv != null)
            {
                if (isForceUpdate || string.IsNullOrEmpty(SelectedItem.thanh_phan_cong_thuc))
                    SelectedItem.thanh_phan_cong_thuc = SelectedItem.thuoc_bvtv.hoat_chat_ky_thuat;
            }
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
