using ModCore.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ModCore.Api
{
    /// <summary>
    /// This class handles all stats for the bot lists ModCore is a part of.
    /// </summary>
    public class BotLists
    {
        // Hello. If you're one of the devs of these bot lists,
        // Your apis suck. I don't care enough to properly implement them
        // OR use your official libs. so here ya go. have fun with my requests.
        // Also the lib I DID use broke my update to .net core 3.0. HOW?!
        private Settings _botsettings;
        private HttpClient _httpclient;
        public BotLists(Settings s)
        {
            this._botsettings = s;
            this._httpclient = new HttpClient();
        }

        public async Task UpdateStatsAsync(int guildcount, int shardcount)
        {
            // top dot gg bad api
            if (_botsettings.BotListOrgEnable)
            {
                using (var _http = new HttpRequestMessage())
                {
                    var json = new JObject();
                    json.Add("server_count", guildcount);
                    json.Add("shard_count", shardcount);
                    var content = new StringContent(json.ToString());
                    _http.Headers.Add("Authorization", _botsettings.DblToken);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    _http.Content = content;
                    _http.Method = HttpMethod.Post;
                    _http.RequestUri = new Uri($"https://top.gg/api/bots/{_botsettings.BotId}/stats");

                    await _httpclient.SendAsync(_http);
                }
            }

            // bots dot gg less bad api but worse site
            if (_botsettings.BotListPwEnable)
            {
                using (var _http = new HttpRequestMessage())
                {
                    var json = new JObject();
                    json.Add("guildCount", guildcount);
                    json.Add("shardCount", shardcount);
                    var content = new StringContent(json.ToString());
                    _http.Headers.Add("Authorization", _botsettings.BotDiscordPlToken);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    _http.Content = content;
                    _http.Method = HttpMethod.Post;
                    _http.RequestUri = new Uri($"https://discord.bots.gg/api/v1/bots/{_botsettings.BotId}/stats");

                    await _httpclient.SendAsync(_http);
                }
            }
        }

        private bool _running = false;
        /// <summary>
        /// Starts a Task that keeps running over the course of the bots life and updates stats every 12h
        /// </summary>
        /// <returns></returns>
        public void StartBotStatUpdater(SharedData SD)
        {
            if (!_running)
            {
                _running = true;

                Task.Run(async () =>
                {
                    // Send new stats at launch
                    await Task.Delay(TimeSpan.FromSeconds(15));
                    await this.UpdateStatsAsync(SD.ModCore.Shards.SelectMany(x => x.Client.Guilds.Values).Count(), SD.ModCore.Shards.Count());

                    // Loop to send stats every 12 hours.
                    while (true)
                    {
                        await Task.Delay(TimeSpan.FromHours(12));
                        await this.UpdateStatsAsync(SD.ModCore.Shards.SelectMany(x => x.Client.Guilds.Values).Count(), SD.ModCore.Shards.Count());
                    }
                }, SD.CTS.Token);
            }
        }
    }
}
