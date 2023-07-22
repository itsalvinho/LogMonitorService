using Microsoft.AspNetCore.Mvc;
using LogMonitorService.Constants;

namespace LogMonitorService.Controllers
{
    // Route /api/v1.0/logs
    [ApiController]
    [Route(ApiConstants.DEFAULT_API_BASE_ROUTE)]
    [ApiVersion("1.0")]
    public class LogsController : ControllerBase
    {
        private readonly ILogger<LogsController> _logger;

        public LogsController(ILogger<LogsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Index()
        {
            _logger.LogDebug("HELLOOO");
            return Ok("Hello I return logs!");
        }
    }
}
