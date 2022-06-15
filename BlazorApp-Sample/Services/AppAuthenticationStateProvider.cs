using BlazorApp_Sample.Helpers;
using BlazorApp_Sample.Proxy.API.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlazorApp_Sample.Services
{

    public class AppAuthenticationStateProvider : AuthenticationStateProvider
    {

        #region --> Dichiarazioni

        private HttpClient _httpclient;
        private readonly ILocalStorageService _localStorageService;
        private readonly JwtSecurityTokenHandler jwtSecurityTokenHandler = new();

        #endregion
        
        #region --> Costruttori

        public AppAuthenticationStateProvider(HttpClient httpclient,
                                              ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
            _httpclient = httpclient;
        }

        #endregion

        #region --> Metodi

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                string token = await _localStorageService.GetItem<string>("token");
                if (string.IsNullOrWhiteSpace(token))
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }
                JwtSecurityToken jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(token);
                DateTime expiry = jwtSecurityToken.ValidTo;

                if (expiry < DateTime.UtcNow)
                {
                    await _localStorageService.RemoveItem("token");
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                //Get Claims from Token and Build Authenticated User Object
                _httpclient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var claims = ParseClaims(jwtSecurityToken);
                var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
                return new AuthenticationState(user);

            }
            catch (Exception)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        private IList<Claim> ParseClaims(JwtSecurityToken jwtSecurityToken)
        {
            IList<Claim> claims = jwtSecurityToken.Claims.ToList();
            // Value is username
            claims.Add(new Claim(ClaimTypes.Name, jwtSecurityToken.Subject));
            return claims;

        }

        internal async Task SignIn()
        {
            string token = await _localStorageService.GetItem<string>("token");
            JwtSecurityToken jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(token);
            _httpclient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var claims = ParseClaims(jwtSecurityToken);
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));

            Task<AuthenticationState> autnentcationState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(autnentcationState);

        }

        internal void SignOut()
        {
            _httpclient.DefaultRequestHeaders.Remove("Authorization");
            ClaimsPrincipal nobody = new ClaimsPrincipal(new ClaimsIdentity());
            Task<AuthenticationState> authenticationState = Task.FromResult(new AuthenticationState(nobody));
            NotifyAuthenticationStateChanged(authenticationState);

        }

        #endregion

    }

}