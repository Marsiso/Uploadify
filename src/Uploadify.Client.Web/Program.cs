using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MudBlazor.Services;
using Polly;
using Polly.Extensions.Http;
using Uploadify.Client.Application.Authentication.Services;
using Uploadify.Client.Application.Authorization.Services;
using Uploadify.Client.Core.Infrastructure.HttpClients;
using Uploadify.Client.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSingleton(builder.HostEnvironment);

builder.Services.AddMudServices();

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.TryAddSingleton<AuthenticationStateProvider, HostAuthenticationStateProvider>();
builder.Services.TryAddSingleton(provider => (HostAuthenticationStateProvider) provider.GetRequiredService<AuthenticationStateProvider>());
builder.Services.AddTransient<AuthorizedHandler>();

builder.RootComponents.Add<App>("#app");

builder.Services.AddHttpClient("default", client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
}).AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy())
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

builder.Services.AddHttpClient("authorizedClient", client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
}).AddHttpMessageHandler<AuthorizedHandler>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy())
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

builder.Services.AddOptions<ApiCallWrapperOptions>().Configure(options => options.IsDevelopment = builder.HostEnvironment.IsDevelopment());
builder.Services.AddSingleton(serviceProvider =>
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

builder.Services.AddTransient(provider => provider.GetRequiredService<IHttpClientFactory>().CreateClient("default"));

var host = builder.Build();

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
