using System.ComponentModel.DataAnnotations;

namespace Post.Command.Api.DTO;

public class NewPostDto : IValidatableObject
{
    public string Author { get; set; }
    public string Message { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Author))
        {
            yield return new ValidationResult("Author is required", [nameof(Author)]);
        }
        if (string.IsNullOrWhiteSpace(Message))
        {
            yield return new ValidationResult("Message is required", [nameof(Message)]);
        }
    }
}

public class EditMessageDto : IValidatableObject
{
    public string Message { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Message))
        {
            yield return new ValidationResult("Message is required", [nameof(Message)]);
        }
    }
}


public class AddCommentDto : IValidatableObject
{
    public string Comment { get; set; }
    public string Username { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Comment))
        {
            yield return new ValidationResult("Comment is required", [nameof(Comment)]);
        }
        if (string.IsNullOrWhiteSpace(Username))
        {
            yield return new ValidationResult("Username is required", [nameof(Username)]);
        }
    }
}

public class EditCommentDto : IValidatableObject
{
    public Guid CommentId { get; set; }
    public string Comment { get; set; }
    public string Username { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CommentId == Guid.Empty)
        {
            yield return new ValidationResult("CommentId is required", [nameof(CommentId)]);
        }
        if (string.IsNullOrWhiteSpace(Comment))
        {
            yield return new ValidationResult("Comment is required", [nameof(Comment)]);
        }
        if (string.IsNullOrWhiteSpace(Username))
        {
            yield return new ValidationResult("Username is required", [nameof(Username)]);
        }
    }
}

public class LikePostDto : IValidatableObject
{

    public Guid PostId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PostId == Guid.Empty)
        {
            yield return new ValidationResult("PostId is required", [nameof(PostId)]);
        }
    }
}

public class DeletePostDto : IValidatableObject
{
    public Guid PostId { get; set; }
    public string Username { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PostId == Guid.Empty)
        {
            yield return new ValidationResult("PostId is required", [nameof(PostId)]);
        }
        if (string.IsNullOrWhiteSpace(Username))
        {
            yield return new ValidationResult("Username is required", [nameof(Username)]);
        }
    }
}


public class RemoveCommentDto : IValidatableObject
{
    public Guid CommentId { get; set; }
    public string Username { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CommentId == Guid.Empty)
        {
            yield return new ValidationResult("CommentId is required", [nameof(CommentId)]);
        }
        if (string.IsNullOrWhiteSpace(Username))
        {
            yield return new ValidationResult("Username is required", [nameof(Username)]);
        }
    }
}

