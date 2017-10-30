using System;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;
using System.IO;
using System.Runtime.InteropServices;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Net.WebSocket;

namespace ModCore
{
    public class ModCoreShard
    {
        public int ShardId { get; private set; }
        public StartTimes StartTimes { get; private set; }

        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public SharedData ShardData { get; set; }
        internal Settings Settings { get; }

        public DatabaseContextBuilder Database { get; }
        
        public ModCoreShard(Settings settings, int id, SharedData sharedData)
        {
            this.Settings = settings;
            this.ShardData = sharedData;
            Database = settings.Database.CreateContextBuilder();
        }

        internal void Initialize()
        {
            // Store the Start Times to use in DI
            // SocketStartTime will be updated in the SocketOpened event,
            // For now we just need to make sure its not null.
            StartTimes = new StartTimes(ShardData.ProcessStartTime, ShardData.ProcessStartTime);

            // Initialize the DiscordClient
            this.Client = new DiscordClient(new DiscordConfiguration
            {
                AutoReconnect = true,
                GatewayCompressionLevel = GatewayCompressionLevel.Stream,
                LargeThreshold = 250,
                LogLevel = LogLevel.Debug,
                Token = Settings.Token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                ShardCount = this.Settings.ShardCount,
                ShardId = this.ShardId
            });
            #if ModCore_is_Windows7
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Environment.OSVersion.Version <= new Version(6, 1, 7601, 65536))
            {
                // NT 6.1 (Win7 SP1)
                Client.SetWebSocketClient<WebSocket4NetCoreClient>();
            }
            #endif

            Client.ClientErrored += async args =>
            {
                await Task.Yield();
                Console.WriteLine(args.Exception);
            };

            this.Interactivity = Client.UseInteractivity(new InteractivityConfiguration()
            {
                PaginationBehaviour = TimeoutBehaviour.Delete,
                PaginationTimeout = TimeSpan.FromSeconds(30),
                Timeout = TimeSpan.FromSeconds(30)
            });

            // Add the instances we need to dependencies
            var deps = new DependencyCollectionBuilder()
                .AddInstance(this.ShardData)
                //.AddInstance(this.Settings)
                .AddInstance(this.Interactivity)
                .AddInstance(this.StartTimes)
                .AddInstance(this.Database)
                .Build();

            // enable commandsnext
            this.Commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                CaseSensitive = false,
                EnableDefaultHelp = true,
                EnableDms = false,
                EnableMentionPrefix = true,
                CustomPrefixPredicate = this.GetPrefixPositionAsync,
                Dependencies = deps
            });

            // register commands
            this.Commands.RegisterCommands(Assembly.GetExecutingAssembly());

            // Update the SocketStartTime
            this.Client.SocketOpened += async () =>
            {
                await Task.Yield();
                StartTimes.SocketStartTime = DateTime.Now;
            };

            // register event handlers
            this.Client.Ready += Client_Ready;
            this.Commands.CommandErrored += Commands_CommandErrored;

            AsyncListenerHandler.InstallListeners(Client, this);
        }

        private Task Client_Ready(ReadyEventArgs e)
        {
            Client.UpdateStatusAsync(new DiscordGame($"I'm on {this.Settings.ShardCount} shard(s)!"));
            return Task.Delay(0);
        }

        private static Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            return Task.Run(async () =>
            {
                e.Context.Client.DebugLogger.LogMessage(LogLevel.Critical, "Commands", e.Exception.ToString(), DateTime.Now);

                if (e.Exception is CommandNotFoundException)
                    return;

                var cfg = e.Context.GetGuildSettings() ?? new GuildSettings();
                var ce = cfg.CommandError;
                var ctx = e.Context;

                switch (ce.Chat)
                {
                    default:
                    case CommandErrorVerbosity.None:
                        break;

                    case CommandErrorVerbosity.Name:
                        await ctx.RespondAsync($"**Command {e.Command.QualifiedName} {e.Command.Arguments} Errored!**\n`{e.Exception.GetType()}`");
                        break;
                    case CommandErrorVerbosity.NameDesc:
                        await ctx.RespondAsync($"**Command {e.Command.QualifiedName} {e.Command.Arguments} Errored!**\n`{e.Exception.GetType()}`:\n{e.Exception.Message}");
                        break;
                    case CommandErrorVerbosity.Exception:
                        MemoryStream stream = new MemoryStream();
                        StreamWriter writer = new StreamWriter(stream);
                        writer.Write(e.Exception.ToString());
                        writer.Flush();
                        stream.Position = 0;
                        await ctx.RespondWithFileAsync(stream, "exception.txt", $"**Command {e.Command.QualifiedName} {e.Command.Arguments} Errored!**\n`{e.Exception.GetType()}`:\n{e.Exception.Message}");
                        break;
                }

                if (cfg.ActionLog.Enable)
                {
                    switch (ce.ActionLog)
                    {
                        default:
                        case CommandErrorVerbosity.None:
                            break;

                        case CommandErrorVerbosity.Name:
                            await ctx.LogMessageAsync($"**Command {e.Command.QualifiedName} {e.Command.Arguments} Errored!**\n`{e.Exception.GetType()}`");
                            break;
                        case CommandErrorVerbosity.NameDesc:
                            await ctx.LogMessageAsync($"**Command {e.Command.QualifiedName} {e.Command.Arguments} Errored!**\n`{e.Exception.GetType()}`:\n{e.Exception.Message}");
                            break;
                        case CommandErrorVerbosity.Exception:
                            var st = e.Exception.StackTrace;

                            st = st.Length > 1000 ? st.Substring(0, 1000) : st;
                            var b = new DiscordEmbedBuilder().WithDescription(st);
                            await ctx.LogMessageAsync($"**Command {e.Command.QualifiedName} {e.Command.Arguments} Errored!**\n`{e.Exception.GetType()}`:\n{e.Exception.Message}", b);
                            break;
                    }
                }

                return;
            });
        }

        public Task RunAsync() =>
            this.Client.ConnectAsync();

        internal async Task DisconnectAndDispose()
        {
            await this.Client.DisconnectAsync();
            this.Client.Dispose();
        }

        public Task<int> GetPrefixPositionAsync(DiscordMessage msg)
        {
            GuildSettings cfg = null;
            using (var db = Database.CreateContext())
                cfg = msg.Channel.Guild.GetGuildSettings(db);
            if (cfg?.Prefix != null)
                return Task.FromResult(msg.GetStringPrefixLength(cfg.Prefix));

            return Task.FromResult(msg.GetStringPrefixLength(Settings.DefaultPrefix));
        }
    }
}
