using ModCore.Common.Database;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Gateway;
using ModCore.Common.Discord.Rest;
using ModCore.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Services.Shard.Modules.Eval.Entities
{
    public class EvalModel
    {
        public Gateway Gateway { get; set; }
        public DiscordRest Rest { get; set; }
        public MessageCreate Context { get; set; }
        public DatabaseContext Database { get; set; }

        public EvalModel(Gateway gateway, DiscordRest rest, MessageCreate context, TransientService<DatabaseContext> database)
        {
            Gateway = gateway;
            Rest = rest;
            Context = context;
            Database = database.GetTransient();
        }
    }
}
