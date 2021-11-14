using Chester.Models;
using Chester.Messages;
using Chester.Messages.Commands;
using Chester.Messages.Events;
using System.Threading.Tasks;
using Chester.Search;

namespace Chester.Services
{
    internal class GameChanger :
        ICommandHandler<UciCommStarted>,
        ICommandHandler<UciReadyRequested>,
        ICommandHandler<StartNewGame>,
        ICommandHandler<SetPosition>,
        ICommandHandler<Go>
    {
        private readonly IMessageBus _messageBus;
        private Game _game;
        ISearchReporter _reporter;

        public GameChanger(IMessageBus messageBus, ISearchReporter reporter)
        {
            _messageBus = messageBus;
            _reporter = reporter;
            _game = new Game(_reporter);
        }

        public async Task HandleAsync(UciCommStarted _)
        {
            await _messageBus.SendAsync(new SendUciMessage("id name Chester"));
            await _messageBus.SendAsync(new SendUciMessage("id author Stefan Eriksson"));
            await _messageBus.SendAsync(new SendUciMessage("uciok"));
        }

        public async Task HandleAsync(UciReadyRequested _)
        {
            await _messageBus.SendAsync(new SendUciMessage("readyok"));
        }

        public Task HandleAsync(StartNewGame _)
        {
            _game = new Game(_reporter);
            return Task.CompletedTask;
        }

        public Task HandleAsync(SetPosition message)
        {
            if (message.Fen != null)
            {
                _game = new Game(message.Fen.Board, message.Fen.NextToMove, _reporter);
            }

            if (message.StartPosition)
            {
                _game = new Game(_reporter);
            }

            foreach (var move in message.Moves.AsHalfMoves())
            {
                var from = move.From.ToModel();
                var to = move.To.ToModel();
                var fromState = _game.At(from);
                var toState = _game.At(to);

                //if (fromState.Piece() != move.Piece.ToModel())
                //    throw new GameError($"SetPosition: move {move} piece does not match actual board piece {fromState}");

                //if (toState.SameColor(fromState))
                //    throw new GameError($"SetPosition: move {move} tries to move piece onto same color {fromState} -> {toState}");

                var gameMove = new Move(fromState, from, to, fromState.IsAttack(toState));
                _game.MakeMove(gameMove);
            }

            return Task.CompletedTask;
        }

        public async Task HandleAsync(Go message)
        {
            var evaluation = _game.Search();

            await _messageBus.SendAsync(new BestMoveEvaluated(evaluation.Move));
        }
    }
}
