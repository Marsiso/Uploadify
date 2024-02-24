using FluentValidation;
using MediatR;
using Uploadify.Server.Application.Authentication.Commands;
using Uploadify.Server.Application.Validations.Extensions;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Domain.Localization.Constants;

namespace Uploadify.Server.Application.Authentication.Validators;

public class CreateUserCommandValidator : AbstractValidator<SignUpCommand>
{
    private readonly IMediator _mediator;

    public CreateUserCommandValidator(IMediator mediator)
    {
        _mediator = mediator;

        RuleFor(command => command.GivenName)
            .NotEmpty()
            .WithMessage(Translations.Validations.GivenNameRequired)
            .MaximumLength(256)
            .WithMessage(Translations.Validations.GivenNameMaxLength);

        RuleFor(command => command.FamilyName)
            .NotEmpty()
            .WithMessage(Translations.Validations.FamilyNameRequired)
            .MaximumLength(256)
            .WithMessage(Translations.Validations.FamilyNameMaxLength);

        RuleFor(command => command.UserName)
            .NotEmpty()
            .WithMessage(Translations.Validations.UserNameRequired)
            .MaximumLength(256)
            .WithMessage(Translations.Validations.UserNameMaxLength)
            .MustAsync(UniqueLogin)
            .WithMessage(Translations.Validations.UserNameUnique);

        RuleFor(command => command.PhoneNumber)
            .NotEmpty()
            .WithMessage(Translations.Validations.PhoneNumberRequired)
            .MaximumLength(256)
            .WithMessage(Translations.Validations.PhoneNumberMaxLength);

        RuleFor(command => command.Email)
            .NotEmpty()
            .WithMessage(Translations.Validations.EmailRequired)
            .EmailAddress()
            .WithMessage(Translations.Validations.EmailFormat)
            .MustAsync(UniqueEmail)
            .WithMessage(Translations.Validations.EmailUnique)
            .MaximumLength(256)
            .WithMessage(Translations.Validations.EmailMaxLength);

        RuleFor(command => command.Password)
            .NotEmpty()
            .WithMessage(Translations.Validations.PasswordRequired)
            .MinimumLength(10)
            .WithMessage(Translations.Validations.PasswordMinLength)
            .HasUpperCaseCharacter()
            .WithMessage(Translations.Validations.PasswordUpperCaseCharacter)
            .HasLowerCaseCharacter()
            .WithMessage(Translations.Validations.PasswordLowerCaseCharacter)
            .HasNumericCharacter()
            .WithMessage(Translations.Validations.PasswordNumericCharacter)
            .HasSpecialCharacter()
            .WithMessage(Translations.Validations.PasswordSpecialCharacter);

        RuleFor(command => command.PasswordRepeat)
            .NotEmpty()
            .WithMessage(Translations.Validations.PasswordRepeatRequired)
            .Equal(form => form.Password)
            .WithMessage(Translations.Validations.PasswordRepeatMatch);
    }

    private async Task<bool> UniqueLogin(string? login, CancellationToken cancellationToken)
    {
        return (await _mediator.Send(new UniqueUserNameQuery(login), cancellationToken)).IsUnique;
    }

    private async Task<bool> UniqueEmail(string? email, CancellationToken cancellationToken)
    {
        return (await _mediator.Send(new UniqueEmailQuery(email), cancellationToken)).IsUnique;
    }
}
