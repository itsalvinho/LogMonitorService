using Microsoft.AspNetCore.Mvc;
using LogMonitorService.Models.API.Responses;

namespace LogMonitorService.Controllers
{
    /// <summary>
    /// API: /healthcheck
    /// To provide an API for testing the health of the application
    /// </summary>
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

        /// <summary>
        /// GET /healthcheck
        /// Provides a JSON response and status code indicating the application is running
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogTrace("Healtcheck enpoint hit!");

            HealthCheckResponse response = new HealthCheckResponse(HEALTHY_MESSAGE);
            return Ok(response);
        }
    }
}
