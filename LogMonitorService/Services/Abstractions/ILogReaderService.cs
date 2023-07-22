namespace LogMonitorService.Services.Abstractions
{
    public interface ILogReaderService
    {
        Task ReadLogsToStreamReadLogsToStream(Stream stream, string logPath, string? searchText = null, long? maxLinesToReturn = null);
    }
}
