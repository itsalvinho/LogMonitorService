using Microsoft.AspNetCore.Mvc;
using LogMonitorService.Constants;
using LogMonitorService.Services.Abstractions;
using System.Net.Mime;
using LogMonitorService.Models.API.Requests;

namespace LogMonitorService.Controllers
{
    // Route /api/v1.0/logs
    [ApiController]
    [Route(ApiConstants.DEFAULT_API_BASE_ROUTE)]
    [ApiVersion("1.0")]
    public class LogsController : ControllerBase
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
        public async Task<ActionResult> GetLogs([FromQuery] GetLogsRequest request, string filename, CancellationToken cancellationToken)
        {
            Response.ContentType = MediaTypeNames.Text.Plain;
            await this._logsControllerService.ReadLogsToStream(Response.Body, filename, request.SearchText, request.NumOfLogsToReturn, cancellationToken);
            return new EmptyResult();
        }
    }
}
