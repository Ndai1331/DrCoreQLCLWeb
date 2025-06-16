using CoreAdminWeb.Enums;
using CoreAdminWeb.Model;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CoreAdminWeb.Pages.CoSoSanXuatGiong
{
    public partial class CoSoSanXuatGiong(IBaseService<CoSoSanXuatGiongModel> MainService,
                                              ICoSoSanXuatGiongChiTietService CoSoSanXuatGiongChiTietService,
                                              IBaseService<TinhModel> TinhThanhService,
                                              IBaseService<XaPhuongModel> XaPhuongService,
                                              IBaseService<CayGiongCayTrongModel> CayGiongCayTrongService) : BlazorCoreBase
    {
        public static List<QuyMoEnum> QuyMoList = new List<
            QuyMoEnum> { QuyMoEnum.QuyMoNho,
            QuyMoEnum.QuyMoVua,
            QuyMoEnum.QuyMoLon };
        private List<CoSoSanXuatGiongModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openDetailDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private CoSoSanXuatGiongModel SelectedItem { get; set; } = new CoSoSanXuatGiongModel();
        private List<CoSoSanXuatGiongChiTietModel> SelectedItemsDetail { get; set; } = new List<CoSoSanXuatGiongChiTietModel>();
        private CoSoSanXuatGiongChiTietModel? SelectedItemDetail { get; set; } = default;
        private string _searchString = "";
        private TinhModel? _selectedTinhFilter { get; set; }
        private XaPhuongModel? _selectedXaFilter { get; set; }
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
                BuilderQuery += $"&filter[_and][][province][_eq]={_selectedTinhFilter?.id}";
            }
            if (_selectedXaFilter?.id > 0)
            {
                BuilderQuery += $"&filter[_and][][ward][_eq]={_selectedXaFilter?.id}";
            }
            
            BuilderQuery += $"&filter[_and][][deleted][_eq]=false";

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<CoSoSanXuatGiongModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<CoSoSanXuatGiongModel>();
            }
        }

        private async Task LoadDetailData()
        {
            var buildQuery = $"sort=cay_giong_cay_trong";
            buildQuery += $"&filter[_and][][co_so_san_xuat_giong][_eq]={SelectedItem.id}";
            buildQuery += $"&filter[_and][][deleted][_eq]=false";
            var result = await CoSoSanXuatGiongChiTietService.GetAllAsync(buildQuery);
            SelectedItemsDetail = result.Data ?? new List<CoSoSanXuatGiongChiTietModel>();
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

        private async Task<IEnumerable<XaPhuongModel>> LoadFilterXaData(string searchText)
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


        private void OpenDeleteModal(CoSoSanXuatGiongModel item)
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
            SelectedItem = new CoSoSanXuatGiongModel();

            openDeleteModal = false;
        }

        private void OpenDetailDeleteModal(CoSoSanXuatGiongChiTietModel item)
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
                SelectedItemsDetail.Add(new CoSoSanXuatGiongChiTietModel()
                {
                    co_so_san_xuat_giong = SelectedItem,
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

        private void OnAddChiTiet()
        {
            if (SelectedItemsDetail == null)
                SelectedItemsDetail = new List<CoSoSanXuatGiongChiTietModel>();
            SelectedItemsDetail.Add(new CoSoSanXuatGiongChiTietModel
            {
                co_so_san_xuat_giong = SelectedItem,
                sort = (SelectedItemsDetail.Max(c => c.sort) ?? 0) + 1,
                code = string.Empty,
                name = string.Empty
            });
        }

        private async Task OpenAddOrUpdateModal(CoSoSanXuatGiongModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item : new CoSoSanXuatGiongModel();
            SelectedItemsDetail = new List<CoSoSanXuatGiongChiTietModel>();

            if (SelectedItem.id > 0)
            {
                await LoadDetailData();
            }

            if (!SelectedItemsDetail.Any())
                SelectedItemsDetail.Add(new CoSoSanXuatGiongChiTietModel()
                {
                    co_so_san_xuat_giong = SelectedItem,
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
                            c.co_so_san_xuat_giong = result.Data;
                            return c;
                        })
                        .ToList();

                    var detailResult = await CoSoSanXuatGiongChiTietService.CreateAsync(chiTieuChitietList);
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
                            c.co_so_san_xuat_giong = SelectedItem;
                            return c;
                        }).ToList();
                    var removeChiTieuList = SelectedItemsDetail
                        .Where(c => c.deleted == true && c.id > 0)
                        .Select(c =>
                        {
                            c.co_so_san_xuat_giong = SelectedItem;
                            c.deleted = true;
                            return c;
                        }).ToList();
                    var updateChiTieuList = SelectedItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id > 0)
                        .Select(c =>
                        {
                            c.co_so_san_xuat_giong = SelectedItem;
                            return c;
                        }).ToList();

                    if (addNewChiTieuList.Any())
                    {
                        var detailResult = await CoSoSanXuatGiongChiTietService.CreateAsync(addNewChiTieuList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (removeChiTieuList.Any())
                    {
                        var detailResult = await CoSoSanXuatGiongChiTietService.DeleteAsync(removeChiTieuList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi xóa chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (updateChiTieuList.Any())
                    {
                        var detailResult = await CoSoSanXuatGiongChiTietService.UpdateAsync(updateChiTieuList);
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
            SelectedItem = new CoSoSanXuatGiongModel();
            openAddOrUpdateModal = false;
        }

        private async Task OnTinhFilterChanged(TinhModel? selected)
        {
            _selectedTinhFilter = selected;
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
