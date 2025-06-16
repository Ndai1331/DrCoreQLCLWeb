using CoreAdminWeb.Enums;
using CoreAdminWeb.Model;
using CoreAdminWeb.Model.TonDuThuocBVTVTrongSanPham;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Services.TonDuThuocBVTVTrongSanPham;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CoreAdminWeb.Pages.TonDuThuocBVTVTrongSanPham
{
    public partial class TonDuThuocBVTVTrongSanPham(IBaseService<TonDuThuocBVTVTrongSanPhamModel> MainService,
        IChiTieuTonDuThuocBVTVService ChiTieuTonDuThuocBVTVService,
        IBaseService<TinhModel> TinhThanhService,
        IBaseService<XaPhuongModel> XaPhuongService) : BlazorCoreBase
    {
        private static List<string> _trangThaiBanTinList = new List<string>() {
            TrangThaiBanGhi.ChoLuu.ToString(),
            TrangThaiBanGhi.DaLuu.ToString(),
            TrangThaiBanGhi.DaXoa.ToString()
        };
        private static List<string> _trangThaiBanTinChiTietList = new List<string>() {
            TrangThaiBanGhiChiTiet.New.ToString(),
            TrangThaiBanGhiChiTiet.Saved.ToString(),
            TrangThaiBanGhiChiTiet.Removed.ToString()
        };

        private List<TonDuThuocBVTVTrongSanPhamModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openDetailDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private TonDuThuocBVTVTrongSanPhamModel SelectedItem { get; set; } = new TonDuThuocBVTVTrongSanPhamModel();
        private List<ChiTieuTonDuThuocBVTVModel> SelectedItemsDetail { get; set; } = new List<ChiTieuTonDuThuocBVTVModel>();
        private ChiTieuTonDuThuocBVTVModel? SelectedItemDetail { get; set; } = default;
        private string _searchString = "";
        private string _searchDonViKiemDinhString = "";
        private TinhModel? _selectedTinhFilter { get; set; }
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
            if (!string.IsNullOrEmpty(_searchDonViKiemDinhString))
            {
                BuilderQuery += $"&filter[_and][][don_vi_kiem_dinh][_contains]={_searchDonViKiemDinhString}";
            }
            if (_selectedTinhFilter?.id > 0)
            {
                BuilderQuery += $"&filter[_and][][tinh_thanh][_eq]={_selectedTinhFilter?.id}";
            }
            BuilderQuery += $"&filter[_and][][deleted][_eq]=false";

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<TonDuThuocBVTVTrongSanPhamModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<TonDuThuocBVTVTrongSanPhamModel>();
            }
        }

        private async Task LoadDetailData()
        {
            var buildQuery = $"sort=chi_tieu_ton_du";
            buildQuery += $"&filter[_and][][ton_du_thuoc_bvtv_trong_san_pham][_eq]={SelectedItem.id}";
            buildQuery += $"&filter[_and][][deleted][_eq]=false";
            var result = await ChiTieuTonDuThuocBVTVService.GetAllAsync(buildQuery);
            SelectedItemsDetail = result.Data ?? new List<ChiTieuTonDuThuocBVTVModel>();
        }

        private async Task<IEnumerable<TinhModel>> LoadTinhFilterData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, TinhThanhService, isIgnoreCheck: true);
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


        private void OpenDeleteModal(TonDuThuocBVTVTrongSanPhamModel item)
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
            SelectedItem = new TonDuThuocBVTVTrongSanPhamModel();

            openDeleteModal = false;
        }

        private void OpenDetailDeleteModal(ChiTieuTonDuThuocBVTVModel item)
        {
            SelectedItemDetail = item;

            openDetailDeleteModal = true;
        }

        private void OnDetailDelete()
        {
            if (SelectedItemDetail == null)
            {
                AlertService.ShowAlert("Không có dữ liệu để xóa", "warning");
                return;
            }

            foreach (var item in SelectedItemsDetail)
            {
                if (item.id > 0 && item.id == SelectedItemDetail.id || item.sort > 0 && item.sort == SelectedItemDetail.sort)
                {
                    item.status = TrangThaiBanGhiChiTiet.Removed;
                }
            }

            SelectedItemDetail = default;

            openDetailDeleteModal = false;

            if (!SelectedItemsDetail.Any(c => c.status != TrangThaiBanGhiChiTiet.Removed))
                SelectedItemsDetail.Add(new ChiTieuTonDuThuocBVTVModel()
                {
                    ton_du_thuoc_bvtv_trong_san_pham = SelectedItem,
                    status = TrangThaiBanGhiChiTiet.New,
                    sort = (SelectedItemsDetail.Max(c => c.sort) ?? 0) + 1
                });
            StateHasChanged();
        }

        private void CloseDetailDeleteModal()
        {
            SelectedItemDetail = default;

            openDetailDeleteModal = false;
        }

        private void OnAddChiTieu()
        {
            if (SelectedItemsDetail == null)
                SelectedItemsDetail = new List<ChiTieuTonDuThuocBVTVModel>();

            SelectedItemsDetail.Add(new ChiTieuTonDuThuocBVTVModel
            {
                ton_du_thuoc_bvtv_trong_san_pham = SelectedItem,
                status = TrangThaiBanGhiChiTiet.New,
                sort = (SelectedItemsDetail.Max(c => c.sort) ?? 0) + 1
            });
        }

        private async Task OpenAddOrUpdateModal(TonDuThuocBVTVTrongSanPhamModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item : new TonDuThuocBVTVTrongSanPhamModel();
            SelectedItemsDetail = new List<ChiTieuTonDuThuocBVTVModel>();

            if (SelectedItem.id > 0)
            {
                await LoadDetailData();
            }

            if (!SelectedItemsDetail.Any())
                SelectedItemsDetail.Add(new ChiTieuTonDuThuocBVTVModel()
                {
                    ton_du_thuoc_bvtv_trong_san_pham = SelectedItem,
                    status = TrangThaiBanGhiChiTiet.New,
                    sort = (SelectedItemsDetail.Max(c => c.sort) ?? 0) + 1
                });

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
                    var chiTieuChitietList = SelectedItemsDetail
                        .Where(c => c.status != TrangThaiBanGhiChiTiet.Removed)
                        .Select(c =>
                        {
                            c.ton_du_thuoc_bvtv_trong_san_pham = result.Data;
                            return c;
                        })
                        .ToList();

                    var detailResult = await ChiTieuTonDuThuocBVTVService.CreateAsync(chiTieuChitietList);
                    if (!detailResult.IsSuccess)
                    {
                        AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                        return;
                    }
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
                    var addNewChiTieuList = SelectedItemsDetail
                        .Where(c => c.status == TrangThaiBanGhiChiTiet.New)
                        .Select(c =>
                        {
                            c.ton_du_thuoc_bvtv_trong_san_pham = SelectedItem;
                            c.status = TrangThaiBanGhiChiTiet.Saved;
                            return c;
                        }).ToList();
                    var removeChiTieuList = SelectedItemsDetail
                        .Where(c => c.status == TrangThaiBanGhiChiTiet.Removed && c.id > 0)
                        .Select(c =>
                        {
                            c.ton_du_thuoc_bvtv_trong_san_pham = SelectedItem;
                            c.deleted = true;
                            return c;
                        }).ToList();
                    var updateChiTieuList = SelectedItemsDetail
                        .Where(c => c.status == TrangThaiBanGhiChiTiet.Saved && c.id > 0)
                        .Select(c =>
                        {
                            c.ton_du_thuoc_bvtv_trong_san_pham = SelectedItem;
                            return c;
                        }).ToList();

                    if (addNewChiTieuList.Any())
                    {
                        var detailResult = await ChiTieuTonDuThuocBVTVService.CreateAsync(addNewChiTieuList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (removeChiTieuList.Any())
                    {
                        var detailResult = await ChiTieuTonDuThuocBVTVService.DeleteAsync(removeChiTieuList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi xóa chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (updateChiTieuList.Any())
                    {
                        var detailResult = await ChiTieuTonDuThuocBVTVService.UpdateAsync(updateChiTieuList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi cập nhật chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

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
            SelectedItem = new TonDuThuocBVTVTrongSanPhamModel();
            openAddOrUpdateModal = false;
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

        private void OnXaChanged(XaPhuongModel? selected)
        {
            SelectedItem.ward = selected;
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
                        case nameof(SelectedItem.ngay_lay_mau):
                            SelectedItem.ngay_lay_mau = null;
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
                        case nameof(SelectedItem.ngay_lay_mau):
                            SelectedItem.ngay_lay_mau = date;
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
