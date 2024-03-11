using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenIddict.Abstractions;
using Uploadify.Server.Application.Auth.Validators;
using Uploadify.Server.Application.Files.Queries;
using Uploadify.Server.Application.Infrastructure.Requests.Services;
using Uploadify.Server.Application.Security.Services;
using Uploadify.Server.Core.Files.Queries;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Data.Infrastructure.EF.Services;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Auth.Constants;
using Uploadify.Server.Domain.Infrastructure.Models;

namespace Uploadify.Server.Application.Infrastructure.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, bool isDevelopment, SystemSettings settings)
    {
        services.AddTransient<ISaveChangesInterceptor, AuditInterceptor>();
        services.AddNpgsql<DataContext>(new NpgsqlConnectionStringBuilder
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
            options.EnableDetailedErrors(isDevelopment);
            options.EnableSensitiveDataLogging(isDevelopment);
        });

        return services;
    }

    public static IServiceCollection AddIdentity(this IServiceCollection services, SystemSettings settings)
    {
        services.AddIdentity<User, Role>(options =>
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

        services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(1));

        services.AddSingleton<IPasswordHasher<User>, ArgonPasswordHasher<User>>();
        services.AddOptions<ArgonPasswordHasherOptions>()
            .Configure(options => options.Pepper = "482A7A9331DD7693FFBCF2C3CD0CAF9D101736B7708563943594F7F08CE062A3CBA0D084ABF3BAA9FB6754F0871034C121C7959DBBB67D488AF6F71FFB9C046A")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services, SystemSettings settings)
    {
        services.AddValidatorsFromAssembly(typeof(CreateUserCommandValidator).Assembly);
        return services;
    }

    public static IServiceCollection AddRequests(this IServiceCollection services, SystemSettings settings)
    {
        services.AddHttpContextAccessor();
        services.AddMediatR(options =>
        {
            options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationPipelineBehaviour<,>));
            options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>));
            options.RegisterServicesFromAssembly(typeof(GetFileQuery).Assembly);
            options.RegisterServicesFromAssembly(typeof(GetFileDetailQuery).Assembly);
        });

        return services;
    }
}
