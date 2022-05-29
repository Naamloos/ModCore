using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModCore.Logic
{
    public class MentionUlongConverter : IArgumentConverter<ulong>
    {
        public static readonly Regex mentionRegex = new Regex(@"^<(#|@)!?([0-9]+)>$");
        public async Task<Optional<ulong>> ConvertAsync(string value, CommandContext ctx)
        {
            if (ulong.TryParse(value, out var result))
                return new Optional<ulong>(result);

            var match = mentionRegex.Match(value);
            if (match.Success && ulong.TryParse(match.Groups[2].Value, out var mention))
                return new Optional<ulong>(mention);

            return default;
        }
    }
}
