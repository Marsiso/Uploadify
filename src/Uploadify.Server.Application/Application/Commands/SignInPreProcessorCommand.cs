using MediatR;
using Microsoft.AspNetCore.Identity;
using Uploadify.Server.Application.Authentication.Helpers;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Infrastructure.Localization;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using Uploadify.Server.Domain.Infrastructure.Requests.Services;

namespace Uploadify.Server.Application.Application.Commands;

public class SignInPreProcessorCommand : BaseRequest<SignInPreProcessorCommandResponse>, ICommand<SignInPreProcessorCommandResponse>
{
    public SignInPreProcessorCommand(string? userName, string? password, bool rememberMe)
    {
        UserName = userName;
        Password = password;
        RememberMe = rememberMe;
    }

    public string? UserName { get; set; }
    public string? Password { get; set; }
    public bool RememberMe { get; set; }
}

public class SignInPreProcessorCommandHandler : ICommandHandler<SignInPreProcessorCommand, SignInPreProcessorCommandResponse>
{
    private readonly DataContext _context;
    private readonly UserManager<User> _manager;
    private readonly SignInManager<User> _signInManager;
    private readonly IMediator _mediator;

    public SignInPreProcessorCommandHandler(DataContext context, UserManager<User> manager, SignInManager<User> signInManager, IMediator mediator)
    {
        _context = context;
        _manager = manager;
        _mediator = mediator;
        _signInManager = signInManager;
    }

    public async Task<SignInPreProcessorCommandResponse> Handle(SignInPreProcessorCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new FindUserQuery(request.UserName), cancellationToken);
        if (response.Status != Status.Ok)
        {
            return new SignInPreProcessorCommandResponse(response.Status, new RequestFailure
            {
                UserFriendlyMessage = response.Failure.UserFriendlyMessage,
                Exception = response.Failure.Exception,
                Errors = new Dictionary<string, string[]>
                {
                    { nameof(request.UserName), new [] { Translations.Validations.InvalidCredentials } }
                }
            });
        }

        if (!await _signInManager.CanSignInAsync(response.User))
        {
            return new SignInPreProcessorCommandResponse(Status.BadRequest, new RequestFailure
            {
                UserFriendlyMessage = Translations.RequestStatuses.Forbidden,
                Errors = new Dictionary<string, string[]>
                {
                    { nameof(request.UserName), new [] { Translations.Validations.UserLockedOut } }
                }
            });
        }

        var result = await _signInManager.PasswordSignInAsync(response.User, request.Password, request.RememberMe, response.User.LockoutEnabled);
        if (result.IsNotAllowed || result.IsLockedOut)
        {
            return new SignInPreProcessorCommandResponse(Status.BadRequest, new RequestFailure
            {
                UserFriendlyMessage = Translations.RequestStatuses.Forbidden,
                Errors = new Dictionary<string, string[]>
                {
                    { nameof(request.UserName), new [] { Translations.Validations.UserLockedOut } }
                }
            });
        }

        if (!result.Succeeded)
        {
            return new SignInPreProcessorCommandResponse(Status.InternalServerError, new RequestFailure
            {
                UserFriendlyMessage = Translations.RequestStatuses.Forbidden,
                Errors = new Dictionary<string, string[]>
                {
                    { nameof(request.UserName), new [] { Translations.Validations.InvalidCredentials } }
                }
            });
        }

        response.User.DateLastLoggedIn = DateTime.UtcNow;

        _context.Update(response.User);

        await _context.SaveChangesAsync(cancellationToken);

        var claims = await _manager.GetClaimsAsync(response.User);
        if (claims.Count > 0)
        {
            await _manager.RemoveClaimsAsync(response.User, claims);
        }

        await _manager.AddClaimsAsync(response.User, AuthenticationHelpers.GetClaimsFrom(response.User));

        return new SignInPreProcessorCommandResponse(response.User);
    }
}

public class SignInPreProcessorCommandResponse : BaseResponse
{
    public SignInPreProcessorCommandResponse()
    {
    }

    public SignInPreProcessorCommandResponse(Status status, RequestFailure? failure) : base(status, failure)
    {
    }

    public SignInPreProcessorCommandResponse(User user)
    {
        Status = Status.Ok;
        User = user;
        Failure = null;
    }

    public User? User { get; set; }
}
