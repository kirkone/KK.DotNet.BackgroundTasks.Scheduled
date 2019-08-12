namespace KK.DotNet.BackgroundTasks.Scheduled.Sample.Web.Pages
{
    using System.Threading.Tasks;
    using KK.DotNet.BackgroundTasks.Scheduled.Sample.Web.Backgroundservices;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;

    public class IndexModel : PageModel
    {
        private readonly ILogger<RuntimeSampleTask> runtimeSampleTaskLogger;

        public Scheduler Scheduler { get; }


        [BindProperty]
        public ScheduledTaskOptions<RuntimeSampleTask> Options { get; set; }

        // When using the default SchedulerHostedService than there is also a Scheduler
        // this can be used to get access to the scheduled task list for example
        public IndexModel(
                Scheduler scheduler,
                ILogger<RuntimeSampleTask> runtimeSampleTaskLogger
            )
        {
            this.Scheduler = scheduler;
            this.runtimeSampleTaskLogger = runtimeSampleTaskLogger;
            this.Options = new ScheduledTaskOptions<RuntimeSampleTask>()
            {
                Name = "Awesome Runntime Task",
                Schedule = "*/15 * * * *"
            };
        }

        public void OnGet()
        {
            //this.Scheduler.SchedulerTasks;
        }

        public IActionResult OnPost()
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            this.Scheduler.AddTask(new RuntimeSampleTask(this.Options, this.runtimeSampleTaskLogger));

            return this.Page();
        }
    }
}
