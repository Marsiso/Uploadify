using System.Net.Http.Headers;
using System.Security.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using OpenIddict.Client.AspNetCore;
using Quartz;
using Uploadify.Server.Application.Infrastructure.Extensions;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Authorization.Constants;
using Uploadify.Server.Domain.Infrastructure.Services;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

var settings = builder.Configuration.GetSection(SystemSettings.SectionName).Get<SystemSettings>() ?? throw new InvalidOperationException();
var services = builder.Services;
var environment = builder.Environment;

services.AddSingleton(settings);
services.AddSingleton(builder.Configuration);
services.AddSingleton(environment);

services.AddDatabase(environment.IsDevelopment(), settings)
    .AddHttpClient()
    .AddAntiforgery(options =>
    {
        options.HeaderName = "X-XSRF-TOKEN";
        options.Cookie.Name = "__Host-X-XSRF-TOKEN";
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    })
    .AddAuthentication(options => options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name  = "X-COOKIE";
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(50);
        options.SlidingExpiration = false;
    });

services.AddQuartz(options =>
{
    options.UseMicrosoftDependencyInjectionJobFactory();
    options.UseSimpleTypeLoader();
    options.UseInMemoryStore();
}).AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

services.AddOpenIddict()
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

services.AddControllersWithViews();
services.AddRazorPages();

services.AddAuthorizationBuilder()
    .AddPolicy("CookieAuthenticationPolicy", options =>
    {
        options.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
        options.RequireAuthenticatedUser();
    });

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
    },
    new RouteConfig
    {
        RouteId = "route2",
        ClusterId = "cluster1",
        Match = new RouteMatch
        {
            Path = "api/role/{**catch-all}"
        }
    },
    new RouteConfig
    {
        RouteId = "route3",
        ClusterId = "cluster1",
        Match = new RouteMatch
        {
            Path = "api/roles/{**catch-all}"
        }
    },
    new RouteConfig
    {
        RouteId = "route4",
        ClusterId = "cluster1",
        Match = new RouteMatch
        {
            Path = "api/permission/{**catch-all}"
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

services.AddReverseProxy()
    .LoadFromMemory(routes, clusters)
    .AddTransforms(options => options.AddRequestTransform(async context =>
    {
        var token = await context.HttpContext.GetTokenAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme, tokenName: OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken);
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
