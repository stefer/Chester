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
            var isMaximizing = nextToMove == Color.White;
            var bestScore = isMaximizing ? -Evaluation.CheckMate : Evaluation.CheckMate;
            Move bestMove = null;
            var l = new object();

            var moves = board.MovesFor(nextToMove).Select(m => EvaluateCopy(board, m));
            var sortedMoves = isMaximizing ? moves.OrderByDescending(x => x.score).ToList() : moves.OrderBy(x => x.score).ToList();

            Parallel.ForEach(sortedMoves, x =>
            {
                var score = isMaximizing
                    ? AlphaBetaMin(x.position, -Evaluation.CheckMate, Evaluation.CheckMate, Options.MaxDepth, nextToMove.Other())
                    : AlphaBetaMax(x.position, -Evaluation.CheckMate, Evaluation.CheckMate, Options.MaxDepth, nextToMove.Other());
                if (isMaximizing && score > bestScore)
                    lock (l)
                    {
                        if (isMaximizing && score > bestScore)
                        {
                            bestScore = score;
                            bestMove = x.move;
                        }
                    }
                else if (!isMaximizing && score < bestScore)
                    lock (l)
                    {
                        if (!isMaximizing && score < bestScore)
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
