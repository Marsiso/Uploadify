using FluentValidation;
using Uploadify.Server.Application.Authentication.Commands;
using Uploadify.Server.Domain.Localization.Constants;

namespace Uploadify.Server.Application.Authentication.Validators;

public class SignInPreProcessorValidator : AbstractValidator<SignInCommand>
{
    public SignInPreProcessorValidator()
    {
        RuleFor(command => command.UserName)
            .NotEmpty()
            .WithMessage(Translations.Validations.UserNameRequired);

        RuleFor(command => command.Password)
            .NotEmpty()
            .WithMessage(Translations.Validations.PasswordRequired);
    }
}
