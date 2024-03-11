using System.Runtime.CompilerServices;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Uploadify.Client.Core.Infrastructure.Services;
using Uploadify.Client.Core.Infrastructure.Translations.Helpers;
using Uploadify.Client.Integration.Resources;
using static System.String;

namespace Uploadify.Client.Application.Resources.Services;

public abstract class BaseResourceService<TService> where TService : class
{
    protected readonly ApiCallWrapper ApiCallWrapper;
    protected readonly IStringLocalizer Localizer;
    protected readonly ILogger<TService> Logger;

    protected BaseResourceService(ApiCallWrapper apiCallWrapper, IStringLocalizer localizer, ILogger<TService> logger)
    {
        ApiCallWrapper = apiCallWrapper;
        Localizer = localizer;
        Logger = logger;
    }

    protected void LogInformation(string message, [CallerMemberName] string? actionName = null) =>
        Logger.LogInformation(Format("Service: '{0}' Action: '{1}' Message: '{2}'.", typeof(TService).Name, actionName, message));

    protected void LogError(string message, [CallerMemberName] string? actionName = null) =>
        Logger.LogError(Format("Service: '{0}' Action: '{1}' Message: '{2}'.", typeof(TService).Name, actionName, message));

    protected void LogError(Exception exception, [CallerMemberName] string? actionName = null) =>
        Logger.LogError(Format("Service: '{0}' Action: '{1}' Message: '{2}'.", typeof(TService).Name, actionName, exception.Message));

    protected string[] HandleServerErrorMessages(RequestFailure? requestFailure)
    {
        if (!IsNullOrWhiteSpace(requestFailure?.UserFriendlyMessage))
        {
            LogInformation(requestFailure.UserFriendlyMessage);
        }

        if (requestFailure is not { Errors.Count: > 0 })
        {
            return Array.Empty<string>();
        }

        return requestFailure.Errors.SelectMany(kvp => kvp.Value, (_, v) => TranslationHelpers.GetTranslation(v, Localizer)).ToArray();
    }
}
