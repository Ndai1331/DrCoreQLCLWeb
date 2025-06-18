using CoreAdminWeb.Model.Menus;
using CoreAdminWeb.Model.User;
using CoreAdminWeb.Services;
using CoreAdminWeb.Services.Auth;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Services.Menus;
using CoreAdminWeb.Services.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace CoreAdminWeb.Shared.Base
{
    public class BlazorCoreBase : ComponentBase
    {
        [Inject]
        protected AuthenticationStateProvider AuthStateProvider { get; set; }

        [Inject]
        protected IUserService UserService { get; set; }

        [Inject]
        protected IMenuService MenuService { get; set; }

        [Inject]
        protected NavigationManager? NavigationManager { get; set; }

        [Inject]
        protected AlertService AlertService { get; set; }

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        protected UserModel? CurrentUser { get; private set; }
        protected bool IsAuthenticated { get; private set; }
        public bool IsLoading { get; set; } = true;

        public static int Page { get; set; } = 1;
        public static int PageSize { get; set; } = 10;
        public static int TotalCount { get; set; }
        public static int TotalPages { get; set; }
        public static int TotalItems { get; set; }
        public static string BuilderQuery { get; set; } = "";
        public static string AcceptFileTypes { get; set; } = "application/vnd.openxmlformats-officedocument.wordprocessingml.document, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.openxmlformats-officedocument.presentationml.presentation, application/pdf,application/zip, application/x-7z-compressed, application/x-rar-compressed, application/x-tar, application/x-gzip, application/x-bzip2, application/x-compressed, application/x-compressed-tar, application/x-compressed-zip, application/x-compressed-rar, application/x-compressed-7z";

        public static List<Model.Base.Status> StatusList = new List<
        Model.Base.Status> { Model.Base.Status.active,
         Model.Base.Status.deactive };
        protected List<MenuResponse> Menus { get; set; } = new List<MenuResponse>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadUserData();
        }

        protected virtual async Task LoadUserData()
        {
            try
            {
                IsLoading = true;
                var authState = await AuthStateProvider.GetAuthenticationStateAsync();
                IsAuthenticated = authState.User.Identity?.IsAuthenticated ?? false;
                if (IsAuthenticated)
                {
                    CurrentUser = await ((AuthStateProvider)AuthStateProvider).GetCurrentUserAsync();
                }
            }
            catch (Exception ex)
            {
                // Handle error
                Console.WriteLine($"Error loading user data: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        protected virtual async Task<List<MenuResponse>> LoadMenuData(int external_system_id = 2)
        {
            try
            {
                IsLoading = true;
                var menus = await MenuService.GetMenusAsync(external_system_id);

                if (menus.Data == null)
                {
                    Console.WriteLine("Menus is null");
                    return new List<MenuResponse>(); // Fallback
                }

                return menus.Data;
            }
            catch (Exception ex)
            {
                // Handle error
                return null;
            }
        }
        protected async Task Logout()
        {
            try
            {
                if (CurrentUser != null)
                {
                    await ((AuthStateProvider)AuthStateProvider).LogoutAsync();
                    NavigationManager.NavigateTo("/login", true);
                }
            }
            catch (Exception ex)
            {
                // Handle error
                Console.WriteLine($"Error during logout: {ex.Message}");
            }
        }

        protected bool HasRole(string roleName)
        {
            return CurrentUser?.role == roleName;
        }

        protected bool HasAnyRole(params string[] roleNames)
        {
            return roleNames.Contains(CurrentUser?.role);
        }

        public static void BuildPaginationQuery(int page, int pageSize, string sort = "sort", bool isAsc = true)
        {
            BuilderQuery = $"limit={pageSize}&offset={(page - 1) * pageSize}&meta=filter_count";
            if (!isAsc)
            {
                BuilderQuery += $"&sort=-{sort}";
            }
            else
            {
                BuilderQuery += $"&sort={sort}";
            }
        }



        public async Task OnInputKeyDownSearch(KeyboardEventArgs e, Func<Task> loadData)
        {
            if (e.Key == "Enter" && !e.ShiftKey)
            {
                await JsRuntime.InvokeVoidAsync("preventEnterKeyDefault", "search");
                await loadData.Invoke();
            }
        }


        public async Task<IEnumerable<T>> LoadBlazorTypeaheadData<T>(string searchText, IBaseService<T> service, string? otherQuery = "", bool isIgnoreCheck = false)
        {
            var query = BuildBaseQuery(searchText, isIgnoreCheck);

            if (!string.IsNullOrEmpty(otherQuery))
                query += $"&{otherQuery}";

            var result = await service.GetAllAsync(query);
            return result.IsSuccess ? result.Data ?? new List<T>() : new List<T>();
        }


        private string BuildBaseQuery(string searchText = "", bool isIgnoreCheck = false)
        {
            var query = "filter[_and][][deleted][_eq]=false&sort=sort";
            if (!string.IsNullOrEmpty(searchText))
            {
                if (!string.IsNullOrEmpty(query))
                    query += "&";
                query += $"filter[_and][][name][_contains]={searchText}";
            }
            return query;
        }


        public async Task OnPageSizeChanged(Func<Task> loadData)
        {
            Page = 1;
            await loadData();
        }

        public async Task PreviousPage(Func<Task> loadData)
        {
            if (Page > 1)
            {
                Page--;
                await loadData();
            }
        }

        public async Task SelectedPage(int page, Func<Task> loadData)
        {
            Page = page;
            await loadData();
        }

        public async Task NextPage(Func<Task> loadData)
        {
            if (Page < TotalPages)
            {
                Page++;
                await loadData();
            }
        }

    }
}