using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Responses
{
    public record OAuth2TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; private set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; private set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; private set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; private set; }

        [JsonPropertyName("scope")]
        public string Scope { get; private set; }
    }
}
