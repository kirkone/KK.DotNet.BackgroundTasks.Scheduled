namespace KK.DotNet.BackgroundTasks.Scheduled
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Cronos;
    using Microsoft.Extensions.Logging;

    public class Scheduler
    {
        private readonly ILogger<Scheduler> logger;
        private readonly List<SchedulerTask> schedulerTasks = new List<SchedulerTask>();
        public ReadOnlyCollection<SchedulerTask> Tasks => this.schedulerTasks.AsReadOnly();

        public Scheduler(
           ILogger<Scheduler> logger
        ) => this.logger = logger;

        public event EventHandler TaskAdded;
        protected virtual void OnTaskAdded() => TaskAdded?.Invoke(this, EventArgs.Empty);

        public void AddTask(IScheduledTask scheduledTask)
        {
            var taskName = string.IsNullOrWhiteSpace(scheduledTask.Options.Name) ? Guid.NewGuid().ToString() : scheduledTask.Options.Name;

            this.logger.LogDebug($"Adding Task: {scheduledTask.Options.Name}");

            this.schedulerTasks.Add(new SchedulerTask
            {
                Name = taskName,
                CronExpression = CronExpression.Parse(
                    expression: scheduledTask.Options.Schedule,
                    format: scheduledTask.Options.CronFormat
                ),
                Task = scheduledTask,
                NextStartTime = DateTime.UtcNow
            });

            this.OnTaskAdded();
        }
    }
}
