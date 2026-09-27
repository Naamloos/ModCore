using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Interactions
{
    [JsonDerivedType(typeof(InteractionMessageResponse))]
    [JsonDerivedType(typeof(InteractionMessageData))]
    [JsonDerivedType(typeof(InteractionAutocompleteData))]
    [JsonDerivedType(typeof(InteractionModalData))]
    public abstract record InteractionResponseData { }
}
