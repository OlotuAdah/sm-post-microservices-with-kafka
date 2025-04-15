
namespace Post.Command.Api.Commands;
public interface ICommandHandler
{
    Task HandleAsync(NewPostCommand command);
    Task HandleAsync(RemoveCommentCommand command);
    Task HandleAsync(LikePostCommand command);
    Task HandleAsync(EditMessageCommand command);
    Task HandleAsync(EditCommentCommand command);
    Task HandleAsync(DeletePostCommand command);
    Task HandleAsync(AddCommentCommand command);
}
