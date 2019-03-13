namespace KK.DotNet.BackgroundTasks
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System.Threading;
    using System.Threading.Tasks;
    using KK.DotNet.BackgroundTasks.Scheduled;

    public class SampleTask : IScheduledTask
    {
        private readonly ILogger<SampleTask> logger;

        public SampleTask(
            IScheduledTaskOptions<SampleTask> options,
            ILogger<SampleTask> logger
        )
        {
            this.Options = options;           
            this.logger = logger;
        }

        public IScheduledTaskOptions<IScheduledTask> Options { get; }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.logger.LogDebug("Sample Task started");

            stoppingToken.Register(() => this.logger.LogDebug("Sample Task forced stopping."));

            //while (!stoppingToken.IsCancellationRequested)
            //{
            this.logger.LogDebug($"{System.DateTime.Now} Sample Task is running in background");

            // await Task.Delay(1000 * 5, stoppingToken);
            //}

            this.logger.LogDebug("Sample Task is finished");
        }
    }
}
