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

            // Add an configuration object to the DI
            // The type parameter should be the type of the target task
            services.AddSingleton<IScheduledTaskOptions<SampleTask>>(
                new ScheduledTaskOptions<SampleTask>
                {
                    // A CronTab expression, see: https://en.wikipedia.org/wiki/Cron
                    Schedule = "*/10 * * * * *",
                    // Seconds are only supported when it is set explicit
                    CronFormat = Cronos.CronFormat.IncludeSeconds
                }
            );

            services.AddSingleton<IScheduledTaskOptions<SuperSampleTask>>(
                new ScheduledTaskOptions<SuperSampleTask>
                {
                    Schedule = "*/1 * * * *"
                }
            );
            
            // Setup the Task
            services.AddScheduledTask<SampleTask>();
            services.AddScheduledTask<SuperSampleTask>();

            // Setup the Host for the scheduled tasks
            // without this the tasks will never run.
            services.AddHostedService<SchedulerHostedService>();

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
