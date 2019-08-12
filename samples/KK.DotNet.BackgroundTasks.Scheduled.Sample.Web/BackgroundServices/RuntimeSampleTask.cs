namespace KK.DotNet.BackgroundTasks.Scheduled.Sample.Web.Backgroundservices
{
    using Microsoft.Extensions.Logging;
    using System.Threading;
    using System.Threading.Tasks;
    using KK.DotNet.BackgroundTasks.Scheduled;

    public class RuntimeSampleTask : IScheduledTask
    {
        private readonly ILogger<RuntimeSampleTask> logger;

        public RuntimeSampleTask(
            IScheduledTaskOptions<RuntimeSampleTask> options,
            ILogger<RuntimeSampleTask> logger
        )
        {
            this.Options = options;
            this.logger = logger;
        }

        public IScheduledTaskOptions<IScheduledTask> Options { get; }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.logger.LogDebug("Runtime Sample Task started");

            _ = stoppingToken.Register(() => this.logger.LogDebug("Runtime Sample Task forced stopping."));

            var runCount = 0;
            while (runCount < 5)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    // Task was killed from outside.
                    this.logger.LogDebug($"{System.DateTime.Now} Runtime Sample Task cancellation requested!");
                    return;
                }

                this.logger.LogDebug($"{System.DateTime.Now} Runtime Sample Task loop {runCount}");
                await Task.Delay(1000, stoppingToken);
                runCount++;
            }

            this.logger.LogDebug("Runtime Sample Task is finished");
        }
    }
}
