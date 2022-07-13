using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModCore.Utils
{
    public class MentionUlongConverter : IArgumentConverter<ulong>
    {
        public static readonly Regex MentionRegex = new(@"^<(#|@)!?([0-9]+)>$", RegexOptions.Compiled);
        public Task<Optional<ulong>> ConvertAsync(string value, CommandContext ctx)
        {
            if (ulong.TryParse(value, out var result))
                return Task.FromResult(new Optional<ulong>(result));

            Match match = MentionRegex.Match(value);
            return Task.FromResult(match.Success && ulong.TryParse(match.Groups[2].Value, out var mention) ? new Optional<ulong>(mention) : default);
        }
    }
}
