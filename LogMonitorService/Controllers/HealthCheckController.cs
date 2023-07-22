using Microsoft.AspNetCore.Mvc;
using LogMonitorService.Models.API.Responses;

namespace LogMonitorService.Controllers
{
    // Route /api/v1.0/logs
    [ApiController]
    [Route("[controller]")]
    public class HealthCheckController : ControllerBase
    {
        private const string HEALTHY_MESSAGE = "Log Monitor Service is healthy";

        private readonly ILogger<HealthCheckController> _logger;

        public HealthCheckController(ILogger<HealthCheckController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Index()
        {
            HealthCheckResponse response = new HealthCheckResponse(HEALTHY_MESSAGE);
            return Ok(response);
        }
    }
}
