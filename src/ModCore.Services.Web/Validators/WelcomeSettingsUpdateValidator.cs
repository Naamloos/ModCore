using ModCore.Common.Discord.Entities.Messages;
using ModCore.Services.Web.RequestBodies;

namespace ModCore.Services.Web.Validators
{
    public class WelcomeSettingsUpdateValidator : IValidator<WelcomeSettingsUpdate>
    {
        public async Task<ValidatorResult<WelcomeSettingsUpdate>> ValidateAsync(HttpContext httpContext, WelcomeSettingsUpdate obj)
        {
            var embeds = obj.MessagePayload.Embeds;
            var content = obj.MessagePayload.Content;

            if (content.HasValue && content.Value.Length > 2000)
            {
                return new ValidatorResult<WelcomeSettingsUpdate>(false, obj, "Content is too long");
            }

            return new ValidatorResult<WelcomeSettingsUpdate>(true, obj, null);
        }
    }
}
