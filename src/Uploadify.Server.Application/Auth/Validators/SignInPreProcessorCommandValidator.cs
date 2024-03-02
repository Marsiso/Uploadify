using FluentValidation;
using Uploadify.Server.Application.Auth.Commands;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;

namespace Uploadify.Server.Application.Auth.Validators;

public class SignInPreProcessorCommandValidator : AbstractValidator<SignInCommand>
{
    public SignInPreProcessorCommandValidator()
    {
        RuleFor(command => command.UserName)
            .NotEmpty()
            .WithMessage(Translations.Validations.UserNameRequired);

        RuleFor(command => command.Password)
            .NotEmpty()
            .WithMessage(Translations.Validations.PasswordRequired);
    }
}
