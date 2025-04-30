using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ModCore.Common.Language
{
    public class I18n
    {
        private Dictionary<string, JsonObject> _locales = new();
        private Regex _resourceNameRegex = new(@"^ModCore\.Common\.Language\.Languages\.(?<locale>.*)\.json$", RegexOptions.Compiled);

        // TODO implement some way to handle i18n for command names
        public I18n()
        {
            var assembly = GetType().Assembly;
            var names = assembly.GetManifestResourceNames();
            var matches = names.Where(x => _resourceNameRegex.IsMatch(x));
            var loaded = matches.Select(x => new
            {
                Locale = _resourceNameRegex.Match(x).Groups["locale"].Value,
                Json = JsonSerializer.Deserialize<JsonObject>(assembly.GetManifestResourceStream(x))
            });
            _locales = loaded.ToDictionary(x => x.Locale, x => x.Json)!;
        }

        public string t(string key, string locale = "en", Dictionary<string, object>? data = null)
        {
            // If key is not found in the specified or fallback locale, return the key itself  
            if (!_locales.ContainsKey(locale) || !_locales.ContainsKey("en"))
                return key;

            // We found our value!  
            JsonObject localeObject = _locales[locale];
            JsonObject fallbackLocaleObject = _locales["en"];
            string value = getValue(localeObject, fallbackLocaleObject, key.Split('.')) ?? key;

            // If we have data, we apply replacements onto the value  
            if (data != null)
            {
                foreach (var valueKey in data.Keys)
                {
                    value = value.Replace($"{{{valueKey}}}", data[valueKey]?.ToString() ?? string.Empty);
                }
            }

            // Finally, return our value!  
            return value;
        }

        private string? getValue(JsonNode? locale, JsonNode? fallbackLocale, string[] key)
        {
            if (locale is JsonValue jsonValue)
            {
                return jsonValue.ToString();
            }
            else if (locale is JsonObject jsonObject && jsonObject.TryGetPropertyValue(key[0], out JsonNode? nextNode) && nextNode != null)
            {
                return getValue(nextNode, fallbackLocale, key.Skip(1).ToArray());
            }
            else if (fallbackLocale is JsonObject fallbackJsonObject && fallbackJsonObject.TryGetPropertyValue(key[0], out JsonNode? fallbackNextNode) && fallbackNextNode != null)
            {
                return getValue(fallbackNextNode, fallbackLocale, key.Skip(1).ToArray());
            }
            return null;
        }

        public JsonObject GetFileFor(string locale)
        {
            if (_locales.ContainsKey(locale))
            {
                return _locales[locale];
            }
            else
            {
                if (_locales.ContainsKey("en"))
                {
                    return _locales["en"];
                }
                throw new FileNotFoundException($"Locale file for {locale} not found, and no fallback locale was found.");
            }
        }

        public IReadOnlyDictionary<string, JsonObject> GetAllLocales()
        {
            return _locales.AsReadOnly();
        }
    }
}
