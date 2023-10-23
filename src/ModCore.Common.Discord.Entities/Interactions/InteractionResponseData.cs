using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Interactions
{
    [JsonDerivedType(typeof(InteractionMessageResponse))]
    public abstract record InteractionResponseData
    {
    }
}