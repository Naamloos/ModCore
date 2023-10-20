﻿using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModCore.Extensions
{
    public static class ExtensionStatics
    {
        public static InteractionExtension UseInteractions(this DiscordClient client, IServiceProvider services)
        {
            var extension = new InteractionExtension(services);
            client.AddExtension(extension);
            return extension;
        }

        public static InteractionExtension GetInteractionExtension(this DiscordClient client)
            => client.GetExtension<InteractionExtension>();

        public static AsyncListenerExtension UseAsyncListeners(this DiscordClient client, IServiceProvider services)
        {
            var extension = new AsyncListenerExtension(services);
            client.AddExtension(extension);
            return extension;
        }

        public static AsyncListenerExtension GetAsyncListenerExtension(this DiscordClient client)
            => client.GetExtension<AsyncListenerExtension>();

        public static string GenerateIdString(string Id, IDictionary<string, string> values)
        {
            if (values == null)
                return Id;

            List<string> data = new List<string> { Id };

            foreach (var val in values)
            {
                data.Add($"{HttpUtility.UrlEncode(val.Key)}={HttpUtility.UrlEncode(val.Value)}");
            }

            return string.Join(' ', data);
        }

        public static (string Id, Dictionary<string, string> Values) DecipherIdString(string input)
        {
            IEnumerable<string> data = input.Split(' ').ToList();
            var id = data.First();
            data = data.Skip(1);

            var values = new Dictionary<string, string>();
            foreach (var val in data)
            {
                var split = val.Split('=');
                values.Add(HttpUtility.UrlDecode(split[0]), HttpUtility.UrlDecode(split[1]));
            }

            return (id, values);
        }
    }
}
