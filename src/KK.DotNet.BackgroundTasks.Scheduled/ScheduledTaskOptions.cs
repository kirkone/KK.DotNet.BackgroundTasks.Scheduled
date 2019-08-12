namespace KK.DotNet.BackgroundTasks.Scheduled
{
    using System;
    using Cronos;

    public class ScheduledTaskOptions<TScheduledTask> : IScheduledTaskOptions<TScheduledTask>
        where TScheduledTask : class, IScheduledTask
    {
        public string Name { get; set; } = Guid.NewGuid().ToString();
        public string Schedule { get; set;}
        public CronFormat CronFormat { get; set; } = CronFormat.Standard;
    }
}
