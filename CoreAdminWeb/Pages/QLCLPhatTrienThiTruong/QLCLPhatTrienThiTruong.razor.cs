using CoreAdminWeb.Model;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CoreAdminWeb.Pages.QLCLPhatTrienThiTruong
{
    public partial class QLCLPhatTrienThiTruong(IBaseService<QLCLPhatTrienThiTruongModel> MainService,
                                              IQLCLPhatTrienThiTruongSanPhamService QLCLPhatTrienThiTruongSanPhamService,
                                              IBaseService<TinhModel> TinhService,
                                              IBaseService<XaPhuongModel> XaPhuongService,
                                              IBaseService<QLCLSanPhamSanXuatModel> SanPhamService) : BlazorCoreBase
    {
        private List<QLCLPhatTrienThiTruongModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openSanPhamDetailDeleteModal = false;
        private bool openAddOrUpdateModal = false;

        private List<Enums.QuyMoEnum> QuyMoList { get; set; } = new List<Enums.QuyMoEnum>() {
            Enums.QuyMoEnum.QuyMoNho,
            Enums.QuyMoEnum.QuyMoVua,
            Enums.QuyMoEnum.QuyMoLon,
        };

        private List<Enums.HinhThucBanHang> HinhThucBanHangList { get; set; } = new List<Enums.HinhThucBanHang>() {
            Enums.HinhThucBanHang.TruyenThong,
            Enums.HinhThucBanHang.ThuongMaiDienTu,
        };

        private List<Enums.ThiTruong> ThiTruongList { get; set; } = new List<Enums.ThiTruong>() {
            Enums.ThiTruong.XuatKhau,
            Enums.ThiTruong.TrongNuoc,
        };

        private QLCLPhatTrienThiTruongModel SelectedItem { get; set; } = new QLCLPhatTrienThiTruongModel();
        private List<QLCLPhatTrienThiTruongSanPhamModel> SelectedSanPhamItemsDetail { get; set; } = new List<QLCLPhatTrienThiTruongSanPhamModel>();
        private QLCLPhatTrienThiTruongSanPhamModel? SelectedSanPhamItemDetail { get; set; } = default;

        private string _searchString = "";
        private TinhModel? _selectedTinhFilter { get; set; }
        private XaPhuongModel? _selectedXaFilter { get; set; }
        private string _titleAddOrUpdate = "Thêm mới";
        private string activeDefTab { get; set; } = "tab1";
        private DateTime? _fromDate { get; set; }
        private DateTime? _toDate { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
               await LoadData();
                _ = Task.Run(async () =>
                {
                    await Task.Delay(500);
                    await JsRuntime.InvokeVoidAsync("initializeDatePicker");
                });
                await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/assets/js/pages/flatpickr.js");
                StateHasChanged();
            }
        }

        private async Task LoadData()
        {
            BuildPaginationQuery(Page, PageSize, "id", false);
            int index = 1;

            BuilderQuery += "&filter[_and][0][deleted][_eq]=false";
            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&filter[_and][{index}][_or][0][code][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][1][name][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][2][ma_so_thue][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][3][dia_chi][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][4][so_giay_phep_dkkd][_contains]={_searchString}";
                BuilderQuery += $"&filter[_and][{index}][_or][5][co_quan_cap][_contains]={_searchString}";
                index++;
            }
            if (_selectedTinhFilter != null)
            {
                BuilderQuery += $"&filter[_and][{index}][province][_eq]={_selectedTinhFilter.id}";
                index++;
            }

            if (_selectedXaFilter != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ward][_eq]={_selectedXaFilter.id}";
                index++;
            }

            if (_fromDate != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ngay_cap][_gte]={_fromDate.Value.ToString("yyyy-MM-dd")}";
                index++;
            }

            if (_toDate != null)
            {
                BuilderQuery += $"&filter[_and][{index}][ngay_cap][_lte]={_toDate.Value.ToString("yyyy-MM-dd")}";
            }

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<QLCLPhatTrienThiTruongModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<QLCLPhatTrienThiTruongModel>();
            }
        }

        private async Task LoadSanPhamDetailData()
        {
            var buildQuery = $"sort=-id";
            buildQuery += $"&filter[_and][][phat_trien_thi_truong][_eq]={SelectedItem.id}";
            // buildQuery += $"&filter[_and][][deleted][_eq]=false";
            var result = await QLCLPhatTrienThiTruongSanPhamService.GetAllAsync(buildQuery);
            SelectedSanPhamItemsDetail = result.Data ?? new List<QLCLPhatTrienThiTruongSanPhamModel>();
        }

        private async Task<IEnumerable<TinhModel>> LoadTinhData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, TinhService);
        }

        private async Task<IEnumerable<XaPhuongModel>> LoadXaCRUDData(string searchText)
        {
            string query = $"sort=-id";
            query += $"&filter[_and][][ProvinceId][_eq]={(SelectedItem.province == null ? 0 : SelectedItem.province?.id)}";
            return await LoadBlazorTypeaheadData(searchText, XaPhuongService, query);
        }


        private async Task<IEnumerable<XaPhuongModel>> LoadXaFilterData(string searchText)
        {
            string query = $"sort=-id";
            query += $"&filter[_and][][ProvinceId][_eq]={(_selectedTinhFilter == null ? 0 : _selectedTinhFilter?.id)}";
            return await LoadBlazorTypeaheadData(searchText, XaPhuongService, query);
        }
        private async Task<IEnumerable<QLCLSanPhamSanXuatModel>> LoadSanPhamData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, SanPhamService);
        }

        private void OpenDeleteModal(QLCLPhatTrienThiTruongModel item)
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
            SelectedItem = new QLCLPhatTrienThiTruongModel();
            openDeleteModal = false;
        }

        private void OpenSanPhamDetailDeleteModal(QLCLPhatTrienThiTruongSanPhamModel item)
        {
            SelectedSanPhamItemDetail = item;
            openSanPhamDetailDeleteModal = true;
        }

        private void OnSanPhamDetailDelete()
        {
            if (SelectedSanPhamItemDetail == null)
            {
                AlertService.ShowAlert("Không có dữ liệu để xóa", "warning");
                return;
            }

            foreach (var item in SelectedSanPhamItemsDetail)
            {
                if (item.id > 0 && item.id == SelectedSanPhamItemDetail.id || item.sort > 0 && item.sort == SelectedSanPhamItemDetail.sort)
                {
                    item.deleted = true;
                }
            }

            SelectedSanPhamItemDetail = default;

            openSanPhamDetailDeleteModal = false;

            if (!SelectedSanPhamItemsDetail.Any(c => c.deleted == null || c.deleted == false))
                SelectedSanPhamItemsDetail.Add(new QLCLPhatTrienThiTruongSanPhamModel()
                {
                    phat_trien_thi_truong = SelectedItem,
                    sort = (SelectedSanPhamItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    san_pham = null,
                    so_luong = 0,
                    description = "",
                    deleted = false,
                });
            StateHasChanged();
        }

        private void CloseSanPhamDetailDeleteModal()
        {
            SelectedSanPhamItemDetail = default;
            openSanPhamDetailDeleteModal = false;
        }

        private void OnAddSanPham()
        {
            if (SelectedSanPhamItemsDetail == null)
                SelectedSanPhamItemsDetail = new List<QLCLPhatTrienThiTruongSanPhamModel>();

            SelectedSanPhamItemsDetail.Add(new QLCLPhatTrienThiTruongSanPhamModel
            {
                phat_trien_thi_truong = SelectedItem,
                sort = (SelectedSanPhamItemsDetail.Max(c => c.sort) ?? 0) + 1,
                san_pham = null,
                so_luong = 0,
                description = "",
                deleted = false,
            });
        }

        private async Task OpenAddOrUpdateModal(QLCLPhatTrienThiTruongModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item : new QLCLPhatTrienThiTruongModel();

            if (SelectedItem.id > 0)
            {
                await LoadSanPhamDetailData();
            }

            if (!SelectedSanPhamItemsDetail.Any())
                SelectedSanPhamItemsDetail.Add(new QLCLPhatTrienThiTruongSanPhamModel()
                {
                    phat_trien_thi_truong = SelectedItem,
                    sort = (SelectedSanPhamItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    san_pham = null,
                    so_luong = 0,
                    description = "",
                    deleted = false,
                });

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
            if (SelectedItem.id <= 0)
            {
                var result = await MainService.CreateAsync(SelectedItem);
                if (result.IsSuccess)
                {
                    var sanPhamChiTietList = SelectedSanPhamItemsDetail
                        .Where(c => c.deleted == false || c.deleted == null)
                        .Select(c =>
                        {
                            c.phat_trien_thi_truong = result.Data;
                            return c;
                        })
                        .ToList();

                    var sanPhamDetailResult = await QLCLPhatTrienThiTruongSanPhamService.CreateAsync(sanPhamChiTietList);
                    if (!sanPhamDetailResult.IsSuccess)
                    {
                        AlertService.ShowAlert(sanPhamDetailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
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


                    var addNewSanPhamChiTietList = SelectedSanPhamItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id == 0)
                        .Select(c =>
                        {
                            c.phat_trien_thi_truong = SelectedItem;
                            return c;
                        }).ToList();
                    var removeSanPhamChiTietList = SelectedSanPhamItemsDetail
                        .Where(c => c.deleted == true && c.id > 0)
                        .Select(c =>
                        {
                            c.phat_trien_thi_truong = SelectedItem;
                            c.deleted = true;
                            return c;
                        }).ToList();
                    var updateSanPhamChiTietList = SelectedSanPhamItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id > 0)
                        .Select(c =>
                        {
                            c.phat_trien_thi_truong = SelectedItem;
                            return c;
                        }).ToList();

                    if (addNewSanPhamChiTietList.Any())
                    {
                        var detailResult = await QLCLPhatTrienThiTruongSanPhamService.CreateAsync(addNewSanPhamChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (removeSanPhamChiTietList.Any())
                    {
                        var detailResult = await QLCLPhatTrienThiTruongSanPhamService.DeleteAsync(removeSanPhamChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi xóa chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (updateSanPhamChiTietList.Any())
                    {
                        var detailResult = await QLCLPhatTrienThiTruongSanPhamService.UpdateAsync(updateSanPhamChiTietList);
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
            SelectedItem = new QLCLPhatTrienThiTruongModel();
            openAddOrUpdateModal = false;
        }
        private async Task OnDateChanged(ChangeEventArgs e, string fieldName)
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

                        case "fromDate":
                            _fromDate = null;
                            await LoadData();
                            break;

                        case "toDate":
                            _toDate = null;
                            await LoadData();
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

                        case "fromDate":
                            _fromDate = date;
                            await LoadData();
                            break;

                        case "toDate":
                            _toDate = date;
                            await LoadData();
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
        }
    }
}
