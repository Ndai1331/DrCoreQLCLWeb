using CoreAdminWeb.Helpers;
using CoreAdminWeb.Model;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Services.Files;
using CoreAdminWeb.Shared.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace CoreAdminWeb.Pages.Folder
{
    public partial class Folder(IBaseService<FolderModel> MainService,
                                IFileService FileService,
                                 IBaseService<LinhVucVanBanModel> LinhVucVanBanService,
                                 IBaseService<PhanLoaiVanBanModel> PhanLoaiVanBanService) : BlazorCoreBase
    {
        private List<FolderModel> MainModels { get; set; } = new();
        private List<FileModel> Files { get; set; } = new();
        private FolderModel SelectedItem { get; set; } = new FolderModel();
        private FolderModel SelectedCreateItem { get; set; } = new FolderModel();
        private FileCRUDModel UploadFileCRUD { get; set; } = new FileCRUDModel();
        private FileModel SelectedFile { get; set; } = new FileModel();
        private string _searchString = "";
        private string _coQuanBanHanhSearchString = "";
        private bool openCreateFolderModal = false;
        private bool openUploadFileModal = false;
        private PhanLoaiVanBanModel _selectedPhanLoaiVanBan { get; set; } = new PhanLoaiVanBanModel();
        private LinhVucVanBanModel _selectedLinhVucVanBan { get; set; } = new LinhVucVanBanModel();
        private PhanLoaiVanBanModel _selectedFilterPhanLoaiVanBan { get; set; } = new PhanLoaiVanBanModel();
        private LinhVucVanBanModel _selectedFilterLinhVucVanBan { get; set; } = new LinhVucVanBanModel();
        private IBrowserFile UploadFile { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            PageSize = 12;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadFolders();
                _ = Task.Run(async () =>
                {
                    await Task.Delay(500);
                    await JsRuntime.InvokeVoidAsync("initializeDatePicker");
                });
                await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/assets/js/pages/flatpickr.js");
                StateHasChanged();
            }
        }

        private async Task LoadFolders()
        {
            var result = await MainService.GetAllAsync("filter[system][_eq]=1");
            if (result.IsSuccess)
            {
                MainModels = FolderHelper.CreateSubMenus(result.Data ?? new List<FolderModel>());
                MainModels.Insert(0, new FolderModel() { id = Guid.Empty, name = "Tất cả thư mục" });
            }
            else
            {
                MainModels = new List<FolderModel>() { new FolderModel() { id = Guid.Empty, name = "Tất cả thư mục" } };
            }
        }

        private async Task LoadFiles()
        {
            int index = 2;

            BuilderQuery = $"limit={PageSize}&offset={(Page - 1) * PageSize}&meta=filter_count";

            BuilderQuery += "&filter[_and][0][type][_nnull]=true";
            if (SelectedItem.id == Guid.Empty)
            {
                BuilderQuery += "&filter[_and][1][folder][_null]=true";
            }
            else
            {
                BuilderQuery += $"&filter[_and][1][folder][_eq]={SelectedItem.id}";
            }

            if (_selectedFilterPhanLoaiVanBan != null && _selectedFilterPhanLoaiVanBan.id > 0)
            {
                BuilderQuery += $"&filter[_and][{index}][phan_loai_vb][_eq]={_selectedFilterPhanLoaiVanBan.id}";
                index++;
            }

            if (_selectedFilterLinhVucVanBan != null && _selectedFilterLinhVucVanBan.id > 0)
            {
                BuilderQuery += $"&filter[_and][{index}][linh_vuc_vb][_eq]={_selectedFilterLinhVucVanBan.id}";
                index++;
            }

            if (!string.IsNullOrEmpty(_searchString))
            {
                BuilderQuery += $"&filter[_and][{index}][_or][0][filename_disk][_contains]={_searchString}"
                + $"&filter[_and][{index}][_or][1][type][_contains]={_searchString}"
                + $"&filter[_and][{index}][_or][2][filename_download][_contains]={_searchString}";
                index++;
            }

            if (!string.IsNullOrEmpty(_coQuanBanHanhSearchString))
            {
                BuilderQuery += $"&filter[_and][{index}][co_quan_ban_hanh][_contains]={_coQuanBanHanhSearchString}";
            }

            var result = await FileService.GetAllFileAsync(BuilderQuery);
            if (result.IsSuccess)
            {
                Files = result.Data ?? new List<FileModel>();

                if (result.Meta != null)
                {
                    TotalItems = result.Meta.filter_count ?? 0;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
                }
            }
        }

        private async Task<IEnumerable<PhanLoaiVanBanModel>> LoadPhanLoaiVanBanData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, PhanLoaiVanBanService);
        }
        private async Task<IEnumerable<LinhVucVanBanModel>> LoadLinhVucVanBanData(string searchText)
        {
            return await LoadBlazorTypeaheadData(searchText, LinhVucVanBanService);
        }

        private void OnPhanLoaiVanBanChanged(PhanLoaiVanBanModel selected)
        {
            _selectedPhanLoaiVanBan = selected;
            UploadFileCRUD.phan_loai_vb = selected.id;
        }
        private void OnLinhVucVanBanChanged(LinhVucVanBanModel selected)
        {
            _selectedLinhVucVanBan = selected;
            UploadFileCRUD.linh_vuc_vb = selected.id;
        }


        private async Task OnPhanLoaiVanBanFilterChanged(PhanLoaiVanBanModel selected)
        {
            _selectedFilterPhanLoaiVanBan = selected;
            await LoadFiles();
        }
        private async Task OnLinhVucVanBanFilterChanged(LinhVucVanBanModel selected)
        {
            _selectedFilterLinhVucVanBan = selected;

            await LoadFiles();
        }


        private async Task SelectOnlyOneFolder(FolderModel selected)
        {
            DeselectAll(MainModels);
            selected.isSelected = true;
            SelectedItem = selected;
            ResetPage();
            await LoadFiles();
            StateHasChanged();
        }

        private void OnViewFileDetail(FileModel selected)
        {
            SelectedFile = selected;
            NavigationManager?.NavigateTo($"/file/{selected.id}");
        }

        private void DeselectAll(List<FolderModel> folders)
        {
            foreach (var f in folders)
            {
                f.isSelected = false;
                if (f.sub_folders != null && f.sub_folders.Any())
                {
                    DeselectAll(f.sub_folders);
                }
            }
        }

        private void OpenCreateFolderModal()
        {

            openCreateFolderModal = true;
        }

        private void CloseCreateFolderModal()
        {
            SelectedCreateItem = new FolderModel();
            openCreateFolderModal = false;
        }

        private async Task CreateFolder()
        {
            if (string.IsNullOrEmpty(SelectedCreateItem.name))
            {
                AlertService.ShowAlert("Vui lòng nhập tên thư mục", "warning");
                return;
            }

            SelectedCreateItem.system = 2;
            SelectedCreateItem.parent = SelectedItem.id == Guid.Empty ? null : SelectedItem.id;
            var result = await MainService.CreateAsync(SelectedCreateItem);
            if (result.IsSuccess)
            {
                await LoadFolders();
                AlertService.ShowAlert("Thêm mới thành công!", "success");
            }
            openCreateFolderModal = false;
        }



        private void HandleFileSelect(InputFileChangeEventArgs e)
        {
            var files = e.GetMultipleFiles();
            if (files != null && files.Any())
            {
                ProcessFile(files[0]);
            }
        }

        private async Task HandleDrop(DragEventArgs e)
        {
            var files = await JsRuntime.InvokeAsync<IReadOnlyList<IBrowserFile>>("getDroppedFiles", e);
            if (files?.Count > 0)
            {
                ProcessFile(files[0]);
            }
        }

        private async Task OnUploadFile()
        {
            try
            {
                UploadFileCRUD.folder = SelectedItem.id == Guid.Empty ? null : SelectedItem.id;
                var result = await FileService.UploadFileAsync(UploadFile, UploadFileCRUD);
                if (result.IsSuccess)
                {
                    await LoadFiles();
                    AlertService.ShowAlert("Thêm mới file thành công!", "success");
                    UploadFile = default!;
                    openUploadFileModal = false;
                }
                else
                {
                    AlertService.ShowAlert(result.Message ?? "Lỗi khi thêm mới file", "danger");
                }
            }
            catch (Exception ex)
            {
                AlertService.ShowAlert(ex.Message ?? "Lỗi khi thêm mới file", "danger");
            }
            finally
            {
                StateHasChanged();
            }
        }

        private void ProcessFile(IBrowserFile file)
        {

            if (file != null)
            {
                var maxAllowSize = 10 * 1024 * 1024;
                if (file.Size <= maxAllowSize) // 10MB max size
                {
                    try
                    {
                        UploadFile = file;
                        AlertService.ShowAlert("Đang xử lý file, vui lòng chờ...", "info");
                    }
                    catch (Exception ex)
                    {
                        UploadFile = default!;
                        AlertService.ShowAlert(ex.Message ?? "Lỗi khi xử lý file vui lòng tải lại file", "danger");
                    }
                }
                else
                {
                    UploadFile = default!;
                    AlertService.ShowAlert("File không được vượt quá 10MB", "danger");
                }
            }
            else
            {
                UploadFile = default!;
                AlertService.ShowAlert("File không hợp lệ", "danger");
            }
        }

        private static Task HandleDragOver(DragEventArgs e)
        {
            e.DataTransfer.DropEffect = "copy";
            return Task.CompletedTask;
        }

        private static Task HandleDragEnter(DragEventArgs e)
        {
            // Optional: Add visual feedback for drag enter
            return Task.CompletedTask;
        }

        private static Task HandleDragLeave(DragEventArgs e)
        {
            // Optional: Remove visual feedback for drag leave
            return Task.CompletedTask;
        }

        private void OpenUploadFileModal()
        {
            UploadFileCRUD = new FileCRUDModel();
            _selectedPhanLoaiVanBan = new PhanLoaiVanBanModel();
            _selectedLinhVucVanBan = new LinhVucVanBanModel();
            openUploadFileModal = true;

            _ = Task.Run(async () =>
            {
                await Task.Delay(500);
                await JsRuntime.InvokeVoidAsync("initializeDatePicker");
            });
        }

        private void CloseUploadFileModal()
        {
            UploadFile = default!;
            UploadFileCRUD = new FileCRUDModel();
            _selectedPhanLoaiVanBan = new PhanLoaiVanBanModel();
            _selectedLinhVucVanBan = new LinhVucVanBanModel();
            SelectedFile = new FileModel();
            openUploadFileModal = false;
        }

        private void OnDateChanged(ChangeEventArgs e, string fieldName)
        {
            try
            {
                var dateStr = e.Value?.ToString();
                if (string.IsNullOrEmpty(dateStr))
                {
                    ReflectionHelper.SetDateFieldValue(this, UploadFileCRUD, fieldName, null);
                }
                else
                {
                    var parts = dateStr.Split('/');
                    if (parts.Length == 3 &&
                        int.TryParse(parts[0], out int day) &&
                        int.TryParse(parts[1], out int month) &&
                        int.TryParse(parts[2], out int year))
                    {
                        var date = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Local);
                        ReflectionHelper.SetDateFieldValue(this, UploadFileCRUD, fieldName, date);
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
