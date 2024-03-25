using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Uploadify.Client.Application.Resources.Services;
using Uploadify.Client.Core.Infrastructure.Services;
using Uploadify.Client.Domain.Resources.Models;
using Uploadify.Client.Integration.Resources;

namespace Uploadify.Client.Application.Files.Services;

public class FileService : BaseResourceService<FileService>
{
    protected readonly IJSRuntime JavaScriptRuntime;

    public FileService(ApiCallWrapper apiCallWrapper, IStringLocalizer localizer, ILogger<FileService> logger, IJSRuntime javaScriptRuntime) : base(apiCallWrapper, localizer, logger)
    {
        JavaScriptRuntime = javaScriptRuntime;
    }

    public async Task<ResourceResponse<FileOverview>> Rename(FileOverview file, string? name, CancellationToken cancellationToken = default)
    {
        var response = await ApiCallWrapper.Call(client => client.ApiFileRenameAsync(new () { FileId = file.FileId, Name = name }, cancellationToken));
        return response?.Status switch
        {
            Status.Ok => new(response.File),
            _ => new(HandleServerErrorMessages(response?.Failure))
        };
    }

    public async Task<ResourceResponse<FileOverview>> Delete(FileOverview file, CancellationToken cancellationToken = default)
    {
        var response = await ApiCallWrapper.Call(client => client.ApiFileDeleteAsync(file.FileId.Value, cancellationToken));
        return response?.Status switch
        {
            Status.Ok => new(response.File),
            _ => new(HandleServerErrorMessages(response?.Failure))
        };
    }

    public async Task<ResourceResponse<FileOverview>> Move(int? fileID, int? destinationFolderId, CancellationToken cancellationToken = default)
    {
        var response = await ApiCallWrapper.Call(client => client.ApiFileMoveAsync(new() { FileId = fileID, DestinationFolderId = destinationFolderId }, cancellationToken));
        return response?.Status switch
        {
            Status.Ok => new(response.File),
            _ => new(HandleServerErrorMessages(response?.Failure))
        };
    }

    public async Task<ResourceResponse<FileOverview>> Upload(FolderOverview folder, IBrowserFile file, CancellationToken cancellationToken = default)
    {
        var response = await ApiCallWrapper.Call(client => client.ApiFilePostAsync(
            folderId: folder.FolderId,
            file: new FileParameter(file.OpenReadStream(2097152000L, cancellationToken), file.Name, file.ContentType),
            cancellationToken: cancellationToken));

        return response?.Status switch
        {
            Status.Created => new(response.File),
            _ => new(HandleServerErrorMessages(response?.Failure))
        };
    }

    public async Task<bool> Download(int fileID, string filename, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await ApiCallWrapper.Call(client => client.ApiFileGetAsync(fileID, cancellationToken));
            if (response == null)
            {
                return false;
            }

            using var stream = new DotNetStreamReference(response.Stream);
            await JavaScriptRuntime.InvokeVoidAsync("downloadFileFromStream", cancellationToken, filename, stream);
            return true;
        }
        catch (Exception exception)
        {
            LogError(exception);
            return false;
        }
    }

    public async Task<ResourceResponse<PublicFilesSummary>> GetPublicFiles(int pageNumber, int pageSize, string? searchTerm, CancellationToken cancellationToken = default)
    {
        var response = await ApiCallWrapper.Call(client => client.ApiFilesPublicAsync(pageNumber:pageNumber, pageSize: pageSize, searchTerm: searchTerm ,cancellationToken: cancellationToken));
        return response?.Status switch
        {
            Status.Ok => new(response.Summary),
            _ => new(HandleServerErrorMessages(response?.Failure))
        };
    }

    public async Task<ResourceResponse> ChangeVisibility(int fileID, bool isPublic, CancellationToken cancellationToken = default)
    {
        var response = await ApiCallWrapper.Call(client => client.ApiFileChangeVisibilityAsync(new ChangeFileVisibilityCommand { FileId = fileID, Visibility = isPublic },cancellationToken: cancellationToken));
        return response?.Status switch
        {
            Status.Ok => new(),
            _ => new(HandleServerErrorMessages(response?.Failure))
        };
    }
}
