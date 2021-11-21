using Chester.Evaluations;
using Chester.Models;
using System.Linq;

namespace Chester.Search
{
    public class AlphaBetaMinMaxSearch : ISearch
    {
        protected SearchOptions Options { get; }
        protected ISearchReporter Reporter { get; }

        private IEvaluator Evaluator { get; }

        public AlphaBetaMinMaxSearch(SearchOptions options, IEvaluator evaluator, ISearchReporter reporter)
        {
            Options = options;
            Reporter = reporter;
            Evaluator = evaluator;
        }

        public virtual Evaluation Search(Board board, Color nextToMove)
        {
            var isMaximizing = nextToMove == Color.White;
            var bestScore = isMaximizing ? -Evaluation.CheckMate : Evaluation.CheckMate;
            Move bestMove = null;

            var moves = board.MovesFor(nextToMove).Select(m => Evaluate(board, m)).ToList();
            var sortedMoves = isMaximizing ? moves.OrderByDescending(x => x.score).ToList() : moves.OrderBy(x => x.score).ToList();

            foreach (var x in sortedMoves)
            {
                var save = board.MakeMove(x.move);

                var score = isMaximizing
                    ? AlphaBetaMin(board, -Evaluation.CheckMate, Evaluation.CheckMate, Options.MaxDepth, nextToMove.Other())
                    : AlphaBetaMax(board, -Evaluation.CheckMate, Evaluation.CheckMate, Options.MaxDepth, nextToMove.Other());

                board.TakeBack(save);

                if (isMaximizing && score > bestScore)
                {
                    bestScore = score;
                    bestMove = x.move;
                }
                else if (!isMaximizing && score < bestScore)
                {
                    bestScore = score;
                    bestMove = x.move;
                }
            };

            return new Evaluation(bestScore, bestMove);
        }

        protected int AlphaBetaMax(Board board, int alpha, int beta, int depthLeft, Color nextToMove)
        {
            if (depthLeft == 0) return Evaluate(board);

            var moves = board.MovesFor(nextToMove);
            var sortedMoves = moves.Select(m => Evaluate(board, m)).OrderByDescending(x => x.score);

            foreach (var (_, move) in sortedMoves)
            {
                var save = board.MakeMove(move);
                var score = AlphaBetaMin(board, alpha, beta, depthLeft - 1, nextToMove.Other());
                board.TakeBack(save);
                if (score >= beta) return beta; // fail hard beta-cutoff
                if (score > alpha)
                {
                    alpha = score;    // alpha acts like max in MiniMax
                }
            }

            return alpha;
        }

        protected int AlphaBetaMin(Board board, int alpha, int beta, int depthLeft, Color nextToMove)
        {
            if (depthLeft == 0) return Evaluate(board);

            var moves = board.MovesFor(nextToMove).ToList();
            var sortedMoves = moves.Select(m => Evaluate(board, m)).OrderBy(x => x.score);

            foreach (var (_, move) in sortedMoves)
            {
                var save = board.MakeMove(move);
                var score = AlphaBetaMax(board, alpha, beta, depthLeft - 1, nextToMove.Other());
                board.TakeBack(save);

                if (score <= alpha) return alpha; // fail hard alpha-cutoff
                if (score < beta)
                {
                    beta = score;        // beta acts like min in MiniMax
                }
            }

            return beta;
        }

        protected int Evaluate(Board position)
        {
            Reporter.NodeVisited();
            return Evaluator.Evaluate(position);
        }

        protected (int score, Board position, Move move) EvaluateCopy(Board board, Move m)
        {
            var position = board.Clone();
            position.MakeMove(m);
            return (Evaluator.Evaluate(position), position, m);
        }

        protected (int score, Move move) Evaluate(Board board, Move m)
        {
            var saved = board.MakeMove(m);
            var eval = Evaluator.Evaluate(board);
            board.TakeBack(saved);
            return (eval, m);
        }

    }
}
