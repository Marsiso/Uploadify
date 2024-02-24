﻿using FluentValidation;
using Uploadify.Server.Application.FileSystem.Commands;
using Uploadify.Server.Domain.Localization.Constants;

namespace Uploadify.Server.Application.FileSystem.Validators;

public class CreateFolderCommandValidator : AbstractValidator<CreateFolderCommand>
{
    public CreateFolderCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage(Translations.Validations.FolderNameRequired)
            .MaximumLength(255)
            .WithMessage(Translations.Validations.FolderNameMaxLength);
    }
}
