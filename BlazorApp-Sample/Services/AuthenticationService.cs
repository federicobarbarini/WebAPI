using BlazorApp_Sample.Helpers;
using BlazorApp_Sample.Proxy.API.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlazorApp_Sample.Services
{

    public interface IAuthenticationService
    {
        Task Login(string username, string password);
        Task Logout();
    }

    public class AuthenticationService : IAuthenticationService
    {
        #region --> Dichiarazioni

        private Client authClient;
        private AppAuthenticationStateProvider _authProvider;
        private ILocalStorageService _localStorageService;

        #endregion

        #region --> Costruttore

        public AuthenticationService(HttpClient httpclient,
                                     AppAuthenticationStateProvider authProvider,
                                     ILocalStorageService localStorageService)
        {
            authClient = new Client(string.Empty, httpclient);
            _authProvider = authProvider;
            _localStorageService = localStorageService;
        }

        #endregion

        #region --> Metodi

        public async Task Login(string username, string password)
        {
            var token = await authClient.LoginAsync(username, password);
            if (string.IsNullOrEmpty(token)) return;
            await _localStorageService.SetItem("token", token);
            await _authProvider.SignIn();
        }

        public async Task Logout()
        {
            await _localStorageService.RemoveItem("token");
            _authProvider.SignOut();
        }

        #endregion


    }
}