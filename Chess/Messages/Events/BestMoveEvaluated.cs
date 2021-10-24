using Chess.Models;

namespace Chess.Messages.Events
{
    internal class BestMoveEvaluated : Event 
    { 
        public Move Move { get; set; }

        public BestMoveEvaluated(Move move)
        {
            Move = move;
        }
    }
}