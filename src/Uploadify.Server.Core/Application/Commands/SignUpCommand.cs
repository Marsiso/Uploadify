using Mapster;
using Microsoft.AspNetCore.Identity;
using Uploadify.Server.Domain.Application.Constants;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Localization.Constants;
using Uploadify.Server.Domain.Requests.Exceptions;
using Uploadify.Server.Domain.Requests.Models;
using Uploadify.Server.Domain.Requests.Services;

namespace Uploadify.Server.Core.Application.Commands;

public class SignUpCommand : BaseRequest<SignUpCommandResponse>, ICommand<SignUpCommandResponse>
{
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

    public SignUpCommandHandler(UserManager<User> manager)
    {
        _manager = manager;
    }

    public async Task<SignUpCommandResponse> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        var user = request.Adapt<User>();
        var result = await _manager.CreateAsync(user);
        if (!result.Succeeded)
        {
            return new SignUpCommandResponse(Status.InternalServerError, new RequestFailure
            {
                UserFriendlyMessage = Translations.RequestStatuses.InternalServerError,
                Exception = new InternalServerException()
            });
        }

        result = await _manager.AddPasswordAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return new SignUpCommandResponse(Status.InternalServerError, new RequestFailure
            {
                UserFriendlyMessage = Translations.RequestStatuses.InternalServerError,
                Exception = new InternalServerException()
            });
        }

        await _manager.AddToRoleAsync(user, Roles.Defaults.DefaultUser);

        return new SignUpCommandResponse(user);
    }
}

public class SignUpCommandResponse : BaseResponse
{
    public SignUpCommandResponse()
    {
    }

    public SignUpCommandResponse(Status status, RequestFailure failure) : base(status, failure)
    {
    }

    public SignUpCommandResponse(User user)
    {
        Status = Status.Created;
        User = user;
    }

    public User? User { get; set; }
}
