using CoreAdminWeb.Enums;
using CoreAdminWeb.Model;
using CoreAdminWeb.Model.CoSoTrongTrotSanXuat;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Services.CoSoTrongTrotSanXuat;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Headers;

namespace CoreAdminWeb.Pages.CoSoTrongTrotSanXuat
{
    public partial class CoSoTrongTrotSanXuat(IBaseService<CoSoTrongTrotSanXuatModel> MainService,
                                              ICoSoTrongTrotSanXuatChiTietCayTrongService CoSoTrongTrotSanXuatChiTietCayTrongService,
                                              IBaseService<TinhModel> TinhThanhService,
                                              IBaseService<XaPhuongModel> XaPhuongService,
                                              IBaseService<CayGiongCayTrongModel> CayGiongCayTrongService) : BlazorCoreBase
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

        private List<CoSoTrongTrotSanXuatModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openDetailDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private CoSoTrongTrotSanXuatModel SelectedItem { get; set; } = new CoSoTrongTrotSanXuatModel();
        private List<CoSoTrongTrotSanXuatChiTietCayTrongModel> SelectedItemsDetail { get; set; } = new List<CoSoTrongTrotSanXuatChiTietCayTrongModel>();
        private CoSoTrongTrotSanXuatChiTietCayTrongModel? SelectedItemDetail { get; set; } = default;
        private string _searchString = "";
        private TinhModel? _selectedTinhFilter { get; set; }
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
                BuilderQuery += $"&filter[_and][][province][_eq]={_selectedTinhFilter?.id}";
            }
            BuilderQuery += $"&filter[_and][][deleted][_eq]=false";

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<CoSoTrongTrotSanXuatModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<CoSoTrongTrotSanXuatModel>();
            }
        }

        private async Task LoadDetailData()
        {
            var buildQuery = $"sort=cay_giong_cay_trong";
            buildQuery += $"&filter[_and][][co_so_trong_trot_san_xuat][_eq]={SelectedItem.id}";
            buildQuery += $"&filter[_and][][deleted][_eq]=false";
            var result = await CoSoTrongTrotSanXuatChiTietCayTrongService.GetAllAsync(buildQuery);
            SelectedItemsDetail = result.Data ?? new List<CoSoTrongTrotSanXuatChiTietCayTrongModel>();
        }

        private async Task<IEnumerable<TinhModel>> LoadTinhFilterData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, TinhThanhService, isIgnoreCheck: true);
        }
        private async Task<IEnumerable<TinhModel>> LoadTinhData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, TinhThanhService, isIgnoreCheck: true);
        }
        private async Task<IEnumerable<CayGiongCayTrongModel>> LoadCayGiongCayTrongData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, CayGiongCayTrongService);
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


        private void OpenDeleteModal(CoSoTrongTrotSanXuatModel item)
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
            SelectedItem = new CoSoTrongTrotSanXuatModel();

            openDeleteModal = false;
        }

        private void OpenDetailDeleteModal(CoSoTrongTrotSanXuatChiTietCayTrongModel item)
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
                SelectedItemsDetail.Add(new CoSoTrongTrotSanXuatChiTietCayTrongModel()
                {
                    co_so_trong_trot_san_xuat = SelectedItem,
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

        private void OnAddChiTieu()
        {
            if (SelectedItemsDetail == null)
                SelectedItemsDetail = new List<CoSoTrongTrotSanXuatChiTietCayTrongModel>();

            SelectedItemsDetail.Add(new CoSoTrongTrotSanXuatChiTietCayTrongModel
            {
                co_so_trong_trot_san_xuat = SelectedItem,
                sort = (SelectedItemsDetail.Max(c => c.sort) ?? 0) + 1,
                code = string.Empty,
                name = string.Empty
            });
        }

        private async Task OpenAddOrUpdateModal(CoSoTrongTrotSanXuatModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item : new CoSoTrongTrotSanXuatModel();
            SelectedItemsDetail = new List<CoSoTrongTrotSanXuatChiTietCayTrongModel>();
            activeDefTab = "tab1";
            if (SelectedItem.id > 0)
            {
                await LoadDetailData();
            }

            if (!SelectedItemsDetail.Any())
                SelectedItemsDetail.Add(new CoSoTrongTrotSanXuatChiTietCayTrongModel()
                {
                    co_so_trong_trot_san_xuat = SelectedItem,
                    sort = (SelectedItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    code = string.Empty,
                    name = string.Empty
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
                            c.co_so_trong_trot_san_xuat = result.Data;
                            return c;
                        })
                        .ToList();

                    var detailResult = await CoSoTrongTrotSanXuatChiTietCayTrongService.CreateAsync(chiTieuChitietList);
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
                            c.co_so_trong_trot_san_xuat = SelectedItem;
                            return c;
                        }).ToList();
                    var removeChiTieuList = SelectedItemsDetail
                        .Where(c => c.deleted == true && c.id > 0)
                        .Select(c =>
                        {
                            c.co_so_trong_trot_san_xuat = SelectedItem;
                            c.deleted = true;
                            return c;
                        }).ToList();
                    var updateChiTieuList = SelectedItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id > 0)
                        .Select(c =>
                        {
                            c.co_so_trong_trot_san_xuat = SelectedItem;
                            return c;
                        }).ToList();

                    if (addNewChiTieuList.Any())
                    {
                        var detailResult = await CoSoTrongTrotSanXuatChiTietCayTrongService.CreateAsync(addNewChiTieuList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (removeChiTieuList.Any())
                    {
                        var detailResult = await CoSoTrongTrotSanXuatChiTietCayTrongService.DeleteAsync(removeChiTieuList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi xóa chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (updateChiTieuList.Any())
                    {
                        var detailResult = await CoSoTrongTrotSanXuatChiTietCayTrongService.UpdateAsync(updateChiTieuList);
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
            SelectedItem = new CoSoTrongTrotSanXuatModel();
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
                        case nameof(SelectedItem.ngay_cap):
                            SelectedItem.ngay_cap = null;
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
                    }
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

            if (activeDefTab == "tab1")
            {
                // Wait for modal to render
                _ = Task.Run(async () =>
                {
                    await Task.Delay(500);
                    await JsRuntime.InvokeVoidAsync("initializeDatePicker");
                });
            }
        }

        private void UpdateSanLuong(ChangeEventArgs e, CoSoTrongTrotSanXuatChiTietCayTrongModel item, string field)
        {
            if (!decimal.TryParse(e.Value?.ToString(), out decimal value))
            {
                AlertService.ShowAlert("Dữ liệu không hợp lệ", "warning");
                return;
            }
            if (value < 0)
            {
                AlertService.ShowAlert("Số lượng không thể nhỏ hơn 0", "warning");
                return;
            }

            switch (field)
            {
                case nameof(item.dien_tich):
                    item.dien_tich = value;
                    break;
                case nameof(item.nang_suat_binh_quan):
                    item.nang_suat_binh_quan = value;
                    break;
            }
            item.san_luong_binh_quan = null;

            if (item.nang_suat_binh_quan.HasValue && item.dien_tich.HasValue)
                item.san_luong_binh_quan = item.nang_suat_binh_quan * item.dien_tich;
        }
    }
}
