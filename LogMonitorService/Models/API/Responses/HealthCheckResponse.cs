using System.Reflection;

namespace LogMonitorService.Models.API.Responses
{
    public class HealthCheckResponse
    {
        public HealthCheckResponse(string status)
        {
            Version = typeof(Program).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            Status = status;
        }

        public string Version { get; }
        public string Status { get; }
    }
}
