using Chess.Evaluations;
using Chess.Models;
using System;
using System.Linq;

namespace Chess
{
    public class Game
    {
        private Board Board { get; init; } = new Board();
        public Color NextToMove { get; private set; } = Color.White;
        private Evaluator _evaluator = new Evaluator();
        private int MaxDepth = 5;

        public void MakeMove(Move move)
        {
            Board.MakeMove(move);
            NextToMove = NextToMove.Other();
        }

        public Evaluation Search()
        {
            return AlphaBetaMax(-Evaluation.CheckMate, Evaluation.CheckMate, MaxDepth, NextToMove, null);
        }

        Evaluation AlphaBetaMax(int alpha, int beta, int depthLeft, Color nextToMove, Move move)
        {
            if (depthLeft == 0) return Evaluate(move);
            
            var alphaMove = new Evaluation(alpha, move);

            var sortedMoves = Board.MovesFor(nextToMove).Select(Evaluate).OrderBy(x => x.Value).Select(x => x.Move);
            foreach (Move m in sortedMoves)
            {
                var score = AlphaBetaMin(alphaMove.Value, beta, depthLeft - 1, nextToMove.Other(), m);
                if (score.Value >= beta) return new Evaluation(beta, m); // fail hard beta-cutoff
                if (score.Value > alphaMove.Value) alphaMove = score;    // alpha acts like max in MiniMax
            }

            return alphaMove;
        }

        Evaluation AlphaBetaMin(int alpha, int beta, int depthLeft, Color nextToMove, Move move)
        {
            if (depthLeft == 0) return Evaluate(move);

            var betaMove = new Evaluation(beta, move);

            var sortedMoves = Board.MovesFor(nextToMove).Select(Evaluate).OrderByDescending(x => x.Value).Select(x => x.Move);
            foreach (Move m in sortedMoves)
            {
                var score = AlphaBetaMax(alpha, betaMove.Value, depthLeft - 1, nextToMove.Other(), m);

                if (score.Value <= alpha)  return new Evaluation(alpha, m); // fail hard alpha-cutoff
                if (score.Value < betaMove.Value)  betaMove = score;        // beta acts like min in MiniMax
            }

            return betaMove;
        }

        private Evaluation Evaluate(Move m)
        {
            var position = Board.Clone();
            position.MakeMove(m);
            return _evaluator.Evaluate(position, m);
        }

        public override string ToString()
        {
            return Board.ToString();
        }
    }
}
