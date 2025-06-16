using CoreAdminWeb.Enums;
using CoreAdminWeb.Model;
using CoreAdminWeb.Model.CayTrongDuocBaoHo;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CoreAdminWeb.Pages.CayTrongDuocBaoHo
{
    public partial class CayTrongDuocBaoHo(IBaseService<CayTrongDuocBaoHoModel> MainService,
                                              IBaseService<CayGiongCayTrongModel> CayGiongCayTrongService,
                                              IBaseService<LoaiCayTrongModel> LoaiCayTrongService,
                                              IBaseService<TinhModel> TinhThanhService,
                                              IBaseService<XaPhuongModel> XaPhuongService) : BlazorCoreBase
    {
        private static List<NguonGoc> _nguonGoc = new List<NguonGoc>() { 
            NguonGoc.NoiDia, 
            NguonGoc.NhapKhau,
            NguonGoc.Khac 
        };

        private List<CayTrongDuocBaoHoModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;

        private CayTrongDuocBaoHoModel SelectedItem { get; set; } = new CayTrongDuocBaoHoModel();

        private string _searchString = "";
        private LoaiCayTrongModel? _selectedLoaiCayTrongFilter { get; set; }
        private int _searchNguonGoc = 0;
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
                StateHasChanged();
            }
        }

        private async Task LoadData()
        {
            BuildPaginationQuery(Page, PageSize);

            BuilderQuery += $"&filter[deleted][_eq]=false";

            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&filter[_and][][cay_giong_cay_trong][name][_contains]={_searchString}";
            }
            if (_selectedLoaiCayTrongFilter?.id > 0)
            {
                BuilderQuery += $"&filter[cay_giong_cay_trong][loai_cay_trong][_eq]={_selectedLoaiCayTrongFilter?.id}";
            }
            if (_searchNguonGoc > 0)
            {
                BuilderQuery += $"&filter[cay_giong_cay_trong][nguon_goc][_eq]={_searchNguonGoc}";
            }

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<CayTrongDuocBaoHoModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<CayTrongDuocBaoHoModel>();
            }
        }

        private async Task<IEnumerable<CayGiongCayTrongModel>> LoadCayGiongCayTrongData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, CayGiongCayTrongService);
        }

        private async Task<IEnumerable<LoaiCayTrongModel>> LoadLoaiCayTrongData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, LoaiCayTrongService);
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

        private void OpenDeleteModal(CayTrongDuocBaoHoModel item)
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
            SelectedItem = new CayTrongDuocBaoHoModel();

            openDeleteModal = false;
        }

        private async Task OpenAddOrUpdateModal(CayTrongDuocBaoHoModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item : new CayTrongDuocBaoHoModel();

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
            SelectedItem = new CayTrongDuocBaoHoModel();
            openAddOrUpdateModal = false;
        }
        private void OnCayGiongCayTrongChanged(CayGiongCayTrongModel? selected)
        {
            SelectedItem.cay_giong_cay_trong = selected;
        }

        private async Task OnLoaiCayTrongFilterChanged(LoaiCayTrongModel? selected)
        {
            _selectedLoaiCayTrongFilter = selected;

            await LoadData();
        }

        private async Task OnNguonGocChanged(ChangeEventArgs? selected)
        {
            if (int.TryParse(selected?.Value?.ToString(), out int intValue))
            {
                _searchNguonGoc = intValue;
            }

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
