using Microsoft.Extensions.Localization;
using ModCore.Common.Language.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ModCore.Common.Language
{
    public sealed class ModCoreLocalizer : IModCoreLocalizer
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private string _locale;

        internal ModCoreLocalizer(IStringLocalizer<SharedResources> localizer, string locale)
        {
            _localizer = localizer;
            _locale = locale;
        }

        public string Locale { get => _locale; }

        public string this[string key] => Translate(key);

        public string this[string key, Dictionary<string, object> values] => Translate(key, values);

        public string Translate(string key, Dictionary<string, object>? values = null)
        {
            var culture = this.GetCultureInfo(Locale);
            var previousCulture = CultureInfo.CurrentCulture;
            var previousUiCulture = CultureInfo.CurrentUICulture;

            try
            {
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;

                var localized = _localizer[key];

                if (localized.ResourceNotFound)
                    return key;

                return values is null
                    ? localized.Value
                    : ReplacePlaceholders(localized.Value, values);
            }
            finally
            {
                CultureInfo.CurrentCulture = previousCulture;
                CultureInfo.CurrentUICulture = previousUiCulture;
            }
        }

        private string ReplacePlaceholders(string value, IDictionary<string, object> values)
        {
            foreach (var kvp in values)
            {
                value = value.Replace($"{{{kvp.Key}}}", kvp.Value?.ToString() ?? string.Empty);
            }
            return value;
        }

        private CultureInfo GetCultureInfo(string locale)
        {
            try
            {
                switch (locale)
                {
                    case "de":
                        return CultureInfo.GetCultureInfo("de");

                    case "nl":
                        return CultureInfo.GetCultureInfo("nl");

                    case "ro":
                        return CultureInfo.GetCultureInfo("ro");

                    case "en-GB":
                    case "en-US":
                    default:
                        return CultureInfo.InvariantCulture; // invariant is english in this case
                }
            }
            catch (CultureNotFoundException)
            {
                // Fallback to invariant culture if the specified locale is not found
                return CultureInfo.InvariantCulture;
            }
        }
    }
}
