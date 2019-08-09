namespace KK.DotNet.BackgroundTasks.Scheduled
{
    using Cronos;
    using System;

    public class SchedulerTask
    {


        public string Name  { get; internal set; }
        public CronExpression CronExpression { get; internal set; }
        public IScheduledTask Task { get; internal set; }

        public DateTime? LastStartTime { get; internal set; }
        public DateTime? NextStartTime { get; internal set; }

        internal void Increment()
        {
            this.LastStartTime = DateTime.UtcNow;
            this.NextStartTime = this.CronExpression.GetNextOccurrence(this.LastStartTime ?? DateTime.Now);
        }

        internal bool ShouldRun(DateTime currentTime)
            => this.NextStartTime < currentTime && this.LastStartTime != this.NextStartTime;
    }
}
