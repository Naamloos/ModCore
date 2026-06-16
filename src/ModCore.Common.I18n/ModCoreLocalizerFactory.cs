using Microsoft.Extensions.Localization;
using ModCore.Common.Language.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ModCore.Common.Language
{
    public class ModCoreLocalizerFactory : IModCoreLocalizerFactory
    {
        private const string FALLBACK_LOCALE = "en";
        private readonly IStringLocalizer<SharedResources> _localizer;

        public ModCoreLocalizerFactory(IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
        }

        public IModCoreLocalizer Get(string? locale = null)
        {
            return new ModCoreLocalizer(_localizer, normalizeLocale(locale));
        }

        private string normalizeLocale(string? locale)
        {
            if (string.IsNullOrWhiteSpace(locale))
                return FALLBACK_LOCALE;

            try
            {
                var culture = CultureInfo.GetCultureInfo(locale);
                return culture.Name;
            }
            catch (CultureNotFoundException)
            {
                return FALLBACK_LOCALE;
            }
        }
    }
}
