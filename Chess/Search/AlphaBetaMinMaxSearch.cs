using Chess.Evaluations;
using Chess.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.Search
{

    public class AlphaBetaMinMaxSearch: ISearch
    {
        protected SearchOptions Options { get; }
        private IEvaluator Evaluator { get; }

        public AlphaBetaMinMaxSearch(SearchOptions options, IEvaluator evaluator)
        {
            Options = options;
            Evaluator = evaluator;
        }

        public virtual Evaluation Search(Board board, Color nextToMove)
        {
            var bestScore = -Evaluation.CheckMate;
            Move bestMove = null;
            var l = new object();

            var sortedMoves = board.MovesFor(nextToMove).Select(m => Evaluate(board, m)).OrderByDescending(x => x.score).ToList();

            if (Options.MaxDepth == 0)
            {
                var move = sortedMoves.FirstOrDefault();
                return new Evaluation(move.score, move.move);
            }

            foreach(var x in sortedMoves)
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
            };

            return new Evaluation(bestScore, bestMove);
        }

        protected int AlphaBetaMax(Board board, int alpha, int beta, int depthLeft, Color nextToMove)
        {
            if (depthLeft == 0) return Evaluate(board, nextToMove);

            var moves = board.MovesFor(nextToMove).ToList();
            var sortedMoves = moves.Select(m => Evaluate(board, m)).OrderByDescending(x => x.score);

            foreach (var (_, position, _) in sortedMoves)
            {
                var score = AlphaBetaMin(position, alpha, beta, depthLeft - 1, nextToMove.Other());
                if (score >= beta) return beta; // fail hard beta-cutoff
                if (score > alpha) alpha = score;    // alpha acts like max in MiniMax
            }

            return alpha;
        }

        protected int AlphaBetaMin(Board board, int alpha, int beta, int depthLeft, Color nextToMove)
        {
            if (depthLeft == 0) return Evaluate(board, nextToMove);

            var moves = board.MovesFor(nextToMove).ToList();
            var sortedMoves = moves.Select(m => Evaluate(board, m)).OrderBy(x => x.score);

            foreach (var (_, position, _) in sortedMoves)
            {
                var score = AlphaBetaMax(position, alpha, beta, depthLeft - 1, nextToMove.Other());

                if (score <= alpha) return alpha; // fail hard alpha-cutoff
                if (score < beta) beta = score;        // beta acts like min in MiniMax
            }

            return beta;
        }

        protected int Evaluate(Board position, Color turnToMove)
        {
            var dir = turnToMove == Color.White ? 1 : -1;
            return dir * Evaluator.Evaluate(position);
        }

        protected (int score, Board position, Move move) Evaluate(Board board, Move m)
        {
            var dir = m.FromSquare.Direction();
            var position = board.Clone();
            position.MakeMove(m);
            return (dir * Evaluator.Evaluate(position), position, m);
        }
    }
}
