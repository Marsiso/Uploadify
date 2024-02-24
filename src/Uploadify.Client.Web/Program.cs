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
using MudBlazor.Services;
using Polly;
using Polly.Extensions.Http;
using Uploadify.Authorization.Extensions;
using Uploadify.Client.Application.Application.Services;
using Uploadify.Client.Application.Authentication.Services;
using Uploadify.Client.Application.Authorization.Services;
using Uploadify.Client.Application.FileSystem.Services;
using Uploadify.Client.Application.Utilities.Services;
using Uploadify.Client.Core.Caching.Services;
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
    .AddSingleton<MobileViewManager>()
    .AddTransient<AuthorizedHandler>()
    .AddPermissions()
    .AddLocalization()
    .AddTransient<IStringLocalizer>(serviceProvider => serviceProvider.GetRequiredService<IStringLocalizer<TranslationDictionary>>())
    .AddBlazoredLocalStorageAsSingleton()
    .AddTransient<RoleService>()
    .AddTransient<PermissionService>()
    .AddTransient<FolderService>()
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
        .OrResult(response => response.StatusCode == HttpStatusCode.InternalServerError)
        .WaitAndRetryAsync(6, retry => TimeSpan.FromSeconds(Math.Pow(2, retry)));
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(response => response.StatusCode == HttpStatusCode.InternalServerError)
        .AdvancedCircuitBreakerAsync(0.25, TimeSpan.FromSeconds(60), 7, TimeSpan.FromSeconds(30));
}
