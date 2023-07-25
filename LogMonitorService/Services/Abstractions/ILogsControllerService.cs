using LogMonitorService.Models.API.Results;

namespace LogMonitorService.Services.Abstractions
{
    public interface ILogsControllerService
    {
        Task<BaseServiceResult> ReadLogsToStream(Stream stream, string filename, string? searchText = null, long? numOfLogsToReturn = null, CancellationToken cancellationToken = default);
    }
}
