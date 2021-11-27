using Chester.Evaluations;
using Chester.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Chester.Search;

public class ParallellAlphaBetaMinMaxSearch : AlphaBetaMinMaxSearch
{

    public ParallellAlphaBetaMinMaxSearch(SearchOptions options, IEvaluator evaluator, ISearchReporter reporter) : base(options, evaluator, reporter)
    {
    }

    public override Evaluation Search(Board board, Color nextToMove)
    {
        var isMaximizing = nextToMove == Color.White;
        var bestScore = isMaximizing ? -Evaluation.CheckMate : Evaluation.CheckMate;
        Move bestMove = null;
        var l = new object();

        Reporter.Reset();

        var moves = board.MovesFor(nextToMove).Select(m => EvaluateCopy(board, m));
        var sortedMoves = isMaximizing ? moves.OrderByDescending(x => x.score).ToList() : moves.OrderBy(x => x.score).ToList();

        Parallel.ForEach(sortedMoves, (x, state, idx) => {
            Reporter.CurrentMove(x.move, idx, x.score);
            var score = isMaximizing
                ? AlphaBetaMin(x.position, -Evaluation.CheckMate, Evaluation.CheckMate, Options.MaxDepth, nextToMove.Other())
                : AlphaBetaMax(x.position, -Evaluation.CheckMate, Evaluation.CheckMate, Options.MaxDepth, nextToMove.Other());

            Reporter.CurrentMove(x.move, idx, score);
            if (isMaximizing && score > bestScore)
                lock (l)
                {
                    if (isMaximizing && score > bestScore)
                    {
                        bestScore = score;
                        bestMove = x.move;
                        Reporter.BestLine(Options.MaxDepth, score, x.position.Line);
                    }
                }
            else if (!isMaximizing && score < bestScore)
                lock (l)
                {
                    if (!isMaximizing && score < bestScore)
                    {
                        bestScore = score;
                        bestMove = x.move;
                        Reporter.BestLine(Options.MaxDepth, score, x.position.Line);
                    }
                }
        });

        return new Evaluation(bestScore, bestMove);
    }
}
