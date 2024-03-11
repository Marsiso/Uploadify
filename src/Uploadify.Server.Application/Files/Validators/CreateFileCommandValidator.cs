using FluentValidation;
using Uploadify.Server.Application.Files.Commands;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;

namespace Uploadify.Server.Application.Files.Validators;

public class CreateFileCommandValidator : AbstractValidator<CreateFileCommand>
{
    public CreateFileCommandValidator()
    {
        RuleFor(command => command.File)
            .NotNull()
            .WithMessage(Translations.Validations.FileRequired);

        When(command => command.File != null, () =>
        {
            RuleFor(command => command.File.Length)
                .GreaterThan(0)
                .WithMessage(Translations.Validations.FileLengthRequired)
                .LessThanOrEqualTo(104857600L)
                .WithMessage(Translations.Validations.FileLengthTooLarge);

            RuleFor(command => command.File.ContentType)
                .NotEmpty()
                .WithMessage(Translations.Validations.FileContentTypeRequired);

            RuleFor(command => command.File.Name)
                .NotEmpty()
                .WithMessage(Translations.Validations.FileNameRequired)
                .MaximumLength(256)
                .WithMessage(Translations.Validations.FileNameMaxLength)
                .Must(filename => !Path.HasExtension(filename))
                .WithMessage(Translations.Validations.FileNameExtensionRequired);
        });
    }
}
