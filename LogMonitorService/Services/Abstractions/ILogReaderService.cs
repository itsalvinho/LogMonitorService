using System.Text;

namespace LogMonitorService.Services.Abstractions
{
    /// <summary>
    /// Service to read logs from anywhere (provided a log path) and write the logs back into a stream.
    /// </summary>
    public interface ILogReaderService
    {
        Task ReadLogsToStream(Stream stream, string logPath, Encoding encoding, string? searchText = null, long maxLinesToReturn = -1, CancellationToken cancellationToken = default);
    }
}
