using LogMonitorService.Services.Abstractions;

namespace LogMonitorService.Services
{
    internal class DefaultLogReaderService : ILogReaderService
    {
        public DefaultLogReaderService() 
        { 

        }

        public async Task ReadLogsToStreamReadLogsToStream(Stream stream, string logPath, string? searchText = null, long? maxLinesToReturn = null)
        {
            using (StreamWriter sw = new StreamWriter(stream, System.Text.Encoding.UTF8))
            {
                await sw.WriteLineAsync("This is a test log.");
                await sw.WriteLineAsync("This is another test log.");
                await sw.WriteLineAsync("A");
                await sw.WriteLineAsync("B");
                await sw.WriteLineAsync("C");

                await sw.FlushAsync();
                await sw.DisposeAsync();
            }
        }
    }
}
