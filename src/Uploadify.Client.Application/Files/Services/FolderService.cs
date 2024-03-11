using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Uploadify.Client.Application.Resources.Services;
using Uploadify.Client.Core.Infrastructure.Services;
using Uploadify.Client.Domain.Resources.Models;
using Uploadify.Client.Integration.Resources;

namespace Uploadify.Client.Application.Files.Services;

public class FolderService : BaseResourceService<FolderService>
{
    public FolderService(ApiCallWrapper apiCallWrapper, IStringLocalizer localizer, ILogger<FolderService> logger) : base(apiCallWrapper, localizer, logger)
    {
    }

    public async Task<ResourceResponse<FolderSummary>> GetSummary(int? folderId, CancellationToken cancellationToken = default)
    {
        var response = folderId.HasValue
            ? await ApiCallWrapper.Call(client => client.ApiFolderSummaryGetAsync(folderId.Value, cancellationToken))
            : await ApiCallWrapper.Call(client => client.ApiFolderSummaryGetAsync(null, cancellationToken));

        return response?.Status switch
        {
            Status.Ok => new(response.Summary),
            _ => new(HandleServerErrorMessages(response?.Failure))
        };
    }

    public async Task<ResourceResponse<FolderDetail>> GetDetail(int folderId, string? name, CancellationToken cancellationToken = default)
    {
        var response = await ApiCallWrapper.Call(client => client.ApiFolderDetailAsync(folderId, cancellationToken));
        return response?.Status switch
        {
            Status.Ok => new(response.Folder),
            _ => new(HandleServerErrorMessages(response?.Failure))
        };
    }

    public async Task<ResourceResponse<FolderOverview>> Rename(FolderOverview overview, string? name, CancellationToken cancellationToken = default)
    {
        var response = await ApiCallWrapper.Call(client => client.ApiFolderRenameAsync(new RenameFolderCommand { FolderId = overview.FolderId, Name = name }, cancellationToken));
        return response?.Status switch
        {
            Status.Ok => new(response.Folder),
            _ => new(HandleServerErrorMessages(response?.Failure))
        };
    }

    public async Task<ResourceResponse<FolderOverview>> Delete(FolderOverview overview, CancellationToken cancellationToken = default)
    {
        var response = await ApiCallWrapper.Call(client => client.ApiFolderDeleteAsync(overview.FolderId.Value, cancellationToken));
        return response?.Status switch
        {
            Status.Ok => new(response.Folder),
            _ => new(HandleServerErrorMessages(response?.Failure))
        };
    }

    public async Task<ResourceResponse<FolderOverview>> Move(int? folderId, int? destinationFolderId, CancellationToken cancellationToken = default)
    {
        var response = await ApiCallWrapper.Call(client => client.ApiFolderMoveAsync(new MoveFolderCommand { FolderId = folderId, DestinationFolderId = destinationFolderId }, cancellationToken));
        return response?.Status switch
        {
            Status.Ok => new(response.Folder),
            _ => new(HandleServerErrorMessages(response?.Failure))
        };
    }

    public async Task<ResourceResponse<FolderOverview>> Create(FolderOverview parent, string? name, CancellationToken cancellationToken = default)
    {
        var response = await ApiCallWrapper.Call(client => client.ApiFolderPostAsync(new CreateFolderCommand { Name = name, ParentId = parent.FolderId }, cancellationToken));
        return response?.Status switch
        {
            Status.Created => new(response.Folder),
            _ => new(HandleServerErrorMessages(response?.Failure))
        };
    }
}
