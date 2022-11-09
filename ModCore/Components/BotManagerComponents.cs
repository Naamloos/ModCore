using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.VisualBasic;
using ModCore.Extensions;
using ModCore.Extensions.Abstractions;
using ModCore.Extensions.Attributes;
using ModCore.Modals;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Components
{
    public class BotManagerComponents : BaseComponentModule
    {
        [Component("fb", ComponentType.Button)]
        public async Task RespondFeedbackAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> context)
        {
            if (!Client.CurrentApplication.Owners.Any(x => x.Id == e.User.Id))
                return;

            if (ulong.TryParse(context["u"], out _) && ulong.TryParse(context["g"], out _))
            {
                await Client.GetInteractionExtension().RespondWithModalAsync<FeedbackResponseModal>(e.Interaction, "Response to feedback.",
                    new Dictionary<string, string>()
                    {
                        { "g", context["g"] },
                        { "u", context["u"] }
                    });
            }
        }
    }
}
