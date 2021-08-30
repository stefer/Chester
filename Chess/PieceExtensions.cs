using System;
using System.Collections.Generic;

namespace Chess
{
    public static class PieceExtensions
    {
        static Dictionary<SquareState, char> pcs = new Dictionary<SquareState, char>()
        {
            [SquareState.King] = 'k',
            [SquareState.Queen] = 'q',
            [SquareState.Rook] = 't',
            [SquareState.Bishop] = 'b',
            [SquareState.Knight] = 'n',
            [SquareState.Pawn] = 'p',
            [SquareState.White | SquareState.King] = 'K',
            [SquareState.White | SquareState.Queen] = 'Q',
            [SquareState.White | SquareState.Rook] = 'T',
            [SquareState.White | SquareState.Bishop] = 'B',
            [SquareState.White | SquareState.Knight] = 'N',
            [SquareState.White | SquareState.Pawn] = 'P',
            [SquareState.Free] = ' ',
            [SquareState.Invalid] = 'X',
        };

        public static bool IsFree(this SquareState p) => p == SquareState.Free;
        public static bool IsInvalid(this SquareState p) => p == SquareState.Invalid;
        public static bool IsOccupied(this SquareState p) => !(p.IsFree() || p.IsInvalid());
        public static bool IsWhite(this SquareState p) => p.HasFlag(SquareState.White) && p.IsOccupied();
        public static bool IsBlack(this SquareState p) => !p.HasFlag(SquareState.White) && p.IsOccupied();
        public static bool HasMoved(this SquareState p) => p.HasFlag(SquareState.Moved);

        public static bool IsKing(this SquareState p) => p.HasFlag(SquareState.King);
        public static bool IsQueen(this SquareState p) => p.HasFlag(SquareState.Queen);
        public static bool IsRook(this SquareState p) => p.HasFlag(SquareState.Rook);
        public static bool IsBishop(this SquareState p) => p.HasFlag(SquareState.Bishop);
        public static bool IsKnight(this SquareState p) => p.HasFlag(SquareState.Knight);
        public static bool IsPawn(this SquareState p) => p.HasFlag(SquareState.Pawn);

        public static SquareState Piece(this SquareState p) => p & SquareState.Pieces;

        public static bool SameColor(this SquareState self, SquareState other) => (self & SquareState.White) == (other & SquareState.White);
        public static bool IsAttack(this SquareState self, SquareState other) => other.IsOccupied() && !self.SameColor(other);

        public static int Direction(this SquareState self) => self.IsWhite() ? 1 : self.IsBlack() ? -1 : throw new InvalidOperationException("Square is neither black nor White");

        public static string AsString(this SquareState self) => pcs.TryGetValue(self & ~SquareState.Moved, out char rep) ? rep.ToString() : "-";
    }
}
