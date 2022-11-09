namespace ModCore.Utils
{
    public class DateLexer
    {
        public static bool IsFillerWord(string s) => s == "me" || s == "in" || s == "at";

        public static bool IsFinishingWord(string s2) => s2 == "to";

        public static bool TryIsNumber(string s, out uint i)
        {
            switch (s)
            {
                case "a":
                case "an":
                case "one":
                    i = 1;
                    return true;
                case "two":
                    i = 2;
                    return true;
                case "three":
                    i = 3;
                    return true;
                case "four":
                    i = 4;
                    return true;
                case "five":
                    i = 5;
                    return true;
                case "six":
                    i = 6;
                    return true;
                case "seven":
                    i = 7;
                    return true;
                case "eight":
                    i = 8;
                    return true;
                case "nine":
                    i = 9;
                    return true;
                case "ten":
                    i = 10;
                    return true;
                case "eleven":
                    i = 11;
                    return true;
                case "twelve":
                    i = 12;
                    return true;
                default:
                    return uint.TryParse(s, out i);
            }
        }
    }
}