using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Utils
{
    public static class ColorConverter
    {
        public static string FromInt(int value)
        {
            return value.ToString("X6");
        }

        public static int FromHex(string value)
        {
            return int.Parse(value.Replace("#", ""), System.Globalization.NumberStyles.HexNumber);
        }
    }
}
