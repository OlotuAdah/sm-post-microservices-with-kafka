using Microsoft.AspNetCore.Mvc;
using Post.Common.DTO;

namespace Post.Command.Api.Controllers;

public class BaseCommandAPIContronller(ILogger<BaseCommandAPIContronller> logger) : ControllerBase
{
    private readonly ILogger<BaseCommandAPIContronller> _logger = logger;

    protected Task<ActionResult> HandleException(Exception ex, string requestName)
    {
        var safeErrorMessage = $"Error occurred while processing your {requestName} request";
        _logger.LogError(ex, safeErrorMessage);
        return Task.FromResult<ActionResult>(StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
        {
            Message = safeErrorMessage
        }));
    }
    protected Task<ActionResult> HandleSuccessResponse(BaseResponse baseResponse, int statusCode = StatusCodes.Status200OK, string requestName = "")
    {
        _logger.LogInformation(baseResponse.Message);
        return Task.FromResult<ActionResult>(StatusCode(statusCode, new BaseResponse
        {
            Message = string.IsNullOrEmpty(requestName) ? baseResponse.Message : baseResponse.Message.Replace("{request}", requestName),
        }));
    }
}
