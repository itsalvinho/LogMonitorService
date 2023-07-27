namespace LogMonitorService.Models.Configuration
{
    /// <summary>
    /// Model representing the appsettings.json's "AppConfig" section.
    /// </summary>
    public class AppConfig
    {
        public string PathToLogs { get; set; }
        public long DefaultNumberOfLogsToReturn { get; set; } = 100;
        public string Encoding { get; set; } = "UTF-8";
    }
}
