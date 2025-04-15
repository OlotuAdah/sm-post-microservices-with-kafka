using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Util;
using CQRS.Core.Commands;
using CQRS.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Post.Command.Infrastructure.Dispatchers;
public class CommandDispatcher() : ICommandDispatcher
{


    private readonly Dictionary<Type, Func<BaseCommand, Task>> _handlers = new();
    private static readonly ILogger _logger = LoggerFactory
      .Create(builder => builder.AddConsole())
      .CreateLogger<CommandDispatcher>();

    public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
    {
        if (_handlers.ContainsKey(typeof(T)))
        {
            throw new InvalidOperationException("You cannot register the same command handler twice!");
        }

        _handlers.Add(typeof(T), x => handler((T)x)); //Where x is BaseCommand and T is the concrete Command Object Type e.g LikePostCommand
    }

    public async Task SendAsync(BaseCommand command)
    {
        try
        {
            //This is the method that will actually dispatch the command object to the appropriate handler
            if (!_handlers.ContainsKey(command.GetType()))
            {
                throw new InvalidOperationException($"No command handler was registered for this command type:{nameof(command)}");
            }
            // Now that you're sure the handler exists, you can retrieve it and invoke it.
            Func<BaseCommand, Task> handler = _handlers[command.GetType()];
            // Execute the handler with the command.
            await handler(command);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while trying to dispatch command {command.GetType().Name}");
            _logger.LogError(ex.Message, ex);
            _logger.LogError(ex.StackTrace);
            // You can also log the exception or handle it in a way that suits your application.
            throw;
        }
    }
}
