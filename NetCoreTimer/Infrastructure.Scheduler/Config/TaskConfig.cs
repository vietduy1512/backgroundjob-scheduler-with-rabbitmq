using System;
using System.Threading;

namespace Infrastructure.Scheduler.Config
{
    public class TaskConfig
    {
        public string Expression { get; set; }
        public Type SchedulerType { set; get; }
        public Timer Timer { get; set; }
        public int ExecutionCount = 0;
    }
}
