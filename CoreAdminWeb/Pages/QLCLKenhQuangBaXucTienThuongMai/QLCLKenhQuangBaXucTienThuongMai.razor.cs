using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using CoreAdminWeb.Enums;

namespace CoreAdminWeb.Pages.QLCLKenhQuangBaXucTienThuongMai
{
    public partial class QLCLKenhQuangBaXucTienThuongMai(
        IBaseService<QLCLKenhQuangBaXucTienThuongMaiModel> MainService,
        IBaseService<TinhModel> TinhService,
        IBaseService<XaPhuongModel> XaPhuongService) : BlazorCoreBase
    {
        private List<QLCLKenhQuangBaXucTienThuongMaiModel> MainModels { get; set; } = new();
        private List<KenhQuangBa> KenhQuangBaList = new() {
            KenhQuangBa.HoiCho,
            KenhQuangBa.TrienLam,
            KenhQuangBa.SanTMDT,
            KenhQuangBa.Website,
            KenhQuangBa.MangXaHoi,
            KenhQuangBa.BaoChi,
            KenhQuangBa.TruyenHinh,
            KenhQuangBa.Khac
        };

        private List<HinhThucQuangBa> HinhThucQuangBaList = new() {
            HinhThucQuangBa.TrucTiep,
            HinhThucQuangBa.TrucTuyen,
            HinhThucQuangBa.KetHop
        };

        private List<PhamViTiepCan> PhamViTiepCanList = new() {
            PhamViTiepCan.DiaPhuong,
            PhamViTiepCan.TrongNuoc,
            PhamViTiepCan.QuocTe
        };

        private List<DoiTuongTiepCan> DoiTuongTiepCanList = new() {
            DoiTuongTiepCan.DoanhNghiep,
            DoiTuongTiepCan.NhaPhanPhoi,
            DoiTuongTiepCan.NguoiTieuDung,
            DoiTuongTiepCan.Khac
        };

        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private string _titleAddOrUpdate = "Thêm mới";
        private string _searchString = "";
         private TinhModel? _selectedTinhFilter { get; set; }
        private XaPhuongModel? _selectedXaFilter { get; set; }
        private DateTime? _fromDate = null;
        private DateTime? _toDate = null;

        private QLCLKenhQuangBaXucTienThuongMaiModel SelectedItem { get; set; } = new QLCLKenhQuangBaXucTienThuongMaiModel();

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
            try
            {
                IsLoading = true;
                BuildPaginationQuery(Page, PageSize);
                int index =1;

                BuilderQuery += "filter[_and][0][deleted][_eq]=false&sort=sort";
                if (!string.IsNullOrEmpty(_searchString))
                {
                    BuilderQuery += $"&filter[_and][{index}][_or][0][code][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{index}][_or][1][name][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{index}][_or][2][dia_diem_to_chuc][_contains]={_searchString}";
                    BuilderQuery += $"&filter[_and][{index}][_or][3][description][_contains]={_searchString}";
                    index++;
                }

                if(_selectedTinhFilter != null)
                {
                    BuilderQuery += $"&filter[_and][{index}][province][_eq]={_selectedTinhFilter.id}";
                    index++;
                }

                if(_selectedXaFilter != null)
                {
                    BuilderQuery += $"&filter[_and][{index}][ward][_eq]={_selectedXaFilter.id}";
                    index++;
                }

                if (_fromDate != null)
                {
                    BuilderQuery += $"&filter[_and][{index}][ngay_to_chuc][_gte]={_fromDate.Value.ToString("yyyy-MM-dd")}";
                    index++;
                }

                if(_toDate != null)
                {
                    BuilderQuery += $"&filter[_and][{index}][ngay_to_chuc][_lte]={_toDate.Value.ToString("yyyy-MM-dd")}";
                    index++;
                }
                

                var result = await MainService.GetAllAsync(BuilderQuery);
                if (result.IsSuccess)
                {
                    MainModels = result.Data ?? new List<QLCLKenhQuangBaXucTienThuongMaiModel>();
                    if (result.Meta != null)
                    {
                        TotalItems = result.Meta.filter_count ?? 0;
                        TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                    }
                }
                else
                {
                    MainModels = new List<QLCLKenhQuangBaXucTienThuongMaiModel>();
                    AlertService.ShowAlert(result.Message ?? "Lỗi khi lấy dữ liệu", "danger");
                }
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi: {ex.Message}", "danger");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task<IEnumerable<TinhModel>> LoadTinhData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, TinhService);
        }

        private async Task<IEnumerable<XaPhuongModel>> LoadXaFilterData(string searchText)
        {
            string query = $"sort=-id";
            query += $"&filter[_and][][ProvinceId][_eq]={(_selectedTinhFilter == null ? 0 : _selectedTinhFilter?.id)}";
            return await LoadBlazorTypeaheadData(searchText, XaPhuongService,query);
        }
        
        private async Task<IEnumerable<XaPhuongModel>> LoadXaCRUDData(string searchText)
        {
            string query = $"sort=-id";
            query += $"&filter[_and][][ProvinceId][_eq]={(SelectedItem.province == null ? 0 : SelectedItem.province?.id)}";
            return await LoadBlazorTypeaheadData(searchText, XaPhuongService,query);
        }

        private void OpenAddOrUpdateModal(QLCLKenhQuangBaXucTienThuongMaiModel? item)
        {
            try
            {
                _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
                SelectedItem = item?.DeepClone() ?? new QLCLKenhQuangBaXucTienThuongMaiModel();
                openAddOrUpdateModal = true;

                // Wait for modal to render
                _ = Task.Run(async () =>
                {
                    await Task.Delay(500);
                    await JsRuntime.InvokeVoidAsync("initializeDatePicker");
                });
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi: {ex.Message}", "danger");
            }
        }

        private void OpenDeleteModal(QLCLKenhQuangBaXucTienThuongMaiModel item)
        {
            try
            {
                SelectedItem = item;
                openDeleteModal = true;
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi: {ex.Message}", "danger");
            }
        }

        private void CloseDeleteModal()
        {
            try
            {
                SelectedItem = new QLCLKenhQuangBaXucTienThuongMaiModel();
                openDeleteModal = false;
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi: {ex.Message}", "danger");
            }
        }

        private void CloseAddOrUpdateModal()
        {
            try
            {
                SelectedItem = new QLCLKenhQuangBaXucTienThuongMaiModel();
                openAddOrUpdateModal = false;
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi: {ex.Message}", "danger");
            }
        }

        private async Task OnValidSubmit()
        {
            try
            {
                var resultCreate = SelectedItem.id == 0 ? await MainService.CreateAsync(SelectedItem) : new();
                var resultUpdate = SelectedItem.id > 0 ? await MainService.UpdateAsync(SelectedItem) : new();
                string message =resultCreate.Message ?? resultUpdate.Message;
                if ((SelectedItem.id == 0 && resultCreate.IsSuccess) || (SelectedItem.id > 0 &&resultUpdate.IsSuccess))
                {
                    await LoadData();
                    openAddOrUpdateModal = false;
                    AlertService.ShowAlert(SelectedItem.id == 0 ? "Thêm mới thành công!" : "Cập nhật thành công!", "success");
                }
                else
                {
                    AlertService.ShowAlert($"Lỗi khi {(SelectedItem.id == 0 ? "thêm mới" : "cập nhật")} dữ liệu :" + message , "danger");
                }
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi: {ex.Message}", "danger");
            }
        }


        private async Task OnDelete()
        {
            try
            {
                if (SelectedItem == null) return;

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
            catch (Exception ex)
            {
                AlertService.ShowAlert($"Lỗi: {ex.Message}", "danger");
            }
        }

        private async Task OnDateChanged(ChangeEventArgs e, string fieldName)
        {
            try
            {
                var dateStr = e.Value?.ToString();
                if (string.IsNullOrEmpty(dateStr))
                {
                    if (fieldName == "ngay_to_chuc")
                        SelectedItem.ngay_to_chuc = null;
                    else if (fieldName == "fromDate")
                    {
                        _fromDate = null;
                        await LoadData();
                    }
                    else if (fieldName == "toDate")
                    {
                        _toDate = null;
                        await LoadData();
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
                    
                    if (fieldName == "ngay_to_chuc")
                        SelectedItem.ngay_to_chuc = date;
                    else if (fieldName == "fromDate")
                    {
                        _fromDate = date;
                        await LoadData();
                    }
                    else if (fieldName == "toDate")
                    {
                        _toDate = date;
                        await LoadData();
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
