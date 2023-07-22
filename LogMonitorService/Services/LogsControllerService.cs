using LogMonitorService.Services.Abstractions;

namespace LogMonitorService.Services
{
    internal class LogsControllerService : ILogsControllerService
    {
        private readonly ILogReaderService _logReaderService;

        public LogsControllerService(ILogReaderService logReaderService) 
        { 
            _logReaderService = logReaderService;
        }

        public async Task ReadLogsToStream(Stream stream, string fileName, string searchText, long maxLinesToReturn)
        {
            // TODO: Implement by using _logReaderService

            throw new NotImplementedException();
        }
    }
}
