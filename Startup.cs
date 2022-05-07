using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportingService.Interface;
using ReportingService.Service;
using System;

namespace ReportingService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private const string _customTempPathResource = "CustomTempPath";
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc();
            services.AddScoped<IForcastService, ForcastService>();
            services.AddScoped<IReportService, ReportService>();
            // Set TEMP path on current role
            string customTempPath = RoleEnvironment.GetLocalResource(_customTempPathResource).RootPath;
            Environment.SetEnvironmentVariable("TMP", customTempPath);
            Environment.SetEnvironmentVariable("TEMP", customTempPath);
            string dbconn = Configuration.GetConnectionString("DBConnection");
            //string dbconn = "Server=tcp:pftsql.database.windows.net;user=pft;password=Track@10421;Database=hangFire;Trusted_Connection=False;MultipleActiveResultSets=true";
            //string dbconn = "Server=localhost;user=root;password=admin;database=tracking_service;";
            services.AddHangfire(x => x.UseSqlServerStorage(dbconn));
            services.AddHangfireServer();

            services.AddSwaggerDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseHangfireDashboard();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

            });
        }
    }
}
