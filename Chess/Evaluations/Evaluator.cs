using Chess.Models;
using System.Linq;

namespace Chess.Evaluations
{
    public interface IEvaluator
    {
        public int Evaluate(Board b);
    }

    public class Evaluator: IEvaluator
    {
        public int Evaluate(Board b)
        {
            var eval = EvaluationTables.FromBoard(b);
            var pieces = b.Pieces.ToList();
            var byValue = pieces.Select(p => eval.PieceValue(p)).Sum();
            var byPosition = pieces.Select(p => eval.Positional(p)).Sum();
            return byValue + byPosition;
        }
    }
}
