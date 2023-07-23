namespace LogMonitorService.Services.Abstractions
{
    public interface ILogReaderService
    {
        Task ReadLogsToStream(Stream stream, string logPath, string? searchText = null, long? maxLinesToReturn = null);
    }
}
