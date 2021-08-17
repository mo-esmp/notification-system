using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApi.Extensions;
using WebApi.Infrastructure;
using WebApi.Infrastructure.BackgroundJobs;

namespace WebApi
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
            services.AddControllers();

            services.AddJwtAuthentication(Configuration);

            services.AddWebApiVersioning();

            services.AddSwagger();

            services.AddHangfireServices(Configuration);

            services.AddNotificationSystem(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime appLifetime, IApiVersionDescriptionProvider provider)
        {
            app.UseApiExceptionHandling();

            app.UseSwagger(options => { options.RouteTemplate = "api-docs/{documentName}/docs.json"; });

            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "api-docs";
                foreach (var description in provider.ApiVersionDescriptions)
                    options.SwaggerEndpoint($"/api-docs/{description.GroupName}/docs.json", description.GroupName.ToUpperInvariant());
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard("/hangfire", new DashboardOptions
                {
                    Authorization = new[] { new HangfireAuthorizationFilter() }
                });
            });

            appLifetime.ApplicationStarted.Register(() => RecurringJobs(app));
        }

        private void RecurringJobs(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var jobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
            var jobHandler = scope.ServiceProvider.GetRequiredService<IBackgroundJobHandler>();
            jobManager.AddOrUpdate("BirthdayJob", () => jobHandler.HandleBirthDayNotificationJobAsync(), Cron.Daily(9, 30));
        }
    }
}