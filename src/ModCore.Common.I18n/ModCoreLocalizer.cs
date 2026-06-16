using Microsoft.Extensions.Localization;
using ModCore.Common.Language.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ModCore.Common.Language
{
    public sealed class ModCoreLocalizer : IModCoreLocalizer
    {
        private const int MaxResolutionDepth = 20;

        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly string _locale;

        internal ModCoreLocalizer(IStringLocalizer<SharedResources> localizer, string locale)
        {
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _locale = locale;
        }

        public string Locale => _locale;

        public string this[string key] => Translate(key);

        public string this[string key, object? values] => Translate(key, values);

        public string Translate(string key, object? values = null)
        {
            var normalizedValues = NormalizeValues(values);

            var culture = GetCultureInfo(Locale);
            var previousCulture = CultureInfo.CurrentCulture;
            var previousUiCulture = CultureInfo.CurrentUICulture;

            try
            {
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;

                return TranslateKey(
                    key,
                    normalizedValues,
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                    depth: 0
                );
            }
            finally
            {
                CultureInfo.CurrentCulture = previousCulture;
                CultureInfo.CurrentUICulture = previousUiCulture;
            }
        }

        private string TranslateKey(
            string key,
            IReadOnlyDictionary<string, object?>? values,
            HashSet<string> resolvingKeys,
            int depth
        )
        {
            if (depth > MaxResolutionDepth)
                return key;

            if (!resolvingKeys.Add(key))
                return key;

            try
            {
                var localized = _localizer[key];

                if (localized.ResourceNotFound)
                    return key;

                return ResolveText(localized.Value, values, resolvingKeys, depth + 1);
            }
            finally
            {
                resolvingKeys.Remove(key);
            }
        }

        private string ResolveText(
            string value,
            IReadOnlyDictionary<string, object?>? values,
            HashSet<string> resolvingKeys,
            int depth
        )
        {
            if (depth > MaxResolutionDepth || string.IsNullOrEmpty(value))
                return value;

            var result = new StringBuilder(value.Length);

            for (var index = 0; index < value.Length; index++)
            {
                var character = value[index];

                if (character == '\\' && index + 1 < value.Length)
                {
                    result.Append(value[index + 1]);
                    index++;
                    continue;
                }

                if (character == '{')
                {
                    var placeholderEnd = FindPlaceholderEnd(value, index + 1);

                    if (placeholderEnd != -1)
                    {
                        var expression = value.Substring(
                            index + 1,
                            placeholderEnd - index - 1
                        );

                        result.Append(ResolveExpression(
                            expression,
                            values,
                            resolvingKeys,
                            depth + 1,
                            fallback: value.Substring(index, placeholderEnd - index + 1)
                        ));

                        index = placeholderEnd;
                        continue;
                    }
                }

                result.Append(character);
            }

            return result.ToString();
        }

        private string ResolveExpression(
            string expression,
            IReadOnlyDictionary<string, object?>? values,
            HashSet<string> resolvingKeys,
            int depth,
            string fallback
        )
        {
            if (depth > MaxResolutionDepth)
                return fallback;

            expression = expression.Trim();

            if (string.IsNullOrEmpty(expression))
                return fallback;

            var parts = SplitExpression(expression, '|');

            if (parts.Count == 3)
            {
                if (values is null)
                    return fallback;

                return ResolveBooleanFormat(
                    parts[0],
                    parts[1],
                    parts[2],
                    values,
                    resolvingKeys,
                    depth + 1,
                    fallback
                );
            }

            return ResolveToken(
                expression,
                values,
                resolvingKeys,
                depth + 1,
                fallback
            );
        }

        private string ResolveBooleanFormat(
            string conditionToken,
            string trueToken,
            string falseToken,
            IReadOnlyDictionary<string, object?> values,
            HashSet<string> resolvingKeys,
            int depth,
            string fallback
        )
        {
            conditionToken = conditionToken.Trim();
            trueToken = trueToken.Trim();
            falseToken = falseToken.Trim();

            if (!values.TryGetValue(conditionToken, out var conditionValue))
                return fallback;

            if (!TryConvertToBoolean(conditionValue, out var condition))
                return fallback;

            return ResolveToken(
                condition ? trueToken : falseToken,
                values,
                resolvingKeys,
                depth + 1,
                fallback
            );
        }

        private string ResolveToken(
            string token,
            IReadOnlyDictionary<string, object?>? values,
            HashSet<string> resolvingKeys,
            int depth,
            string fallback
        )
        {
            if (depth > MaxResolutionDepth)
                return fallback;

            token = token.Trim();

            if (IsQuotedString(token))
            {
                var inlineTemplate = token.Substring(1, token.Length - 2);

                return ResolveText(
                    inlineTemplate,
                    values,
                    resolvingKeys,
                    depth + 1
                );
            }

            if (token.StartsWith("t:", StringComparison.OrdinalIgnoreCase))
            {
                var translationKey = token.Substring(2).Trim();

                if (string.IsNullOrEmpty(translationKey))
                    return fallback;

                return TranslateKey(
                    translationKey,
                    values,
                    resolvingKeys,
                    depth + 1
                );
            }

            if (values is not null && values.TryGetValue(token, out var value))
            {
                var replacement = Convert.ToString(value, CultureInfo.CurrentCulture) ?? string.Empty;

                return ResolveText(
                    replacement,
                    values,
                    resolvingKeys,
                    depth + 1
                );
            }

            return ResolveText(
                token,
                values,
                resolvingKeys,
                depth + 1
            );
        }

        private static int FindPlaceholderEnd(string value, int startIndex)
        {
            var inQuotes = false;

            for (var index = startIndex; index < value.Length; index++)
            {
                var character = value[index];

                if (character == '\\')
                {
                    index++;
                    continue;
                }

                if (character == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (!inQuotes && character == '}')
                    return index;
            }

            return -1;
        }

        private static List<string> SplitExpression(string expression, char separator)
        {
            var parts = new List<string>();
            var current = new StringBuilder();
            var inQuotes = false;

            for (var index = 0; index < expression.Length; index++)
            {
                var character = expression[index];

                if (character == '\\')
                {
                    current.Append(character);

                    if (index + 1 < expression.Length)
                    {
                        current.Append(expression[index + 1]);
                        index++;
                    }

                    continue;
                }

                if (character == '"')
                {
                    inQuotes = !inQuotes;
                    current.Append(character);
                    continue;
                }

                if (!inQuotes && character == separator)
                {
                    parts.Add(current.ToString().Trim());
                    current.Clear();
                    continue;
                }

                current.Append(character);
            }

            parts.Add(current.ToString().Trim());

            return parts;
        }

        private static bool IsQuotedString(string value)
        {
            return value.Length >= 2
                && value[0] == '"'
                && value[value.Length - 1] == '"'
                && !IsEscaped(value, value.Length - 1);
        }

        private static bool IsEscaped(string value, int index)
        {
            var backslashCount = 0;

            for (var cursor = index - 1; cursor >= 0 && value[cursor] == '\\'; cursor--)
                backslashCount++;

            return backslashCount % 2 != 0;
        }

        private static IReadOnlyDictionary<string, object?>? NormalizeValues(object? values)
        {
            if (values is null)
                return null;

            if (values is IReadOnlyDictionary<string, object?> readOnlyDictionary)
                return readOnlyDictionary;

            if (values is IEnumerable<KeyValuePair<string, object?>> keyValuePairs)
            {
                return keyValuePairs.ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value,
                    StringComparer.OrdinalIgnoreCase
                );
            }

            var type = values.GetType();

            if (IsSimpleValue(type))
                return null;

            return type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(property => property.GetIndexParameters().Length == 0)
                .ToDictionary(
                    property => property.Name,
                    property => property.GetValue(values),
                    StringComparer.OrdinalIgnoreCase
                );
        }

        private static bool TryConvertToBoolean(object? value, out bool result)
        {
            switch (value)
            {
                case bool boolean:
                    result = boolean;
                    return true;

                case string stringValue:
                    return bool.TryParse(stringValue, out result);

                default:
                    result = false;
                    return false;
            }
        }

        private static bool IsSimpleValue(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            return type.IsPrimitive
                || type.IsEnum
                || type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset)
                || type == typeof(Guid)
                || type == typeof(TimeSpan);
        }

        private static CultureInfo GetCultureInfo(string locale)
        {
            try
            {
                return locale switch
                {
                    "de" => CultureInfo.GetCultureInfo("de"),
                    "nl" => CultureInfo.GetCultureInfo("nl"),
                    "ro" => CultureInfo.GetCultureInfo("ro"),

                    // In this project, invariant resources are the English fallback.
                    "en-GB" => CultureInfo.InvariantCulture,
                    "en-US" => CultureInfo.InvariantCulture,

                    _ => CultureInfo.InvariantCulture
                };
            }
            catch (CultureNotFoundException)
            {
                return CultureInfo.InvariantCulture;
            }
        }
    }
}