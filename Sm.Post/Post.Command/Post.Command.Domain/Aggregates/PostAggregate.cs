using CQRS.Core.Domain;
using Microsoft.VisualBasic.FileIO;
using Post.Common.Events;

namespace Post.Command.Domain.Aggregates;
public class PostAggregate : AggregateRoot
{
    private bool _active;
    private string _author;
    private string _content;

    private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();
    public bool Active
    {
        get { return _active; }
        set { _active = value; }
    }
    public string Author
    {
        get { return _author; }
        set { _author = value; }
    }
    public string Content
    {
        get { return _content; }
        set { _content = value; }
    }



    public PostAggregate()
    {

    }
    public PostAggregate(Guid id, string author, string message)
    {
        RaiseEvent(new PostCreatedEvent
        {
            Id = id,
            Author = author,
            Message = message,
            DatePosted = DateTime.UtcNow
        });
    }

    //The Apply method is used to apply the events to the aggregate for PostCreatedEvent 
    public void Apply(PostCreatedEvent @event)
    {
        _id = @event.Id;
        _active = true;
        _author = @event.Author;
        _content = @event.Message;
    }

    #region Edit comment

    public void EditMessage(string message)
    {
        if (!_active)
        {
            throw new Exception("Post is not active. You can't edit the message of an inactive post!");
        }
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new InvalidOperationException($" The value of {nameof(message)}  cannot be empty. Please provide a valid {nameof(message)}.");
        }
        RaiseEvent(new MessageUpdatedEvent
        {
            Id = _id,
            Message = message
        });
    }

    //The Apply method is used to apply the events to the aggregate for MessageUpdatedEvent. Overloaded method.
    public void Apply(MessageUpdatedEvent @event)
    {
        _id = @event.Id;

    }
    #endregion

    #region LikePost


    public void LikePost()
    {
        if (!_active)
        {
            throw new Exception("Post is not active. You can't like an inactive post!");
        }
        RaiseEvent(new PostLikedEvent
        {
            Id = _id
        });

    }

    public void Apply(PostLikedEvent @event)
    {
        _id = @event.Id;

    }

    #endregion

    #region AddComment


    public void AddComment(string comment, string username)
    {
        if (!_active)
        {
            throw new Exception("Post is not active. You can't comment on an inactive post!");
        }
        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new InvalidOperationException($" The value of {nameof(comment)}  cannot be empty. Please provide a valid {nameof(comment)}.");
        }
        RaiseEvent(new CommentAddedEvent
        {
            Id = _id,
            CommentId = Guid.NewGuid(),
            Comment = comment,
            Username = username,
            CommentDate = DateTime.Now
        });
    }

    public void Apply(CommentAddedEvent @event)
    {
        _id = @event.Id;
        _comments.Add(@event.CommentId, new Tuple<string, string>(@event.Comment, @event.Username));
    }
    #endregion

    #region EditComment

    public void EditComment(Guid commentId, string comment, string username)
    {
        if (!_active)
        {
            throw new Exception("Post is not active. You can't edit a comment on an inactive post!");
        }
        if (!_comments.ContainsKey(commentId))
        {
            throw new Exception($"Comment with id {commentId} does not exist!");
        }
        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new InvalidOperationException($" The value of {nameof(comment)}  cannot be empty. Please provide a valid {nameof(comment)}.");
        }

        if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new Exception("You are not authorized to edit this comment!");
        }
        RaiseEvent(new CommentUpdatedEvent
        {
            Id = _id,
            CommentId = commentId,
            Comment = comment,
            Username = username,
            EditDate = DateTime.UtcNow
        });
    }

    public void Apply(CommentUpdatedEvent @event)
    {
        _id = @event.Id;
        _comments[@event.CommentId] = new Tuple<string, string>(@event.Comment, @event.Username);
    }

    #endregion

    #region RemoveComment

    public void RemoveComment(Guid commentId, string username)
    {
        if (!_active)
        {
            throw new Exception("Post is not active. You can't remove a comment on an inactive post!");
        }
        if (!_comments.ContainsKey(commentId))
        {
            throw new Exception($"Comment with id {commentId} does not exist!");
        }
        if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new Exception("You are not authorized to remove this comment!");
        }
        RaiseEvent(new CommentRemovedEvent
        {
            Id = _id,
            CommentId = commentId
        });
    }

    public void Apply(CommentRemovedEvent @event)
    {
        _id = @event.Id;
        _comments.Remove(@event.CommentId);
    }

    #endregion

    #region DeletePost

    public void DeletePost(string username)
    {
        if (!_active)
        {
            throw new Exception("Post is not active. You can't delete an inactive post!");
        }
        if (!_author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new Exception("You are not authorized to remove this comment!");
        }
        RaiseEvent(new PostRemovedEvent
        {
            Id = _id
        });
    }

    public void Apply(PostRemovedEvent @event)
    {
        _id = @event.Id;
        _active = false;
    }

    #endregion
}
