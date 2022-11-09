using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Database;
using ModCore.Database.JsonEntities;
using ModCore.Extensions;
using ModCore.Extensions.Abstractions;
using ModCore.Extensions.Attributes;
using ModCore.Modals;
using ModCore.Utils.Extensions;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Components
{
    public class RoleMenuConfigComponents : BaseComponentModule
    {
        private DatabaseContextBuilder database;

        public RoleMenuConfigComponents(DatabaseContextBuilder database)
        {
            this.database = database;
        }

        [Component("rolemenu", ComponentType.Button)]
        public async Task DisplayRoleMenuAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> context)
        {
            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Loading Role Menu...").AsEphemeral(true));

            using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();
                var menu = settings.RoleMenus.FirstOrDefault(x => x.Name == context["n"]);

                if (menu != null)
                {
                    var roles = e.Guild.Roles.Values.Where(x => menu.RoleIds.Contains(x.Id));
                    var options = new List<DiscordSelectComponentOption>();
                    options.Add(new DiscordSelectComponentOption("Clear roles", "clear", "Clears your roles", emoji: new DiscordComponentEmoji("🗑")));
                    foreach (var role in roles)
                    {
                        options.Add(new DiscordSelectComponentOption(role.Name, role.Id.ToString()));
                    }

                    var customId = ExtensionStatics.GenerateIdString("rm.use", new Dictionary<string, string>()
                    {
                        {"n", menu.Name }
                    });


                    await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"📖 Role Menu {context["n"]}")
                        .AddComponents(new DiscordSelectComponent(customId, "Select roles...", options, maxOptions: options.Count)));
                    return;
                }
                await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"⛔ This Role menu does no longer exist! Notify a server admin that this does not work!"));
            }
        }

        [Component("rm.use", ComponentType.StringSelect)]
        public async Task UseRoleMenuAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> context)
        {
            var values = e.Interaction.Data.Values.Where(x => x != "clear").Select(x => ulong.Parse(x));
            var roles = e.Guild.Roles.Values.Where(x => values.Contains(x.Id));
            var clear = e.Interaction.Data.Values.Any(x => x == "clear");

            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();
                var menu = settings.RoleMenus.FirstOrDefault(x => x.Name == context["n"]);

                if (menu != null)
                {
                    var member = e.User as DiscordMember;

                    if (!clear)
                    {
                        if (roles.Any(x => !menu.RoleIds.Contains(x.Id)))
                        {
                            // somehow user did an invalid selection
                            return;
                        }

                        foreach (var role in roles)
                        {
                            await member.GrantRoleAsync(role);
                        }

                        await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Granted you the selected roles!"));
                    }
                    else
                    {
                        var removeRoles = e.Guild.Roles.Values.Where(x => menu.RoleIds.Contains(x.Id)).Where(x => member.Roles.Any(y => y.Id == x.Id));
                        foreach (var role in removeRoles)
                        {
                            await member.RevokeRoleAsync(role, "ModCore rolemenu");
                            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Removed all roles you had from this menu!"));
                        }
                    }
                }
            }
        }

        [ComponentPermissions(Permissions.ManageGuild)]
        [Component("rm.setroles", ComponentType.RoleSelect)]
        public async Task SetRolesAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> context)
        {
            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));
            var roles = e.Interaction.Data.Resolved.Roles.Values;

            using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.RoleMenus.Add(new GuildRoleMenu()
                {
                    CreatorId = e.User.Id,
                    Name = context["n"],
                    RoleIds = roles.Select(x => x.Id).ToList()
                });

                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 Created role menu with name {context["n"]}! Use `/rolemenu` to post it in a channel!")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "rm", "Back to Role Menu config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        [ComponentPermissions(Permissions.ManageGuild)]
        [Component("rm.show", ComponentType.StringSelect)]
        public async Task ShowMenuAsync(ComponentInteractionCreateEventArgs e)
        {
            var value = e.Interaction.Data.Values.First();
            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();
                var menu = settings.RoleMenus.FirstOrDefault(x => x.Name == value);

                if (menu != null)
                {
                    var embed = new DiscordEmbedBuilder()
                        .WithTitle($"📖 Roles for menu {value}")
                        .WithDescription(string.Join(", ", menu.RoleIds.Select(x => $"<@&{x}>")));

                    await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed)
                        .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "rm", "Back to Role Menu config", emoji: new DiscordComponentEmoji("🏃"))));
                }
            }
        }

        [ComponentPermissions(Permissions.ManageGuild)]
        [Component("rm.delete", ComponentType.StringSelect)]
        public async Task DeleteMenuAsync(ComponentInteractionCreateEventArgs e)
        {
            var value = e.Interaction.Data.Values.First();
            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.RoleMenus.RemoveAll(x => x.Name == value);
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 Removed Role Menu with name {value}! Existing menus with this ID will no longer be useable.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "rm", "Back to Role Menu config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        [Component("rm.post", ComponentType.StringSelect)]
        public async Task PostRoleMenuAsync(ComponentInteractionCreateEventArgs e)
        {
            await Client.GetInteractionExtension().RespondWithModalAsync<PostRoleMenuModal>(e.Interaction, "Post RoleMenu...", new Dictionary<string, string>()
            {
                {"n", e.Interaction.Data.Values[0] }
            });
        }

        [ComponentPermissions(Permissions.ManageGuild)]
        [Component("rm.create", ComponentType.Button)]
        public async Task CreateRoleMenuAsync(ComponentInteractionCreateEventArgs e)
        {
            await Client.GetInteractionExtension().RespondWithModalAsync<RoleMenuNameModal>(e.Interaction, "Choose a name for your new role menu...");
        }

        public static async Task PostMenuAsync(DiscordInteraction interaction, InteractionResponseType responseType, DatabaseContext db)
        {
            using (db)
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)interaction.Guild.Id);
                var settings = guild.GetSettings();

                settings.RoleMenus = settings.RoleMenus.GroupBy(x => x.Name).Select(x => x.First()).ToList();

                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();

                var menuList = string.Join(", ", settings.RoleMenus.Select(x => x.Name));

                var embed = new DiscordEmbedBuilder()
                    .WithTitle("📖 RoleMenu Configuration")
                    .WithDescription("Role menus allow members to select roles from a simple list. **Do note that it is generally a bad idea to have the same roles in different menus!**")
                    .AddField("Available menus", string.IsNullOrEmpty(menuList) ? "No menus available." : menuList);

                var enableId = ExtensionStatics.GenerateIdString("sb.toggle", new Dictionary<string, string>() { { "on", "true" } });
                var disableId = ExtensionStatics.GenerateIdString("sb.toggle", new Dictionary<string, string>() { { "on", "false" } });

                var deleteOptions = new List<DiscordSelectComponentOption>();
                foreach (var option in settings.RoleMenus)
                    deleteOptions.Add(new DiscordSelectComponentOption($"Delete: {option.Name}", option.Name, "This action is not recoverable!"));

                var showOptions = new List<DiscordSelectComponentOption>();
                foreach (var option in settings.RoleMenus)
                    showOptions.Add(new DiscordSelectComponentOption($"{option.Name}", option.Name));

                var response = new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed)
                    .WithContent("")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Success, "rm.create", "Create new menu..."));

                if (settings.RoleMenus.Any())
                {
                    response.AddComponents(new DiscordSelectComponent("rm.post", "Post menu in this channel...", showOptions));
                    response.AddComponents(new DiscordSelectComponent("rm.show", "Show info about menu...", showOptions));
                    response.AddComponents(new DiscordSelectComponent("rm.delete", "Delete menu...", deleteOptions));
                }

                response.AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "cfg", "Back to Config", emoji: new DiscordComponentEmoji("🏃")));
                await interaction.CreateResponseAsync(responseType, response);
            }
        }
    }
}
