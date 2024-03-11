using System.Text.Json.Serialization;
using CommunityToolkit.Diagnostics;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Application.Files.Helpers;
using Uploadify.Server.Application.Files.Models;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Core.Files.Queries;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;
using Uploadify.Server.Domain.Infrastructure.Requests.Contracts;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using static Uploadify.Server.Domain.Infrastructure.Requests.Models.Status;
using File = Uploadify.Server.Domain.Files.Models.File;

namespace Uploadify.Server.Application.Files.Commands;

public class CreateFileCommand : IBaseRequest<CreateFileCommandResponse>, ICommand<CreateFileCommandResponse>, IRequestWithUserName
{
    [JsonIgnore]
    public string? UserName { get; set; }

    public int? FolderId { get; set; }
    public int? CategoryId { get; set; }
    public IFormFile? File { get; set; }
}

public class CreateFileCommandHandler : ICommandHandler<CreateFileCommand, CreateFileCommandResponse>
{
    private readonly DataContext _context;
    private readonly ISender _sender;

    public CreateFileCommandHandler(DataContext context, ISender sender)
    {
        _context = context;
        _sender = sender;
    }

    public async Task<CreateFileCommandResponse> Handle(CreateFileCommand request, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(request.File);
        Guard.IsNotNull(request.FolderId);

        var userResponse = await _sender.Send(new GetUserQuery(request.UserName), cancellationToken: default);
        if (userResponse is not { Status: Ok, User: not null })
        {
            return new(userResponse);
        }

        var folderResponse = await _sender.Send(new GetFolderQuery(request.FolderId), cancellationToken: default);
        if (folderResponse is not { Status: Ok, Folder: not null })
        {
            return new(folderResponse);
        }

        if (folderResponse.Folder.UserId != userResponse.User.Id)
        {
            return new(Forbidden, new() { UserFriendlyMessage = Translations.RequestStatuses.Forbidden} );
        }


        var filename = FileSystemHelpers.GetFileName(request.File.ContentDisposition);
        var file = new File
        {
            SafeName = Guid.NewGuid().ToString(),
            UnsafeName = filename
        };

        var path = FileSystemHelpers.GetFilePath(file.SafeName);
        var executionStrategy = _context.Database.CreateExecutionStrategy();

        await ExecutionStrategyExtensions.ExecuteAsync(executionStrategy, async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken: default);

            try
            {
                await using (var stream = new FileStream(path, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream, cancellationToken);
                }

                file.FolderId = request.FolderId.Value;
                file.CategoryId = request.CategoryId;
                file.UnsafeName = filename;
                file.SafeName = Guid.NewGuid().ToString();
                file.Size = request.File.Length;
                file.Extension = Path.GetExtension(filename);
                file.MimeType = request.File.ContentType;
                file.Location = path;

                await _context.AddAsync(file, cancellationToken: default);
                await _context.SaveChangesAsync(cancellationToken: default);

                await transaction.CommitAsync(cancellationToken: default);
            }
            catch (Exception exception)
            {
                await transaction.RollbackAsync(cancellationToken: default);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                file = null;
            }
        }).ConfigureAwait(false);

        if (file == null)
        {
            return new(InternalServerError, new () { UserFriendlyMessage = Translations.RequestStatuses.InternalServerError });
        }

        var overview = file.Adapt<FileOverview>();

        overview.Name = file.SafeName;
        overview.CreatedBy = file.UserCreatedBy?.FullName;
        overview.UpdatedBy = file.UserUpdatedBy?.FullName;

        return new(overview);
    }
}

public class CreateFileCommandResponse : BaseResponse
{
    public CreateFileCommandResponse()
    {
    }

    public CreateFileCommandResponse(BaseResponse? response) : base(response)
    {
    }

    public CreateFileCommandResponse(Status status, RequestFailure? failure = null) : base(status, failure)
    {
    }

    public CreateFileCommandResponse(FileOverview? file) : base(Created)
    {
        File = file;
    }

    public FileOverview? File { get; set; }
}
