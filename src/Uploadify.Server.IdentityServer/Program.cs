using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;
using OpenIddict.Abstractions;
using Quartz;
using Uploadify.Server.Application.Application.Commands;
using Uploadify.Server.Application.Authentication.Validators;
using Uploadify.Server.Application.Infrastructure.Requests.Services;
using Uploadify.Server.Application.Infrastructure.Seed.Services;
using Uploadify.Server.Application.Security.Services;
using Uploadify.Server.Core.Application.Commands;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Data.Infrastructure.EF.Services;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Infrastructure.Authorization.Constants;
using Uploadify.Server.Domain.Infrastructure.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

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

builder.Services.AddIdentity<User, Role>(options =>
    {
        options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
        options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
        options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
        options.ClaimsIdentity.EmailClaimType = OpenIddictConstants.Claims.Email;
        options.ClaimsIdentity.SecurityStampClaimType = Claims.SecurityStamp;

        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedPhoneNumber = false;
        options.SignIn.RequireConfirmedEmail = false;

        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 10;
        options.Password.RequiredUniqueChars = 1;

        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(1));

builder.Services.AddOptions<ArgonPasswordHasherOptions>()
    .Configure(options => options.Pepper = "482A7A9331DD7693FFBCF2C3CD0CAF9D101736B7708563943594F7F08CE062A3CBA0D084ABF3BAA9FB6754F0871034C121C7959DBBB67D488AF6F71FFB9C046A")
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "X-IDENTITY-PROVIDER-COOKIE";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.MaxAge = TimeSpan.FromHours(1);
});

builder.Services.ConfigureExternalCookie(options =>
{
    options.Cookie.Name = "X-IDENTITY-PROVIDER-EXTERNAL-COOKIE";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.MaxAge = TimeSpan.FromHours(1);
});

builder.Services.AddSingleton<IPasswordHasher<User>, ArgonPasswordHasher<User>>();

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "X-IDENTITY-PROVIDER-XSRF";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.MaxAge = TimeSpan.FromHours(1);
});

builder.Services.AddValidatorsFromAssembly(typeof(CreateUserCommandValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(SignInPreProcessorCommand).Assembly);
builder.Services.AddHttpContextAccessor();
builder.Services.AddMediatR(options =>
{
    options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthenticationPipelineBehaviour<,>));
    options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>));
    options.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly);
    options.RegisterServicesFromAssembly(typeof(SignInPreProcessorCommand).Assembly);
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
    .AddServer(options =>
    {
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

builder.Services.AddHostedService<Worker>();

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
