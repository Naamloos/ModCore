using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.Discord.Gateway.Events;
using ModCore.Common.Utils;

namespace ModCore.Common.Discord.Gateway.EventData.Incoming
{
    [JsonConverter(typeof(MessageUpdateJsonSerializer))]
    public record MessageUpdate : Message, IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }

        [JsonPropertyName("member")]
        public Member Member { get; set; } = default!;

        [JsonIgnore]
        public JsonElement RawData { get; internal set; }

        public bool HasField(string name) =>
            RawData.ValueKind == JsonValueKind.Object && RawData.TryGetProperty(name, out _);

        public Message ApplyTo(Message previous, JsonSerializerOptions? options = null)
        {
            if (RawData.ValueKind != JsonValueKind.Object)
                throw new InvalidOperationException("This update has no received payload.");
            options ??= JsonSerializerOptionsFactory.GetOptions();
            var message = JsonSerializer.SerializeToNode(previous, options)!.AsObject();
            foreach (var field in RawData.EnumerateObject())
                message[field.Name] = JsonNode.Parse(field.Value.GetRawText());
            return message.Deserialize<Message>(options)!;
        }
    }

    public sealed class MessageUpdateJsonSerializer : JsonConverter<MessageUpdate>
    {
        private static readonly PropertyInfo[] messageProperties = typeof(Message)
            .GetProperties()
            .Where(property => property.CanRead && property.CanWrite)
            .ToArray();

        public override MessageUpdate Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            using var document = JsonDocument.ParseValue(ref reader);
            var data = document.RootElement;
            var message = data.Deserialize<Message>(options)!;
            var update = new MessageUpdate { RawData = data.Clone() };
            foreach (var property in messageProperties)
                property.SetValue(update, property.GetValue(message));
            if (data.TryGetProperty("guild_id", out var guild))
                update.GuildId = guild.Deserialize<Snowflake>(options);
            if (data.TryGetProperty("member", out var member))
                update.Member = member.Deserialize<Member>(options)!;
            return update;
        }

        public override void Write(
            Utf8JsonWriter writer,
            MessageUpdate value,
            JsonSerializerOptions options
        )
        {
            if (value.RawData.ValueKind == JsonValueKind.Object)
            {
                value.RawData.WriteTo(writer);
                return;
            }
            var data = JsonSerializer.SerializeToNode<Message>(value, options)!.AsObject();
            if (value.GuildId.HasValue)
                data["guild_id"] = JsonSerializer.SerializeToNode(value.GuildId.Value, options);
            if (value.Member != null)
                data["member"] = JsonSerializer.SerializeToNode(value.Member, options);
            data.WriteTo(writer, options);
        }
    }
}
