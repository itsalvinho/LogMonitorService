using LogMonitorService.Services.Abstractions;

namespace LogMonitorService.Services
{
    internal class DefaultLogReaderService : ILogReaderService
    {
        public DefaultLogReaderService() 
        { 

        }

        public Task ReadLogsToStream(Stream stream, string logFile, string searchText, long maxLinesToReturn)
        {
            // TODO: implement to read from a file location

            throw new NotImplementedException();
        }
    }
}
