using Microsoft.AspNetCore.Mvc;
using ModCore.CoreApi.Entities;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;

namespace ModCore.Web.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private ModCore core;

        public ApiController(CoreContainer cont)
        {
            this.core = cont.mcore;
        }

        public string Index()
        {
            return "ModCore API";
        }

        [HttpGet("token/{token}")]
        public bool Token(string token)
        {
            // allow cross-domain shit for w/e reason
            Response.Headers.Add("Access-Control-Allow-Origin", "*");

            return core.SharedData.ApiToken == token;
        }

        // GET ping
        [HttpGet("ping")]
        public string Ping()
        {
            return "pong!";
        }

        [HttpGet("default")]
        public string Dp()
        {
            return "Default prefix: " + core.SharedData.DefaultPrefix;
        }

        [HttpGet("roles/{gid}")]
        public string Roles(ulong gid)
        {
            // Check token.
            var h = Request.Headers["Token"];
            if (string.IsNullOrEmpty(h) || core.SharedData.ApiToken != h)
            {
                this.Unauthorized();
                return null;
            }

            var arl = new ApiRoleList();
            var guildsfound = core.Shards.SelectMany(x => x.Client.Guilds.Values.Where(g => g.Id == gid)).ToArray();
            if (guildsfound.Length > 0)
            {
                foreach (var r in guildsfound[0].Roles)
                {
                    arl.Roles.Add(new ApiRole { RoleId = r.Key, RoleName = r.Value.Name });
                }
                Response.ContentType = "application/json";
                return JsonConvert.SerializeObject(arl);
            }
            NotFound();
            return null;
        }

        [HttpGet("emptyconfig")]
        public JObject EmptyConfig()
        {
            // allow cross-domain shit for w/e reason
            Response.Headers.Add("Access-Control-Allow-Origin", "*");

            Response.ContentType = "application/json";
            return JObject.FromObject(new GuildSettings() { Prefix = "data served by ModCore." });
        }

        [HttpPost("webhook/{userid}/{channelid}/{usertoken}")]
        public async Task WebhookAsync(ulong userid, ulong channelid, string usertoken, [FromBody]ApiWebhookBody request)
        {
            Console.WriteLine("seems to request fine");
            if (core.Shards.Any(x => x.Client.Guilds.Any(y => y.Value.Channels.ContainsKey(channelid))))
            {
                // Shard with a guild with this channel exists.
                var shard = core.Shards.First(x => x.Client.Guilds.Any(y => y.Value.Channels.ContainsKey(channelid)));

                // Get guild
                var guild = shard.Client.Guilds.First(x => x.Value.Channels.ContainsKey(channelid)).Value;

                // Check whether user is in guild
                if (!guild.Members.ContainsKey(userid))
                {
                    Unauthorized();
                    return;
                }
                // Check token for user
                using (var ctx = shard.Database.CreateContext())
                {
                    if (ctx.UserDatas.Any(x => x.UserId == (long)userid))
                    {
                        var ud = ctx.UserDatas.First(x => x.UserId == (long)userid);
                        var data = ud.GetData();

                        // token incorrect for this user id, returning unauthorized
                        // or no token present but user data exists
                        if (string.IsNullOrEmpty(data.Token) || data.Token != usertoken)
                        {
                            Unauthorized();
                            return;
                        }
                    }
                    else
                    {
                        // no token present, returning unauthorized
                        Unauthorized();
                        return;
                    }
                }

                // Check whether this user can manage channels and manage webhooks
                var channel = guild.Channels[channelid];

                // long ass statement
                if(!channel.PermissionsFor(guild.Members[userid]).HasPermission(Permissions.ManageChannels | Permissions.ManageWebhooks))
                {
                    Unauthorized();
                    return;
                }

                // All checks are OK, now read the body
                ApiWebhookBody webhookbody = request;

                Console.WriteLine(webhookbody.ActionType);
                switch (webhookbody.ActionType)
                {
                    default:
                        BadRequest();
                        return;

                    case "command":
                        // handle command webhook
                        var commandparams = webhookbody.ActionParams.ToObject<CommandActionParams>();

                        var usr = await shard.Client.GetUserAsync(userid);

                        string fullcmd = commandparams.CommandName + " " + commandparams.Arguments;

                        var cmd = shard.Commands.FindCommand(fullcmd, out var args);

                        var prefix = core.Settings.DefaultPrefix;

                        using (var ctx = shard.Database.CreateContext())
                        {
                            var gstng = guild.GetGuildSettings(ctx);

                            if (gstng != null) // ugly syntax suck my ass :^)
                                if (!string.IsNullOrEmpty(gstng.Prefix))
                                {
                                    prefix = gstng.Prefix;
                                }
                        }

                        var context = shard.Commands.CreateFakeContext(usr, channel,
                            fullcmd, prefix, cmd, args);

                        try
                        {
                            await shard.Commands.ExecuteCommandAsync(context).ConfigureAwait(false);
                        }
                        catch (Exception)
                        {
                            BadRequest();
                            return;
                        }

                        break;

                    case "message":
                        // handle message webhook
                        var messageparams = webhookbody.ActionParams.ToObject<MessageActionParams>();
                        Console.WriteLine($"Sending {messageparams.Content}");
                        await channel.SendMessageAsync(messageparams.Content);
                        break;
                }
                Ok();
                return;
            }

            Unauthorized();
        }
    }
}
