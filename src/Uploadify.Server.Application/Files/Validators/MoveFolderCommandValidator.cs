using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Application.Files.Commands;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;

namespace Uploadify.Server.Application.Files.Validators;

public class MoveFolderCommandValidator : AbstractValidator<MoveFolderCommand>
{
    public static readonly Func<DataContext, int, int, Task<bool>> FolderNameValidationQuery = EF.CompileAsyncQuery((DataContext context, int folderId, int parentId) =>
        context.Folders.Where(folder => folder.Id == parentId)
            .All(folder => folder.Children.All(subfolder => subfolder.Name != context.Folders.First(f => f.Id == folderId).Name)));

    public MoveFolderCommandValidator(DataContext context)
    {
        RuleFor(folder => folder.FolderId)
            .GreaterThan(0)
            .WithMessage(Translations.Validations.FolderParentFolderIdRequired);

        When(command => command.FolderId > 0 && command.DestinationFolderId > 0, () =>
        {
            RuleFor(command => command)
                .MustAsync((command, _, _) => FolderNameValidationQuery(context, command.FolderId, command.DestinationFolderId))
                .WithMessage(Translations.Validations.FolderNameExists);
        });
    }
}
