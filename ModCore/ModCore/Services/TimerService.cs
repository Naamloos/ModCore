﻿using DSharpPlus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ModCore.Services
{
    public class TimerService
    {
        private ILogger logger;
        private DatabaseContext database;
        private TimerEvent nextTimer = null;
        private DiscordClient client;
        private CancellationTokenSource cts;

        public TimerService(ILogger<TimerService> logger, DatabaseService databaseService, DiscordClient client)
        {
            this.logger = logger;
            this.database = databaseService.GetDatabase();
            this.client = client;
            this.cts = new CancellationTokenSource();
        }

        public async Task Start(CancellationToken stoppingToken)
        {
            logger.LogInformation("Started timer service.");
            var now = DateTimeOffset.Now;
            var expired = database.TimerEvents.Where(x => x.Dispatch < now);

            if (expired.Count() > 0)
            {
                var lexpired = expired.ToList();
                foreach (var timer in lexpired)
                {
                    await DispatchEvent(timer);
                }
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                if (nextTimer == null)
                {
                    await Task.Delay(1500);
                    NextTimer();
                }
                else
                {
                    // TODO add behavior for newer event trigger.
                    var timeUntilDispatch = nextTimer.Dispatch.Subtract(DateTimeOffset.Now).TotalMilliseconds;
                    logger.LogInformation($"Found new timer to wait for... waiting {timeUntilDispatch} milliseconds.");
                    if (timeUntilDispatch > 0)
                        try
                        {
                            await Task.Delay((int)timeUntilDispatch, cts.Token);
                        }
                        catch (Exception ex) { }

                    if (!cts.IsCancellationRequested && !stoppingToken.IsCancellationRequested)
                        await DispatchEvent(nextTimer);

                    cts = new CancellationTokenSource();
                    NextTimer();
                }
            }
        }

        private void NextTimer()
        {
            if (database.TimerEvents.Count() > 0)
            {
                var smallest = database.TimerEvents.Min(x => x.Dispatch);
                nextTimer = database.TimerEvents.First(x => x.Dispatch == smallest);
            }
        }

        public void Enqueue(TimerEvent timer)
        {
            var dbtimer = database.TimerEvents.Add(timer);
            database.SaveChanges();

            if (nextTimer != null && timer.Dispatch.ToUnixTimeMilliseconds() < nextTimer.Dispatch.ToUnixTimeMilliseconds())
                cts.Cancel();
        }

        private async Task DispatchEvent(TimerEvent timer)
        {
            logger.LogInformation("timer dispatched.");
            database.TimerEvents.Remove(timer);
            database.SaveChanges();
            // TODO: just create a new Task?
            switch (timer.Type)
            {
                case TimerType.Reminder:
                default:
                    _ = Task.Run(() => sendReminder(timer));
                    break;

                case TimerType.Unban:
                    _ = Task.Run(() => unbanUser(timer));
                    break;
            }
        }

        private async Task sendReminder(TimerEvent timer)
        {
            var channel = await client.GetChannelAsync((ulong)timer.ChannelId);
            var user = await client.GetUserAsync((ulong)timer.UserId);
            await channel.SendMessageAsync($"\u23f0 **{user.Mention}** Reminder from <t:{timer.Creation.ToUnixTimeSeconds()}:R>:\n{timer.Message.Unmention()}");
        }

        private async Task unbanUser(TimerEvent timer)
        {
            var guild = await client.GetGuildAsync((ulong)timer.GuildId);
            var user = await client.GetUserAsync((ulong)timer.UserId);
            await guild.UnbanMemberAsync(user);

            // TODO Log expired tempban
        }
    }
}
