using ModCore.Services.Jobs.Attributes;
using ModCore.Services.Jobs.Jobs;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Services.Jobs
{
    public class ModCoreJobBuilder
    {
        private IScheduler scheduler;

        public ModCoreJobBuilder(IScheduler scheduler) 
        {
            this.scheduler = scheduler;    
        }

        public void BuildAndScheduleJobs()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsAssignableTo(typeof(BaseModCoreJob)) && x != typeof(BaseModCoreJob));

            foreach(var type in types)
            {
                var attr = type.GetCustomAttribute<ModCoreJobAttribute>();
                if(attr is null)
                {
                    throw new NotImplementedException($"Unimplemented job! {type.FullName}");
                }

                var job = JobBuilder.Create(type)
                    .WithIdentity(attr.JobName)
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity(attr.JobName + "-trigger")
                    .WithCronSchedule(attr.CronTrigger)
                    .StartNow()
                    .Build();

                scheduler.ScheduleJob(job, trigger);
            }
        }
    }
}
