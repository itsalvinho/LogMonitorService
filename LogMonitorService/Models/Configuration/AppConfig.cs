namespace LogMonitorService.Models.Configuration
{
    public class AppConfig
    {
        public string PathToLogs { get; set; }
        public long DefaultNumberOfLogsToReturn { get; set; } = 100;
    }
}
