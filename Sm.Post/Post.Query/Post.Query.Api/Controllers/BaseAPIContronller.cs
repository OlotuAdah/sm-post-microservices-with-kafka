using Microsoft.AspNetCore.Mvc;
using Post.Query.Api.DTO;
using Post.Query.Domain.Entities;
namespace Post.Query.Api.Controllers;

public class BaseAPIContronller(ILogger<BaseAPIContronller> logger) : ControllerBase
{
    private readonly ILogger<BaseAPIContronller> _logger = logger;

    protected Task<ActionResult> HandleException(Exception ex, string requestName)
    {
        var safeErrorMessage = $"Error occurred while processing your {requestName} request";
        _logger.LogError(ex, safeErrorMessage);

        return Task.FromResult<ActionResult>(StatusCode(StatusCodes.Status500InternalServerError, new PostLookupResponse
        {
            Message = safeErrorMessage
        }));

    }
    protected Task<ActionResult> HandleSuccessResponse(List<PostEntity> posts, string requestName)
    {
        if (posts == null || posts.Count < 1) return Task.FromResult<ActionResult>(NoContent());
        _logger.LogInformation($"Successfully returned {posts.Count} post{(posts.Count > 1 ? "s" : string.Empty)} for {requestName} request");
        return Task.FromResult<ActionResult>(Ok(new PostLookupResponse
        {
            Posts = posts,
            Message = $"Successfully returned {posts.Count} post{(posts.Count > 1 ? "s" : string.Empty)}!"
        }));

    }
}
