using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ModCore.Integrations
{
    public class PronounDB
    {
        private HttpClient _client;

        public PronounDB()
        {
            this._client = new HttpClient();
            _client.BaseAddress = new Uri("https://pronoundb.org/api/v1/");
        }

        public async Task<string> GetPronounsForDiscordUserAsync(ulong id)
        {
            var response = await _client.GetAsync($"lookup?platform=discord&id={id}");
            var responseObject = JsonObject.Parse(await response.Content.ReadAsStringAsync());

            return _pronounMapping[responseObject["pronouns"].GetValue<string>()];
        }

        // thankies uwu https://github.com/Captain8771/PronounDBLib/blob/master/PronounDBLib/PronounDBClient.cs#L20-L40
        private static readonly IReadOnlyDictionary<string, string> _pronounMapping = new Dictionary<string, string>()
        {
            { "unspecified", "Unspecified" },
            { "hh", "He/Him" },
            { "hi", "He/It" },
            { "hs", "He/She" },
            { "ht", "He/They" },
            { "ih", "It/Him" },
            { "ii", "It/Its" },
            { "is", "It/She" },
            { "it", "It/They" },
            { "shh", "She/He" },
            { "sh", "She/Her" },
            { "si", "She/It" },
            { "st", "She/They" },
            { "th", "They/He" },
            { "ti", "They/It" },
            { "ts", "They/She" },
            { "tt", "They/Them" },
            { "any", "Any Pronouns" },
            { "other", "Other Pronouns" },
            { "ask", "Ask Me My Pronouns" },
            { "avoid", "Avoid Pronouns, Use My Name" }
        }.AsReadOnly();
    }
}
