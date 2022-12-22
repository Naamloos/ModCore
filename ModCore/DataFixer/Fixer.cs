using DSharpPlus;
using DSharpPlus.Entities;
using ModCore.Database.DatabaseEntities;
using ModCore.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.DataFixer
{
    public class Fixer
    {
        private Settings settings;

        public Fixer(Settings settings)
        {
            this.settings = settings;
        }

        public async Task FixDataAsync()
        {
            Console.WriteLine("Starting fixing data");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var rest = new DiscordRestClient(new DiscordConfiguration()
            {
                Token = settings.Token,
                TokenType = TokenType.Bot
            });

            var modcoreDiscord = await rest.GetGuildAsync(709152601978961990);
            var naamloos = await modcoreDiscord.GetMemberAsync(127408598010560513);
            await naamloos.SendMessageAsync("Starting Database FixerUpperThingie. I will notify you when I'm done!");

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            var database = settings.Database.CreateContextBuilder();

            using (var context = database.CreateContext())
            {
                // Fetch ALL star data
                var starData = context.StarDatas.ToList();

                // Delete stardata for guilds that no longer exist
                List<long> deletedGuilds = new List<long>();
                List<DiscordGuild> foundGuilds = new List<DiscordGuild>();

                foreach (var guildId in starData.Select(x => x.GuildId).Distinct())
                {
                    try
                    {
                        var guild = await rest.GetGuildAsync((ulong)guildId);
                        foundGuilds.Add(guild);
                    }
                    catch (Exception)
                    {
                        deletedGuilds.Add(guildId);
                    }
                }
                var removeData = starData.Where(x => deletedGuilds.Contains(x.GuildId));
                starData.RemoveAll(x => removeData.Contains(x));
                context.StarDatas.RemoveRange(removeData);
                //await context.SaveChangesAsync();
                await naamloos.SendMessageAsync($"I deleted all stardata for guilds that have been removed! `{removeData.Count()}` datas");

                // Distinct by guild+message id, we search through guilds
                // And iterate over their channels to try and find the correct
                // channel a message exists in.
                // if message is not found, we can delete it.
                List<(DiscordGuild, IEnumerable<DiscordChannel>)> guildsWithChannels = new List<(DiscordGuild, IEnumerable<DiscordChannel>)>();
                foreach (var g in foundGuilds)
                {
                    guildsWithChannels.Add((g, await g.GetChannelsAsync()));
                }

                int fixes = 0;
                int alreadyValid = 0;
                int deleted = 0;
                int totaldeleted = 0;
                int totalfixed = 0;

                // This will loop over all distinct star messages and attempt to find
                // the correct message id.
                var distinctified = starData.DistinctBy(x => x.GuildId + "|" + x.MessageId);
                foreach (var msg in distinctified)
                {
                    var guild = guildsWithChannels.First(x => x.Item1.Id == (ulong)msg.GuildId);

                    long correctChannel = 0;

                    try
                    {
                        await rest.GetMessageAsync((ulong)msg.ChannelId, (ulong)msg.MessageId);
                        correctChannel = msg.ChannelId;
                        alreadyValid++;
                        continue;
                    }
                    catch (Exception) { }

                    // channel ID is not valid so we'll try iterating
                    if (correctChannel == 0)
                    {
                        foreach (var channel in guild.Item2)
                        {
                            try
                            {
                                await rest.GetMessageAsync(channel.Id, (ulong)msg.MessageId);
                                correctChannel = (long)channel.Id;
                                fixes++;
                                break;
                            }
                            catch (Exception) { }
                        }
                    }

                    if (correctChannel == 0)
                    {
                        // nothing we can do. delete this.
                        deleted++;
                        var deletion = context.StarDatas.Where(x => x.GuildId == msg.GuildId && x.MessageId == msg.MessageId).ToList();
                        totaldeleted += deletion.Count();
                        context.StarDatas.RemoveRange(deletion);
                    }
                    else
                    {
                        // update all same stars
                        var update = context.StarDatas.Where(x => x.GuildId == msg.GuildId && x.MessageId == msg.MessageId).ToList();
                        foreach (var u in update)
                        {
                            u.ChannelId = correctChannel;
                        }
                        totalfixed += update.Count();
                        context.StarDatas.UpdateRange(update);
                    }
                    await context.SaveChangesAsync();
                }

                stopwatch.Stop();
                await naamloos.SendMessageAsync($"I managed to fix up {fixes} broken channel refs, {alreadyValid} were already valid. " +
                    $"{deleted} no longer existed. This took: `{stopwatch.Elapsed}`.\n\n" +
                    $"In total, I modified `{totalfixed}` datas and deleted `{totaldeleted}` datas.");

                Console.WriteLine("buh bai");
            }
        }
    }
}
