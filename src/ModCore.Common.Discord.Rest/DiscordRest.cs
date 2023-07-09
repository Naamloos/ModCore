﻿using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Serializer;
using ModCore.Common.Discord.Rest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Rest
{
    public class DiscordRest
    {
        private DiscordRestConfiguration Configuration;
        private RatelimitedRest RatelimitedRest;
        private JsonSerializerOptions JsonSerializerOptions;

        public DiscordRest(Action<DiscordRestConfiguration> configure, IServiceProvider services)
        {
            Configuration = new DiscordRestConfiguration();
            configure(Configuration);
            JsonSerializerOptions = new JsonSerializerOptions();
            JsonSerializerOptions.Converters.Add(new SnowflakeJsonSerializer());
            RatelimitedRest = new RatelimitedRest(Configuration, JsonSerializerOptions);
        }

        public async Task<User> GetCurrentUserAsync()
        {
            string route = "users/:user";
            string url = "users/@me";
            
            HttpResponseMessage response = await RatelimitedRest.RequestAsync(HttpMethod.Get, route, url);

            return await JsonSerializer.DeserializeAsync<User>(await response.Content.ReadAsStreamAsync(), JsonSerializerOptions);
        }

        public async Task<User> GetUserAsync(Snowflake snowflake)
        {
            string route = "users/:user";
            string url = $"users/{snowflake}";

            HttpResponseMessage response = await RatelimitedRest.RequestAsync(HttpMethod.Get, route, url);

            return await JsonSerializer.DeserializeAsync<User>(await response.Content.ReadAsStreamAsync(), JsonSerializerOptions);
        }
    }
}
