namespace LogMonitorService.Models.API.Requests
{
    /// <summary>
    /// Request object for the /api/v{#}/logs/{filename} endpoint
    /// </summary>
    public class GetLogsRequest
    {
        public string? SearchText { get; set; }
        public long? NumOfLogsToReturn { get; set; }
    }
}
