using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Common.Entities;
using Post.Query.Api.Queries;

namespace Post.Query.Api.Controllers;

[ApiController]
[Route("api/v1/post-lookup/posts")]
public class PostLookupController(ILogger<PostLookupController> logger, IQueryDispatcher<PostEntity> queryDispatcher) : BaseQueryAPIContronller(logger)
{
    private readonly IQueryDispatcher<PostEntity> _queryDispatcher = queryDispatcher;

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PostEntity>>> FindAllPostQuery()
    {
        try
        {
            List<PostEntity> posts = await _queryDispatcher.SendQueryAsync(new FindAllPostQuery());
            return await HandleSuccessResponse(posts, nameof(FindAllPostQuery));
        }
        catch (Exception ex)
        {
            return await HandleException(ex, nameof(FindAllPostQuery));
        }
    }
    [HttpGet("id/{id}/")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PostEntity>>> FindPostByIdQuery(Guid id)
    {
        try
        {
            List<PostEntity> posts = await _queryDispatcher.SendQueryAsync(new FindPostByIdQuery() { Id = id });
            return await HandleSuccessResponse(posts, nameof(FindPostByIdQuery));
        }
        catch (Exception ex)
        {
            return await HandleException(ex, nameof(FindPostByIdQuery));
        }
    }


    [HttpGet("author/{author}/")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PostEntity>>> FindPostByAuthorQuery(string author)
    {
        try
        {
            List<PostEntity> posts = await _queryDispatcher.SendQueryAsync(new FindPostByAuthorQuery() { Author = author });
            return await HandleSuccessResponse(posts, nameof(FindPostByAuthorQuery));
        }
        catch (Exception ex)
        {
            return await HandleException(ex, nameof(FindPostByAuthorQuery));
        }
    }
    [HttpGet("post-with-comments/")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PostEntity>>> FindPostWithCommentsQuery()
    {
        try
        {
            List<PostEntity> posts = await _queryDispatcher.SendQueryAsync(new FindPostWithCommentsQuery() { });
            return await HandleSuccessResponse(posts, nameof(FindPostWithCommentsQuery));
        }
        catch (Exception ex)
        {
            return await HandleException(ex, nameof(FindPostWithCommentsQuery));
        }
    }
    [HttpGet("post-with-likes/number-of-likes/{numberOfLikes}/")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PostEntity>>> FindPostWithLikesQuery(int numberOfLikes)
    {
        try
        {
            List<PostEntity> posts = await _queryDispatcher.SendQueryAsync(new FindPostWithLikesQuery() { NumberOfLikes = numberOfLikes });
            return await HandleSuccessResponse(posts, nameof(FindPostWithLikesQuery));
        }
        catch (Exception ex)
        {
            return await HandleException(ex, nameof(FindPostWithLikesQuery));
        }
    }

}