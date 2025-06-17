using CoreAdminWeb.Enums;
using CoreAdminWeb.Model;
using CoreAdminWeb.Model.CoSoBiDichBenh;
using CoreAdminWeb.Model.CoSoTrongTrotSanXuat;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Services.CoSoBiDichBenh;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using CoreAdminWeb.Helpers;

namespace CoreAdminWeb.Pages.CoSoBiDichBenh
{
    public partial class CoSoBiDichBenh(IBaseService<CoSoBiDichBenhModel> MainService,
                                              ICoSoBiDichBenhChiTietService CoSoBiDichBenhChiTietService,
                                              IBaseService<TinhModel> TinhThanhService,
                                              IBaseService<XaPhuongModel> XaPhuongService,
                                              IBaseService<CayGiongCayTrongModel> CayGiongCayTrongService,
                                              IBaseService<ViSinhVatGayHaiModel> ViSinhVatGayHaiService,
                                              IBaseService<CoSoTrongTrotSanXuatModel> CoSoTrongTrotSanXuatService) : BlazorCoreBase
    {
        private static List<MuaVu> _muaVuList = new List<MuaVu>() {
            MuaVu.VuDongXuan,
            MuaVu.VuHeThu,
            MuaVu.VuThuDong,
            MuaVu.VuXuan,
            MuaVu.VuHe,
            MuaVu.VuDong,
            MuaVu.VuThu
        };

        private List<CoSoBiDichBenhModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openDetailDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private CoSoBiDichBenhModel SelectedItem { get; set; } = new CoSoBiDichBenhModel();
        private List<CoSoBiDichBenhChiTietModel> SelectedItemsDetail { get; set; } = new List<CoSoBiDichBenhChiTietModel>();
        private CoSoBiDichBenhChiTietModel? SelectedItemDetail { get; set; } = default;
        private string _searchString = "";
        private CoSoTrongTrotSanXuatModel? _selectedDonViFilter { get; set; }
        private ViSinhVatGayHaiModel? _selectedViSinhVatGayHaiFilter { get; set; }
        private DateTime? _selectedFromDateFilter { get; set; }
        private DateTime? _selectedToDateFilter { get; set; }
        private string _titleAddOrUpdate = "Thêm mới";

        private string activeDefTab { get; set; } = "tab1";

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

                // Wait for modal to render
                _ = Task.Run(async () =>
                {
                    await Task.Delay(500);
                    await JsRuntime.InvokeVoidAsync("initializeDatePicker");
                });
            }
        }

        private async Task LoadData()
        {
            BuildPaginationQuery(Page, PageSize);
            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&filter[_and][][name][_contains]={_searchString}";
            }
            if (_selectedDonViFilter?.id > 0)
            {
                BuilderQuery += $"&filter[_and][][co_so_trong_trot_san_xuat][_eq]={_selectedDonViFilter?.id}";
            }
            //if (_selectedViSinhVatGayHaiFilter?.id > 0)
            //{
            //    BuilderQuery += $"&filter[_and][][co_so_trong_trot_san_xuat][_eq]={_selectedViSinhVatGayHaiFilter?.id}";
            //}
            BuilderQuery += $"&filter[_and][][deleted][_eq]=false";

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<CoSoBiDichBenhModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<CoSoBiDichBenhModel>();
            }
        }

        private async Task LoadDetailData()
        {
            var buildQuery = $"sort=vi_sinh_vat_gay_hai";
            buildQuery += $"&filter[_and][][co_so_bi_dich_benh][_eq]={SelectedItem.id}";
            buildQuery += $"&filter[_and][][deleted][_eq]=false";
            var result = await CoSoBiDichBenhChiTietService.GetAllAsync(buildQuery);
            SelectedItemsDetail = result.Data ?? new List<CoSoBiDichBenhChiTietModel>();
        }

        private async Task<IEnumerable<TinhModel>> LoadTinhData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, TinhThanhService, isIgnoreCheck: true);
        }
        private async Task<IEnumerable<CayGiongCayTrongModel>> LoadCayGiongCayTrongData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, CayGiongCayTrongService);
        }
        private async Task<IEnumerable<ViSinhVatGayHaiModel>> LoadViSinhVatGayHaiData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, ViSinhVatGayHaiService);
        }
        private async Task<IEnumerable<CoSoTrongTrotSanXuatModel>> LoadCoSoTrongTrotSanXuatData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, CoSoTrongTrotSanXuatService);
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


        private void OpenDeleteModal(CoSoBiDichBenhModel item)
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
            SelectedItem = new CoSoBiDichBenhModel();

            openDeleteModal = false;
        }

        private void OpenDetailDeleteModal(CoSoBiDichBenhChiTietModel item)
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
                    item.deleted = true;
                }
            }

            SelectedItemDetail = default;

            openDetailDeleteModal = false;

            if (!SelectedItemsDetail.Any(c => c.deleted == null || c.deleted == false))
                SelectedItemsDetail.Add(new CoSoBiDichBenhChiTietModel()
                {
                    co_so_bi_dich_benh = SelectedItem,
                    sort = (SelectedItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    code = string.Empty,
                    name = string.Empty
                });
            StateHasChanged();
        }

        private void CloseDetailDeleteModal()
        {
            SelectedItemDetail = default;

            openDetailDeleteModal = false;
        }

        private void OnAddThietHai()
        {
            if (SelectedItemsDetail == null)
                SelectedItemsDetail = new List<CoSoBiDichBenhChiTietModel>();

            SelectedItemsDetail.Add(new CoSoBiDichBenhChiTietModel
            {
                co_so_bi_dich_benh = SelectedItem,
                sort = (SelectedItemsDetail.Max(c => c.sort) ?? 0) + 1,
                code = string.Empty,
                name = string.Empty
            });
        }

        private async Task OpenAddOrUpdateModal(CoSoBiDichBenhModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item.DeepClone() : new CoSoBiDichBenhModel();
            SelectedItemsDetail = new List<CoSoBiDichBenhChiTietModel>();

            if (SelectedItem.id > 0)
            {
                await LoadDetailData();
            }

            if (!SelectedItemsDetail.Any())
                SelectedItemsDetail.Add(new CoSoBiDichBenhChiTietModel()
                {
                    co_so_bi_dich_benh = SelectedItem,
                    sort = (SelectedItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    code = string.Empty,
                    name = string.Empty,
                    status = Model.Base.Status.active
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
                        .Where(c => c.deleted == false || c.deleted == null)
                        .Select(c =>
                        {
                            c.co_so_bi_dich_benh = result.Data;
                            return c;
                        })
                        .ToList();

                    var detailResult = await CoSoBiDichBenhChiTietService.CreateAsync(chiTieuChitietList);
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
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id == 0)
                        .Select(c =>
                        {
                            c.co_so_bi_dich_benh = SelectedItem;
                            return c;
                        }).ToList();
                    var removeChiTieuList = SelectedItemsDetail
                        .Where(c => c.deleted == true && c.id > 0)
                        .Select(c =>
                        {
                            c.co_so_bi_dich_benh = SelectedItem;
                            c.deleted = true;
                            return c;
                        }).ToList();
                    var updateChiTieuList = SelectedItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id > 0)
                        .Select(c =>
                        {
                            c.co_so_bi_dich_benh = SelectedItem;
                            return c;
                        }).ToList();

                    if (addNewChiTieuList.Any())
                    {
                        var detailResult = await CoSoBiDichBenhChiTietService.CreateAsync(addNewChiTieuList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (removeChiTieuList.Any())
                    {
                        var detailResult = await CoSoBiDichBenhChiTietService.DeleteAsync(removeChiTieuList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi xóa chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (updateChiTieuList.Any())
                    {
                        var detailResult = await CoSoBiDichBenhChiTietService.UpdateAsync(updateChiTieuList);
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
            SelectedItem = new CoSoBiDichBenhModel();
            openAddOrUpdateModal = false;
        }

        private async Task OnDonViFilterChanged(CoSoTrongTrotSanXuatModel? selected)
        {
            _selectedDonViFilter = selected;
            await LoadData();
        }

        private async Task OnViSinhVatGayHaiFilterChanged(ViSinhVatGayHaiModel? selected)
        {
            _selectedViSinhVatGayHaiFilter = selected;

            await LoadData();
        }

        private void OnTinhChanged(TinhModel? selected)
        {
            SelectedItem.province = selected;
            SelectedItem.ward = null;
        }

        private async Task OnDateChanged(ChangeEventArgs e, string fieldName, bool isFilter = false)
        {
            try
            {
                var dateStr = e.Value?.ToString();
                if (string.IsNullOrEmpty(dateStr))
                {
                    switch (fieldName)
                    {
                        case nameof(SelectedItem.thoi_diem_ghi_nhan):
                            SelectedItem.thoi_diem_ghi_nhan = null;
                            break;
                        case nameof(SelectedItem.thoi_gian_bi_benh_tu):
                            SelectedItem.thoi_gian_bi_benh_tu = null;
                            break;
                        case nameof(SelectedItem.thoi_gian_bi_benh_den):
                            SelectedItem.thoi_gian_bi_benh_den = null;
                            break;
                        case nameof(_selectedFromDateFilter):
                            _selectedFromDateFilter = null;
                            break;
                        case nameof(_selectedToDateFilter):
                            _selectedToDateFilter = null;
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
                        case nameof(SelectedItem.thoi_diem_ghi_nhan):
                            SelectedItem.thoi_diem_ghi_nhan = date;
                            break;
                        case nameof(SelectedItem.thoi_gian_bi_benh_tu):
                            SelectedItem.thoi_gian_bi_benh_tu = date;
                            break;
                        case nameof(SelectedItem.thoi_gian_bi_benh_den):
                            SelectedItem.thoi_gian_bi_benh_den = date;
                            break;
                        case nameof(_selectedFromDateFilter):
                            _selectedFromDateFilter = date;
                            break;
                        case nameof(_selectedToDateFilter):
                            _selectedToDateFilter = date;
                            break;
                    }
                }

                if (isFilter)
                    await LoadData();
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi khi xử lý ngày: {ex.Message}", "danger");
            }
        }

        private void OnValueChanged(ChangeEventArgs e, string fieldName, CoSoBiDichBenhChiTietModel item)
        {
            try
            {
                if (decimal.TryParse(e.Value?.ToString(), out var value))
                {
                    switch (fieldName)
                    {
                        case nameof(item.muc_nhe):
                            item.muc_nhe = value;
                            break;
                        case nameof(item.muc_trung_binh):
                            item.muc_trung_binh = value;
                            break;
                        case nameof(item.muc_nang):
                            item.muc_nang = value;
                            break;
                        case nameof(item.mat_trang):
                            item.mat_trang = value;
                            break;
                    }
                }

                item.dien_tich = (item.muc_nhe ?? 0) + (item.muc_trung_binh ?? 0) + (item.muc_nang ?? 0) + (item.mat_trang ?? 0);
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi khi xử lý ngày: {ex.Message}", "danger");
            }
        }
    }
}
