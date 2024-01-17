using FluentValidation;

namespace Uploadify.Server.Application.Validations.Extensions;

public static class FluentValidationRuleBuilderExtensions
{
       public const string SpecialCharacters = @"!@#$%^&*()_+-=~`[]{};:',<.>/?\|";

    public static IRuleBuilderOptions<TProperty, string?> Url<TProperty>(this IRuleBuilder<TProperty, string?> ruleBuilder) =>
        ruleBuilder.Must(url =>
        {
            bool hasValidUri = !string.IsNullOrWhiteSpace(url);

            if (hasValidUri)
            {
                hasValidUri = Uri.TryCreate(url, UriKind.Absolute, out Uri? uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
            }

            return hasValidUri;
        });

    public static IRuleBuilderOptions<TProperty, string?> HasNumericCharacter<TProperty>(this IRuleBuilder<TProperty, string?> ruleBuilder) =>
        ruleBuilder.Must(value =>
        {
            ReadOnlySpan<char> valueSpan = value.AsSpan();

            if (valueSpan.IsEmpty)
            {
                return false;
            }

            foreach (char c in valueSpan)
            {
                if (char.IsNumber(c))
                {
                    return true;
                }
            }

            return false;
        });

    public static IRuleBuilderOptions<TProperty, string?> HasSpecialCharacter<TProperty>(this IRuleBuilder<TProperty, string?> ruleBuilder) =>
        ruleBuilder.Must(value =>
        {
            ReadOnlySpan<char> valueSpan = value.AsSpan();

            if (valueSpan.IsEmpty)
            {
                return false;
            }

            foreach (char c in valueSpan)
            {
                if (SpecialCharacters.Contains(c))
                {
                    return true;
                }
            }

            return false;
        });

    public static IRuleBuilderOptions<TProperty, string?> HasLowerCaseCharacter<TProperty>(this IRuleBuilder<TProperty, string?> ruleBuilder) =>
        ruleBuilder.Must(value =>
        {
            ReadOnlySpan<char> valueSpan = value.AsSpan();

            if (valueSpan.IsEmpty)
            {
                return false;
            }

            foreach (char c in valueSpan)
            {
                if (char.IsLower(c))
                {
                    return true;
                }
            }

            return false;
        });

    public static IRuleBuilderOptions<TProperty, string?> HasUpperCaseCharacter<TProperty>(this IRuleBuilder<TProperty, string?> ruleBuilder) =>
        ruleBuilder.Must(value =>
        {
            ReadOnlySpan<char> valueSpan = value.AsSpan();

            if (valueSpan.IsEmpty)
            {
                return false;
            }

            foreach (char c in valueSpan)
            {
                if (char.IsUpper(c))
                {
                    return true;
                }
            }

            return false;
        });
}
