using LogMonitorService.Models.API.Results;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LogMonitorService.Controllers
{
    /// <summary>
    /// Base controller for APIs to use with friendly methods to generate responses based on service results
    /// </summary>
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// For API controllers to stream data back to the Response.Body based on service result
        /// </summary>
        public async Task StreamResponseFromServiceResult(ServiceResult badServiceResult, string errorMessageOverride = null)
        {
            int status = 0;   
            switch (badServiceResult.ResultType)
            {
                case ResultType.NoPermission:
                    status = StatusCodes.Status401Unauthorized;
                    break;
                case ResultType.InvalidRequest:
                    status = StatusCodes.Status400BadRequest;
                    break;
                case ResultType.NotFound:
                    status = StatusCodes.Status404NotFound;
                    break;
                case ResultType.UnknownError:
                    status = StatusCodes.Status500InternalServerError;
                    break;
                case ResultType.Success:
                    status = StatusCodes.Status200OK;
                    break;
                default:
                    throw new NotImplementedException("Unhandled result type");
            }

            var response = JsonSerializer.Serialize(new
            {
                status = status,
                message = errorMessageOverride ?? badServiceResult.Exception.Message
            });

            using (StreamWriter sw = new StreamWriter(Response.Body, System.Text.Encoding.UTF8))
            {
                await sw.WriteLineAsync(response);
                await sw.FlushAsync();
                await sw.DisposeAsync();
            }
        }

        /// <summary>
        /// For API controllers to generate an IActionResut to return based on the service result
        /// </summary>
        public IActionResult GenerateResponseFromServiceResult(ServiceResult badServiceResult, string errorMessageOverride = null)
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
                case ResultType.NotFound:
                    return NotFound(response);
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
