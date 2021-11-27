using Chester.Models;

namespace Chester.Messages.Events;

internal class BestMoveEvaluated : Event
{
    public Move Move { get; set; }

    public BestMoveEvaluated(Move move)
    {
        Move = move;
    }
}
