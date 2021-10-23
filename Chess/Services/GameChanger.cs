using Chess.Messages;
using Chess.Messages.Commands;
using Chess.Messages.Events;
using System.Threading.Tasks;

namespace Chess.Services
{
    internal interface IGameChanger: ICommandHandler<UciCommStarted> { }

    internal class GameChanger: IGameChanger,
        ICommandHandler<UciCommStarted>
    {
        private readonly IMessageBus _messageBus;

        public GameChanger(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        public async Task HandleAsync(UciCommStarted message)
        {
            await _messageBus.SendAsync(new SendUciMessage("id name Chester"));
            await _messageBus.SendAsync(new SendUciMessage("id author Stefan Eriksson"));
            await _messageBus.SendAsync(new SendUciMessage("uciok"));
        }
    }
}
