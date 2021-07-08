using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Services
{
    public class BotMetaService
    {
        public DateTimeOffset StartTime { get; set; } = DateTime.Now;

        public DateTimeOffset SocketStartTime { get; set; } = DateTime.Now;
    }
}
