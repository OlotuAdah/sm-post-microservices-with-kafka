using System.ComponentModel.DataAnnotations;

namespace Post.Command.Api.DTO;

public class NewPostDto
{
    public string Author { get; set; }
    public string Message { get; set; }
}

public class EditMessageDto
{
    public string Message { get; set; }
}

public class AddCommentDto
{
    public string Comment { get; set; }
    public string Username { get; set; }
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

public class LikePostDto
{
    public Guid PostId { get; set; }
}

public class DeletePostDto
{
    public Guid PostId { get; set; }
    public string Username { get; set; }
}

public class RemoveCommentDto
{
    public Guid CommentId { get; set; }
    public string Username { get; set; }
}