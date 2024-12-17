using InertiaCore;
using InertiaCore.Extensions;
using ModCore.Common.SettingsHelper;
using ModCore.Services.Web.Middleware;
using System.Diagnostics;
using ModCore.Common.Discord.Rest;
using ModCore.Common.Database;
using ModCore.Services.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Globalization;
using ModCore.Common.Discord.Entities.Serializer;
using System.Text.Json.Serialization;
using System.Text.Json;
using ModCore.Common.Cache;

namespace ModCore.Common.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var settings = SettingsHelper.SettingsHelper.EnsureSettingsExist();

            builder.Configuration.AddJsonFile("settings.json");
            builder.Configuration.AddEnvironmentVariables();

            var jsonOptions = new JsonSerializerOptions(JsonSerializerOptions.Default)
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                Converters = { new OptionalJsonSerializerFactory() },
                WriteIndented = true
            };

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                    options.JsonSerializerOptions.Converters.Add(new OptionalJsonSerializerFactory());
                    options.JsonSerializerOptions.WriteIndented = true;
                });

            builder.Services.AddInertia();

            builder.Services.AddViteHelper(options =>
            {
                options.PublicDirectory = "wwwroot";
                options.BuildDirectory = "build";
                options.ManifestFilename = "manifest.json";
            });

            builder.Services.AddDiscordRest(config => { });
            builder.Services.AddDbContext<DatabaseContext>();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddTransient<UserDiscordRest>();

            builder.Services.AddSession();

            builder.Services.AddModcoreCacheService();

            builder.Services.AddSingleton(jsonOptions);
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "Discord";
            })
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                })
                .AddDiscord(options =>
                {
                    options.ClientId = builder.Configuration["discord_client_id"];
                    options.ClientSecret = builder.Configuration["discord_client_secret"];
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.SaveTokens = true;
                    options.ClaimActions.MapCustomJson("urn:discord:avatar:url", user =>
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "https://cdn.discordapp.com/avatars/{0}/{1}.{2}",
                            user.GetString("id"),
                            user.GetString("avatar"),
                            user.GetString("avatar").StartsWith("a_") ? "gif" : "png"));
                    options.Prompt = "none";
                    options.ClaimActions.MapJsonKey("urn:discord:id", "id");

                    options.Scope.Add("email"); // Could possibly be used in the future for email notifications?
                    options.Scope.Add("identify"); // Identify current user in dashboard
                    options.Scope.Add("guilds"); // List guilds for user
                    options.Scope.Add("guilds.join"); // For support guild, User can easily join :3
                    options.Scope.Add("guilds.members.read"); // List guild members, for configuration purposes.
                });

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();

            app.UseInertia();

            app.MapControllers();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<InertiaPropsMiddleware>();
            app.UseMiddleware<AttributeMiddleware>();

            app.Run();
        }
    }
}