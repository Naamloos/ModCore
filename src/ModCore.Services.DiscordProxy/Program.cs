using ModCore.Services.DiscordProxy.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpClient("discord", client =>
{
    client.BaseAddress = new Uri("https://discord.com/api/");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("ModCore_Proxy/3.0");
    client.Timeout = TimeSpan.FromSeconds(60); // If Discord doesn't respond within 60 seconds, something is very wrong, so we should timeout instead of hanging indefinitely.
});

builder.Services.AddSingleton<DiscordRateLimiter>();
builder.Services.AddScoped<DiscordProxyService>();

builder.Services.AddLogging();

builder.WebHost.UseUrls("http://0.0.0.0:8085");

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
