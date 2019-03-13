using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KK.DotNet.BackgroundTasks.Scheduled;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KK.DotNet.BackgroundTasks.Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Begin - Scheduled Tast Setup

            // The SchedulerHostedService will be registered automaticly when registering te first task
            // If you want to use you own implementation you must register it bevore any task.
            // services.AddSingleton<IHostedService, CustomSchedulerHostedService>();

            // Add an configuration object to the DI
            // The type parameter must be the type of the target task
            // services.AddSingleton<IScheduledTaskOptions<SampleTask>>(
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
            // services.AddScheduledTaskOptions(
            //     new ScheduledTaskOptions<SampleTask>()
            //     {
            //         Schedule = "*/1 * * * *"
            //     }
            // );

            // Set options with an action
            // services.AddScheduledTaskOptions<SampleTask>( options =>
            //     {
            //         options.Schedule = "*/1 * * * *";
            //     }
            // );

            // Set options from Configuration
            // services.AddScheduledTaskOptions<SampleTask>(
            //     Configuration.GetSection("Tasks:Sample")
            // );

            services.AddScheduledTaskOptions<SampleTask>(
                Configuration.GetSection("Tasks:Sample")
            );

            // Setup the Task
            // When options provided elseware simply add the Task
            services.AddScheduledTask<SampleTask>();

            // You can also configure the task when adding it
            services.AddScheduledTask<SuperSampleTask>(options =>
                {
                    options.Schedule = "*/10 * * * * *";
                    options.CronFormat = Cronos.CronFormat.IncludeSeconds;
                }
            );

            // End - Scheduled Tast Setup
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
