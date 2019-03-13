namespace KK.DotNet.BackgroundTasks
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System.Threading;
    using System.Threading.Tasks;
    using KK.DotNet.BackgroundTasks.Scheduled;

    public class SuperSampleTask : IScheduledTask
    {
        private readonly ILogger<SuperSampleTask> logger;

        public SuperSampleTask(
            IScheduledTaskOptions<SuperSampleTask> options,
            ILogger<SuperSampleTask> logger
        )
        {
            this.Options = options;           
            this.logger = logger;
        }

        public IScheduledTaskOptions<IScheduledTask> Options { get; }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.logger.LogDebug("Super Sample Task started");

            stoppingToken.Register(() => this.logger.LogDebug("Super Sample Task forced stopping."));

            //while (!stoppingToken.IsCancellationRequested)
            //{
            this.logger.LogDebug($"{System.DateTime.Now} Super Sample Task is running in background");

            // await Task.Delay(1000 * 5, stoppingToken);
            //}

            this.logger.LogDebug("Super Sample Task is finished");
        }
    }
}
