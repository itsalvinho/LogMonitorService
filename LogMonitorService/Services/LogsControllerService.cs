using LogMonitorService.Exceptions;
using LogMonitorService.Models.API.Results;
using LogMonitorService.Models.Configuration;
using LogMonitorService.Services.Abstractions;

namespace LogMonitorService.Services
{
    public class LogsControllerService : ILogsControllerService
    {
        private readonly ILogger<LogsControllerService> _logger;
        private readonly AppConfig _appConfig;
        private readonly ILogReaderService _logReaderService;

        public LogsControllerService(
            ILogger<LogsControllerService> logger,
            AppConfig appConfig,
            ILogReaderService logReaderService)
        {
            _logger = logger;
            _appConfig = appConfig;
            _logReaderService = logReaderService;
        }
 
        public async Task<BaseServiceResult> ReadLogsToStream(Stream stream, string filename, string? searchText = null, long? numOfLogsToReturn = null, CancellationToken cancellationToken = default)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(filename))
                return new BaseServiceResult(ResultType.InvalidRequest, new InvalidRequestException("filename must be provided."));

            if (numOfLogsToReturn != null && numOfLogsToReturn <= 0)
                return new BaseServiceResult(ResultType.InvalidRequest, new InvalidRequestException("numOfLogsToReturn must be greater than or equal to 1"));

            long maxLines = numOfLogsToReturn ?? _appConfig.DefaultNumberOfLogsToReturn;
            string logPath = Path.Combine(_appConfig.PathToLogs, filename);

            try
            {
                await _logReaderService.ReadLogsToStream(stream, logPath, searchText, maxLines, cancellationToken);
                return new BaseServiceResult(ResultType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error trying to read logs to stream", ex);
                return new BaseServiceResult(ResultType.UnknownError, ex);
            }
        }
    }
}
