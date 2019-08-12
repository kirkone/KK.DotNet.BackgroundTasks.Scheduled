namespace KK.DotNet.BackgroundTasks.Scheduled
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal class SchedulerHostedService : BackgroundService, ISchedulerHostedService
    {
        private readonly ILogger logger;
        private Task executingTask;
        private CancellationTokenSource ctsService;
        private CancellationTokenSource ctsDelay;

        public event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException;

        private readonly Scheduler scheduler;

        public SchedulerHostedService(
            IEnumerable<IScheduledTask> scheduledTasks,
            Scheduler scheduler,
            ILogger<SchedulerHostedService> logger
        )
        {
            this.logger = logger;
            this.scheduler = scheduler;

            this.scheduler.TaskAdded += this.InterruptDelay;

            foreach (var scheduledTask in scheduledTasks)
            {
                this.scheduler.AddTask(scheduledTask);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogDebug("Scheduler Hosted Service is starting.");

            // Create a linked token so we can trigger cancellation outside of this token's cancellation
            this.ctsService = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Store the task we're executing
            this.executingTask = this.ExecuteAsync(this.ctsService.Token);

            // If the task is completed then return it, otherwise it's running
            return this.executingTask.IsCompleted ? this.executingTask : Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogDebug("Scheduler Hosted Service is stopping.");

            // Stop called without start
            if (this.executingTask == null)
            {
                return;
            }

            // Signal cancellation to the executing method
            this.ctsService.Cancel();

            // Wait until the task completes or the stop token triggers
            _ = await Task.WhenAny(this.executingTask, Task.Delay(-1, cancellationToken));

            // Throw if cancellation triggered
            cancellationToken.ThrowIfCancellationRequested();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await this.ExecuteOnceAsync(cancellationToken);
            }
        }

        private void InterruptDelay(object sender, EventArgs e) => this.ctsDelay?.Cancel();

        private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(TaskScheduler.Current);

            // [1] 00:00:00
            // [2] 00:01:01
            var referenceTime = DateTime.UtcNow;

            var tasksThatShouldRun = this.scheduler.Tasks
                .Where(t => t.ShouldRun(referenceTime))
                .ToList();

            foreach (var taskThatShouldRun in tasksThatShouldRun)
            {
                _ = await taskFactory.StartNew(
                    async () =>
                    {
                        Thread.CurrentThread.Name = taskThatShouldRun.Name;
                        try
                        {
                            await taskThatShouldRun.Task.ExecuteAsync(cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            var args = new UnobservedTaskExceptionEventArgs(
                                ex as AggregateException ?? new AggregateException(ex));

                            this.UnobservedTaskException?.Invoke(this, args);

                            if (!args.Observed)
                            {
                                throw;
                            }
                        }
                    },
                    cancellationToken);
                // [1] 00:01:00
                // [2] 00:02:00
                taskThatShouldRun.Increment();
            }

            // [1] 00:01:00 < 00:00:00 == false (super)
            // [2] 00:02:00 < 00:01:01 == false (super)
            var nextTask = this.scheduler.Tasks
                .Where(t => !t.ShouldRun(referenceTime))
                .OrderBy(d => d.NextStartTime)
                .First();

            // Important: here we use the nullable value direct because nextTasks contains a NextRunTime.
            // See the where with the ShouldRun above
            if (nextTask.NextStartTime != null)
            {
                this.ctsDelay = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                try
                {
                    await Task.Delay(
                        nextTask.NextStartTime.Value.Subtract(referenceTime),
                        this.ctsDelay.Token
                    );
                    // [1] 00:01:00
                    // [2] 00:02:00

                }
                catch (OperationCanceledException)
                {
                    this.logger.LogDebug("Task delay interrupted on purpose.");
                }
            }
            else
            {
                //This code will never be reached.
            }

            this.ctsDelay.Dispose();
            this.ctsDelay = null;
        }
    }
}
