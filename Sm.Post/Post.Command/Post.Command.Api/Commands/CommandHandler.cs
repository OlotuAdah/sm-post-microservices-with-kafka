using CQRS.Core.Handlers;
using Post.Command.Domain.Aggregates;

namespace Post.Command.Api.Commands;
public class CommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler) : ICommandHandler
{

    private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler = eventSourcingHandler;
    public async Task HandleAsync(NewPostCommand command)
    {
        try
        {
            var aggregate = new PostAggregate(command.Id, command.Author, command.Message);
            await _eventSourcingHandler.SaveAsync(aggregate);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task HandleAsync(RemoveCommentCommand command)
    {
        var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.RemoveComment(command.CommentId, command.Username);
        await _eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(LikePostCommand command)
    {
        var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.LikePost();
        await _eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(EditMessageCommand command)
    {
        var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.EditMessage(command.Message);
        await _eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(EditCommentCommand command)
    {
        var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.EditComment(command.CommentId, command.Comment, command.Username);
        await _eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(DeletePostCommand command)
    {
        var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.DeletePost(command.Username);
        await _eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(AddCommentCommand command)
    {
        var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.AddComment(command.Comment, command.Username);
        await _eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(RestoreReadDbCommand command)
    {
        await _eventSourcingHandler.RepublishEventsAsync();
    }
}
