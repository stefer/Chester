using System.Collections.Generic;
using System.Linq;

namespace Chess
{
    public class Evaluator
    {
        private static readonly Dictionary<SquareState, int> PieceValues = new Dictionary<SquareState, int>()
        {
            [SquareState.King] = Evaluation.CheckMate,
            [SquareState.Queen] = 900,
            [SquareState.Rook] = 500,
            [SquareState.Bishop] = 320,
            [SquareState.Knight] = 310,
            [SquareState.Pawn] = 100,
        };

        /// <summary>
        /// https://www.chessprogramming.org/Simplified_Evaluation_Function
        /// </summary>
        private static readonly Dictionary<SquareState, int[][]> PositionalValues = new Dictionary<SquareState, int[][]>()
        {
            [SquareState.Pawn] = new int[][]
            {
                new int[] { 0,  0,  0,  0,  0,  0,  0,  0 },
                new int[] {50, 50, 50, 50, 50, 50, 50, 50 },
                new int[] {10, 10, 20, 30, 30, 20, 10, 10 },
                new int[] { 5,  5, 10, 25, 25, 10,  5,  5 },
                new int[] { 0,  0,  0, 20, 20,  0,  0,  0 },
                new int[] { 5, -5,-10,  0,  0,-10, -5,  5 },
                new int[] { 5, 10, 10,-20,-20, 10, 10,  5 },
                new int[] { 0,  0,  0,  0,  0,  0,  0,  0 }
            },
            [SquareState.Bishop] = new int[][]
            {
                new int[] {-20,-10,-10,-10,-10,-10,-10,-20 },
                new int[] {-10,  0,  0,  0,  0,  0,  0,-10 },
                new int[] {-10,  0,  5, 10, 10,  5,  0,-10 },
                new int[] {-10,  5,  5, 10, 10,  5,  5,-10 },
                new int[] {-10,  0, 10, 10, 10, 10,  0,-10 },
                new int[] {-10, 10, 10, 10, 10, 10, 10,-10 },
                new int[] {-10,  5,  0,  0,  0,  0,  5,-10 },
                new int[] {-20,-10,-10,-10,-10,-10,-10,-20 }
            },
            [SquareState.Knight] = new int[][]
            {
                new int[] {-50,-40,-30,-30,-30,-30,-40,-50 },
                new int[] {-40,-20,  0,  0,  0,  0,-20,-40 },
                new int[] {-30,  0, 10, 15, 15, 10,  0,-30 },
                new int[] {-30,  5, 15, 20, 20, 15,  5,-30 },
                new int[] {-30,  0, 15, 20, 20, 15,  0,-30 },
                new int[] {-30,  5, 10, 15, 15, 10,  5,-30 },
                new int[] {-40,-20,  0,  5,  5,  0,-20,-40 },
                new int[] {-50,-40,-30,-30,-30,-30,-40,-50 }
            },
            [SquareState.Rook] = new int[][]
            {
                new int[] {-50,-40,-30,-30,-30,-30,-40,-50 },
                new int[] {-40,-20,  0,  0,  0,  0,-20,-40 },
                new int[] {-30,  0, 10, 15, 15, 10,  0,-30 },
                new int[] {-30,  5, 15, 20, 20, 15,  5,-30 },
                new int[] {-30,  0, 15, 20, 20, 15,  0,-30 },
                new int[] {-30,  5, 10, 15, 15, 10,  5,-30 },
                new int[] {-40,-20,  0,  5,  5,  0,-20,-40 },
                new int[] {-50,-40,-30,-30,-30,-30,-40,-50 }
            },
            [SquareState.Queen] = new int[][]
            {
                new int[] {-20,-10,-10, -5, -5,-10,-10,-20 },
                new int[] {-10,  0,  0,  0,  0,  0,  0,-10 },
                new int[] {-10,  0,  5,  5,  5,  5,  0,-10 },
                new int[] { -5,  0,  5,  5,  5,  5,  0, -5 },
                new int[] {  0,  0,  5,  5,  5,  5,  0, -5 },
                new int[] {-10,  5,  5,  5,  5,  5,  0,-10 },
                new int[] {-10,  0,  5,  0,  0,  0,  0,-10 },
                new int[] {-20,-10,-10, -5, -5,-10,-10,-20 }
            },
            [SquareState.King] = new int[][]
            {
                new int[] {-30,-40,-40,-50,-50,-40,-40,-30 },
                new int[] {-30,-40,-40,-50,-50,-40,-40,-30 },
                new int[] {-30,-40,-40,-50,-50,-40,-40,-30 },
                new int[] {-30,-40,-40,-50,-50,-40,-40,-30 },
                new int[] {-20,-30,-30,-40,-40,-30,-30,-20 },
                new int[] {-10,-20,-20,-20,-20,-20,-20,-10 },
                new int[] { 20, 20,  0,  0,  0,  0, 20, 20 },
                new int[] { 20, 30, 10,  0,  0, 10, 30, 20 }
            }
        };

        public Evaluation Evaluate(Board b, Move m, Color color)
        {
            var byValue = b.Pieces.Select(PieceValue).Sum();
            var byPosition = b.Pieces.Select(Positional).Sum();
            return new Evaluation(byValue + byPosition, m);
        }

        private static int Positional(Piece s)
        {
            var rank = s.SquareState.IsWhite() ? s.Position.Rank : 7 - s.Position.Rank;
            return s.SquareState.Direction() * PositionalValues[s.SquareState.Piece()][rank][s.Position.File];
        }

        private static int PieceValue(Piece s) => s.SquareState.Direction() * PieceValues[s.SquareState.Piece()];

    }
}
