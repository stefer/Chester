using Chester.Messages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chester.Services
{
    internal class MessageBus : IMessageBus
    {
        private readonly ConcurrentDictionary<Type, List<ICommandHandler<Message>>> _handlers = new();
        private readonly IServiceProvider _services;

        public MessageBus(IServiceProvider services)
        {
            _services = services;
        }


        public void AddHandler<TCommand>(ICommandHandler<TCommand> handler) where TCommand : Message
        {
            _handlers.AddOrUpdate(
                typeof(TCommand),
                new List<ICommandHandler<Message>>() { (ICommandHandler<Message>)handler },
                (type, list) =>
                {
                    list.Add((ICommandHandler<Message>)handler);
                    return list;
                });
        }

        public Task SendAsync<TCommand>(TCommand command) where TCommand : Message
        {
            var handlers = _services.GetRequiredService<IEnumerable<ICommandHandler<TCommand>>>();

            var tasks = handlers.Select(h => h.HandleAsync(command));
            return Task.WhenAll(tasks);
        }
    }
}
