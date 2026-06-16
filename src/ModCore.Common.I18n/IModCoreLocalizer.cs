using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Common.Language
{
    public interface IModCoreLocalizer
    {
        string Locale { get; }
        string this[string key] { get; }
        string this[string key, Dictionary<string, object> values] { get; }
        string Translate(string key, Dictionary<string, object>? values = null);
    }
}
