using CQRS.Core.Infrastructure;
using Post.Command.Api.Commands;
using Post.Command.Infrastructure.Dispatchers;

namespace Post.Command.Api;

public static class CommandHandlerRegistration
{
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        // Register the command handler
        services.AddScoped<ICommandHandler, CommandHandler>();
        // Register the command dispatcher
        services.AddScoped<ICommandDispatcher, CommandDispatcher>(sp =>
        {
            var dispatcher = new CommandDispatcher();
            var commandHandler = sp.GetRequiredService<ICommandHandler>();
            // Register all command handlers
            dispatcher.RegisterHandler<NewPostCommand>(commandHandler.HandleAsync);
            dispatcher.RegisterHandler<EditMessageCommand>(commandHandler.HandleAsync);
            dispatcher.RegisterHandler<LikePostCommand>(commandHandler.HandleAsync);
            dispatcher.RegisterHandler<DeletePostCommand>(commandHandler.HandleAsync);
            dispatcher.RegisterHandler<AddCommentCommand>(commandHandler.HandleAsync);
            dispatcher.RegisterHandler<EditCommentCommand>(commandHandler.HandleAsync);
            dispatcher.RegisterHandler<RemoveCommentCommand>(commandHandler.HandleAsync);
            return dispatcher;
        });
        return services;
    }
}