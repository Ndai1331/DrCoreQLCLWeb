using CoreAdminWeb.Model;
using CoreAdminWeb.Model.CoSoTrongTrotSanXuat;
using CoreAdminWeb.Model.SanXuatUngDungCongNgheCao;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Services.SanXuatUngDungCongNgheCao;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace CoreAdminWeb.Pages.SanXuatUngDungCongNgheCao
{
    public partial class SanXuatUngDungCongNgheCao(IBaseService<SanXuatUngDungCongNgheCaoModel> MainService,
                                              ISanXuatUngDungCongNgheCaoLoaiCayTrongService SanXuatUngDungCongNgheCaoLoaiCayTrongService,
                                              IBaseService<TinhModel> TinhThanhService,
                                              IBaseService<XaPhuongModel> XaPhuongService,
                                              IBaseService<CoSoTrongTrotSanXuatModel> CoSoTrongTrotSanXuatService,
                                              IBaseService<LoaiCayTrongModel> LoaiCayTrongService) : BlazorCoreBase
    {
        private List<SanXuatUngDungCongNgheCaoModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openLoaiCayTrongDetailDeleteModal = false;
        private bool openAddOrUpdateModal = false;

        private SanXuatUngDungCongNgheCaoModel SelectedItem { get; set; } = new SanXuatUngDungCongNgheCaoModel();
        private List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel> SelectedLoaiCayTrongItemsDetail { get; set; } = new List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>();
        private SanXuatUngDungCongNgheCaoLoaiCayTrongModel? SelectedLoaiCayTrongItemDetail { get; set; } = default;

        private string _searchString = "";
        private TinhModel? _selectedTinhFilter { get; set; }
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
            if (_selectedTinhFilter?.id > 0)
            {
                BuilderQuery += $"&filter[_and][][province][_eq]={_selectedTinhFilter?.id}";
            }
            if (_selectedFromDateFilter.HasValue)
            {
                var fromDate = _selectedFromDateFilter.Value.ToString("yyyy-MM-dd");
                BuilderQuery += $"&filter[_and][][thoi_gian_bat_dau][_gte]={fromDate}";
            }
            if (_selectedToDateFilter.HasValue)
            {
                var toDate = _selectedToDateFilter.Value.ToString("yyyy-MM-dd");
                BuilderQuery += $"&filter[_and][][thoi_gian_ket_thuc][_lte]={toDate}";
            }
            BuilderQuery += $"&filter[_and][][deleted][_eq]=false";

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<SanXuatUngDungCongNgheCaoModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<SanXuatUngDungCongNgheCaoModel>();
            }
        }

        private async Task LoadCayTrongDetailData()
        {
            var buildQuery = $"sort=loai_cay_trong";
            buildQuery += $"&filter[_and][][san_xuat_ung_dung_cong_nghe_cao][_eq]={SelectedItem.id}";
            buildQuery += $"&filter[_and][][deleted][_eq]=false";
            var result = await SanXuatUngDungCongNgheCaoLoaiCayTrongService.GetAllAsync(buildQuery);
            SelectedLoaiCayTrongItemsDetail = result.Data ?? new List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>();
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
        private async Task<IEnumerable<LoaiCayTrongModel>> LoadLoaiCayTrongData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, LoaiCayTrongService);
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

        private void OpenDeleteModal(SanXuatUngDungCongNgheCaoModel item)
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
            SelectedItem = new SanXuatUngDungCongNgheCaoModel();

            openDeleteModal = false;
        }

        private void OpenLoaiCayTrongDetailDeleteModal(SanXuatUngDungCongNgheCaoLoaiCayTrongModel item)
        {
            SelectedLoaiCayTrongItemDetail = item;

            openLoaiCayTrongDetailDeleteModal = true;
        }

        private void OnLoaiCayTrongDetailDelete()
        {
            if (SelectedLoaiCayTrongItemDetail == null)
            {
                AlertService.ShowAlert("Không có dữ liệu để xóa", "warning");
                return;
            }

            foreach (var item in SelectedLoaiCayTrongItemsDetail)
            {
                if (item.id > 0 && item.id == SelectedLoaiCayTrongItemDetail.id || item.sort > 0 && item.sort == SelectedLoaiCayTrongItemDetail.sort)
                {
                    item.deleted = true;
                }
            }

            SelectedLoaiCayTrongItemDetail = default;

            openLoaiCayTrongDetailDeleteModal = false;

            if (!SelectedLoaiCayTrongItemsDetail.Any(c => c.deleted == null || c.deleted == false))
                SelectedLoaiCayTrongItemsDetail.Add(new SanXuatUngDungCongNgheCaoLoaiCayTrongModel()
                {
                    san_xuat_ung_dung_cong_nghe_cao = SelectedItem,
                    sort = (SelectedLoaiCayTrongItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    code = string.Empty,
                    name = string.Empty
                });
            StateHasChanged();
        }

        private void CloseLoaiCayTrongDetailDeleteModal()
        {
            SelectedLoaiCayTrongItemDetail = default;

            openLoaiCayTrongDetailDeleteModal = false;
        }

        private void OnAddLoaiCayTrong()
        {
            if (SelectedLoaiCayTrongItemsDetail == null)
                SelectedLoaiCayTrongItemsDetail = new List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>();

            SelectedLoaiCayTrongItemsDetail.Add(new SanXuatUngDungCongNgheCaoLoaiCayTrongModel
            {
                san_xuat_ung_dung_cong_nghe_cao = SelectedItem,
                sort = (SelectedLoaiCayTrongItemsDetail.Max(c => c.sort) ?? 0) + 1,
                code = string.Empty,
                name = string.Empty
            });
        }

        private async Task OpenAddOrUpdateModal(SanXuatUngDungCongNgheCaoModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item : new SanXuatUngDungCongNgheCaoModel();
            SelectedLoaiCayTrongItemsDetail = new List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>();

            if (SelectedItem.id > 0)
            {
                await LoadCayTrongDetailData();
            }

            if (!SelectedLoaiCayTrongItemsDetail.Any())
                SelectedLoaiCayTrongItemsDetail.Add(new SanXuatUngDungCongNgheCaoLoaiCayTrongModel()
                {
                    san_xuat_ung_dung_cong_nghe_cao = SelectedItem,
                    sort = (SelectedLoaiCayTrongItemsDetail.Max(c => c.sort) ?? 0) + 1,
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
                    var loaiCayTrongChitietList = SelectedLoaiCayTrongItemsDetail
                        .Where(c => c.deleted == false || c.deleted == null)
                        .Select(c =>
                        {
                            c.san_xuat_ung_dung_cong_nghe_cao = result.Data;
                            return c;
                        })
                        .ToList();
                    
                    var loaiCayTrongDetailResult = await SanXuatUngDungCongNgheCaoLoaiCayTrongService.CreateAsync(loaiCayTrongChitietList);
                    if (!loaiCayTrongDetailResult.IsSuccess)
                    {
                        AlertService.ShowAlert(loaiCayTrongDetailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
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
                    var addNewLoaiCayTrongChiTietList = SelectedLoaiCayTrongItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id == 0)
                        .Select(c =>
                        {
                            c.san_xuat_ung_dung_cong_nghe_cao = SelectedItem;
                            return c;
                        }).ToList();
                    var removeLoaiCayTrongChiTietList = SelectedLoaiCayTrongItemsDetail
                        .Where(c => c.deleted == true && c.id > 0)
                        .Select(c =>
                        {
                            c.san_xuat_ung_dung_cong_nghe_cao = SelectedItem;
                            c.deleted = true;
                            return c;
                        }).ToList();
                    var updateLoaiCayTrongChiTietList = SelectedLoaiCayTrongItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id > 0)
                        .Select(c =>
                        {
                            c.san_xuat_ung_dung_cong_nghe_cao = SelectedItem;
                            return c;
                        }).ToList();

                    if (addNewLoaiCayTrongChiTietList.Any())
                    {
                        var detailResult = await SanXuatUngDungCongNgheCaoLoaiCayTrongService.CreateAsync(addNewLoaiCayTrongChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (removeLoaiCayTrongChiTietList.Any())
                    {
                        var detailResult = await SanXuatUngDungCongNgheCaoLoaiCayTrongService.DeleteAsync(removeLoaiCayTrongChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi xóa chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (updateLoaiCayTrongChiTietList.Any())
                    {
                        var detailResult = await SanXuatUngDungCongNgheCaoLoaiCayTrongService.UpdateAsync(updateLoaiCayTrongChiTietList);
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
            SelectedItem = new SanXuatUngDungCongNgheCaoModel();
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
        private async Task OnDateChanged(ChangeEventArgs e, string fieldName, bool isFilter = false)
        {
            try
            {
                var dateStr = e.Value?.ToString();
                if (string.IsNullOrEmpty(dateStr))
                {
                    switch (fieldName)
                    {
                        case nameof(_selectedFromDateFilter):
                            _selectedFromDateFilter = null;
                            break;
                        case nameof(_selectedToDateFilter):
                            _selectedToDateFilter = null;
                            break;
                        case nameof(SelectedItem.thoi_gian_bat_dau):
                            SelectedItem.thoi_gian_bat_dau = null;
                            break;
                        case nameof(SelectedItem.thoi_gian_ket_thuc):
                            SelectedItem.thoi_gian_ket_thuc = null;
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
                        case nameof(_selectedFromDateFilter):
                            _selectedFromDateFilter = date;
                            break;
                        case nameof(_selectedToDateFilter):
                            _selectedToDateFilter = date;
                            break;
                        case nameof(SelectedItem.thoi_gian_bat_dau):
                            SelectedItem.thoi_gian_bat_dau = date;
                            break;
                        case nameof(SelectedItem.thoi_gian_ket_thuc):
                            SelectedItem.thoi_gian_ket_thuc = date;
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
    }
}
