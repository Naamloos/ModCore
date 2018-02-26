using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;

namespace ModCore.Logic
{
    public class AugmentedBoolConverter : IArgumentConverter<bool>
    {
        public Task<Optional<bool>> ConvertAsync(string value, CommandContext ctx)
        {
            return Task.FromResult(TryConvert(value, ctx, out var b) ? new Optional<bool>(b) : new Optional<bool>());
        }

        public static bool TryConvert(string value, CommandContext ctx, out bool result)
        {
            switch (value.ToLower())
            {
                case "y":
                case "ye":
                case "ya":
                case "yup":
                case "yee":
                case "davaj":
                case "yes":
                case "1":
                case "on":
                case "enable":
                case "да":
                    result = true;
                    return true;

                case "n":
                case "nah":
                case "nope":
                case "nyet":
                case "nada":
                case "no":
                case "0":
                case "off":
                case "disable":
                case "нет":
                    result = false;
                    return true;

                default:
                    return bool.TryParse(value, out result);
            }
        }
    }
}
