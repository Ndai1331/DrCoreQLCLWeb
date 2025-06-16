using System.Security.Claims;
using CoreAdminWeb.Model.User;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.Users;
using Microsoft.AspNetCore.Components.Authorization;

namespace CoreAdminWeb.Services.Auth
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        public UserModel? CurrentUser { get; private set; }
        private readonly IUserService _userService;
        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        public AuthStateProvider(IUserService userService)
        {
            _userService = userService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                string accessToken = _userService.GetAccessTokenAsync();
                if (!string.IsNullOrEmpty(accessToken))
                {
                    RequestClient.AttachToken(accessToken);
                }

                if(CurrentUser == null)
                {
                    var userResponse = await _userService.GetCurrentUserAsync();
                    if (userResponse?.Data != null)
                    {
                        CurrentUser = userResponse.Data;
                        var identity = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, userResponse.Data.id),
                            new Claim(ClaimTypes.Name, $"{userResponse.Data.first_name} {userResponse.Data.last_name}"),
                            new Claim(ClaimTypes.Email, userResponse.Data.email),
                            new Claim(ClaimTypes.Role, userResponse.Data.role ?? "user")
                        }, "DrCoreAuth");

                        _currentUser = new ClaimsPrincipal(identity);
                    }
                    else
                    {
                        CurrentUser = null;
                    }
                }
            }
            catch
            {
                CurrentUser = null;
            }

            return new AuthenticationState(_currentUser);
        }

        public async Task<UserModel?> GetCurrentUserAsync()
        {
            return CurrentUser;
        }

        public async Task LogoutAsync()
        {
            try
            {
                var refreshToken = _userService.GetRefreshTokenAsync();
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    await _userService.LogoutAsync(refreshToken);
                }
            }
            catch
            {
                // Ignore logout errors
            }
            finally
            {
                ClearUser();
            }
        }

        public void ClearUser()
        {
            CurrentUser = null; 
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
} 