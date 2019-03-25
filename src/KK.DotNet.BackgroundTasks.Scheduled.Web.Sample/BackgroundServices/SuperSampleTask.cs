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

            this.logger.LogDebug($"{System.DateTime.Now} Super Sample Task is running in background");

            var runCount = 0;
            while (runCount < 5)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    // Task was killed from outside.
                    this.logger.LogDebug($"{System.DateTime.Now} Super Sample Task cancellation requested!");
                    return;
                }

                this.logger.LogDebug($"{System.DateTime.Now} Super Sample Task loop {runCount}");
                await Task.Delay(1000, stoppingToken);
                runCount++;
            }

            this.logger.LogDebug("Super Sample Task is finished");
        }
    }
}
