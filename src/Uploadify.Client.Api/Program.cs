using System.Net.Http.Headers;
using System.Security.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using OpenIddict.Client.AspNetCore;
using Quartz;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Data.Infrastructure.EF.Services;
using Uploadify.Server.Domain.Authorization.Constants;
using Uploadify.Server.Domain.Infrastructure.Services.Models;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

var settings = builder.Configuration.GetSection(SystemSettings.SectionName).Get<SystemSettings>() ?? throw new InvalidOperationException();

builder.Services.AddSingleton(settings);
builder.Services.AddSingleton(builder.Configuration);
builder.Services.AddSingleton(builder.Environment);

builder.Services.AddTransient<ISaveChangesInterceptor, AuditInterceptor>();
builder.Services.AddNpgsql<DataContext>(new NpgsqlConnectionStringBuilder
{
    Username = settings.Database.Username,
    Password = settings.Database.Password,
    Host = settings.Database.Host,
    Database = settings.Database.Database,
    Pooling = settings.Database.Pooling,
    Port = settings.Database.Port
}.ConnectionString, options =>
{
    options.EnableRetryOnFailure(5);
    options.MigrationsAssembly("Uploadify.Server.Data");
}, options =>
{
    options.EnableDetailedErrors(builder.Environment.IsDevelopment());
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
});

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
    options.Cookie.Name = "__Host-X-XSRF-TOKEN";
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddAuthentication(options => options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name  = "X-COOKIE";
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(50);
        options.SlidingExpiration = false;
    });

builder.Services.AddQuartz(options =>
{
    options.UseMicrosoftDependencyInjectionJobFactory();
    options.UseSimpleTypeLoader();
    options.UseInMemoryStore();
});

builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
            .UseDbContext<DataContext>();

        options.UseQuartz();
    })
    .AddClient(options =>
    {
        options.AllowAuthorizationCodeFlow();

        options.AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
            .EnableStatusCodePagesIntegration()
            .EnableRedirectionEndpointPassthrough()
            .EnablePostLogoutRedirectionEndpointPassthrough();

        options.UseSystemNetHttp()
            .SetProductInformation(typeof(Program).Assembly);

        options.AddRegistration(new OpenIddictClientRegistration
        {
            Issuer = new Uri(settings.IdentityProvider.Issuer, UriKind.Absolute),

            ClientId = settings.Client.ID,
            ClientSecret = settings.Client.Secret,
            Scopes =
            {
                Scopes.Name,
                Scopes.Email,
                Scopes.Phone,
                Scopes.Roles,
                Scopes.Api
            },

            RedirectUri = new Uri("callback/login/local", UriKind.Relative),
            PostLogoutRedirectUri = new Uri("callback/logout/local", UriKind.Relative)
        });
    });

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddAuthorization(options => options.AddPolicy("CookieAuthenticationPolicy", builder =>
{
    builder.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
    builder.RequireAuthenticatedUser();
}));

builder.Services.AddHttpClient();

var routes = new[]
{
    new RouteConfig
    {
        RouteId = "route1",
        ClusterId = "cluster1",
        Match = new RouteMatch
        {
            Path = "api/user/{**catch-all}"
        }
    }
};

var clusters = new[]
{
    new ClusterConfig()
    {
        ClusterId = "cluster1",
        Destinations = new Dictionary<string, DestinationConfig>
        {
            { "destination1", new DestinationConfig { Address = settings.ReverseProxy.Uri } }
        },
        HttpClient = new HttpClientConfig { MaxConnectionsPerServer = 10, SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13 }
    }
};

builder.Services.AddReverseProxy()
    .LoadFromMemory(routes, clusters)
    .AddTransforms(builder => builder.AddRequestTransform(async context =>
    {
        var token = await context.HttpContext.GetTokenAsync(
            scheme: CookieAuthenticationDefaults.AuthenticationScheme,
            tokenName: OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken);

        context.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue(OpenIddictConstants.Schemes.Bearer, token);
    }));

var application = builder.Build();

if (application.Environment.IsDevelopment())
{
    application.UseDeveloperExceptionPage();
    application.UseWebAssemblyDebugging();
}

application.UseCors(options =>
{
    options.AllowAnyHeader();
    options.AllowAnyOrigin();
    options.AllowAnyMethod();
});

application.UseHttpsRedirection();
application.UseBlazorFrameworkFiles();
application.UseStaticFiles();

application.UseRouting();
application.UseAuthentication();
application.UseAuthorization();

application.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapControllers();
    endpoints.MapReverseProxy();
    endpoints.MapFallbackToPage("/_Host");
});

application.Run();
