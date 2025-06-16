using CoreAdminWeb.Enums;
using CoreAdminWeb.Model;
using CoreAdminWeb.Model.CoSoChungNhanATTP;
using CoreAdminWeb.Model.CoSoTrongTrotSanXuat;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CoreAdminWeb.Pages.CoSoChungNhanATTP
{
    public partial class CoSoChungNhanATTP(IBaseService<CoSoChungNhanATTPModel> MainService,
                                              IBaseService<TinhModel> TinhThanhService,
                                              IBaseService<XaPhuongModel> XaPhuongService,
                                              IBaseService<CoSoTrongTrotSanXuatModel> CoSoTrongTrotSanXuatService) : BlazorCoreBase
    {
        private static List<TrangThaiGiayPhep> _trangThaiGiayPhep = new List<TrangThaiGiayPhep>() {
            TrangThaiGiayPhep.DangHoatDong,
            TrangThaiGiayPhep.HetHan,
            TrangThaiGiayPhep.DinhChi,
            TrangThaiGiayPhep.ThuHoi
        };
        private static List<HinhThucCap> _hinhThucCap = new List<HinhThucCap>() {
            HinhThucCap.CapMoi,
            HinhThucCap.CapLai,
            HinhThucCap.GiaHan,
            HinhThucCap.DieuChinh,
            HinhThucCap.CapTam,
            HinhThucCap.CapDoi
        };
        private static List<LoaiGiayChungNhan> _loaiGiayChungNhan = new List<LoaiGiayChungNhan>() {
            LoaiGiayChungNhan.AnToanThucPham,
            LoaiGiayChungNhan.VietGap,
            LoaiGiayChungNhan.ChungNhanHuuCo
        };

        private List<CoSoChungNhanATTPModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;

        private CoSoChungNhanATTPModel SelectedItem { get; set; } = new CoSoChungNhanATTPModel();

        private string _searchString = "";
        private string _searchLoaiGCNString = "";
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
                await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/assets/js/pages/flatpickr.js");
                StateHasChanged();
            }
        }

        private async Task LoadData()
        {
            BuildPaginationQuery(Page, PageSize);
            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&filter[_and][][so_gcn][_contains]={_searchString}";
            }
            if (!string.IsNullOrEmpty(_searchLoaiGCNString))
            {
                BuilderQuery += $"&filter[_and][][loai_gcn][_eq]={_searchLoaiGCNString}";
            }
            if (!string.IsNullOrEmpty(_searchStatusString))
            {
                BuilderQuery += $"&filter[_and][][status][_eq]={_searchStatusString}";
            }
            BuilderQuery += $"&filter[_and][][deleted][_eq]=false";

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<CoSoChungNhanATTPModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<CoSoChungNhanATTPModel>();
            }
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
        private async Task<IEnumerable<CoSoTrongTrotSanXuatModel>> LoadCoSoTrongTrotSanXuatData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, CoSoTrongTrotSanXuatService);
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

        private void OpenDeleteModal(CoSoChungNhanATTPModel item)
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
            SelectedItem = new CoSoChungNhanATTPModel();

            openDeleteModal = false;
        }

        private async Task OpenAddOrUpdateModal(CoSoChungNhanATTPModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item : new CoSoChungNhanATTPModel();

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
            SelectedItem = new CoSoChungNhanATTPModel();
            openAddOrUpdateModal = false;
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
        private void OnCoSoTrongTrotSanXuatChanged(CoSoTrongTrotSanXuatModel? selected)
        {
            SelectedItem.co_so_trong_trot_san_xuat = selected;
            if (selected != null)
            {
                SelectedItem.province = selected.province;
                SelectedItem.ward = selected.ward;
            }
        }
        private async Task OnLoaiGiayChungNhanChanged(ChangeEventArgs? selected)
        {
            if (int.TryParse(selected?.Value?.ToString(), out int intValue))
            {
                _searchLoaiGCNString = intValue.ToString();
            }

            await LoadData();
        }
        private async Task OnTrangThaiGiayPhepChanged(ChangeEventArgs? selected)
        {
            if (int.TryParse(selected?.Value?.ToString(), out int intValue))
            {
                _searchStatusString = intValue.ToString();
            }

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
                        case nameof(SelectedItem.ngay_cap):
                            SelectedItem.ngay_cap = null;
                            break;
                        case nameof(SelectedItem.ngay_het_han):
                            SelectedItem.ngay_het_han = null;
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
                        case nameof(SelectedItem.ngay_cap):
                            SelectedItem.ngay_cap = date;
                            break;
                        case nameof(SelectedItem.ngay_het_han):
                            SelectedItem.ngay_het_han = date;
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
