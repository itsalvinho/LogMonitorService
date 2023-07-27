using LogMonitorService.Constants;
using LogMonitorService.Services.Abstractions;
using LogMonitorService.Utilities;
using System.Text;

namespace LogMonitorService.Services
{
    /// <summary>
    /// Implementation of ILogReaderService for reading logs to a stream.
    /// This specific default implementation opens a stream from local storage to read. 
    /// Other implementations could open a stream from a cloud storage provider and read from there instead.
    /// </summary>
    internal class DefaultLogReaderService : ILogReaderService
    {
        private readonly ILogger<DefaultLogReaderService> _logger;

        public DefaultLogReaderService(ILogger<DefaultLogReaderService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Reads logs from a provided file from newest to oldest and writes it back to the provided stream.
        /// You can also specify a search string to search for logs containing the provided text as a substring.
        /// </summary>
        /// <param name="stream">The stream to write into.</param>
        /// <param name="logPath">The full path to the log file to read from.</param>
        /// <param name="encoding">The encoding of the log file.</param>
        /// <param name="searchText">The text to search in the logs for, it will only write the lines containing the substring.</param>
        /// <param name="maxLinesToReturn">The maximum number of lines to return in the stream.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation early.</param>
        public async Task ReadLogsToStream(Stream stream, string logPath, Encoding encoding, string? searchText = null, long maxLinesToReturn = -1, CancellationToken cancellationToken = default)
        {
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                { LogContextKeys.FULL_LOG_PATH, logPath },
                { LogContextKeys.ENCODING, encoding }
            }))
            using (FileStream fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.Read)) // Open File Stream, using FileShare.Read will allow reads while other processes are using it
            using (StreamWriter sw = new StreamWriter(stream, encoding))                                    // Open StreamWriter - to write to provided stream
            using (ReverseStreamReader rsReader = new ReverseStreamReader(fs, encoding))                    // Custom reader to read a stream's data in reverse to write back to the provided stream
            {
                _logger.LogDebug("Starting to read file");

                // If search text was provided, convert to byte array
                byte[]? searchBytes = !string.IsNullOrWhiteSpace(searchText) ?
                    encoding.GetBytes(searchText) :
                    null;

                long linesReturned = 0;
                string line = await rsReader.ReadLineAsync(searchBytes);

                // Read until end of file or we've returned over maxLinesToReturn. If maxLinesToReturn is -1, then it'll read till the entire file is parsed.
                while (
                    !rsReader.IsAtEnd() && 
                    (maxLinesToReturn == -1 || linesReturned < maxLinesToReturn)  &&
                    !cancellationToken.IsCancellationRequested)
                {
                    // Write line
                    await sw.WriteLineAsync(line);
                    linesReturned++;

                    _logger.LogTrace("Writing line to stream (Num of lines returned: {LinesReturned}): {LogLine}", linesReturned, line);

                    // Read next line
                    line = await rsReader.ReadLineAsync(searchBytes);
                }

                
                _logger.LogDebug("Finished reading file, flushing stream writer.");
                // Manual flushing causes it to occur asynchronously, otherwise, after the "Using" is finished, it disposes it synchronously. Async is more performant to free up the thread. 
                await sw.FlushAsync();
                await sw.DisposeAsync();
            }
        }
    }
}
