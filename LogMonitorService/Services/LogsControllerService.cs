using LogMonitorService.Models.Configuration;
using LogMonitorService.Services.Abstractions;

namespace LogMonitorService.Services
{
    internal class LogsControllerService : ILogsControllerService
    {
        private readonly AppConfig _appConfig;
        private readonly ILogReaderService _logReaderService;

        public LogsControllerService(
            AppConfig appConfig,
            ILogReaderService logReaderService)
        {
            _appConfig = appConfig;
            _logReaderService = logReaderService;
        }

        public async Task ReadLogsToStream(Stream stream, string filename, string? searchText = null, long? maxLinesToReturn = null)
        {
            // TODO: Add validation for controller

            long maxLines = maxLinesToReturn ?? _appConfig.DefaultNumberOfLogsToReturn;
            string logPath = Path.Combine(_appConfig.PathToLogs, filename);

            await _logReaderService.ReadLogsToStream(stream, logPath, searchText, maxLines);
        }
    }
}
