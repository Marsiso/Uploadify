using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Application.Files.Commands;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;
using static System.String;

namespace Uploadify.Server.Application.Files.Validators;

public class RenameFolderCommandValidator : AbstractValidator<RenameFolderCommand>
{
    private static readonly Func<DataContext, int, string, Task<bool>> FolderNameValidationQuery = EF.CompileAsyncQuery((DataContext context, int folderId, string name) =>
        context.Folders.Where(folder => folder.Id == folderId)
            .All(folder => context.Folders.Where(parent => parent.Id == folder.ParentId).All(parent => parent.Children.All(child => child.Id == folderId || child.Name != name))));

    public RenameFolderCommandValidator(DataContext context)
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage(Translations.Validations.FolderNameRequired)
            .MaximumLength(255)
            .WithMessage(Translations.Validations.FolderNameMaxLength);

        When(command => command.FolderId.HasValue && !IsNullOrWhiteSpace(command.Name), () =>
        {
            RuleFor(command => command.Name)
                .MustAsync((command, _, _) => FolderNameValidationQuery(context, command.FolderId.Value, command.Name))
                .WithMessage(Translations.Validations.FolderNameExists);
        });
    }
}
