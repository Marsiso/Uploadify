using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;
using Uploadify.Server.Domain.Infrastructure.Requests.Contracts;
using Uploadify.Server.Domain.Infrastructure.Requests.Exceptions;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using static Uploadify.Server.Domain.Infrastructure.Requests.Models.Status;
using File = Uploadify.Server.Domain.Files.Models.File;

namespace Uploadify.Server.Core.Files.Queries;

public class GetFileQuery : IQuery<GetFileQueryResponse>
{
    public int? FileId { get; set; }
}

public class GetFileQueryHandler : IQueryHandler<GetFileQuery, GetFileQueryResponse>
{
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

        var file = await _context.Files
            .Include(file => file.Folder)
            .Include(file => file.UserCreatedBy)
            .Include(file => file.UserUpdatedBy)
            .SingleOrDefaultAsync(file => file.Id == request.FileId.Value, cancellationToken: cancellationToken);

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
    public GetFileQueryResponse()
    {
    }

    public GetFileQueryResponse(Status status, RequestFailure? failure = null) : base(status, failure)
    {
    }

    public GetFileQueryResponse(File file) : base(Ok)
    {
        File = file;
    }

    public File? File { get; set; }
}
