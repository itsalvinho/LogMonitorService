using LogMonitorService.Models.API.Results;
using Microsoft.AspNetCore.Mvc;

namespace LogMonitorService.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        public IActionResult GenerateResponseFromServiceResult(BaseServiceResult badServiceResult, string errorMessageOverride = null)
        {
            var response = new
            {
                message = errorMessageOverride ?? badServiceResult.Exception.Message
            };

            switch (badServiceResult.ResultType)
            {
                case ResultType.NoPermission:
                    return Unauthorized(response);
                case ResultType.InvalidRequest:
                    return BadRequest(response);
                case ResultType.UnknownError:
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                case ResultType.Success:
                    return Ok();
                default:
                    throw new NotImplementedException("Unhandled result type");
            }
        }
    }
}
