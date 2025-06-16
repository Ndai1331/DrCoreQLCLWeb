using CoreAdminWeb.Enums;
using CoreAdminWeb.Model;
using CoreAdminWeb.Model.DuBaoDichBenh;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Services.DuBaoDichBenh;
using CoreAdminWeb.Services.SanXuatUngDungCongNgheCao;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace CoreAdminWeb.Pages.DuBaoDichBenh
{
    public partial class DuBaoDichBenh(IBaseService<DuBaoDichBenhModel> MainService,
                                       IDuBaoDichBenhChiTietService DuBaoDichBenhChiTietService,
                                       IBaseService<ViSinhVatGayHaiModel> ViSinhVatGayHaiService,
                                       IBaseService<LoaiCayTrongModel> LoaiCayTrongService) : BlazorCoreBase
    {
        private static List<MucDoNguyCo> _mucDoNguyCoList = new List<MucDoNguyCo>() {
            MucDoNguyCo.Thap,
            MucDoNguyCo.TrungBinh,
            MucDoNguyCo.Cao
        };
        private static List<KhaNangAnhHuong> _khaNangAnhHuongList = new List<KhaNangAnhHuong>() {
            KhaNangAnhHuong.MucNhe,
            KhaNangAnhHuong.MucTrungBinh,
            KhaNangAnhHuong.MucNang,
            KhaNangAnhHuong.MatTrang
        };

        private List<DuBaoDichBenhModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openDetailDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private DuBaoDichBenhModel SelectedItem { get; set; } = new DuBaoDichBenhModel();
        private List<DuBaoDichBenhChiTietModel> SelectedItemsDetail { get; set; } = new List<DuBaoDichBenhChiTietModel>();
        private DuBaoDichBenhChiTietModel? SelectedItemDetail { get; set; } = default;
        private string _searchString = "";
        private LoaiCayTrongModel? _selectedLoaiCayTrongFilter { get; set; }
        private ViSinhVatGayHaiModel? _selectedViSinhVatGayHaiFilter { get; set; }
        private DateTime? _selectedFromDateFilter { get; set; }
        private DateTime? _selectedToDateFilter { get; set; }
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
            if (_selectedViSinhVatGayHaiFilter?.id > 0)
            {
                BuilderQuery += $"&filter[_and][][vi_sinh_vat_gay_hai][_eq]={_selectedViSinhVatGayHaiFilter?.id}";
            }
            if (_selectedLoaiCayTrongFilter?.id > 0)
            {
                BuilderQuery += $"&filter[_and][][chi_tiet][loai_cay_trong][_eq]={_selectedLoaiCayTrongFilter?.id}";
            }
            if (_selectedFromDateFilter.HasValue)
            {
                var filterDate = _selectedFromDateFilter.Value.ToString("yyyy-MM-dd");
                BuilderQuery += $"&filter[_and][][ngay_du_bao][_gte]={filterDate}";
            }
            if (_selectedToDateFilter.HasValue)
            {
                var toDate = _selectedToDateFilter.Value.ToString("yyyy-MM-dd");
                BuilderQuery += $"&filter[_and][][ngay_du_bao][_lte]={toDate}";
            }
            BuilderQuery += $"&filter[_and][][deleted][_eq]=false";

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<DuBaoDichBenhModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<DuBaoDichBenhModel>();
            }
        }

        private async Task LoadDetailData()
        {
            var buildQuery = $"sort=loai_cay_trong";
            buildQuery += $"&filter[_and][][du_bao_dich_benh][_eq]={SelectedItem.id}";
            buildQuery += $"&filter[_and][][deleted][_eq]=false";
            var result = await DuBaoDichBenhChiTietService.GetAllAsync(buildQuery);
            SelectedItemsDetail = result.Data ?? new List<DuBaoDichBenhChiTietModel>();
        }

        private async Task<IEnumerable<ViSinhVatGayHaiModel>> LoadViSinhVatGayHaiData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, ViSinhVatGayHaiService);
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


        private void OpenDeleteModal(DuBaoDichBenhModel item)
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
            SelectedItem = new DuBaoDichBenhModel();

            openDeleteModal = false;
        }

        private async Task OpenAddOrUpdateModal(DuBaoDichBenhModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item : new DuBaoDichBenhModel();

            if (SelectedItem.id > 0)
                await LoadDetailData();

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
                    var loaiCayTrongChitietList = SelectedItemsDetail
                        .Where(c => c.deleted == false || c.deleted == null)
                        .Select(c =>
                        {
                            c.du_bao_dich_benh = result.Data;
                            return c;
                        })
                        .ToList();

                    var loaiCayTrongDetailResult = await DuBaoDichBenhChiTietService.CreateAsync(loaiCayTrongChitietList);
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
                    var addNewLoaiCayTrongChiTietList = SelectedItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id == 0)
                        .Select(c =>
                        {
                            c.du_bao_dich_benh = SelectedItem;
                            return c;
                        }).ToList();
                    var removeLoaiCayTrongChiTietList = SelectedItemsDetail
                        .Where(c => c.deleted == true && c.id > 0)
                        .Select(c =>
                        {
                            c.du_bao_dich_benh = SelectedItem;
                            c.deleted = true;
                            return c;
                        }).ToList();
                    var updateLoaiCayTrongChiTietList = SelectedItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id > 0)
                        .Select(c =>
                        {
                            c.du_bao_dich_benh = SelectedItem;
                            return c;
                        }).ToList();

                    if (addNewLoaiCayTrongChiTietList.Any())
                    {
                        var detailResult = await DuBaoDichBenhChiTietService.CreateAsync(addNewLoaiCayTrongChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (removeLoaiCayTrongChiTietList.Any())
                    {
                        var detailResult = await DuBaoDichBenhChiTietService.DeleteAsync(removeLoaiCayTrongChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi xóa chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (updateLoaiCayTrongChiTietList.Any())
                    {
                        var detailResult = await DuBaoDichBenhChiTietService.UpdateAsync(updateLoaiCayTrongChiTietList);
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
            SelectedItem = new DuBaoDichBenhModel();
            openAddOrUpdateModal = false;
        }

        private void OpenDetailDeleteModal(DuBaoDichBenhChiTietModel item)
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
                SelectedItemsDetail.Add(new DuBaoDichBenhChiTietModel()
                {
                    du_bao_dich_benh = SelectedItem,
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

        private void OnAddDetail()
        {
            if (SelectedItemsDetail == null)
                SelectedItemsDetail = new List<DuBaoDichBenhChiTietModel>();

            SelectedItemsDetail.Add(new DuBaoDichBenhChiTietModel
            {
                du_bao_dich_benh = SelectedItem,
                sort = (SelectedItemsDetail.Max(c => c.sort) ?? 0) + 1,
                code = string.Empty,
                name = string.Empty
            });
        }

        private async Task OnLoaiCayTrongFilterChanged(LoaiCayTrongModel? selected)
        {
            _selectedLoaiCayTrongFilter = selected;

            await LoadData();
        }
        
        private async Task OnViSinhVatGayHaiFilterChanged(ViSinhVatGayHaiModel? selected)
        {
            _selectedViSinhVatGayHaiFilter = selected;

            await LoadData();
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
                        case nameof(SelectedItem.ngay_du_bao):
                            SelectedItem.ngay_du_bao = null;
                            break;
                        case nameof(SelectedItem.tu_ngay):
                            SelectedItem.tu_ngay = null;
                            break;
                        case nameof(SelectedItem.den_ngay):
                            SelectedItem.den_ngay = null;
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
                        case nameof(SelectedItem.ngay_du_bao):
                            SelectedItem.ngay_du_bao = date;
                            break;
                        case nameof(SelectedItem.tu_ngay):
                            SelectedItem.tu_ngay = date;
                            break;
                        case nameof(SelectedItem.den_ngay):
                            SelectedItem.den_ngay = date;
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
    }
}
