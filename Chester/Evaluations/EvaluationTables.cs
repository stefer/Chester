using Chester.Models;
using System.Collections.Generic;
using System.Linq;

namespace Chester.Evaluations
{
    public abstract class EvaluationTables
    {
        public abstract int Positional(Piece s);
        public abstract int PieceValue(Piece s);

        public static readonly Dictionary<SquareState, int> PieceStageValues = new()
        {
            [SquareState.King] = 0,
            [SquareState.Queen] = 4,
            [SquareState.Rook] = 2,
            [SquareState.Bishop] = 1,
            [SquareState.Knight] = 1,
            [SquareState.Pawn] = 0
        };

        public static int MiddleGameThreshold = 32 - 1; // One major piece taken
        public static int EndGameThreshold = 10;        // Queen and one major piece

        public static EvaluationTables FromBoard(Board b)
        {
            var stageValue = b.Pieces.Select(p => PieceStageValues[p.SquareState.Piece()]).Sum();

            if (stageValue < EndGameThreshold) return EndGameEvaluationTables.Instance;
            if (stageValue < MiddleGameThreshold) return MiddleGameEvaluationTables.Instance;
            return InitialGameEvaluationTables.Instance;
        }
    }
}
