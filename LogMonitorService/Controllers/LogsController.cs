using Microsoft.AspNetCore.Mvc;
using LogMonitorService.Constants;
using LogMonitorService.Services.Abstractions;

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

        [HttpGet]
        public ActionResult Index()
        {
            // TODO: use _logsControllerService to read logs

            return Ok("Hello I return logs!");
        }
    }
}
