using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Command.Api.Commands;
using Post.Command.Api.DTO;

namespace Post.Command.Api.Controllers;

[ApiController]
[Route("api/v1/posts")]
public class NewPostController(ILogger<NewPostController> logger, ICommandDispatcher commandDispatcher) : ControllerBase
{
    private readonly ILogger<NewPostController> _logger = logger;
    private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;

    [HttpPost("add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<NewPostResponse>> NewPostAsync(NewPostCommand command)
    {
        _logger.LogInformation("Received new post command");
        try
        {
            await _commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status201Created, new NewPostResponse { Message = "Post created successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error while trying to add new post");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while trying to add new post");
            return BadRequest(ex.Message);
        }
    }
}