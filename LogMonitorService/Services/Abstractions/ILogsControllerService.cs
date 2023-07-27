using LogMonitorService.Models.API.Results;

namespace LogMonitorService.Services.Abstractions
{
    /// <summary>
    /// Service used by the LogsController as an abstraction layer between the API and business logic to handle API requests
    /// </summary>
    public interface ILogsControllerService
    {
        Task<ServiceResult> ReadLogsToStream(Stream stream, string filename, string? searchText = null, long? numOfLogsToReturn = null, CancellationToken cancellationToken = default);
    }
}
