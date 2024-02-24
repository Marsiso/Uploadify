using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Uploadify.Client.Application.Resources.Services;
using Uploadify.Client.Core.Infrastructure.Services;
using Uploadify.Client.Domain.Localization.Constants;
using Uploadify.Client.Domain.Resources.Models;
using Uploadify.Client.Integration.Resources;

namespace Uploadify.Client.Application.Application.Services;

public class RoleService : BaseResourceService<RoleService>
{
    public RoleService(ApiCallWrapper apiCallWrapper, IStringLocalizer localizer, ILogger<RoleService> logger) : base(apiCallWrapper, localizer, logger)
    {
    }

    public async Task<ResourceResponse<RolesSummary>> GetSummary(int pageNumber = 1, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await ApiCallWrapper.Call(client => client.ApiRolesSummaryAsync(pageNumber, 50, cancellationToken));
            if (response is not { Status: Status.Ok, Summary: not null })
            {
                return new(GetErrorMessages(response?.Failure));
            }

            return new(response.Summary);
        }
        catch (Exception exception)
        {
            LogError(exception);
        }

        return new([Localizer[Translations.RequestStatuses.InternalServerError]]);
    }
}
