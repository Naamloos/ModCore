using DSharpPlus;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ModCore.Entities
{
    public class ModCoreEmojis
    {
        [JsonIgnore]
        public DiscordEmoji JumpLink
        {
            get
            {
                return DiscordEmoji.FromGuildEmote(_client, _jumplink);
            }
        }
        [JsonProperty("jumplink")]
        private ulong _jumplink = 577900935913930792;

        [JsonIgnore]
        private DiscordClient _client = null;

        private ModCoreEmojis()
        {

        }

        /// <summary>
        /// Loads emojis for this instance of ModCore.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static ModCoreEmojis LoadEmojis(DiscordClient client)
        {
            if (!File.Exists("emojis.json"))
            {
                var json = JsonConvert.SerializeObject(new Settings(), Formatting.Indented);
                File.WriteAllText("emojis.json", json, new UTF8Encoding(false));
                Console.WriteLine("emojis file was not found, a new one was generated.");
            }

            var input = File.ReadAllText("emojis.json", new UTF8Encoding(false));
            var emojis = JsonConvert.DeserializeObject<ModCoreEmojis>(input);

            // update with possible new fields.
            File.WriteAllText("emojis.json", JsonConvert.SerializeObject(emojis), new UTF8Encoding(false));

            emojis._client = client;

            return emojis;
        }
    }
}
