using Quartz;
using Uploadify.Server.Application.Infrastructure.Extensions;
using Uploadify.Server.Application.Infrastructure.Services;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Infrastructure.Models;

var builder = WebApplication.CreateBuilder(args);

var settings = builder.Configuration.GetSection(SystemSettings.SectionName).Get<SystemSettings>() ?? throw new InvalidOperationException();
var services = builder.Services;
var environment = builder.Environment;

services.AddSingleton(settings);
services.AddSingleton(builder.Configuration);
services.AddSingleton(environment);

services.AddDatabase(environment.IsDevelopment(), settings)
    .AddIdentity(settings)
    .AddValidators(settings)
    .AddRequests(settings)
    .AddControllersWithViews();

services.AddAntiforgery(options =>
{
    options.Cookie.Name = "X-IDP-XSRF-TOKEN";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.MaxAge = TimeSpan.FromHours(1);
}).ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "X-IDP-COOKIE";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.MaxAge = TimeSpan.FromHours(1);
}).ConfigureExternalCookie(options =>
{
    options.Cookie.Name = "X-IDP-EXTERNAL-COOKIE";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.MaxAge = TimeSpan.FromHours(1);
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
    .AddServer(options =>
    {
        options.SetIssuer(settings.IdentityProvider.Issuer);

        options.SetAuthorizationEndpointUris("connect/authorize")
            .SetLogoutEndpointUris("connect/logout")
            .SetIntrospectionEndpointUris("connect/introspect")
            .SetTokenEndpointUris("connect/token")
            .SetUserinfoEndpointUris("connect/userinfo")
            .SetVerificationEndpointUris("connect/verify");

        options.AllowAuthorizationCodeFlow();

        options.AddDevelopmentEncryptionCertificate();
        options.AddDevelopmentSigningCertificate();

        options.RegisterScopes(settings.IdentityProvider.SupportedScopes);
        options.RegisterClaims(settings.IdentityProvider.SupportedScopes);

        options.UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough()
            .EnableLogoutEndpointPassthrough()
            .EnableTokenEndpointPassthrough()
            .EnableUserinfoEndpointPassthrough()
            .EnableStatusCodePagesIntegration();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

services.AddHostedService<Worker>();

var application = builder.Build();

application.UseCors(options =>
{
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.AllowAnyOrigin();
});

if (!application.Environment.IsDevelopment())
{
    application.UseExceptionHandler("/Home/Error");
    application.UseHsts();
}

application.UseHttpsRedirection();
application.UseStaticFiles();
application.UseRouting();
application.UseAuthentication();
application.UseAuthorization();

application.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

application.Run();
