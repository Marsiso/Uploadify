using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Localization.Constants;
using Uploadify.Server.Domain.Requests.Exceptions;
using Uploadify.Server.Domain.Requests.Models;
using Uploadify.Server.Domain.Requests.Services;
using static Uploadify.Server.Domain.Requests.Models.Status;
using File = Uploadify.Server.Domain.FileSystem.Models.File;

namespace Uploadify.Server.Core.FileSystem.Queries;

public class GetFileQuery : IQuery<GetFileQueryResponse>
{
    public int? FileId { get; set; }
}

public class GetFileQueryHandler : IQueryHandler<GetFileQuery, GetFileQueryResponse>
{
    private readonly Func<DataContext, int, Task<File?>> Query = EF.CompileAsyncQuery((DataContext context, int fileId) =>
        context.Files.Include(folder => folder.UserCreatedBy)
            .Include(folder => folder.UserUpdatedBy)
            .SingleOrDefault(file => file.Id == fileId));

    private readonly DataContext _context;

    public GetFileQueryHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<GetFileQueryResponse> Handle(GetFileQuery request, CancellationToken cancellationToken)
    {
        if (!request.FileId.HasValue)
        {
            return new(BadRequest, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.BadRequest,
                Exception = new BadRequestException(nameof(GetFileQuery))
            });
        }

        var file = await Query(_context, request.FileId.Value);
        if (file == null)
        {
            return new(NotFound, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.NotFound,
                Exception = new EntityNotFoundException(request.FileId.Value.ToString(), nameof(File))
            });
        }

        return new(file);
    }
}

public class GetFileQueryResponse : BaseResponse
{
    public GetFileQueryResponse(Status status, RequestFailure? failure = null) : base(status, failure)
    {
    }

    public GetFileQueryResponse(File? file)
    {
        Status = Ok;
        File = file;
    }

    public File? File { get; set; }
}
