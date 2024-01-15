using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;
using NSwag;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using Uploadify.Server.Application.Application.Commands;
using Uploadify.Server.Application.Authentication.Validators;
using Uploadify.Server.Application.Infrastructure.Requests.Services;
using Uploadify.Server.Application.Security.Services;
using Uploadify.Server.Core.Application.Commands;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Data.Infrastructure.EF.Services;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Authorization.Constants;
using Uploadify.Server.Domain.Infrastructure.Services.Models;

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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(options =>
{
    options.PostProcess = document =>
    {
        document.Info = new OpenApiInfo
        {
            Version = "v1",
            Title = "Uploadify Resource Server",
            Description = "An ASP.NET Core RESTful API.",
            Contact = new OpenApiContact { Name = "LinkedIn", Url = "https://www.linkedin.com/in/marek-ol%C5%A1%C3%A1k-1715b724a/" },
            License = new OpenApiLicense { Name = "MIT", Url = "https://en.wikipedia.org/wiki/MIT_License" }
        };
    };
});

builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer(settings.IdentityProvider.Issuer);
        options.AddAudiences(settings.Api.ID);

        options.UseIntrospection()
            .SetClientId(settings.Api.ID)
            .SetClientSecret(settings.Api.Secret);

        options.UseSystemNetHttp();
        options.UseAspNetCore();
    });

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();

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

var application = builder.Build();

if (application.Environment.IsDevelopment())
{
    application.UseOpenApi();
    application.UseSwaggerUi(options =>
    {
        options.DocumentTitle = "Uploadify Resource Server";
    });

    application.UseReDoc(options => options.Path = "/redoc");
}

application.UseCors(options =>
{
    options.AllowAnyHeader();
    options.AllowAnyOrigin();
    options.AllowAnyMethod();
});

application.UseHttpsRedirection();
application.UseStaticFiles();
application.UseRouting();
application.UseAuthentication();
application.UseAuthorization();

application.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

application.Run();
