using Microsoft.AspNetCore.Mvc;
using LogMonitorService.Models.API.Responses;

namespace LogMonitorService.Controllers
{
    // Route /healthcheck
    [ApiController]
    [Route("[controller]")]
    public class HealthCheckController : BaseController
    {
        private const string HEALTHY_MESSAGE = "Log Monitor Service is healthy";

        private readonly ILogger<HealthCheckController> _logger;

        public HealthCheckController(ILogger<HealthCheckController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogTrace("Healtcheck enpoint hit!");

            HealthCheckResponse response = new HealthCheckResponse(HEALTHY_MESSAGE);
            return Ok(response);
        }
    }
}
