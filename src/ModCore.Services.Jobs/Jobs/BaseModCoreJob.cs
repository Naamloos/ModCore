using ModCore.Services.Jobs.Attributes;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Services.Jobs.Jobs
{
    public abstract class BaseModCoreJob : IJob
    {
        public abstract Task Execute(IJobExecutionContext context);
    }
}
