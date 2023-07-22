namespace LogMonitorService.Services.Abstractions
{
    public interface ILogsControllerService
    {
        Task ReadLogsToStream(Stream stream, string fileName, string searchText, long maxLinesToReturn);
    }
}
