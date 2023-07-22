using System.Reflection;

namespace LogMonitorService.Models.Responses
{
    public class HealthCheckResponse
    {
        public HealthCheckResponse(string status)
        {
            this.Version = typeof(Program).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            this.Status = status;
        }

        public string Version { get; }
        public string Status { get; }
    }
}
