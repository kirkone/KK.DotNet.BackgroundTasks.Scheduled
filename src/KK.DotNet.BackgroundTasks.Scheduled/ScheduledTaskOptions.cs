namespace KK.DotNet.BackgroundTasks.Scheduled
{
    using Cronos;

    public class ScheduledTaskOptions<TScheduledTask> : IScheduledTaskOptions<TScheduledTask>
        where TScheduledTask : class, IScheduledTask
    {
        public string Schedule { get; set;}
        public CronFormat CronFormat { get; set; } = CronFormat.Standard;
    }
}
