using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Uploadify.Client.Integration.Resources;

namespace Uploadify.Client.Core.Infrastructure.Services;

public class ApiCallWrapper
{
    private readonly HttpClient _httpClient;
    private readonly ApiCallWrapperOptions _options;
    public readonly ILogger<ApiCallWrapper> _logger;

    public ApiCallWrapper(HttpClient httpClient, IOptions<ApiCallWrapperOptions> options, ILogger<ApiCallWrapper> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<TResponse?> Call<TResponse>(Func<UploadifyClient, Task<ApiCallResponse<TResponse>>> client) where TResponse : BaseResponse
    {
        try
        {
            return (await client.Invoke(new UploadifyClient(_httpClient))).Result;
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

    public void LogInformation(string? service, Exception exception, [CallerMemberName] string? memberName = null)
    {
        if (_options.IsDevelopment)
        {
            _logger.LogInformation($"Service: '{service}' Action: '{memberName}' Message: '{exception.Message}' Inner exception: '{exception.InnerException}'.");
        }
    }

    public void LogError(string? service, Exception exception, [CallerMemberName] string? memberName = null)
    {
        if (_options.IsDevelopment)
        {
            _logger.LogError($"Service: '{service}' Action: '{memberName}' Message: '{exception.Message}' Inner exception: '{exception.InnerException}'.");
            return;
        }

        _logger.LogError($"Service: '{service}' Action: '{memberName}' Message: 'We apologize for the inconvenience, but it seems we're experiencing some technical difficulties.'.");
    }
}
