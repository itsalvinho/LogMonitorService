using LogMonitorService.Constants;
using LogMonitorService.Exceptions;
using LogMonitorService.Models.API.Results;
using LogMonitorService.Models.Configuration;
using LogMonitorService.Services.Abstractions;
using System.Text;

namespace LogMonitorService.Services
{
    /// <summary>
    /// Service used by the LogsController as an abstraction layer between the API and business logic to handle API requests
    /// </summary>
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

        /// <summary>
        /// Reads logs from a provided file from newest to oldest and writes it back to the provided stream.
        /// You can also specify a search string to search for logs containing the provided text as a substring.
        /// The directory of the file is provided in the appsettings.json of the application, injected via AppConfig.
        /// </summary>
        /// <param name="stream">The stream to write into.</param>
        /// <param name="filename">The log file to read from contained in the directory configured by in the appsettings.</param>
        /// <param name="searchText">The text to search in the logs for, it will only write the lines containing the substring.</param>
        /// <param name="numOfLogsToReturn">The maximum number of lines to return in the stream.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation early.</param>
        public async Task<ServiceResult> ReadLogsToStream(Stream stream, string filename, string? searchText = null, long? numOfLogsToReturn = null, CancellationToken cancellationToken = default)
        {
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                { LogContextKeys.FILENAME, filename },
                { LogContextKeys.SEARCH_TEXT, searchText },
                { LogContextKeys.NUM_OF_LOGS_TO_RETURN, numOfLogsToReturn }
            }))
            {
                // Validate
                if (string.IsNullOrWhiteSpace(filename))
                    return new ServiceResult(ResultType.InvalidRequest, new InvalidRequestException("filename must be provided."));

                if (numOfLogsToReturn != null && numOfLogsToReturn <= 0)
                    return new ServiceResult(ResultType.InvalidRequest, new InvalidRequestException("numOfLogsToReturn must be greater than or equal to 1"));

                
                // Perform the read 
                try
                {
                    long maxLines = numOfLogsToReturn ?? _appConfig.DefaultNumberOfLogsToReturn;
                    string logPath = Path.Combine(_appConfig.PathToLogs, filename);
                    Encoding encoding = Encoding.GetEncoding(_appConfig.Encoding);

                    await _logReaderService.ReadLogsToStream(stream, logPath, encoding, searchText, maxLines, cancellationToken);

                    return new ServiceResult(ResultType.Success);
                }
                catch (FileNotFoundException ex)
                {
                    _logger.LogError(ex, "Could not find log file");
                    return new ServiceResult(ResultType.NotFound, ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error trying to read logs to stream");
                    return new ServiceResult(ResultType.UnknownError, ex);
                }
            }
        }
    }
}
