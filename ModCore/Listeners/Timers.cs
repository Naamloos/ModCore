using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Humanizer;
using Microsoft.Extensions.Logging;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;
using ModCore.Logic.Extensions;

namespace ModCore.Listeners
{
	public static class Timers
	{
		[AsyncListener(EventTypes.Ready)]
		public static async Task OnReady(ModCoreShard shard, ReadyEventArgs eventargs)
		{
			using (var db = shard.Database.CreateContext())
			{
				if (!db.Timers.Any())
					return;

				var guildids = shard.Client.Guilds.Select(xg => (long)xg.Key).ToArray();
				var timers = db.Timers.Where(xt => guildids.Contains(xt.GuildId)).ToArray();
				if (!timers.Any())
					return;

				// lock timers
				await shard.SharedData.TimerSempahore.WaitAsync();
				try
				{
					var now = DateTimeOffset.UtcNow;
					var pasttimers = timers.Where(xt => xt.DispatchAt <= now.AddSeconds(30)).ToArray();
					if (pasttimers.Any())
					{
						foreach (var timer in pasttimers)
						{
							// dispatch past timers
							_ = DispatchTimer(new TimerData(null, timer, shard.Client, shard.Database, shard.SharedData, null));
						}

						db.Timers.RemoveRange(pasttimers);
						await db.SaveChangesAsync();
					}
				}
				catch (Exception ex)
				{
					shard.Client.Logger.Log(LogLevel.Error, "ModCore", 
						$"Caught Exception in Timer Ready: {ex.GetType().ToString()}\n{ex.StackTrace}", DateTime.UtcNow);
				}
				finally
				{
					// unlock the timers
					shard.SharedData.TimerSempahore.Release();
				}

				await RescheduleTimers(shard.Client, shard.Database, shard.SharedData);
			}
		}

		public static async Task TimerCallback(Task task, object state)
		{
			var timerdata = state as TimerData;
			var shard = timerdata.Context;
			var shared = timerdata.Shared;

			// lock the timers
			shared.TimerSempahore.Wait();

			try
			{
				// remove the timer
				using (var db = timerdata.Database.CreateContext())
				{
					db.Timers.Remove(timerdata.DbTimer);
					db.SaveChanges();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Caught Exception in Timer Callback: {ex.GetType().ToString()}\n{ex.StackTrace}");
			}
			finally
			{
				timerdata.Shared.TimerData = null;
				// release the lock
				shared.TimerSempahore.Release();
			}

			// dispatch the timer
			_ = Task.Run(LocalDispatch);

			// schedule new one
			await RescheduleTimers(shard, timerdata.Database, timerdata.Shared);

			Task LocalDispatch()
				=> DispatchTimer(timerdata);
		}

		private static async Task DispatchTimer(TimerData tdata)
		{
			var timer = tdata.DbTimer;
			var client = tdata.Context;
			if (timer.ActionType == TimerActionType.Reminder)
			{
				DiscordChannel channel = null;
				DiscordMessage original = null;
				try
				{
					channel = await client.GetChannelAsync((ulong)timer.ChannelId);
				}
				catch
				{
					return;
				}

				if (channel == null)
					return;

				var data = timer.GetData<TimerReminderData>();
				try 
				{
					if (data.MessageId != null)
						original = await channel.GetMessageAsync(data.MessageId.Value, true);
				}
				catch (Exception) { } // message no longer exists

				var emoji = DiscordEmoji.FromName(client, ":alarm_clock:");
				var user = (ulong)timer.UserId;

				var message = $"<@{user}>";
				var unixTimestamp = new DateTimeOffset(timer.DispatchAt);

				var msg = new DiscordMessageBuilder();

				msg.AddEmbed(
					new DiscordEmbedBuilder()
					.WithTitle("⏰ Reminder")
					.WithDescription($"You wanted to be reminded <t:{unixTimestamp.ToUnixTimeSeconds()}:R>")
					.AddField("✏️ Reminder Text", data.ReminderText)
					);

				msg.WithReply(data.MessageId, true, false);

				if (original == null)
				{
					msg.WithContent(message);
				}

				msg.WithAllowedMention(new UserMention(user));

				await msg.SendAsync(channel);

				// ALWAYS filter stuff so i set it to false. No need to @everyone in a reminder.
			}
			else if (timer.ActionType == TimerActionType.Unban)
			{
				var data = timer.GetData<TimerUnbanData>();
				if (client.Guilds.Any(x => x.Key == (ulong)timer.GuildId))
				{
					using (var db = tdata.Database.CreateContext())
					{
						var guild = client.Guilds[(ulong)timer.GuildId];
						try
						{
							await guild.UnbanMemberAsync((ulong)data.UserId);
						}
						catch
						{
							// ignored
						}

						var settings = guild.GetGuildSettings(db);

						var embed = new DiscordEmbedBuilder()
							.WithTitle($"Tempban Expired")
							.WithDescription($"{data.DisplayName}#{data.Discriminator} has been unbanned. ({data.UserId})")
							.WithColor(DiscordColor.Green);
						await guild.ModLogAsync(db, embed);
					}
				}
			}
			else if (timer.ActionType == TimerActionType.Unmute)
			{
				// DEPRECATED
				//var data = timer.GetData<TimerUnmuteData>();
				//if (client.Guilds.Any(x => x.Key == (ulong)timer.GuildId))
				//{
				//	using (var db = tdata.Database.CreateContext())
				//	{
				//		var guild = client.Guilds[(ulong)timer.GuildId];
				//		var member = await guild.GetMemberAsync((ulong)data.UserId);
				//		var role = (DiscordRole)null;
				//		try
				//		{
				//			role = guild.GetRole((ulong)data.MuteRoleId);
				//		}
				//		catch (Exception)
				//		{
				//			try
				//			{
				//				role = guild.GetRole(guild.GetGuildSettings(db).MuteRoleId);
				//			}
				//			catch (Exception)
				//			{
				//				await client.LogAutoActionAsync(guild, db, $"**[IMPORTANT]**\nFailed to unmute member: {data.DisplayName}#{data.Discriminator} (ID: {data.UserId})\nMute role does not exist!");
				//				return;
				//			}
				//		}
				//		await member.RevokeRoleAsync(role, "");
				//		await client.LogAutoActionAsync(guild, db, $"Member unmuted: {data.DisplayName}#{data.Discriminator} (ID: {data.UserId})");
				//	}
				//}
			}
			else if (timer.ActionType == TimerActionType.Pin)
			{
				var data = timer.GetData<TimerPinData>();
				if (client.Guilds.Any(x => x.Key == (ulong)timer.GuildId))
				{
					using (var db = tdata.Database.CreateContext())
					{
						var guild = client.Guilds[(ulong)timer.GuildId];
						var channel = guild.GetChannel((ulong)data.ChannelId);
						var message = await channel.GetMessageAsync((ulong)data.MessageId);
						await message.PinAsync();

						var embed = new DiscordEmbedBuilder()
								.WithTitle($"Message pin scheduled")
								.AddField("Channel", $"<#{channel.Id}>")
								.AddField("Message", $"{message.Id}")
								.AddField("Content", $"{message.Content.Truncate(1000)}")
								.WithColor(DiscordColor.Purple);
						await guild.ModLogAsync(db, embed);
					}
				}
			}
			else if (timer.ActionType == TimerActionType.Unpin)
			{
				var data = timer.GetData<TimerPinData>();
				if (client.Guilds.Any(x => x.Key == (ulong)timer.GuildId))
				{
					using (var db = tdata.Database.CreateContext())
					{
						var guild = client.Guilds[(ulong)timer.GuildId];
						var channel = guild.GetChannel((ulong)data.ChannelId);
						var message = await channel.GetMessageAsync((ulong)data.MessageId);
						await message.UnpinAsync();
						var embed = new DiscordEmbedBuilder()
								.WithTitle($"Message unpin scheduled")
								.AddField("Channel", $"<#{channel.Id}>")
								.AddField("Message", $"{message.Id}")
								.AddField("Content", $"{message.Content.Truncate(1000)}")
								.WithColor(DiscordColor.Purple);
						await guild.ModLogAsync(db, embed);
					}
				}
			}
		}

		public static async Task<TimerData> RescheduleTimers(DiscordClient client, DatabaseContextBuilder database, SharedData shared)
		{
			// lock the timers
			await shared.TimerSempahore.WaitAsync();

			var tdata = shared.TimerData;

			try
			{
				// set a new timer
				DatabaseTimer[] timers = null;
				bool force = false;

				using (var db = database.CreateContext())
				{
					var ids = client.Guilds.Select(xg => (long)xg.Key).ToArray();
					timers = db.Timers.Where(xt => ids.Contains(xt.GuildId))
						.OrderBy(xt => xt.DispatchAt)
						.ToArray();

					if (shared.TimerData != null)
						force = db.Timers.Count(xt => xt.Id == shared.TimerData.DbTimer.Id) == 0; // .Any() throws
				}

				var nearest = timers.FirstOrDefault();
				if (nearest == null)
				{
					// there's no nearest timer
					return null;
				}

				if (tdata != null && tdata.DbTimer.Id == nearest.Id)
				{
					// it's the same timer
					return tdata;
				}


				if (CancelIfLaterThan(nearest.DispatchAt, shared, force))
				{
					var cts = new CancellationTokenSource();
					var task = Task.Delay(nearest.DispatchAt - DateTimeOffset.UtcNow, cts.Token);
					tdata = new TimerData(task, nearest, client, database, shared, cts);
					_ = task.ContinueWith(TimerCallback, tdata, TaskContinuationOptions.OnlyOnRanToCompletion);
					shared.TimerData = tdata;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Caught Exception in Timer Reschedule: {ex.GetType().ToString()}\n{ex.StackTrace}");
			}
			finally
			{
				// release the lock
				shared.TimerSempahore.Release();
			}

			return tdata;
		}

		public static async Task<TimerData> UnscheduleTimerAsync(DatabaseTimer timer, DiscordClient shard, DatabaseContextBuilder database, SharedData shared)
		{
			// lock the timers
			await shared.TimerSempahore.WaitAsync();

			try
			{
				// remove the requested timer
				using (var db = database.CreateContext())
				{
					db.Timers.Remove(timer);
					await db.SaveChangesAsync();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Caught Exception in Timer Unschedule: {ex.GetType().ToString()}\n{ex.StackTrace}");
			}
			finally
			{
				// release the lock
				shared.TimerSempahore.Release();
			}

			// trigger a reschedule
			return await RescheduleTimers(shard, database, shared);
		}

		public static async Task<TimerData> UnscheduleTimersAsync(List<DatabaseTimer> timers, DiscordClient shard, DatabaseContextBuilder database, SharedData shared)
		{
			// lock the timers
			await shared.TimerSempahore.WaitAsync();

			try
			{
				// remove the requested timers
				using (var db = database.CreateContext())
				{
					db.Timers.RemoveRange(timers);
					await db.SaveChangesAsync();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Caught Exception in Timer Unschedule: {ex.GetType().ToString()}\n{ex.StackTrace}");
			}
			finally
			{
				// release the lock
				shared.TimerSempahore.Release();
			}

			// trigger a reschedule
			return await RescheduleTimers(shard, database, shared);
		}

		public static DatabaseTimer FindNearestTimer(TimerActionType actionType, ulong userId, ulong channelId, ulong guildId, DatabaseContextBuilder database)
		{
			using (var db = database.CreateContext())
				return db.Timers.FirstOrDefault(xt => xt.ActionType == actionType && xt.UserId == (long)userId && xt.ChannelId == (long)channelId && xt.GuildId == (long)guildId);
		}

		public static DatabaseTimer FindTimer(int id, TimerActionType actionType, ulong userId, DatabaseContextBuilder database)
		{
			using (var db = database.CreateContext())
				return db.Timers.FirstOrDefault(xt => xt.Id == id && xt.ActionType == actionType && xt.UserId == (long)userId);
		}

		private static bool CancelIfLaterThan(DateTimeOffset dto, SharedData shared, bool force)
		{
			// check if a timer is going
			if (force || shared.TimerData == null || shared.TimerData.DispatchTime >= dto)
			{
				var timerdata = shared.TimerData;
				timerdata?.Cancel?.Cancel();
				return true;
			}

			return false;
		}
	}
}
