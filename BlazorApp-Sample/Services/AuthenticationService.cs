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
        UserInfo User { get; }
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
            _authProvider.AuthenticationStateChanged += _authProvider_AuthenticationStateChanged; 
        }

        #endregion

        #region --> Proprietà

        public UserInfo User { get; private set; }

        #endregion

        #region --> Metodi

        //public async Task Initialize()
        //{

        //    await _authProvider.GetAuthenticationStateAsync();
        //    string token = await _localStorageService.GetItem<string>("token");
        //    if (string.IsNullOrEmpty(token))
        //    {
        //        return;
        //    };

        //    try
        //    {
        //        //--> Valido il token
        //        _httpclient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        //        var claims = await authClient.ValidateAsync();

        //        //--> Leggo l'utente
        //        var userid = claims["UserId"];
        //        User = await authClient.UserAsync(userid);
        //    }
        //    catch (Exception ex)
        //    {
        //        //--> Eseguo il log out
        //        await Logout();
        //    };
        //}

        public async Task Login(string username, string password)
        {
            var token = await authClient.LoginAsync(username, password);
            if (string.IsNullOrEmpty(token)) return;
            await _localStorageService.SetItem("token", token);
            await _authProvider.SignIn();
            //await Initialize();
        }

        public async Task Logout()
        {
            User = null;
            await _localStorageService.RemoveItem("token");
            _authProvider.SignOut();
        }

        #endregion

        #region --> Eventi

        private async void _authProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
        {
            if (task.Result == null) return;
            if (task.Result.User == null) return;
            if (task.Result.User.Identity == null) return;
            if (!task.Result.User.Identity.IsAuthenticated) return;

            try
            {
                //--> Leggo l'utente
                var claims = task.Result.User.Claims.ToList();
                var userid = (from x in claims where x.Type == "UserId" select x.Value).FirstOrDefault().ToReal();
                User = await authClient.UserAsync(userid);
            }
            catch (Exception ex)
            { }
        }

        #endregion

    }
}