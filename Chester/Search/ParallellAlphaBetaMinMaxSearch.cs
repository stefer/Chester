using Chester.Evaluations;
using Chester.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Chester.Search
{

    public interface ISearchReporter
    {
        public void CurrentMove(Move move, long moveNumber, int score);
    }

    public class ParallellAlphaBetaMinMaxSearch : AlphaBetaMinMaxSearch
    {
        private readonly ISearchReporter _reporter;

        public ParallellAlphaBetaMinMaxSearch(SearchOptions options, IEvaluator evaluator, ISearchReporter reporter) : base(options, evaluator)
        {
            _reporter = reporter;
        }

        public override Evaluation Search(Board board, Color nextToMove)
        {
            var isMaximizing = nextToMove == Color.White;
            var bestScore = isMaximizing ? -Evaluation.CheckMate : Evaluation.CheckMate;
            Move bestMove = null;
            var l = new object();

            var moves = board.MovesFor(nextToMove).Select(m => EvaluateCopy(board, m));
            var sortedMoves = isMaximizing ? moves.OrderByDescending(x => x.score).ToList() : moves.OrderBy(x => x.score).ToList();

            Parallel.ForEach(sortedMoves, (x, state, idx) =>
            {

                var score = isMaximizing
                    ? AlphaBetaMin(x.position, -Evaluation.CheckMate, Evaluation.CheckMate, Options.MaxDepth, nextToMove.Other())
                    : AlphaBetaMax(x.position, -Evaluation.CheckMate, Evaluation.CheckMate, Options.MaxDepth, nextToMove.Other());

                _reporter.CurrentMove(x.move, idx, score);

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
