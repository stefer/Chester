using Chess.Models;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Evaluations
{
    public class Evaluator
    {
        public Evaluation Evaluate(Board b, Move m)
        {
            var dir = m.FromSquare.Direction();
            var eval = EvaluationTables.FromBoard(b);
            var byValue = b.Pieces.Select(p => eval.PieceValue(p)).Sum();
            var byPosition = b.Pieces.Select(p => eval.Positional(p)).Sum();
            return new Evaluation((byValue + byPosition) * dir, m);
        }
    }
}
