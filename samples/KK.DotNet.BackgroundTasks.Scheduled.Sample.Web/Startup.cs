namespace KK.DotNet.BackgroundTasks.Scheduled.Sample.Web
{
    using KK.DotNet.BackgroundTasks.Scheduled.Sample.Web.Backgroundservices;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public Startup(IConfiguration configuration) => this.Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _ = services.Configure<CookiePolicyOptions>(options =>
              {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                  options.MinimumSameSitePolicy = SameSiteMode.None;
              });


            _ = services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Begin - Scheduled Tast Setup

            // The SchedulerHostedService will be registered automaticly when registering te first task
            // If you want to use you own implementation you must register it bevore any task.
            // services.AddSingleton<IHostedService, CustomSchedulerHostedService>();

            // Add an configuration object to the DI
            // The type parameter must be the type of the target task
            // _ = services.AddSingleton<IScheduledTaskOptions<SampleTask>>(
            //     new ScheduledTaskOptions<SampleTask>()
            //     {
            //         // A CronTab expression, see: https://en.wikipedia.org/wiki/Cron
            //         Schedule = "*/10 * * * * *",
            //         // Seconds are only supported when it is set explicit
            //         CronFormat = Cronos.CronFormat.IncludeSeconds
            //     }
            // );

            // Or simply use the extension methods
            // Set options from an object
            // _ = services.AddScheduledTaskOptions(
            //     new ScheduledTaskOptions<SampleTask>()
            //     {
            //         Schedule = "*/1 * * * *"
            //     }
            // );

            // Set options with an action
            // _ = services.AddScheduledTaskOptions<SampleTask>( options =>
            //     {
            //         options.Schedule = "*/1 * * * *";
            //     }
            // );

            // Set options from Configuration
            // _ = services.AddScheduledTaskOptions<SampleTask>(
            //     this.Configuration.GetSection("Tasks:Sample")
            // );

            _ = services.AddScheduledTaskOptions<SampleTask>(
                this.Configuration.GetSection("Tasks:Sample")
            );

            // Setup the Task
            // When options provided elseware simply add the Task
            _ = services.AddScheduledTask<SampleTask>();

            // You can also configure the task when adding it
            _ = services.AddScheduledTask<SuperSampleTask>(options =>
                {
                    options.Schedule = "*/10 * * * * *";
                    options.CronFormat = Cronos.CronFormat.IncludeSeconds;
                }
            );

            // Setup the host for the scheduled tasks
            // without this the tasks will never run.
            // This will also register a Scheduler in the DI.
            // Have a look at `Index.cshtml.cs` how to use it.
            _ = services.AddScheduler();

            // If a custom implemented scheduler should be used use this kind of registration
            // CustomSchedulerHostedService must implement ISchedulerHostedService
            // services.AddHostedService<CustomSchedulerHostedService>();

            // End - Scheduled Tast Setup
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                _ = app.UseDeveloperExceptionPage();
            }
            else
            {
                _ = app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                _ = app.UseHsts();
            }

            _ = app.UseHttpsRedirection();
            _ = app.UseStaticFiles();
            _ = app.UseCookiePolicy();

            _ = app.UseMvc();
        }
    }
}
