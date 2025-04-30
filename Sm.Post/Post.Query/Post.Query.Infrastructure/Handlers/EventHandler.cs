using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;
using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Interfaces.Repositories;

namespace Post.Query.Infrastructure.Handlers;
public class EventHandler(IPostRepo postRepo, ICommentRepo commentRepo, ILogger<EventHandler> logger) : IEventHandler
{
    private readonly IPostRepo _postRepo = postRepo;
    private readonly ICommentRepo _commentRepo = commentRepo;
    private readonly ILogger<EventHandler> _logger = logger;

    public async Task On(PostCreatedEvent @event)
    {
        try
        {
            var post = new PostEntity
            {
                PostId = @event.Id,
                Author = @event.Author,
                DatePosted = @event.DatePosted,
                Message = @event.Message
            };
            await _postRepo.CreateAsync(post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling PostCreatedEvent");
            throw;
        }
    }

    public async Task On(MessageUpdatedEvent @event)
    {
        var post = await _postRepo.GetByIdAsync(@event.Id);
        if (post == null) return;
        post.Message = @event.Message;
        await _postRepo.UpdateAsync(post).ConfigureAwait(false);
    }

    public async Task On(PostLikedEvent @event)
    {
        var post = await _postRepo.GetByIdAsync(@event.Id);
        if (post == null) return;
        post.Likes++;
        await _postRepo.UpdateAsync(post).ConfigureAwait(false);
    }

    public async Task On(CommentAddedEvent @event)
    {
        var comment = new CommentEntity
        {
            CommentId = @event.CommentId,
            PostId = @event.Id,
            Comment = @event.Comment,
            CommentDate = @event.CommentDate,
            Username = @event.Username,
            Edited = false // new comment, so not edited
        };
        await _commentRepo.CreateAsync(comment).ConfigureAwait(false);
    }


    public async Task On(CommentUpdatedEvent @event)
    {
        var comment = await _commentRepo.GetByIdAsync(@event.CommentId);
        if (comment == null) return;
        comment.Comment = @event.Comment;
        comment.Edited = true; // comment is edited
        comment.CommentDate = @event.EditDate;
        await _commentRepo.UpdateAsync(comment).ConfigureAwait(false);
    }

    public async Task On(CommentRemovedEvent @event)
    {
        var comment = await _commentRepo.GetByIdAsync(@event.CommentId);
        if (comment == null) return;
        await _commentRepo.DeleteAsync(comment.CommentId).ConfigureAwait(false);

    }

    public async Task On(PostRemovedEvent @event)
    {
        var post = await _postRepo.GetByIdAsync(@event.Id);
        if (post == null) return;
        await _postRepo.DeleteAsync(post.PostId).ConfigureAwait(false);

    }
}
