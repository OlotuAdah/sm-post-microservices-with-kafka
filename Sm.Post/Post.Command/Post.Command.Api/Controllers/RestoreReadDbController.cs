using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Command.Api.Commands;
using Post.Common.DTO;

namespace Post.Command.Api.Controllers;

[ApiController]
[Route("api/v1/restore-read-db")]
public class RestoreReadDbController(ILogger<RestoreReadDbController> logger) : BaseCommandAPIContronller(logger)
{
    [HttpPut]
    public async Task<IActionResult> RestoreReadDb([FromServices] ICommandDispatcher dispatcher)
    {
        try
        {
            var command = new RestoreReadDbCommand();
            await dispatcher.SendAsync(command);
            return Ok(new BaseResponse { Message = "Read database restored successfully." });
        }
        catch (Exception ex)
        {
            return await HandleException(ex, nameof(RestoreReadDb));
        }
    }

}