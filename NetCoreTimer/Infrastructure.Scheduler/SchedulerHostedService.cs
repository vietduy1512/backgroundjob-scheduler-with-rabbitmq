using Cronos;
using Infrastructure.Scheduler.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Scheduler
{

    public class SchedulerHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<SchedulerHostedService> _logger;
        private readonly IEnumerable<TaskConfig> _taskConfigs;
        private readonly IServiceProvider _serviceProvider;

        bool disposed = false;

        public SchedulerHostedService(ILogger<SchedulerHostedService> logger, List<TaskConfig> taskConfigs, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _taskConfigs = taskConfigs;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handle Scheduler to Execute tasks.");

            foreach (var taskConfig in _taskConfigs)
            {
                var currentTime = DateTime.UtcNow;
                var timespan = CronExpression.Parse(taskConfig.Expression).GetNextOccurrence(currentTime) - currentTime;

                taskConfig.Timer = new Timer(async (state) =>
                {
                    var count = Interlocked.Increment(ref taskConfig.ExecutionCount);

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var scheduler = scope.ServiceProvider.GetService(taskConfig.SchedulerType) as IScheduledTask;
                        await scheduler.ExecuteAsync(cancellationToken);
                    }
                    _logger.LogInformation(
                        "Scheduler Hosted Service is working. Count: {Count}", count);
                },
                null, TimeSpan.Zero, timespan.Value);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Scheduler Hosted Service is stopping.");

            foreach (var taskConfig in _taskConfigs)
            {
                taskConfig.Timer?.Change(Timeout.Infinite, 0);
            }
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                foreach (var taskConfig in _taskConfigs)
                {
                    taskConfig.Timer?.Dispose();
                }
            }

            disposed = true;
        }
    }
}
