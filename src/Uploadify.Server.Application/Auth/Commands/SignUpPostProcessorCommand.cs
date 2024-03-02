using MediatR;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Files.Constants;
using Uploadify.Server.Domain.Files.Models;
using Uploadify.Server.Domain.Infrastructure.Requests.Contracts;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using static Uploadify.Server.Domain.Infrastructure.Requests.Models.Status;

namespace Uploadify.Server.Application.Auth.Commands;

public class SignUpPostProcessorCommand : ICommand<SignUpPostProcessorCommandResponse>
{
    public SignUpPostProcessorCommand(User user)
    {
        User = user;
    }

    public User User { get; set; }
}

public class SignUpPostProcessorCommandHandler : IRequestHandler<SignUpPostProcessorCommand, SignUpPostProcessorCommandResponse>
{
    private readonly DataContext _context;

    public SignUpPostProcessorCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<SignUpPostProcessorCommandResponse> Handle(SignUpPostProcessorCommand request, CancellationToken cancellationToken)
    {
        var rootFolder = new Folder { UserId = request.User.Id, Name = Folders.RootName };

        await _context.Folders.AddAsync(rootFolder, cancellationToken: default);
        await _context.SaveChangesAsync(cancellationToken: default);

        return new(request.User, rootFolder);
    }
}

public class SignUpPostProcessorCommandResponse : BaseResponse
{
    public SignUpPostProcessorCommandResponse()
    {
    }

    public SignUpPostProcessorCommandResponse(Status status, RequestFailure? failure = null) : base(status, failure)
    {
    }

    public SignUpPostProcessorCommandResponse(User? user, Folder? folder) : base(Created)
    {
        User = user;
        RootFolder = folder;
    }

    public User? User { get; set; }
    public Folder? RootFolder { get; set; }
}
