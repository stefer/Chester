using Chess.Evaluations;
using Chess.Models;

namespace Chess
{
    public class Game
    {
        public Board Board { get; init; } = new Board();
        public Evaluator _evaluator = new Evaluator();

        public Evaluation Evaluate(Move m)
        {
            var position = Board.Clone();
            position.MakeMove(m);
            return _evaluator.Evaluate(position, m);
        }
    }
}
