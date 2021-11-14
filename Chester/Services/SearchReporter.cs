using Chester.Messages;
using Chester.Messages.Events;
using Chester.Models;
using Chester.Search;

namespace Chester.Services
{
    internal class SearchReporter : ISearchReporter
    {
        private readonly IMessageBus _bus;

        public SearchReporter(IMessageBus bus)
        {
            _bus = bus;
        }

        public void CurrentMove(Move move, long moveNumber, int score)
        {
            // Do not wait
            _bus.SendAsync(new Info { CurrentMove = move, CurrentMoveNumber = moveNumber, Score = score });
        }
    }
}
