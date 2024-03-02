using Microsoft.Extensions.Localization;
using static System.String;

namespace Uploadify.Client.Core.Infrastructure.Translations.Helpers;

public static class TranslationHelpers
{
    public static string GetTranslation(string? translationKey, IStringLocalizer localizer)
    {
        if (IsNullOrWhiteSpace(translationKey) || localizer[translationKey].ResourceNotFound)
        {
            return localizer[Domain.Localization.Constants.Translations.Common.TranslationNotFound];
        }

        return localizer[translationKey];
    }
}
