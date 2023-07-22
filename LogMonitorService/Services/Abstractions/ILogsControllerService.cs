namespace LogMonitorService.Services.Abstractions
{
    public interface ILogsControllerService
    {
        Task ReadLogsToStream(Stream stream, string filename, string? searchText = null, long? maxLinesToReturn = null);
    }
}
