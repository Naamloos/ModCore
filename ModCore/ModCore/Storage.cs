using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

// This is overengineering:tm:

namespace ModCore
{
    public class StorageBuilder
    {
        private string basePath { get; set; }

        private ulong? guildId { get; set; }

        private ulong? userId { get; set; }

        private ulong? channelId { get; set; }

        public StorageBuilder()
        {
            basePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "storage");
        }

        public StorageBuilder ForGuild(ulong id)
        {
            guildId = id;
            return this;
        }

        public StorageBuilder ForUser(ulong id)
        {
            userId = id;
            return this;
        }

        public StorageBuilder ForChannel(ulong id)
        {
            channelId = id;
            return this;
        }

        public Storage Build()
        {
            string path = basePath;

            if (channelId.HasValue)
                path = Path.Combine(path, "channels", channelId.Value.ToString());
            else if (guildId.HasValue)
                path = Path.Combine(path, "guilds", guildId.Value.ToString());
            if (guildId.HasValue)
                path = Path.Combine(path, "users", userId.Value.ToString());

            return new Storage(path);
        }
    }

    public class Storage
    {
        private string path;

        public Storage(string path)
        {
            if (!path.StartsWith(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "storage")))
                throw new InvalidOperationException("Storage class doesn't point to ModCore storage!!!");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            this.path = path;
        }

        public string GetPath()
        {
            return path;
        }
    }
}
