using CoreAdminWeb.Enums;
using CoreAdminWeb.Model;
using CoreAdminWeb.Model.DienTichGieoTrongCayHangNam;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Services.DienTichGieoTrongCayHangNam;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CoreAdminWeb.Pages.DienTichGieoTrongCayHangNam
{
    public partial class DienTichGieoTrongCayHangNam(IBaseService<DienTichGieoTrongCayHangNamModel> MainService,
                                              IDienTichGieoTrongCayHangNamCayTrongService DienTichGieoTrongCayHangNamCayTrongService,
                                              IDienTichGieoTrongCayHangNamCoCauChuLucService DienTichGieoTrongCayHangNamCoCauChuLucService,
                                              IDienTichGieoTrongCayHangNamTheoMuaVuService DienTichGieoTrongCayHangNamTheoMuaVuService,
                                              IBaseService<TinhModel> TinhThanhService,
                                              IBaseService<XaPhuongModel> XaPhuongService,
                                              IBaseService<CayGiongCayTrongModel> CayGiongCayTrongService,
                                              IBaseService<LoaiHinhCanhTacModel> LoaiHinhCanhTacService) : BlazorCoreBase
    {
        private static List<string> _muaVu = new List<string>() {
            MuaVu.VuThuDong.ToString(),
            MuaVu.VuHeThu.ToString(),
            MuaVu.VuDongXuan.ToString(),
            MuaVu.VuXuan.ToString(),
            MuaVu.VuHe.ToString(),
            MuaVu.VuThu.ToString(),
            MuaVu.VuDong.ToString()
        };
        private static List<string> _trangThaiBanTinChiTietList = new List<string>() {
            TrangThaiBanGhiChiTiet.New.ToString(),
            TrangThaiBanGhiChiTiet.Saved.ToString(),
            TrangThaiBanGhiChiTiet.Removed.ToString()
        };

        private List<DienTichGieoTrongCayHangNamModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openCayTrongDetailDeleteModal = false;
        private bool openMuaVuDetailDeleteModal = false;
        private bool openCoCauChuLucDetailDeleteModal = false;
        private bool openAddOrUpdateModal = false;

        private DienTichGieoTrongCayHangNamModel SelectedItem { get; set; } = new DienTichGieoTrongCayHangNamModel();
        private List<DienTichGieoTrongCayHangNamCayTrongModel> SelectedCayTrongItemsDetail { get; set; } = new List<DienTichGieoTrongCayHangNamCayTrongModel>();
        private DienTichGieoTrongCayHangNamCayTrongModel? SelectedCayTrongItemDetail { get; set; } = default;
        private List<DienTichGieoTrongCayHangNamCoCauChuLucModel> SelectedCoCauChuLucItemsDetail { get; set; } = new List<DienTichGieoTrongCayHangNamCoCauChuLucModel>();
        private DienTichGieoTrongCayHangNamCoCauChuLucModel? SelectedCoCauChuLucItemDetail { get; set; } = default;
        private List<DienTichGieoTrongCayHangNamTheoMuaVuModel> SelectedMuaVuItemsDetail { get; set; } = new List<DienTichGieoTrongCayHangNamTheoMuaVuModel>();
        private DienTichGieoTrongCayHangNamTheoMuaVuModel? SelectedMuaVuItemDetail { get; set; } = default;

        private string _searchString = "";
        private TinhModel? _selectedTinhFilter { get; set; }
        private LoaiHinhCanhTacModel? _selectedLoaiHinhCanhTacFilter { get; set; }
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
            if (_selectedLoaiHinhCanhTacFilter?.id > 0)
            {
                BuilderQuery += $"&filter[_and][][loai_hinh_canh_tac][_eq]={_selectedLoaiHinhCanhTacFilter?.id}";
            }
            BuilderQuery += $"&filter[_and][][deleted][_eq]=false";

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<DienTichGieoTrongCayHangNamModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<DienTichGieoTrongCayHangNamModel>();
            }
        }

        private async Task LoadCayTrongDetailData()
        {
            var buildQuery = $"sort=cay_giong_cay_trong";
            buildQuery += $"&filter[_and][][dien_tich_gieo_trong_cay_hang_nam][_eq]={SelectedItem.id}";
            buildQuery += $"&filter[_and][][deleted][_eq]=false";
            var result = await DienTichGieoTrongCayHangNamCayTrongService.GetAllAsync(buildQuery);
            SelectedCayTrongItemsDetail = result.Data ?? new List<DienTichGieoTrongCayHangNamCayTrongModel>();
        }
        
        private async Task LoadCoCauChuLucDetailData()
        {
            var buildQuery = $"sort=cay_giong_cay_trong";
            buildQuery += $"&filter[_and][][dien_tich_gieo_trong_cay_hang_nam][_eq]={SelectedItem.id}";
            buildQuery += $"&filter[_and][][deleted][_eq]=false";
            var result = await DienTichGieoTrongCayHangNamCayTrongService.GetAllAsync(buildQuery);
            SelectedCayTrongItemsDetail = result.Data ?? new List<DienTichGieoTrongCayHangNamCayTrongModel>();
        }
        
        private async Task LoadMuaVuDetailData()
        {
            var buildQuery = $"sort=mua_vu";
            buildQuery += $"&filter[_and][][dien_tich_gieo_trong_cay_hang_nam][_eq]={SelectedItem.id}";
            buildQuery += $"&filter[_and][][deleted][_eq]=false";
            var result = await DienTichGieoTrongCayHangNamCayTrongService.GetAllAsync(buildQuery);
            SelectedCayTrongItemsDetail = result.Data ?? new List<DienTichGieoTrongCayHangNamCayTrongModel>();
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
        private async Task<IEnumerable<CayGiongCayTrongModel>> LoadCayGiongCayTrongData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, CayGiongCayTrongService);
        }
        private async Task<IEnumerable<LoaiHinhCanhTacModel>> LoadLoaiHinhCanhTacData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, LoaiHinhCanhTacService);
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

        private void OpenDeleteModal(DienTichGieoTrongCayHangNamModel item)
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
            SelectedItem = new DienTichGieoTrongCayHangNamModel();

            openDeleteModal = false;
        }

        private void OpenCayTrongDetailDeleteModal(DienTichGieoTrongCayHangNamCayTrongModel item)
        {
            SelectedCayTrongItemDetail = item;

            openCayTrongDetailDeleteModal = true;
        }

        private void OnCayTrongDetailDelete()
        {
            if (SelectedCayTrongItemDetail == null)
            {
                AlertService.ShowAlert("Không có dữ liệu để xóa", "warning");
                return;
            }

            foreach (var item in SelectedCayTrongItemsDetail)
            {
                if (item.id > 0 && item.id == SelectedCayTrongItemDetail.id || item.sort > 0 && item.sort == SelectedCayTrongItemDetail.sort)
                {
                    item.deleted = true;
                }
            }

            SelectedCayTrongItemDetail = default;

            openCayTrongDetailDeleteModal = false;

            if (!SelectedCayTrongItemsDetail.Any(c => c.deleted == null || c.deleted == false))
                SelectedCayTrongItemsDetail.Add(new DienTichGieoTrongCayHangNamCayTrongModel()
                {
                    dien_tich_gieo_trong_cay_hang_nam = SelectedItem,
                    sort = (SelectedCayTrongItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    code = string.Empty,
                    name = string.Empty
                });
            StateHasChanged();
        }

        private void CloseCayTrongDetailDeleteModal()
        {
            SelectedCayTrongItemDetail = default;

            openCayTrongDetailDeleteModal = false;
        }

        private void OnAddCayTrong()
        {
            if (SelectedCayTrongItemsDetail == null)
                SelectedCayTrongItemsDetail = new List<DienTichGieoTrongCayHangNamCayTrongModel>();

            SelectedCayTrongItemsDetail.Add(new DienTichGieoTrongCayHangNamCayTrongModel
            {
                dien_tich_gieo_trong_cay_hang_nam = SelectedItem,
                sort = (SelectedCayTrongItemsDetail.Max(c => c.sort) ?? 0) + 1,
                code = string.Empty,
                name = string.Empty
            });
        }

        private void OpenCoCauChuLucDetailDeleteModal(DienTichGieoTrongCayHangNamCoCauChuLucModel item)
        {
            SelectedCoCauChuLucItemDetail = item;

            openCoCauChuLucDetailDeleteModal = true;
        }

        private void OnCoCauChuLucDetailDelete()
        {
            if (SelectedCoCauChuLucItemDetail == null)
            {
                AlertService.ShowAlert("Không có dữ liệu để xóa", "warning");
                return;
            }

            foreach (var item in SelectedCoCauChuLucItemsDetail)
            {
                if (item.id > 0 && item.id == SelectedCoCauChuLucItemDetail.id || item.sort > 0 && item.sort == SelectedCoCauChuLucItemDetail.sort)
                {
                    item.deleted = true;
                }
            }

            SelectedCoCauChuLucItemDetail = default;

            openCoCauChuLucDetailDeleteModal = false;

            if (!SelectedCoCauChuLucItemsDetail.Any(c => c.deleted == null || c.deleted == false))
                SelectedCoCauChuLucItemsDetail.Add(new DienTichGieoTrongCayHangNamCoCauChuLucModel()
                {
                    dien_tich_gieo_trong_cay_hang_nam = SelectedItem,
                    sort = (SelectedCoCauChuLucItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    code = string.Empty,
                    name = string.Empty
                });
            StateHasChanged();
        }

        private void CloseCoCauChuLucDetailDeleteModal()
        {
            SelectedCoCauChuLucItemDetail = default;

            openCoCauChuLucDetailDeleteModal = false;
        }

        private void OnAddCoCauChuLuc()
        {
            if (SelectedCoCauChuLucItemsDetail == null)
                SelectedCoCauChuLucItemsDetail = new List<DienTichGieoTrongCayHangNamCoCauChuLucModel>();

            SelectedCoCauChuLucItemsDetail.Add(new DienTichGieoTrongCayHangNamCoCauChuLucModel
            {
                dien_tich_gieo_trong_cay_hang_nam = SelectedItem,
                sort = (SelectedCoCauChuLucItemsDetail.Max(c => c.sort) ?? 0) + 1,
                code = string.Empty,
                name = string.Empty
            });
        }

        private void OpenMuaVuDetailDeleteModal(DienTichGieoTrongCayHangNamTheoMuaVuModel item)
        {
            SelectedMuaVuItemDetail = item;

            openMuaVuDetailDeleteModal = true;
        }

        private void OnMuaVuDetailDelete()
        {
            if (SelectedMuaVuItemDetail == null)
            {
                AlertService.ShowAlert("Không có dữ liệu để xóa", "warning");
                return;
            }

            foreach (var item in SelectedMuaVuItemsDetail)
            {
                if (item.id > 0 && item.id == SelectedMuaVuItemDetail.id || item.sort > 0 && item.sort == SelectedMuaVuItemDetail.sort)
                {
                    item.deleted = true;
                }
            }

            SelectedMuaVuItemDetail = default;

            openMuaVuDetailDeleteModal = false;

            if (!SelectedMuaVuItemsDetail.Any(c => c.deleted == null || c.deleted == false))
                SelectedMuaVuItemsDetail.Add(new DienTichGieoTrongCayHangNamTheoMuaVuModel()
                {
                    dien_tich_gieo_trong_cay_hang_nam = SelectedItem,
                    sort = (SelectedMuaVuItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    code = string.Empty,
                    name = string.Empty
                });
            StateHasChanged();
        }

        private void CloseMuaVuDetailDeleteModal()
        {
            SelectedMuaVuItemDetail = default;

            openMuaVuDetailDeleteModal = false;
        }

        private void OnAddMuaVu()
        {
            if (SelectedMuaVuItemsDetail == null)
                SelectedMuaVuItemsDetail = new List<DienTichGieoTrongCayHangNamTheoMuaVuModel>();

            SelectedMuaVuItemsDetail.Add(new DienTichGieoTrongCayHangNamTheoMuaVuModel
            {
                dien_tich_gieo_trong_cay_hang_nam = SelectedItem,
                sort = (SelectedMuaVuItemsDetail.Max(c => c.sort) ?? 0) + 1,
                code = string.Empty,
                name = string.Empty
            });
        }

        private async Task OpenAddOrUpdateModal(DienTichGieoTrongCayHangNamModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item : new DienTichGieoTrongCayHangNamModel();
            SelectedCayTrongItemsDetail = new List<DienTichGieoTrongCayHangNamCayTrongModel>();

            if (SelectedItem.id > 0)
            {
                await LoadCayTrongDetailData();
            }

            if (!SelectedCayTrongItemsDetail.Any())
                SelectedCayTrongItemsDetail.Add(new DienTichGieoTrongCayHangNamCayTrongModel()
                {
                    dien_tich_gieo_trong_cay_hang_nam = SelectedItem,
                    sort = (SelectedCayTrongItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    code = string.Empty,
                    name = string.Empty
                });
            
            if (!SelectedCoCauChuLucItemsDetail.Any())
                SelectedCoCauChuLucItemsDetail.Add(new DienTichGieoTrongCayHangNamCoCauChuLucModel()
                {
                    dien_tich_gieo_trong_cay_hang_nam = SelectedItem,
                    sort = (SelectedCoCauChuLucItemsDetail.Max(c => c.sort) ?? 0) + 1,
                    code = string.Empty,
                    name = string.Empty
                });
            
            if (!SelectedMuaVuItemsDetail.Any())
                SelectedMuaVuItemsDetail.Add(new DienTichGieoTrongCayHangNamTheoMuaVuModel()
                {
                    dien_tich_gieo_trong_cay_hang_nam = SelectedItem,
                    sort = (SelectedMuaVuItemsDetail.Max(c => c.sort) ?? 0) + 1,
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
                    var cayTrongChitietList = SelectedCayTrongItemsDetail
                        .Where(c => c.deleted == false || c.deleted == null)
                        .Select(c =>
                        {
                            c.dien_tich_gieo_trong_cay_hang_nam = result.Data;
                            return c;
                        })
                        .ToList();
                    
                    var coCauChuLucChitietList = SelectedCoCauChuLucItemsDetail
                        .Where(c => c.deleted == false || c.deleted == null)
                        .Select(c =>
                        {
                            c.dien_tich_gieo_trong_cay_hang_nam = result.Data;
                            return c;
                        })
                        .ToList();
                    
                    var muaVuChitietList = SelectedMuaVuItemsDetail
                        .Where(c => c.deleted == false || c.deleted == null)
                        .Select(c =>
                        {
                            c.dien_tich_gieo_trong_cay_hang_nam = result.Data;
                            return c;
                        })
                        .ToList();

                    var cayTrongDetailResult = await DienTichGieoTrongCayHangNamCayTrongService.CreateAsync(cayTrongChitietList);
                    if (!cayTrongDetailResult.IsSuccess)
                    {
                        AlertService.ShowAlert(cayTrongDetailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                        return;
                    }
                    var coCauChuLucDetailResult = await DienTichGieoTrongCayHangNamCoCauChuLucService.CreateAsync(coCauChuLucChitietList);
                    if (!coCauChuLucDetailResult.IsSuccess)
                    {
                        AlertService.ShowAlert(coCauChuLucDetailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                        return;
                    }
                    var muaVuDetailResult = await DienTichGieoTrongCayHangNamTheoMuaVuService.CreateAsync(muaVuChitietList);
                    if (!muaVuDetailResult.IsSuccess)
                    {
                        AlertService.ShowAlert(muaVuDetailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
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
                    var addNewCayTrongChiTietList = SelectedCayTrongItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id == 0)
                        .Select(c =>
                        {
                            c.dien_tich_gieo_trong_cay_hang_nam = SelectedItem;
                            return c;
                        }).ToList();
                    var removeCayTrongChiTietList = SelectedCayTrongItemsDetail
                        .Where(c => c.deleted == true && c.id > 0)
                        .Select(c =>
                        {
                            c.dien_tich_gieo_trong_cay_hang_nam = SelectedItem;
                            c.deleted = true;
                            return c;
                        }).ToList();
                    var updateCayTrongChiTietList = SelectedCayTrongItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id > 0)
                        .Select(c =>
                        {
                            c.dien_tich_gieo_trong_cay_hang_nam = SelectedItem;
                            return c;
                        }).ToList();

                    if (addNewCayTrongChiTietList.Any())
                    {
                        var detailResult = await DienTichGieoTrongCayHangNamCayTrongService.CreateAsync(addNewCayTrongChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (removeCayTrongChiTietList.Any())
                    {
                        var detailResult = await DienTichGieoTrongCayHangNamCayTrongService.DeleteAsync(removeCayTrongChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi xóa chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (updateCayTrongChiTietList.Any())
                    {
                        var detailResult = await DienTichGieoTrongCayHangNamCayTrongService.UpdateAsync(updateCayTrongChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi cập nhật chi tiết dữ liệu", "danger");
                            return;
                        }
                    }


                    var addNewCoCauChuLucChiTietList = SelectedCoCauChuLucItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id == 0)
                        .Select(c =>
                        {
                            c.dien_tich_gieo_trong_cay_hang_nam = SelectedItem;
                            return c;
                        }).ToList();
                    var removeCoCauChuLucChiTietList = SelectedCoCauChuLucItemsDetail
                        .Where(c => c.deleted == true && c.id > 0)
                        .Select(c =>
                        {
                            c.dien_tich_gieo_trong_cay_hang_nam = SelectedItem;
                            c.deleted = true;
                            return c;
                        }).ToList();
                    var updateCoCauChuLucChiTietList = SelectedCoCauChuLucItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id > 0)
                        .Select(c =>
                        {
                            c.dien_tich_gieo_trong_cay_hang_nam = SelectedItem;
                            return c;
                        }).ToList();

                    if (addNewCoCauChuLucChiTietList.Any())
                    {
                        var detailResult = await DienTichGieoTrongCayHangNamCoCauChuLucService.CreateAsync(addNewCoCauChuLucChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (removeCoCauChuLucChiTietList.Any())
                    {
                        var detailResult = await DienTichGieoTrongCayHangNamCoCauChuLucService.DeleteAsync(removeCoCauChuLucChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi xóa chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (updateCoCauChuLucChiTietList.Any())
                    {
                        var detailResult = await DienTichGieoTrongCayHangNamCoCauChuLucService.UpdateAsync(updateCoCauChuLucChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi cập nhật chi tiết dữ liệu", "danger");
                            return;
                        }
                    }


                    var addNewMuaVuChiTietList = SelectedMuaVuItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id == 0)
                        .Select(c =>
                        {
                            c.dien_tich_gieo_trong_cay_hang_nam = SelectedItem;
                            return c;
                        }).ToList();
                    var removeMuaVuChiTietList = SelectedMuaVuItemsDetail
                        .Where(c => c.deleted == true && c.id > 0)
                        .Select(c =>
                        {
                            c.dien_tich_gieo_trong_cay_hang_nam = SelectedItem;
                            c.deleted = true;
                            return c;
                        }).ToList();
                    var updateMuaVuChiTietList = SelectedMuaVuItemsDetail
                        .Where(c => (c.deleted == false || c.deleted == null) && c.id > 0)
                        .Select(c =>
                        {
                            c.dien_tich_gieo_trong_cay_hang_nam = SelectedItem;
                            return c;
                        }).ToList();

                    if (addNewMuaVuChiTietList.Any())
                    {
                        var detailResult = await DienTichGieoTrongCayHangNamTheoMuaVuService.CreateAsync(addNewMuaVuChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi thêm mới chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (removeMuaVuChiTietList.Any())
                    {
                        var detailResult = await DienTichGieoTrongCayHangNamTheoMuaVuService.DeleteAsync(removeMuaVuChiTietList);
                        if (!detailResult.IsSuccess)
                        {
                            AlertService.ShowAlert(detailResult.Message ?? "Lỗi khi xóa chi tiết dữ liệu", "danger");
                            return;
                        }
                    }

                    if (updateMuaVuChiTietList.Any())
                    {
                        var detailResult = await DienTichGieoTrongCayHangNamTheoMuaVuService.UpdateAsync(updateMuaVuChiTietList);
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
            SelectedItem = new DienTichGieoTrongCayHangNamModel();
            openAddOrUpdateModal = false;
        }

        private async Task OnTinhFilterChanged(TinhModel? selected)
        {
            _selectedTinhFilter = selected;

            await LoadData();
        }
        
        private async Task OnLoaiHinhCanhTacFilterChanged(LoaiHinhCanhTacModel? selected)
        {
            _selectedLoaiHinhCanhTacFilter = selected;

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
                        case nameof(SelectedItem.ngay_du_lieu):
                            SelectedItem.ngay_du_lieu = null;
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
                        case nameof(SelectedItem.ngay_du_lieu):
                            SelectedItem.ngay_du_lieu = date;
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
    }
}
