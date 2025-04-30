using AutoMapper;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Command.Api.Commands;
using Post.Command.Api.DTO;

namespace Post.Command.Api.Controllers;

[ApiController]
[Route("api/v1/posts")]
public class PostController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IMapper _mapper;
    private readonly ILogger<PostController> _logger;

    public PostController(
        ICommandDispatcher commandDispatcher,
        IMapper mapper,
        ILogger<PostController> logger)
    {
        _commandDispatcher = commandDispatcher;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost("add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<NewPostResponse>> NewPost([FromBody] NewPostDto dto)
    {
        try
        {
            var command = _mapper.Map<NewPostCommand>(dto);
            await _commandDispatcher.SendAsync(command);

            return StatusCode(StatusCodes.Status201Created,
                new NewPostResponse
                {
                    Id = command.Id,
                    Message = "Post created successfully"
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating new post");
            return BadRequest(new { ex.Message });
        }
    }

    [HttpPut("{id}/edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> EditMessage(Guid id, [FromBody] EditMessageDto dto)
    {
        try
        {
            var command = _mapper.Map<EditMessageCommand>(dto);
            command.Id = id;

            await _commandDispatcher.SendAsync(command);
            return Ok(new { Message = "Message updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing message for post {PostId}", id);
            return BadRequest(new { ex.Message });
        }
    }
    [HttpPut("like")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> LikePost([FromBody] LikePostDto dto)
    {
        try
        {
            var command = _mapper.Map<LikePostCommand>(dto);
            command.Id = dto.PostId;
            await _commandDispatcher.SendAsync(command);
            return Ok(new { Message = "Like post request completed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while process like on post {PostId}", dto.PostId);
            return BadRequest(new { ex.Message });
        }
    }
    [HttpDelete("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeletePost([FromBody] DeletePostDto dto)
    {
        try
        {
            var command = _mapper.Map<DeletePostCommand>(dto);
            command.Id = dto.PostId;
            await _commandDispatcher.SendAsync(command);
            return Ok(new { Message = "Post deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while process delete post request: {PostId}", dto.PostId);
            return BadRequest(new { ex.Message });
        }
    }

    [HttpPost("{id}/comments/add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddComment(
        Guid id,
        [FromBody] AddCommentDto dto)
    {
        try
        {
            var command = _mapper.Map<AddCommentCommand>(dto);
            command.Id = id;

            await _commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status201Created,
                new { Message = "Comment added successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding comment to post {PostId}", id);
            return BadRequest(new { ex.Message });
        }
    }
    [HttpPost("{id}/comments/edit")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> EditComment(
         Guid id,
        [FromBody] EditCommentDto dto)
    {
        try
        {
            var command = _mapper.Map<EditCommentCommand>(dto);
            command.Id = id;
            await _commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status201Created,
                new { Message = "Comment edited successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error editing comment with id ${dto.CommentId}");
            return BadRequest(new { ex.Message });
        }
    }
    [HttpDelete("{id}/comments/remove")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> RemoveComment(
         Guid id,
        [FromBody] RemoveCommentDto dto)
    {
        try
        {
            var command = _mapper.Map<RemoveCommentCommand>(dto);
            command.Id = id;
            await _commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status201Created, new { Message = "Comment deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting comment with id ${dto.CommentId}");
            return BadRequest(new { ex.Message });
        }
    }
}