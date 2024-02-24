using FluentValidation;
using Uploadify.Server.Application.FileSystem.Commands;
using Uploadify.Server.Domain.Localization.Constants;

namespace Uploadify.Server.Application.FileSystem.Validators;

public class CreateFileCommandValidator : AbstractValidator<CreateFileCommand>
{
    public CreateFileCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage(Translations.Validations.FileNameRequired)
            .MaximumLength(256)
            .WithMessage(Translations.Validations.FileNameMaxLength);
    }
}
