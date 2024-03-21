using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Services.Jobs.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModCoreJobAttribute : Attribute
    {
        public string JobName { get; set; }
        public string CronTrigger { get; set; }

        public ModCoreJobAttribute(string jobName, string cronTrigger) 
        {
            JobName = jobName;
            CronTrigger = cronTrigger;
        }
    }
}
