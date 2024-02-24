using MediatR;
using Microsoft.AspNetCore.Identity;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Localization.Constants;
using Uploadify.Server.Domain.Requests.Models;
using Uploadify.Server.Domain.Requests.Services;
using static Uploadify.Server.Domain.Requests.Models.Status;

namespace Uploadify.Server.Application.Authentication.Commands;

public class SignInCommand : BaseRequest<SignInCommandResponse>, ICommand<SignInCommandResponse>
{
    public SignInCommand(string? userName, string? password, bool rememberMe)
    {
        UserName = userName;
        Password = password;
        RememberMe = rememberMe;
    }

    public string? UserName { get; set; }
    public string? Password { get; set; }
    public bool RememberMe { get; set; }
}

public class SignInCommandHandler : ICommandHandler<SignInCommand, SignInCommandResponse>
{
    private readonly DataContext _context;
    private readonly UserManager<User> _manager;
    private readonly IMediator _mediator;
    private readonly SignInManager<User> _signInManager;

    public SignInCommandHandler(DataContext context, UserManager<User> manager, SignInManager<User> signInManager, IMediator mediator)
    {
        _context = context;
        _manager = manager;
        _mediator = mediator;
        _signInManager = signInManager;
    }

    public async Task<SignInCommandResponse> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetUserQuery(request.UserName), cancellationToken);
        if (response.Status != Ok)
        {
            return new(response.Status, new()
            {
                UserFriendlyMessage = response.Failure.UserFriendlyMessage,
                Exception = response.Failure.Exception,
                Errors = new()
                {
                    { nameof(request.UserName), [Translations.Validations.InvalidLoginForm] }
                }
            });
        }

        if (!await _signInManager.CanSignInAsync(response.User))
        {
            return new(BadRequest, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.Forbidden,
                Errors = new() { { nameof(request.UserName), [Translations.Validations.UserLockedOut] } }
            });
        }

        response.User.DateLastLoggedIn = DateTime.UtcNow;

        _context.Update(response.User);

        await _context.SaveChangesAsync(cancellationToken);

        var result = await _signInManager.PasswordSignInAsync(response.User, request.Password, request.RememberMe, response.User.LockoutEnabled);
        if (result.IsNotAllowed || result.IsLockedOut)
        {
            return new(BadRequest, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.Forbidden,
                Errors = new() { { nameof(request.UserName), [Translations.Validations.UserLockedOut] } }
            });
        }

        if (!result.Succeeded)
        {
            return new(InternalServerError, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.Forbidden,
                Errors = new() { { nameof(request.UserName), [Translations.Validations.InvalidLoginForm] } }
            });
        }

        return new(response.User);
    }
}

public class SignInCommandResponse : BaseResponse
{
    public SignInCommandResponse()
    {
    }

    public SignInCommandResponse(Status status, RequestFailure? failure) : base(status, failure)
    {
    }

    public SignInCommandResponse(User user) : base(Ok)
    {
        User = user;
    }

    public User? User { get; set; }
}
