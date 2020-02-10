using Cronos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppWithScheduler.Code.Scheduling
{
    public class SchedulerTaskWrapper
    {
        public CronExpression Schedule { get; set; }
        public IScheduledTask Task { get; set; }

        public DateTime LastRunTime { get; set; }
        public DateTime NextRunTime { get; set; }

        public void Increment()
        {
            LastRunTime = NextRunTime;
            NextRunTime = Schedule.GetNextOccurrence(NextRunTime).Value;
        }

        public bool ShouldRun(DateTime currentTime)
        {
            return NextRunTime < currentTime && LastRunTime != NextRunTime;
        }
    }
}
