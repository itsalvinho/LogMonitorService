using LogMonitorService.Services.Abstractions;

namespace LogMonitorService.Services
{
    internal class DefaultLogReaderService : ILogReaderService
    {
        private readonly ILogger<DefaultLogReaderService> _logger;

        public DefaultLogReaderService(ILogger<DefaultLogReaderService> logger)
        {
            _logger = logger;
        }

        public async Task ReadLogsToStream(Stream stream, string logPath, string? searchText = null, long? maxLinesToReturn = null, CancellationToken cancellationToken = default)
        {
            // TODO: Remove test data and implement the actual reading and parsing of a log file
            // It should also check if the log path exists before reading and throw an exception indicating that it doesn't exist

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
