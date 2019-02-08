namespace KK.AspNetCore.BackgroundTasks.Scheduled
{
    using Cronos;

    public interface IScheduledTaskOptions<out TScheduledTask>
        where TScheduledTask : IScheduledTask
    {      
        string Schedule { get; set;}
        CronFormat CronFormat { get; set; }  
    }    
}