using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModCore.Common.Configuration;
using StackExchange.Redis;
using System.Security.Cryptography;
using System.Text;

namespace ModCore.Common.Cache
{
    public static class GatewayIdentifyLimiter
    {
        public static async Task WaitAsync(IServiceProvider services, int shardId, int maxConcurrency, CancellationToken cancellationToken)
        {
            var configuration = services.GetRequiredService<IConfiguration>();
            var token = configuration.GetRequiredSection(ConfigurationHelper.GetConfigKeyString(ConfigKey.DiscordToken)).Value!;
            var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
            var key = $"ModCore:gateway:identify:{tokenHash}:{shardId % maxConcurrency}";
            var database = services.GetRequiredService<IConnectionMultiplexer>().GetDatabase();
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (await database.StringSetAsync(key, "1", TimeSpan.FromMilliseconds(5500), when: When.NotExists).WaitAsync(cancellationToken))
                    return;
                await Task.Delay(250, cancellationToken);
            }
        }
    }
}
