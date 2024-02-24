using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Uploadify.Server.Domain.Application.Constants;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Localization.Constants;
using Uploadify.Server.Domain.Requests.Exceptions;
using Uploadify.Server.Domain.Requests.Models;
using Uploadify.Server.Domain.Requests.Services;
using static Uploadify.Server.Domain.Requests.Models.Status;

namespace Uploadify.Server.Application.Authentication.Commands;

public class SignUpCommand : BaseRequest<SignUpCommandResponse>, ICommand<SignUpCommandResponse>
{
    public SignUpCommand()
    {
    }

    public SignUpCommand(string? userName, string? email, string? phoneNumber, string? givenName, string? familyName, string? password, string? passwordRepeat)
    {
        UserName = userName;
        Email = email;
        PhoneNumber = phoneNumber;
        GivenName = givenName;
        FamilyName = familyName;
        Password = password;
        PasswordRepeat = passwordRepeat;
    }

    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? GivenName { get; set; }
    public string? FamilyName { get; set; }
    public string? Password { get; set; }
    public string? PasswordRepeat { get; set; }
}

public class SignUpCommandHandler : ICommandHandler<SignUpCommand, SignUpCommandResponse>
{
    private readonly UserManager<User> _manager;
    private readonly ISender _sender;

    public SignUpCommandHandler(UserManager<User> manager, ISender sender)
    {
        _manager = manager;
        _sender = sender;
    }

    public async Task<SignUpCommandResponse> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        var user = request.Adapt<User>();
        var result = await _manager.CreateAsync(user);
        if (!result.Succeeded)
        {
            return new(InternalServerError, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.InternalServerError,
                Exception = new InternalServerException()
            });
        }

        result = await _manager.AddPasswordAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return new(InternalServerError, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.InternalServerError,
                Exception = new InternalServerException()
            });
        }

        await _manager.AddToRoleAsync(user, Roles.Defaults.DefaultUser);

        var postProcessorCommandResponse = await _sender.Send(new SignUpPostProcessorCommand(user), cancellationToken: default);
        if (postProcessorCommandResponse is not { Status: Ok, User: not null, RootFolder: not null })
        {
            return new(postProcessorCommandResponse);
        }

        return new(user);
    }
}

public class SignUpCommandResponse : BaseResponse
{
    public SignUpCommandResponse()
    {
    }

    public SignUpCommandResponse(BaseResponse? response) : base(response)
    {
    }

    public SignUpCommandResponse(Status status, RequestFailure failure) : base(status, failure)
    {
    }

    public SignUpCommandResponse(User user) : base(Created) => User = user;

    public User? User { get; set; }
}
