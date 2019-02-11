namespace KK.AspNetCore.BackgroundTasks.Scheduled
{
    using Cronos;
    using System;

    internal class SchedulerTask
    {
        public CronExpression CronExpression { get; set; }
        public IScheduledTask Task { get; set; }

        public DateTime? LastStartTime { get; set; }
        public DateTime? NextStartTime { get; set; }

        public void Increment()
        {
            this.LastStartTime = DateTime.UtcNow;
            this.NextStartTime = this.CronExpression.GetNextOccurrence(this.LastStartTime ?? DateTime.Now);
        }

        public bool ShouldRun(DateTime currentTime)
        {            
            return this.NextStartTime < currentTime && this.LastStartTime != this.NextStartTime;
        }
    }
}
