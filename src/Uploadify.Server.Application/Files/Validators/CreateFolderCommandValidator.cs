using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Application.Files.Commands;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;
using static System.String;

namespace Uploadify.Server.Application.Files.Validators;

public class CreateFolderCommandValidator : AbstractValidator<CreateFolderCommand>
{
    private static readonly Func<DataContext, int, string, Task<bool>> FolderNameValidationQuery = EF.CompileAsyncQuery((DataContext context, int parentId, string name) =>
        context.Folders.All(folder => folder.ParentId != parentId || folder.Name != name));

    public CreateFolderCommandValidator(DataContext context)
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage(Translations.Validations.FolderNameRequired)
            .MaximumLength(255)
            .WithMessage(Translations.Validations.FolderNameMaxLength);

        When(command => command.ParentId.HasValue && !IsNullOrWhiteSpace(command.Name), () =>
        {
            RuleFor(command => command.Name)
                .MustAsync((command, _, _) => FolderNameValidationQuery(context, command.ParentId.Value, command.Name))
                .WithMessage(Translations.Validations.FolderNameExists);
        });
    }
}
