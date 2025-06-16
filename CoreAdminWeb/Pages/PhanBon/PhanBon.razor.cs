using CoreAdminWeb.Enums;
using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Model.LoaiPhanBon;
using CoreAdminWeb.Model.PhanBon;
using CoreAdminWeb.Model.QuanLyCoSoSanXuatPhanBon;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Shared.Base;

namespace CoreAdminWeb.Pages.PhanBon
{
    public partial class PhanBon(IBaseService<PhanBonModel> MainService,
                                           IBaseService<LoaiPhanBonModel> LoaiPhanBonService,
                                           IBaseService<QuanLyCoSoSanXuatPhanBonModel> CoSoSanXuatPhanBonService,
                                           IBaseService<DonViTinhModel> DonViTinhService) : BlazorCoreBase
    {
        private static List<int> NhomCoSoList = new List<int>() { (int)NhomCoSo.toChuc, (int)NhomCoSo.caNhan, (int)NhomCoSo.khac };

        private List<PhanBonModel> MainModels { get; set; } = new();
        private bool openDeleteModal = false;
        private bool openAddOrUpdateModal = false;
        private PhanBonModel SelectedItem { get; set; } = new PhanBonModel();
        private string _searchString = "";
        private string _titleAddOrUpdate = "Thêm mới";

        private List<string> _nguonGocList = new List<string>
        {
            NguonGocPhanBon.NhapKhau.ToString(),
            NguonGocPhanBon.SanXuat.ToString(),
            NguonGocPhanBon.Khac.ToString()
        };

        private QuanLyCoSoSanXuatPhanBonModel? _selectedDonViFilter { get; set; }
        private LoaiPhanBonModel? _selectedLoaiPhanBonFilter { get; set; }

        private LoaiPhanBonModel? _selectedLoaiPhanBon { get; set; }
        private QuanLyCoSoSanXuatPhanBonModel? _selectedCoSoSanXuatPhanBon { get; set; }
        private DonViTinhModel? _selectedDonViTinh { get; set; }

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
            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&filter[_and][][name][_contains]={_searchString}";
            }
            if (_selectedDonViFilter != null && _selectedDonViFilter.id > 0)
            {
                BuilderQuery += $"&filter[_and][][co_so_san_xuat_phan_bon_id][_eq]={_selectedDonViFilter.id}";
            }

            if (_selectedDonViFilter != null && _selectedDonViFilter.id > 0)
            {
                BuilderQuery += $"&filter[_and][][loai_phan_bon_id][_eq]={_selectedDonViFilter.id}";
            }

            BuilderQuery += $"&filter[_and][][deleted][_eq]=false";

            var result = await MainService.GetAllAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                MainModels = result.Data ?? new List<PhanBonModel>();
                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
            else
            {
                MainModels = new List<PhanBonModel>();
            }
        }

        private async Task<IEnumerable<DonViTinhModel>> LoadDonViTinhData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, DonViTinhService);
        }

        private async Task<IEnumerable<QuanLyCoSoSanXuatPhanBonModel>> LoadCoSoSanXuatPhanBonData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, CoSoSanXuatPhanBonService);
        }

        private async Task<IEnumerable<LoaiPhanBonModel>> LoadLoaiPhanBonData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, LoaiPhanBonService);
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


        private void OpenDeleteModal(PhanBonModel item)
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
            SelectedItem = new PhanBonModel();
            openDeleteModal = false;
        }

        private void OpenAddOrUpdateModal(PhanBonModel? item)
        {
            _titleAddOrUpdate = item != null ? "Sửa" : "Thêm mới";
            SelectedItem = item != null ? item.DeepClone() : new PhanBonModel();

            openAddOrUpdateModal = true;
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
            SelectedItem = new PhanBonModel();
            openAddOrUpdateModal = false;
        }

        private async Task OnSelectedFilterCoSoSanXuatPhanBonChanged(QuanLyCoSoSanXuatPhanBonModel? selected)
        {
            _selectedDonViFilter = selected;
            await LoadData();
        }

        private async Task OnLoaiPhanBonFilterChanged(LoaiPhanBonModel? selected)
        {
            _selectedLoaiPhanBonFilter = selected;
            await LoadData();
        }

        private void OnLoaiPhanBonChanged(LoaiPhanBonModel? selected)
        {
            SelectedItem.loai_phan_bon = selected;
        }

        private void OnCoSoSanXuatChanged(QuanLyCoSoSanXuatPhanBonModel? selected)
        {
            SelectedItem.co_so_san_xuat_phan_bon = selected;
        }

        private void OnDonViTinhChanged(DonViTinhModel selected)
        {
            SelectedItem.don_vi_tinh = selected;
        }
    }
}
