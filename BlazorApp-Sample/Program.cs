using BlazorApp_Sample.Helpers;
using BlazorApp_Sample.Services;
using BlazorApp_Sample;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiUrl = new Uri(builder.Configuration["apiUrl"].ToReal());
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = apiUrl });
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>()
                .AddScoped<ILocalStorageService, LocalStorageService>()
                .AddScoped<AppAuthenticationStateProvider>()
                .AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<AppAuthenticationStateProvider>());

await builder.Build().RunAsync();
