using System.Text.RegularExpressions;

namespace ModCore.Services.DiscordProxy
{
    public static partial class DiscordRouteKey
    {
        public static string Normalize(string method, string path)
        {
            path = path.Trim('/');

            var parts = path.Split(
                '/',
                StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < parts.Length; i++)
            {
                if (IsSnowflake(parts[i]) && !IsMajorParameter(parts, i))
                {
                    parts[i] = ":id";
                }
            }

            return $"{method.ToUpperInvariant()} /{string.Join('/', parts)}";
        }

        private static bool IsMajorParameter(string[] parts, int index)
        {
            if (index == 0)
                return false;

            var previous = parts[index - 1];

            return previous is "channels" or "guilds" or "webhooks";
        }

        private static bool IsSnowflake(string value)
        {
            return SnowflakeRegex().IsMatch(value);
        }

        [GeneratedRegex(@"^\d{16,20}$")]
        private static partial Regex SnowflakeRegex();
    }
}
