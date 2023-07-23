namespace LogMonitorService.Models.API.Requests
{
    public class GetLogsRequest
    {
        public string? SearchText { get; set; }
        public long? NumOfLogsToReturn { get; set; }
    }
}
