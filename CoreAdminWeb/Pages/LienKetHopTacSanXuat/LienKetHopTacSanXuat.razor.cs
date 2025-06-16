using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Model.CoSoTrongTrotSanXuat;
using CoreAdminWeb.Model.LienKetHopTacSanXuat;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Services.LienKetHopTacSanXuat;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace CoreAdminWeb.Pages.LienKetHopTacSanXuat
{
    public partial class LienKetHopTacSanXuat(IBaseService<LienKetHopTacSanXuatModel> MainService,
                                              ILienKetHopTacSanXuatDonViThamGiaService LienKetHopTacSanXuatDonViThamGiaService,
                                              IBaseService<CayGiongCayTrongModel> CayGiongCayTrongService,
                                              IBaseService<CoSoTrongTrotSanXuatModel> CoSoTrongTrotSanXuatService,
                                              IBaseService<HinhThucLienKetModel> HinhThucLienKetService) : BlazorCoreBase
    {
        private List<LienKetHopTacSanXuatModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openDonViThamGiaDetailDeleteModal = false;
        private bool openAddOrUpdateModal = false;

        private LienKetHopTacSanXuatModel SelectedItem { get; set; } = new LienKetHopTacSanXuatModel();
        private List<LienKetHopTacSanXuatDonViThamGiaModel> SelectedDonViThamGiaItemsDetail { get; set; } = new List<LienKetHopTacSanXuatDonViThamGiaModel>();
        private LienKetHopTacSanXuatDonViThamGiaModel? SelectedDonViThamGiaItemDetail { get; set; } = default;

        private string _searchString = "";
        private string _searchDonViChuTriString = "";
        private CayGiongCayTrongModel? _selectedCayGiongCayTrongFilter { get; set; }
        private DateTime? _selectedFromDateFilter { get; set; }
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
            if (!string.IsNullOrEmpty(_searchDonViChuTriString))
            {
                BuilderQuery += $"&filter[_and][][don_vi_chu_tri][_contains]={_searchDonViChuTriString}";
            }
            if (_selectedCayGiongCayTrongFilter?.id > 0)
            {
                BuilderQuery += $"&filter[_and][][cay_trong][_eq]={_selectedCayGiongCayTrongFilter?.id}";
            }
            if (_selectedFromDateFilter.HasValue)
            {
                var fromDate = _selectedFromDateFilter.Value.ToString("yyyy-MM-dd");
                BuilderQuery += $"&filter[_and][][thoi_gian_tu][_gte]={fromDate}";
            }
            BuilderQuery += $"&filter[_and][][deleted][_eq]=false";

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<LienKetHopTacSanXuatModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<LienKetHopTacSanXuatModel>();
            }
        }

        private async Task LoadCayTrongDetailData()
        {
            var buildQuery = $"sort=co_so_trong_trot_san_xuat";
            buildQuery += $"&filter[_and][][lien_ket_hop_tac_san_xuat][_eq]={SelectedItem.id}";
            buildQuery += $"&filter[_and][][deleted][_eq]=false";
            var result = await LienKetHopTacSanXuatDonViThamGiaService.GetAllAsync(buildQuery);
            SelectedDonViThamGiaItemsDetail = result.Data ?? new List<LienKetHopTacSanXuatDonViThamGiaModel>();
        }

        private async Task<IEnumerable<CayGiongCayTrongModel>> LoadCayGiongCayTrongData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, CayGiongCayTrongService);
        }
        private async Task<IEnumerable<CoSoTrongTrotSanXuatModel>> LoadCoSoTrongTrotSanXuatData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, CoSoTrongTrotSanXuatService);
        }
        private async Task<IEnumerable<HinhThucLienKetModel>> LoadHinhThucLienKetData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, HinhThucLienKetService);
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

        private void OpenDeleteModal(LienKetHopTacSanXuatModel item)
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
            SelectedItem = new LienKetHopTacSanXuatModel();

            openDeleteModal = false;
        }

        private void OpenDonViThamGiaDetailDeleteModal(LienKetHopTacSanXuatDonViThamGiaModel item)
        {
            SelectedDonViThamGiaItemDetail = item;

            openDonViThamGiaDetailDeleteModal = true;
        }

        private void OnDonViThamGIaDetailDelete()
        {
            if (SelectedDonViThamGiaItemDetail == null)
            {
                AlertService.ShowAlert("Không có dữ liệu để xóa", "warning");
                return;
            }

            foreach (var item in SelectedDonViThamGiaItemsDetail)
            {
                if (item.id > 0 && item.id == SelectedDonViThamGiaItemDetail.id || item.sort > 0 && item.sort == SelectedDonViThamGiaItemDetail.sort)
                {
                    item.deleted = true;
                }
            }

            SelectedDonViThamGiaItemDetail = default;

            openDonViThamGiaDetailDeleteModal = false;

            if (!SelectedDonViThamGiaItemsDetail.Any(c => c.deleted == null || c.deleted == false))
                SelectedDonViThamGiaItemsDetail.Add(new LienKetHopTacSanXuatDonViThamGiaModel()
                {
                    lien_ket_hop_tac_san_xuat = SelectedItem,
                    sort = (SelectedDonViThamGiaItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    code = string.Empty,
                    name = string.Empty
                });
            StateHasChanged();
        }

        private void CloseDonViThamGiaDetailDeleteModal()
        {
            SelectedDonViThamGiaItemDetail = default;

            openDonViThamGiaDetailDeleteModal = false;
        }

        private void OnAddDonViThamGia()
        {
            if (SelectedDonViThamGiaItemsDetail == null)
                SelectedDonViThamGiaItemsDetail = new List<LienKetHopTacSanXuatDonViThamGiaModel>();

            SelectedDonViThamGiaItemsDetail.Add(new LienKetHopTacSanXuatDonViThamGiaModel
            {
                lien_ket_hop_tac_san_xuat = SelectedItem,
                sort = (SelectedDonViThamGiaItemsDetail.Max(c => c.sort) ?? 0) + 1,
                code = string.Empty,
                name = string.Empty
            });
        }

        private async Task OpenAddOrUpdateModal(LienKetHopTacSanXuatModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item.DeepClone() : new LienKetHopTacSanXuatModel();
            SelectedDonViThamGiaItemsDetail = new List<LienKetHopTacSanXuatDonViThamGiaModel>();

            if (SelectedItem.id > 0)
            {
                await LoadCayTrongDetailData();
            }

            if (!SelectedDonViThamGiaItemsDetail.Any())
                SelectedDonViThamGiaItemsDetail.Add(new LienKetHopTacSanXuatDonViThamGiaModel()
                {
                    lien_ket_hop_tac_san_xuat = SelectedItem,
                    sort = (SelectedDonViThamGiaItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    code = string.Empty,
                    name = string.Empty
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
            if (SelectedItem.id == 0)
            {
                var result = await MainService.CreateAsync(SelectedItem);
                if (result.IsSuccess)
                {
                    var donViThamGiaChitietList = SelectedDonViThamGiaItemsDetail
                        .Where(c => c.deleted == false || c.deleted == null)
                        .Select(c =>
                        {
                            c.lien_ket_hop_tac_san_xuat = result.Data;
                            return c;
                        })
                        .ToList();

                    var donViThamGiaDetailResult = await LienKetHopTacSanXuatDonViThamGiaService.CreateAsync(donViThamGiaChitietList);
                    if (!donViThamGiaDetailResult.IsSuccess)
                    {
                        AlertService.ShowAlert(donViThamGiaDetailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
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
                    var addNewDonViThamGiaChiTietList = SelectedDonViThamGiaItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id == 0)
                        .Select(c =>
                        {
                            c.lien_ket_hop_tac_san_xuat = SelectedItem;
                            return c;
                        }).ToList();
                    var removeDonViThamGiaChiTietList = SelectedDonViThamGiaItemsDetail
                        .Where(c => c.deleted == true && c.id > 0)
                        .Select(c =>
                        {
                            c.lien_ket_hop_tac_san_xuat = SelectedItem;
                            c.deleted = true;
                            return c;
                        }).ToList();
                    var updateDonViThamGiaChiTietList = SelectedDonViThamGiaItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id > 0)
                        .Select(c =>
                        {
                            c.lien_ket_hop_tac_san_xuat = SelectedItem;
                            return c;
                        }).ToList();

                    if (addNewDonViThamGiaChiTietList.Any())
                    {
                        var detailResult = await LienKetHopTacSanXuatDonViThamGiaService.CreateAsync(addNewDonViThamGiaChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (removeDonViThamGiaChiTietList.Any())
                    {
                        var detailResult = await LienKetHopTacSanXuatDonViThamGiaService.DeleteAsync(removeDonViThamGiaChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi xóa chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (updateDonViThamGiaChiTietList.Any())
                    {
                        var detailResult = await LienKetHopTacSanXuatDonViThamGiaService.UpdateAsync(updateDonViThamGiaChiTietList);
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
            SelectedItem = new LienKetHopTacSanXuatModel();
            openAddOrUpdateModal = false;
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
                        case nameof(SelectedItem.thoi_gian_tu):
                            SelectedItem.thoi_gian_tu = null;
                            break;
                        case nameof(SelectedItem.thoi_gian_den):
                            SelectedItem.thoi_gian_den = null;
                            break;
                        case nameof(_selectedFromDateFilter):
                            _selectedFromDateFilter = null;
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
                        case nameof(SelectedItem.thoi_gian_tu):
                            SelectedItem.thoi_gian_tu = date;
                            break;
                        case nameof(SelectedItem.thoi_gian_den):
                            SelectedItem.thoi_gian_den = date;
                            break;
                        case nameof(_selectedFromDateFilter):
                            _selectedFromDateFilter = date;
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

        private async Task OnCayTrongFilterChanged(CayGiongCayTrongModel? selected)
        {
            _selectedCayGiongCayTrongFilter = selected;
            await LoadData();
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
