﻿using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Model.User;

namespace CoreAdminWeb.Providers
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public ApiAuthenticationStateProvider(
            ILocalStorageService localStorage
        )
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = "";
            try
            {
                InjectServiceForHttpClient();
                savedToken = await _localStorage.GetItemAsync<string>("accessToken");

                if (string.IsNullOrWhiteSpace(savedToken))
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }
                RequestClient.AttachToken(savedToken);

                var claims = ParseClaimsFromJwt1(savedToken);

                if (!CheckExpiredToken(claims))
                {
                    await _localStorage.RemoveItemAsync("claims");
                    await _localStorage.RemoveItemAsync("accessToken");
                    await _localStorage.RemoveItemAsync("userName");
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                return new AuthenticationState(
                    new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"))
                );
            }
            catch
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public static bool CheckExpiredToken(IEnumerable<Claim> claims)
        {
            var expiredClaim = claims.FirstOrDefault(x => x.Type == "exp");
            var epochTime = long.Parse(expiredClaim?.Value ?? "0");
            DateTime tokenTime = DateTime.UnixEpoch.AddSeconds(epochTime);
            if (tokenTime.ToLocalTime() < DateTime.Now)
            {
                return false;
            }

            return true;
        }

        private void InjectServiceForHttpClient()
        {
            RequestClient.InjectServices(_localStorage);
        }

        public void MarkUserAsAuthenticated(string userName)
        {
            var authenticatedUser = new ClaimsPrincipal(
                new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userName) }, "apiauth")
            );
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public async Task MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            await _localStorage.RemoveItemAsync("accessToken");
            await _localStorage.RemoveItemAsync("claims");
            await _localStorage.RemoveItemAsync("userName");
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));

            NotifyAuthenticationStateChanged(authState);
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt1(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();

            var decodedValue = handler.ReadJwtToken(jwt);
            return decodedValue.Claims;
        }
    }
}
