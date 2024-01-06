using Mapster;
using Microsoft.AspNetCore.Identity;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Infrastructure.Localization;
using Uploadify.Server.Domain.Infrastructure.Requests.Exceptions;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using Uploadify.Server.Domain.Infrastructure.Requests.Services;

namespace Uploadify.Server.Core.Application.Commands;

public class CreateUserCommand : BaseRequest<CreateUserCommandResponse>, ICommand<CreateUserCommandResponse>
{
    public CreateUserCommand(string? userName, string? email, string? phoneNumber, string? givenName, string? familyName, string? password, string? passwordRepeat)
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

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CreateUserCommandResponse>
{
    private readonly UserManager<User> _manager;

    public CreateUserCommandHandler(UserManager<User> manager)
    {
        _manager = manager;
    }

    public async Task<CreateUserCommandResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = request.Adapt<User>();
        var result = await _manager.CreateAsync(user);
        if (!result.Succeeded)
        {
            return new CreateUserCommandResponse(Status.InternalServerError, new RequestFailure
            {
                UserFriendlyMessage = Translations.RequestStatuses.InternalServerError,
                Exception = new InternalServerException()
            });
        }

        result = await _manager.AddPasswordAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return new CreateUserCommandResponse(Status.InternalServerError, new RequestFailure
            {
                UserFriendlyMessage = Translations.RequestStatuses.InternalServerError,
                Exception = new InternalServerException()
            });
        }

        return new CreateUserCommandResponse(user);
    }
}

public class CreateUserCommandResponse : BaseResponse
{
    public CreateUserCommandResponse()
    {
    }

    public CreateUserCommandResponse(Status status, RequestFailure failure) : base(status, failure)
    {
    }

    public CreateUserCommandResponse(User user)
    {
        Status = Status.Created;
        User = user;
        Failure = null;
    }

    public User? User { get; set; }
}
