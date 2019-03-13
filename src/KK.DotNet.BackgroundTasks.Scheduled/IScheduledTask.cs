namespace KK.DotNet.BackgroundTasks.Scheduled
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IScheduledTask
    {
        IScheduledTaskOptions<IScheduledTask> Options { get; }

        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
