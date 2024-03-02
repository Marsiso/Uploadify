using System.Globalization;
using System.Net;
using System.Net.Mime;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Services;
using Polly;
using Polly.Extensions.Http;
using Uploadify.Authorization.Extensions;
using Uploadify.Client.Application.Application.Services;
using Uploadify.Client.Application.Auth.Services;
using Uploadify.Client.Application.Files.Services;
using Uploadify.Client.Application.Utilities.Services;
using Uploadify.Client.Core.Infrastructure.Caching.Services;
using Uploadify.Client.Core.Infrastructure.Services;
using Uploadify.Client.Domain.Localization.Constants;
using Uploadify.Client.Web;
using Uploadify.Client.Web.Resources;
using static System.String;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var services = builder.Services;

services.AddSingleton(builder.HostEnvironment)
    .AddOptions()
    .AddMudServices()
    .AddMudBlazorDialog()
    .AddMudBlazorSnackbar(options =>
    {
        options.MaxDisplayedSnackbars = 3;
        options.NewestOnTop = true;
        options.ShowCloseIcon = false;
        options.SnackbarVariant = Variant.Text;
        options.PositionClass = Defaults.Classes.Position.TopCenter;
    })
    .AddSingleton<MobileViewManager>()
    .AddTransient<AuthorizedHandler>()
    .AddPermissions()
    .AddLocalization()
    .AddTransient<IStringLocalizer>(serviceProvider => serviceProvider.GetRequiredService<IStringLocalizer<TranslationDictionary>>())
    .AddBlazoredLocalStorageAsSingleton()
    .AddTransient<RoleService>()
    .AddTransient<PermissionService>()
    .AddTransient<FolderService>()
    .AddSingleton<FileService>()
    .AddSingleton<CacheService>();

services.TryAddSingleton<AuthenticationStateProvider, HostAuthenticationStateProvider>();
services.TryAddSingleton(provider => (HostAuthenticationStateProvider) provider.GetRequiredService<AuthenticationStateProvider>());

builder.RootComponents.Add<App>("#app");

services.AddHttpClient("default", client =>
{
    client.BaseAddress = new(builder.HostEnvironment.BaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new(MediaTypeNames.Application.Json));
}).AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy())
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

services.AddHttpClient("authorizedClient", client =>
{
    client.BaseAddress = new(builder.HostEnvironment.BaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new(MediaTypeNames.Application.Json));
}).AddHttpMessageHandler<AuthorizedHandler>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy())
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

services.AddOptions<ApiCallWrapperOptions>().Configure(options => options.IsDevelopment = builder.HostEnvironment.IsDevelopment());
services.AddSingleton(serviceProvider =>
{
    var javascriptRuntime = serviceProvider.GetRequiredService<IJSRuntime>();
    var token = ((IJSInProcessRuntime)javascriptRuntime).Invoke<string>("getAntiForgeryToken");

    var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("authorizedClient");

    httpClient.DefaultRequestHeaders.Add("X-XSRF-TOKEN", token);

    return new ApiCallWrapper(
        httpClient,
        serviceProvider.GetRequiredService<IOptions<ApiCallWrapperOptions>>(),
        serviceProvider.GetRequiredService<ILogger<ApiCallWrapper>>());
});

services.AddTransient(provider => provider.GetRequiredService<IHttpClientFactory>().CreateClient("default"));

var host = builder.Build();

CultureInfo culture;
var javascriptRuntime = host.Services.GetRequiredService<IJSRuntime>();
var cultureString = await javascriptRuntime.InvokeAsync<string>("culture.get");

if (IsNullOrWhiteSpace(cultureString))
{
    culture = new(Locales.Default);
    await javascriptRuntime.InvokeVoidAsync("culture.set", Locales.Default);
}
else
{
    culture = new(cultureString);
}

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

await host.RunAsync();
return;

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(response => response.StatusCode == HttpStatusCode.ServiceUnavailable)
        .OrResult(response => response.StatusCode == HttpStatusCode.RequestTimeout)
        .WaitAndRetryAsync(6, retry => TimeSpan.FromSeconds(Math.Pow(2, retry)));
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(response => response.StatusCode == HttpStatusCode.ServiceUnavailable)
        .OrResult(response => response.StatusCode == HttpStatusCode.RequestTimeout)
        .AdvancedCircuitBreakerAsync(0.25, TimeSpan.FromSeconds(60), 7, TimeSpan.FromSeconds(30));
}
