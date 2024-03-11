using FluentValidation;
using Uploadify.Server.Application.Files.Commands;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;

namespace Uploadify.Server.Application.Files.Validators;

public class RenameFileCommandValidator : AbstractValidator<RenameFileCommand>
{
    public RenameFileCommandValidator()
    {
        RuleFor(file => file.Name)
            .NotEmpty()
            .WithMessage(Translations.Validations.FileNameRequired)
            .MaximumLength(255)
            .WithMessage(Translations.Validations.FileNameMaxLength);
    }
}
