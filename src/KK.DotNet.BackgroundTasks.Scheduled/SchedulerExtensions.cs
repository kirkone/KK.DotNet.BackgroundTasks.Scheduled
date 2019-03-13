namespace KK.DotNet.BackgroundTasks.Scheduled
{
    using Microsoft.Extensions.DependencyInjection;

    public static class SchedulerExtensions
    {
        public static IServiceCollection AddScheduledTask<TScheduledTask>(this IServiceCollection services)
            where TScheduledTask : class, IScheduledTask
        {
            return services.AddSingleton<IScheduledTask, TScheduledTask>();
        }
    }
}
