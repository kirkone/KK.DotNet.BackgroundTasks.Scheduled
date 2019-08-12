namespace KK.DotNet.BackgroundTasks.Scheduled
{
    using Cronos;

    public interface IScheduledTaskOptions<out TScheduledTask>
        where TScheduledTask : IScheduledTask
    {
        string Name { get; set;}
        string Schedule { get; set;}
        CronFormat CronFormat { get; set; }
    }
}
