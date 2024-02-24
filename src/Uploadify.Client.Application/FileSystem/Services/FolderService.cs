using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Uploadify.Client.Application.Resources.Services;
using Uploadify.Client.Core.Infrastructure.Services;
using Uploadify.Client.Domain.Resources.Models;
using Uploadify.Client.Integration.Resources;

namespace Uploadify.Client.Application.FileSystem.Services;

public class FolderService : BaseResourceService<FolderService>
{
    public FolderService(ApiCallWrapper apiCallWrapper, IStringLocalizer localizer, ILogger<FolderService> logger) : base(apiCallWrapper, localizer, logger)
    {
    }

    public async Task<ResourceResponse<FolderSummary>> GetSummary(CancellationToken cancellationToken = default)
    {
        var response = await ApiCallWrapper.Call(client => client.ApiFoldersSummaryAsync(cancellationToken));
        return response?.Status switch
        {
            Status.Ok => new(response.Summary),
            _ => new(GetErrorMessages(response?.Failure))
        };
    }
}
