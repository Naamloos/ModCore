using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;

namespace ModCore.Logic
{
    public class AugmentedBoolConverter : IArgumentConverter<bool>
    {
        public async Task<Optional<bool>> ConvertAsync(string value, CommandContext ctx)
        {
            await Task.Delay(0);
            bool result = false;
            bool parses = true;
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
                    break;
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
                    break;
                default:
                    parses = bool.TryParse(value, out result);
                    break;
            }
            if (parses)
                return new Optional<bool>(result);
            else
                return new Optional<bool>();
        }

        public bool TryConvert(string value, CommandContext ctx, out bool result)
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
