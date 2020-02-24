using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Infrastructure.Scheduler.Config
{
    public class SchedulerBuilder
    {
        protected readonly IServiceCollection _services;
        private readonly Dictionary<Type, Func<SchedulerConfig, TaskConfig>> _taskBuilderInfos;

        public SchedulerBuilder(IServiceCollection services, Dictionary<Type, Func<SchedulerConfig, TaskConfig>> taskBuilderInfos)
        {
            _services = services;
            _taskBuilderInfos = taskBuilderInfos;
        }

        public SchedulerBuilder AddTask<TTask>(Func<SchedulerConfig, TaskConfig> taskConfigFunc)
        {
            _taskBuilderInfos.Add(typeof(TTask), taskConfigFunc);
            return new SchedulerBuilder(_services, _taskBuilderInfos);
        }

        public IServiceCollection Build(IConfiguration configuration)
        {
            var config = configuration.GetSection(nameof(SchedulerConfig)).Get<SchedulerConfig>();

            var taskConfigInfos = new List<TaskConfig>();
            foreach (var taskBuilder in _taskBuilderInfos)
            {
                var taskConfig = taskBuilder.Value(config);
                taskConfig.SchedulerType = taskBuilder.Key;
                taskConfigInfos.Add(taskConfig);

                _services.AddScoped(taskBuilder.Key);
            }
            _services.AddSingleton(taskConfigInfos);
            _services.AddHostedService<SchedulerHostedService>();

            return _services;
        }
    }
}
