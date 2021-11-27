using Chester.Messages;
using Chester.Messages.Commands;
using Chester.Messages.Events;
using Chester.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chester.Services
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
                ["isready"] = CmdIsReady,
                ["ucinewgame"] = CmdNewGame,
                ["position"] = CmdPosition,
                ["go"] = CmdGo
            };
        }


        public async Task CmdUci(IEnumerable<string> args) => await messageBus.SendAsync(new UciCommStarted());
        public async Task CmdIsReady(IEnumerable<string> args) => await messageBus.SendAsync(new UciReadyRequested());
        public async Task CmdNewGame(IEnumerable<string> args) => await messageBus.SendAsync(new StartNewGame());

        private async Task CmdPosition(IEnumerable<string> args)
        {
            // position [fen <fenstring> | startpos ]  moves <move1> .... <movei>
            var typeString = args.First().Trim().ToLower();
            var restArgs = args.Skip(1);
            var setPositionCmd = new SetPosition();

            switch (typeString)
            {
                case "fen":
                    var fenParser = new FenParser(restArgs.First());
                    setPositionCmd.Fen = fenParser.Parse();
                    restArgs = args.Skip(1);
                    break;

                case "startpos":
                    setPositionCmd.StartPosition = true;
                    break;

                case "moves":
                    break;

                default:
                    throw new ArgumentException($"Unknown UCI position type {typeString} in {string.Join(", ", args)}");
            }

            if (restArgs.Any() && restArgs.First() == "moves")
            {
                var reader = new MoveTextReader(string.Join(" ", restArgs.Skip(1)));
                setPositionCmd.Moves = reader.ReadAll();
            }

            await messageBus.SendAsync(setPositionCmd);
        }

        private async Task CmdGo(IEnumerable<string> arg) => await messageBus.SendAsync(new Go());

        public async Task ExecuteAsync(string commandLine)
        {
            var split = commandLine.Split(' ');
            var cmd = split[0].Trim().ToLower();
            var args = split.Skip(1);

            if (_handlers.TryGetValue(cmd, out var handler))
                await handler(args);
        }
    }
}
