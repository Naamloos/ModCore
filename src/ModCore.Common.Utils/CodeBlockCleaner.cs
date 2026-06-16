using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Common.Utils
{
    public static class CodeBlockCleaner
    {
        public static string InCodeBlock(this string text)
        {
            return text.Replace("`", "\u200B`");
        }
    }
}
