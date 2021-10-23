using Chess.Messages;
using Chess.Messages.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.Services
{
    internal class UciInterpreter : IUciInterpretator
    {
        private readonly IMessageBus messageBus;

        private readonly Dictionary<string, Func<IEnumerable<string>, Task>> _handlers;

        public UciInterpreter(IMessageBus messageBus)
        {
            this.messageBus = messageBus;

            _handlers = new()
            {
                ["uci"] = CmdUci,
            };
        }

        public async Task ExecuteAsync(string commandLine)
        {
            var split = commandLine.Split(' ');
            var cmd = split[0].Trim().ToLower();
            var args = split.Skip(1);

            if (_handlers.TryGetValue(cmd, out Func<IEnumerable<string>, Task> handler))
                await handler(args);

        }

        public async Task CmdUci(IEnumerable<string> args)
        {
            await messageBus.SendAsync(new UciCommStarted());
        }
    }
}
