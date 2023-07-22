using LogMonitorService.Services.Abstractions;

namespace LogMonitorService.Services
{
    internal class DefaultLogReaderService : ILogReaderService
    {
        public DefaultLogReaderService() 
        { 

        }

        public Task ReadLogsToStreamReadLogsToStream(Stream stream, string logPath, string? searchText = null, long? maxLinesToReturn = null)
        {
            // TODO: implement to read from a file location

            throw new NotImplementedException();
        }
    }
}
