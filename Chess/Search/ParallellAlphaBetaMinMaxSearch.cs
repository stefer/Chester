using Chess.Evaluations;
using Chess.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.Search
{

    public class ParallellAlphaBetaMinMaxSearch: AlphaBetaMinMaxSearch
    {
        public ParallellAlphaBetaMinMaxSearch(SearchOptions options, IEvaluator evaluator): base(options, evaluator)
        {
        }

        public override Evaluation Search(Board board, Color nextToMove)
        {
            var bestScore = -Evaluation.CheckMate;
            Move bestMove = null;
            var l = new object();

            var sortedMoves = board.MovesFor(nextToMove).Select(m => EvaluateCopy(board, m)).OrderByDescending(x => x.score).ToList();

            if (Options.MaxDepth == 0)
            {
                var move = sortedMoves.FirstOrDefault();
                return new Evaluation(move.score, move.move);
            }

            Parallel.ForEach(sortedMoves, x =>
            {
                var score = AlphaBetaMin(x.position, -Evaluation.CheckMate, Evaluation.CheckMate, Options.MaxDepth, nextToMove.Other());
                if (score > bestScore)
                    lock (l)
                    {
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestMove = x.move;
                        }
                    }
            });

            return new Evaluation(bestScore, bestMove);
        }
    }
}
