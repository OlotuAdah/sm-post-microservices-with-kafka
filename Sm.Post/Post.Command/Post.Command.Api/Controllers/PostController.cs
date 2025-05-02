using AutoMapper;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Command.Api.Commands;
using Post.Command.Api.DTO;

namespace Post.Command.Api.Controllers;

[ApiController]
[Route("api/v1/posts")]
public class PostController(
    ICommandDispatcher commandDispatcher,
    IMapper mapper,
    ILogger<PostController> logger) : BaseCommandAPIContronller(logger)
{
    private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;
    private readonly IMapper _mapper = mapper;

    [HttpPost("add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<NewPostResponse>> NewPost([FromBody] NewPostDto dto)
    {
        try
        {
            NewPostCommand? command = _mapper.Map<NewPostCommand>(dto);
            await _commandDispatcher.SendAsync(command);
            return await HandleSuccessResponse(new NewPostResponse { Message = "Post created successfully" }, StatusCodes.Status201Created);

        }
        catch (Exception ex)
        {
            return await HandleException(ex, nameof(NewPost));
        }
    }

    [HttpPut("{id}/edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> EditMessage(Guid id, [FromBody] EditMessageDto dto)
    {
        try
        {
            EditMessageCommand? command = _mapper.Map<EditMessageCommand>(dto);
            command.Id = id;
            await _commandDispatcher.SendAsync(command);
            return await HandleSuccessResponse(new NewPostResponse { Message = "Message updated successfully" });
        }
        catch (Exception ex)
        {
            return await HandleException(ex, nameof(EditMessage));
        }
    }
    [HttpPut("like")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> LikePost([FromBody] LikePostDto dto)
    {
        try
        {
            LikePostCommand? command = _mapper.Map<LikePostCommand>(dto);
            command.Id = dto.PostId;
            await _commandDispatcher.SendAsync(command);
            return await HandleSuccessResponse(new NewPostResponse { Message = "Like post request completed successfully" });
        }
        catch (Exception ex)
        {
            return await HandleException(ex, nameof(LikePostDto));
        }
    }
    [HttpDelete("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeletePost([FromBody] DeletePostDto dto)
    {
        try
        {
            DeletePostCommand? command = _mapper.Map<DeletePostCommand>(dto);
            command.Id = dto.PostId;
            await _commandDispatcher.SendAsync(command);
            return await HandleSuccessResponse(new NewPostResponse { Message = "Post deleted successfully" });
        }
        catch (Exception ex)
        {
            return await HandleException(ex, nameof(DeletePost));
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
            AddCommentCommand? command = _mapper.Map<AddCommentCommand>(dto);
            command.Id = id;

            await _commandDispatcher.SendAsync(command);
            return await HandleSuccessResponse(new NewPostResponse { Message = "Comment added successfully" });
        }
        catch (Exception ex)
        {
            return await HandleException(ex, nameof(AddComment));
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
            EditCommentCommand? command = _mapper.Map<EditCommentCommand>(dto);
            command.Id = id;
            await _commandDispatcher.SendAsync(command);
            return await HandleSuccessResponse(new NewPostResponse { Message = "Comment edited successfully" });
        }
        catch (Exception ex)
        {
            return await HandleException(ex, nameof(EditComment));
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
            RemoveCommentCommand? command = _mapper.Map<RemoveCommentCommand>(dto);
            command.Id = id;
            await _commandDispatcher.SendAsync(command);
            return await HandleSuccessResponse(new NewPostResponse { Message = "Comment deleted successfully" });
        }
        catch (Exception ex)
        {
            return await HandleException(ex, nameof(RemoveComment));
        }
    }
}