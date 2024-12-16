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

            builder.Services.AddControllers();

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

            app.Run();
        }
    }
}