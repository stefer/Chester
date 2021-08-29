using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
    public class Evaluator
    {
        private static Dictionary<SquareState, int> PieceValues = new Dictionary<SquareState, int>()
        {
            [SquareState.King] = Evaluation.CheckMate,
            [SquareState.Queen] = 900,
            [SquareState.Tower] = 500,
            [SquareState.Bishop] = 320,
            [SquareState.Knight] = 310,
            [SquareState.Pawn] = 100,
        };

        public Evaluation Evaluate(Board b, Move m, Color color)
        {
            var positional = b.Search(s => s.IsOccupied()).Select(s => s.SquareState.Direction() * PieceValues[s.SquareState.Piece()]).Sum();
            return new Evaluation(positional, m);
        }
    }
}
