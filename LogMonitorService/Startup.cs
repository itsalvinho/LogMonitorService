using LogMonitorService.Models.Configuration;
using LogMonitorService.Services;
using LogMonitorService.Services.Abstractions;

namespace LogMonitorService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            AppConfig appConfig = Configuration.GetSection("AppConfig").Get<AppConfig>();
            if (appConfig == null) throw new Exception("AppConfig must be provided in appsettings.json");

            // Add services for configuring the application
            services.AddSingleton<AppConfig>(appConfig);
            
            // Add services for retrieval of data
            services.AddSingleton<ILogReaderService, DefaultLogReaderService>();

            // Add services for controllers
            services.AddSingleton<ILogsControllerService, LogsControllerService>();

            // Add controllers for APIs
            services.AddControllers();
            services.AddApiVersioning(opt =>
            {
                opt.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                opt.AssumeDefaultVersionWhenUnspecified = true;
            });
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
