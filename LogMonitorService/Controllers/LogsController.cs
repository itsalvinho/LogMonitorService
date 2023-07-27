using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using LogMonitorService.Constants;
using LogMonitorService.Services.Abstractions;
using LogMonitorService.Models.API.Requests;
using LogMonitorService.Models.API.Results;

namespace LogMonitorService.Controllers
{
    /// <summary>
    /// API: /api/v1.0/logs
    /// A controller for interacting with logs
    /// </summary>
    [ApiController]
    [Route(ApiConstants.DEFAULT_API_BASE_ROUTE)]
    [ApiVersion("1.0")]
    public class LogsController : BaseController
    {
        private readonly ILogger<LogsController> _logger;
        private readonly ILogsControllerService _logsControllerService;

        public LogsController(ILogger<LogsController> logger, ILogsControllerService logsControllerService)
        {
            _logger = logger;
            _logsControllerService = logsControllerService;
        }

        /// <summary>
        /// GET /api/v{#}/logs/{filename}
        /// Writes to the Response.Body in plain text the logs that the user requests
        /// </summary>
        /// <param name="request">The query parameters for the API (SearchText: for searching a substring in each log line) (NumOfLogsToReturn: the max number of logs to return)</param>
        /// <param name="filename">The log file to read from, contained in the directory configured by in the appsettings.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation early.</param>
        [HttpGet("{filename}")]
        [ActionName("GetLogs")]
        public async Task<IActionResult> GetLogs([FromQuery] GetLogsRequest request, string filename, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Get logs endpoint hit!");

            // Return empty result and plain text to stream data back
            Response.ContentType = MediaTypeNames.Text.Plain;

            // Streams the data back into the Response.Body which is a Stream object 
            ServiceResult result = await this._logsControllerService.ReadLogsToStream(Response.Body, filename, request.SearchText, request.NumOfLogsToReturn, cancellationToken);

            if (result.ResultType != ResultType.Success)
            {
                await StreamResponseFromServiceResult(result);
            }
            return new EmptyResult();
        }
    }
}
