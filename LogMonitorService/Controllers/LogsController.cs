using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using LogMonitorService.Constants;
using LogMonitorService.Services.Abstractions;
using LogMonitorService.Models.API.Requests;
using LogMonitorService.Models.API.Results;

namespace LogMonitorService.Controllers
{
    // Route /api/v1.0/logs
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

        [HttpGet("{filename}")]
        [ActionName("GetLogs")]
        public async Task<IActionResult> GetLogs([FromQuery] GetLogsRequest request, string filename, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Get logs endpoint hit!");

            // Streams the data back into the Response.Body which is a Stream object 
            BaseServiceResult result = await this._logsControllerService.ReadLogsToStream(Response.Body, filename, request.SearchText, request.NumOfLogsToReturn, cancellationToken);

            if (result.ResultType == ResultType.Success)
            {
                // Return empty result and plain text to stream data back
                Response.ContentType = MediaTypeNames.Text.Plain;
                return new EmptyResult();
            }
            else
            {
                return GenerateResponseFromServiceResult(result);
            }
        }
    }
}
