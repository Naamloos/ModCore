using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record Embed
    {
        [JsonPropertyName("title")]
        public Optional<string> Title { get; set; }

        [JsonPropertyName("type")]
        public Optional<string> Type { get; set; }

        [JsonPropertyName("description")]
        public Optional<string> Description { get; set; }

        [JsonPropertyName("url")]
        public Optional<string> Url { get; set; }

        [JsonPropertyName("timestamp")]
        public Optional<DateTimeOffset> Timestamp { get; set; }

        [JsonPropertyName("color")]
        public Optional<int> Color { get; set; }

        [JsonPropertyName("footer")]
        public Optional<EmbedFooter> Footer { get; set; }

        [JsonPropertyName("image")]
        public Optional<EmbedImage> Image { get; set; }

        [JsonPropertyName("thumbnail")]
        public Optional<EmbedThumbnail> Thumbnail { get; set; }

        [JsonPropertyName("video")]
        public Optional<EmbedVideo> Video { get; set; }

        [JsonPropertyName("provider")]
        public Optional<EmbedProvider> Provider { get; set; }

        [JsonPropertyName("author")]
        public Optional<EmbedAuthor> Author { get; set; }

        [JsonPropertyName("fields")]
        public Optional<List<EmbedField>> Fields { get; set; }

        public Embed WithTitle(string title)
        {
            this.Title = title;
            return this;
        }

        public Embed WithDescription(string description)
        {
            this.Description = description;
            return this;
        }

        public Embed WithUrl(string url)
        {
            this.Url = url;
            return this;
        }

        public Embed WithTimestamp(DateTimeOffset timestamp)
        {
            this.Timestamp = timestamp;
            return this;
        }

        public Embed WithColor(int color)
        {
            this.Color = color;
            return this;
        }

        public Embed WithFooter(string text, string iconUrl = null)
        {
            this.Footer = new EmbedFooter()
            {
                IconUrl = iconUrl == null ? Optional<string>.None : iconUrl,
                Text = text
            };
            return this;
        }

        public Embed WithImage(string imageUrl)
        {
            this.Image = new EmbedImage()
            {
                Url = imageUrl
            };
            return this;
        }

        public Embed WithThumbnail(string thumbnailUrl)
        {
            this.Thumbnail = new EmbedThumbnail()
            {
                Url = thumbnailUrl
            };
            return this;
        }

        public Embed WithAuthor(string name, string? url = null, string? iconUrl = null)
        {
            this.Author = new EmbedAuthor()
            {
                IconUrl = iconUrl == null ? Optional<string>.None : iconUrl,
                Url = url == null ? Optional<string>.None : Url,
                Name = name
            };
            return this;
        }

        public Embed WithField(string title, string content, bool inline = false)
        {
            if (!this.Fields.HasValue)
            {
                this.Fields = new List<EmbedField>();
            }

            Fields.Value.Add(new()
            {
                Name = title,
                Value = content,
                Inline = inline
            });

            return this;
        }

        public Embed ClearFields()
        {
            this.Fields = Optional<List<EmbedField>>.None;
            return this;
        }
    }
}