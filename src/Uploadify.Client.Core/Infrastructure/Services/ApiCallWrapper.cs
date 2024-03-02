using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Uploadify.Client.Integration.Resources;

namespace Uploadify.Client.Core.Infrastructure.Services;

public class ApiCallWrapper
{
    public readonly HttpClient HttpClient;
    public readonly ApiCallWrapperOptions Options;
    public readonly ILogger<ApiCallWrapper> Logger;

    public ApiCallWrapper(HttpClient httpClient, IOptions<ApiCallWrapperOptions> options, ILogger<ApiCallWrapper> logger)
    {
        HttpClient = httpClient;
        Options = options.Value;
        Logger = logger;
    }

    public async Task<TResponse?> Call<TResponse>(Func<UploadifyClient, Task<ApiCallResponse<TResponse>>> client) where TResponse : BaseResponse
    {
        try
        {
            return (await client.Invoke(new(HttpClient))).Result;
        }
        catch (ApiCallException<TResponse> exception)
        {
            LogInformation(nameof(ApiCallWrapper), exception);
            return exception.Result;
        }
        catch (ApiCallException exception)
        {
            LogInformation(nameof(ApiCallWrapper), exception);

            var response = Activator.CreateInstance<TResponse>();

            response.Status = (Status)exception.StatusCode;
            return response;
        }
        catch (Exception exception)
        {
            LogError(nameof(ApiCallWrapper), exception);

            var response = Activator.CreateInstance<TResponse>();

            response.Status = Status.InternalServerError;
            return response;
        }
    }

    public async Task<TResponse?> Call<TResponse>(Func<UploadifyClient, Task<TResponse>> client) where TResponse : class
    {
        try
        {
            return await client.Invoke(new(HttpClient));
        }
        catch (Exception exception)
        {
            LogError(nameof(ApiCallWrapper), exception);
            return null;
        }
    }

    public void LogInformation(string? service, Exception exception, [CallerMemberName] string? memberName = null)
    {
        if (Options.IsDevelopment)
        {
            Logger.LogInformation($"Service: '{service}' Action: '{memberName}' Message: '{exception.Message}' Inner exception: '{exception.InnerException}'.");
        }
    }

    public void LogError(string? service, Exception exception, [CallerMemberName] string? memberName = null)
    {
        if (Options.IsDevelopment)
        {
            Logger.LogError($"Service: '{service}' Action: '{memberName}' Message: '{exception.Message}' Inner exception: '{exception.InnerException}'.");
            return;
        }

        Logger.LogError($"Service: '{service}' Action: '{memberName}' Message: 'We apologize for the inconvenience, but it seems we're experiencing some technical difficulties.'.");
    }
}
