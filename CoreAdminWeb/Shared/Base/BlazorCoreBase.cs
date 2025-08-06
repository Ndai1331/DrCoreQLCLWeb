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
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        protected AuthenticationStateProvider AuthStateProvider { get; set; }

        [Inject]
        protected IUserService UserService { get; set; }

        [Inject]
        protected IMenuService MenuService { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected AlertService AlertService { get; set; }

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        protected UserModel CurrentUser { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        protected bool IsAuthenticated { get; set; }
        public bool IsLoading { get; set; } = false;

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public string BuilderQuery { get; set; } = "";
        public string AcceptFileTypes { get; set; } = "application/vnd.openxmlformats-officedocument.wordprocessingml.document, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.openxmlformats-officedocument.presentationml.presentation, application/pdf,application/zip, application/x-7z-compressed, application/x-rar-compressed, application/x-tar, application/x-gzip, application/x-bzip2, application/x-compressed, application/x-compressed-tar, application/x-compressed-zip, application/x-compressed-rar, application/x-compressed-7z";

        protected List<MenuResponse> Menus { get; set; } = new List<MenuResponse>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            ResetPage();
        }
        public void ResetPage()
        {
            Page = 1;
            PageSize = 10;
            TotalCount = 0;
            TotalPages = 0;
            TotalItems = 0;
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            if (AuthStateProvider == null)
            {
                return false;
            }

            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            return authState?.User?.Identity?.IsAuthenticated ?? false;
        }

        protected virtual async Task<List<MenuResponse>> LoadMenuData(int external_system_id = 2)
        {
            try
            {
                if (MenuService == null)
                {
                    Console.WriteLine("MenuService is null");
                    return new List<MenuResponse>(); // Fallback
                }

                var menus = await MenuService.GetMenusAsync(external_system_id);

                if (menus.Data == null)
                {
                    Console.WriteLine("Menus is null");
                    return new List<MenuResponse>(); // Fallback
                }

                return menus.Data;
            }
            catch (Exception)
            {
                // Handle error
                return new List<MenuResponse>();
            }
        }
        protected async Task Logout()
        {
            try
            {
                if (AuthStateProvider == null)
                {
                    Console.WriteLine("AuthStateProvider is null");
                    return; // Fallback
                }

                await ((ApiAuthenticationStateProvider)AuthStateProvider).MarkUserAsLoggedOut();
                NavigationManager?.NavigateTo("/signin", true);
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

        public void BuildPaginationQuery(int page, int pageSize, string sort = "id", bool isAsc = false)
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
                if (JsRuntime != null)
                {
                    await JsRuntime.InvokeVoidAsync("preventEnterKeyDefault", "search");
                }
                await loadData.Invoke();
            }
        }


        public static async Task<T?> LoadDefaultData<T>(IBaseService<T> service)
        {
            var query = BuildBaseQuery(string.Empty);
            var result = await service.GetAllAsync(query);
            return result != null && result.IsSuccess ? result.Data!.FirstOrDefault() : default;
        }

        public static async Task<IEnumerable<T>> LoadBlazorTypeaheadData<T>(string searchText, IBaseService<T> service, string? otherQuery = "")
        {
            var query = BuildBaseQuery(searchText);

            if (!string.IsNullOrEmpty(otherQuery))
            {
                query += $"&{otherQuery}";
            }

            var result = await service.GetAllAsync(query);
            return result.IsSuccess ? result.Data ?? new List<T>() : new List<T>();
        }

        public static async Task<List<T>> LoadDataInTable<T>(
            IEnumerable<T> allItems,
            string filter,
            CancellationToken token,
            IBaseService<T> service,
            string? otherQuery = default)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(allItems);

                // Debouncing - wait 300ms before making API call
                await Task.Delay(300, token);

                // If not in cache, load from API
                Console.WriteLine($"Loading filter data from API for '{filter}'");
                var result = await LoadBlazorTypeaheadData(filter ?? string.Empty, service, otherQuery);
                return result?.ToList() ?? new List<T>();
            }
            catch (OperationCanceledException)
            {
                // Filter operation was cancelled (user typed more characters)
                Console.WriteLine($"Filter operation cancelled for '{filter}'");
                return new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in filterFunction: {ex.Message}");
                return new List<T>();
            }
        }

        private static string BuildBaseQuery(string searchText = "")
        {
            var query = "filter[_and][][deleted][_eq]=false&sort=sort";
            if (!string.IsNullOrEmpty(searchText))
            {
                if (!string.IsNullOrEmpty(query))
                {
                    query += "&";
                }

                query += $"filter[_and][][name][_contains]={searchText}";
            }
            return query;
        }

        public async Task OnPageSizeChanged(int newSize, Func<Task> loadData)
        {
            Page = 1;
            PageSize = newSize;
            await loadData();
        }

        public async Task SelectedPage(int page, Func<Task> loadData)
        {
            Page = page;
            await loadData();
        }
    }
}