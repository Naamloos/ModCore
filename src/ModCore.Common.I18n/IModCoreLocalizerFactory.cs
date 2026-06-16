using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Common.Language
{
    public interface IModCoreLocalizerFactory
    {
        IModCoreLocalizer Get(string? locale = null);
    }
}
