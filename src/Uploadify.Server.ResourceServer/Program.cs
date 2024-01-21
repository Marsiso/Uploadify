using NSwag;
using OpenIddict.Validation.AspNetCore;
using Uploadify.Authorization.Extensions;
using Uploadify.Server.Application.Infrastructure.Extensions;
using Uploadify.Server.Domain.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

var settings = builder.Configuration.GetSection(SystemSettings.SectionName).Get<SystemSettings>() ?? throw new InvalidOperationException();
var services = builder.Services;
var environment = builder.Environment;

services.AddSingleton(settings);
services.AddSingleton(builder.Configuration);
services.AddSingleton(environment);

services.AddDatabase(environment.IsDevelopment(), settings)
    .AddIdentity(settings)
    .AddValidations(settings)
    .AddRequests(settings)
    .AddControllers();

services.AddEndpointsApiExplorer();
services.AddOpenApiDocument(options =>
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

services.AddOpenIddict()
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

services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
services.AddPermissions();

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
