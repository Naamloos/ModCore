using System.Text.Json;
using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Components;

namespace ModCore.Common.Discord.Entities.Serializer
{
    public sealed class ComponentJsonSerializer : JsonConverter<Component>
    {
        public override Component? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            using var document = JsonDocument.ParseValue(ref reader);
            var root = document.RootElement;
            var type = root.GetProperty("type").GetInt32() switch
            {
                1 => typeof(ActionRow),
                2 => typeof(Button),
                3 => typeof(StringSelect),
                4 => typeof(TextInput),
                5 => typeof(UserSelect),
                6 => typeof(RoleSelect),
                7 => typeof(MentionableSelect),
                8 => typeof(ChannelSelect),
                9 => typeof(Section),
                10 => typeof(TextDisplay),
                11 => typeof(Thumbnail),
                12 => typeof(MediaGallery),
                13 => typeof(Components.File),
                14 => typeof(Separator),
                17 => typeof(Container),
                18 => typeof(Label),
                19 => typeof(FileUpload),
                21 => typeof(RadioGroup),
                22 => typeof(CheckboxGroup),
                23 => typeof(Checkbox),
                _ => typeof(UnknownComponent),
            };
            return (Component?)root.Deserialize(type, options);
        }

        public override void Write(
            Utf8JsonWriter writer,
            Component value,
            JsonSerializerOptions options
        )
        {
            if (value.GetType() != typeof(Component))
            {
                JsonSerializer.Serialize(writer, value, value.GetType(), options);
                return;
            }
            writer.WriteStartObject();
            writer.WriteNumber("type", (int)value.Type);
            if (value.Id.HasValue)
                writer.WriteNumber("id", value.Id.Value);
            foreach (var property in value.AdditionalData)
            {
                writer.WritePropertyName(property.Key);
                property.Value.WriteTo(writer);
            }
            writer.WriteEndObject();
        }
    }
}
