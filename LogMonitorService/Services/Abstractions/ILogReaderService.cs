namespace LogMonitorService.Services.Abstractions
{
    public interface ILogReaderService
    {
        Task ReadLogsToStream(Stream stream, string logFile, string searchText, long maxLinesToReturn);
    }
}
