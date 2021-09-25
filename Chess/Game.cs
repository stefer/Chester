using Chess.Evaluations;
using Chess.Models;
using System;

namespace Chess
{
    public class Game
    {
        private Board Board { get; init; } = new Board();
        public Color NextToMove { get; private set; } = Color.White;
        private Evaluator _evaluator = new Evaluator();
        private int MaxDepth = 5;

        public Evaluation Evaluate(Move m)
        {
            var position = Board.Clone();
            position.MakeMove(m);
            return _evaluator.Evaluate(position, m);
        }

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
            if (depthLeft == 0)
            {
                var position = Board.Clone();
                position.MakeMove(move);
                return _evaluator.Evaluate(position, move);
            }
            var alphaMove = new Evaluation(alpha, move);

            foreach (Move m in Board.MovesFor(nextToMove))
            {
                var score = AlphaBetaMin(alphaMove.Value, beta, depthLeft - 1, nextToMove.Other(), m);
                if (score.Value >= beta)
                    return new Evaluation(beta, m);   // fail hard beta-cutoff
                if (score.Value > alphaMove.Value)
                    alphaMove = score; // alpha acts like max in MiniMax
            }

            return alphaMove;
        }

        Evaluation AlphaBetaMin(int alpha, int beta, int depthLeft, Color nextToMove, Move move)
        {
            if (depthLeft == 0)
            {
                var position = Board.Clone();
                position.MakeMove(move);
                return _evaluator.Evaluate(position, move);
            }
            var betaMove = new Evaluation(beta, move);

            foreach (Move m in Board.MovesFor(nextToMove))
            {
                var score = AlphaBetaMax(alpha, betaMove.Value, depthLeft - 1, nextToMove.Other(), m);

                if (score.Value <= alpha)
                    return new Evaluation(alpha, move); // fail hard alpha-cutoff
                if (score.Value < betaMove.Value)
                    betaMove = score; // beta acts like min in MiniMax
            }
            return betaMove;
        }

        public override string ToString()
        {
            return Board.ToString();
        }
    }
}
