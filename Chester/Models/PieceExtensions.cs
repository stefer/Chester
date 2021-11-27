using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Chester.Models
{
    public static class PieceExtensions
    {
        private static readonly Dictionary<SquareState, char> pcs = new()
        {
            [SquareState.Black | SquareState.King] = 'k',
            [SquareState.Black | SquareState.Queen] = 'q',
            [SquareState.Black | SquareState.Rook] = 'r',
            [SquareState.Black | SquareState.Bishop] = 'b',
            [SquareState.Black | SquareState.Knight] = 'n',
            [SquareState.Black | SquareState.Pawn] = 'p',
            [SquareState.White | SquareState.King] = 'K',
            [SquareState.White | SquareState.Queen] = 'Q',
            [SquareState.White | SquareState.Rook] = 'R',
            [SquareState.White | SquareState.Bishop] = 'B',
            [SquareState.White | SquareState.Knight] = 'N',
            [SquareState.White | SquareState.Pawn] = 'P',
            [SquareState.Free] = ' ',
            [SquareState.Invalid] = 'X',
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFree(this SquareState p) => p == SquareState.Free;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInvalid(this SquareState p) => p == SquareState.Invalid;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOccupied(this SquareState p) => p != SquareState.Invalid && (p & SquareState.Pieces) != SquareState.Free;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhite(this SquareState p) => (p & SquareState.Colors) == SquareState.White;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBlack(this SquareState p) => (p & SquareState.Colors) == SquareState.Black;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsKing(this SquareState p) => p.HasFlag(SquareState.King);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsQueen(this SquareState p) => p.HasFlag(SquareState.Queen);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRook(this SquareState p) => p.HasFlag(SquareState.Rook);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBishop(this SquareState p) => p.HasFlag(SquareState.Bishop);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsKnight(this SquareState p) => p.HasFlag(SquareState.Knight);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPawn(this SquareState p) => p.HasFlag(SquareState.Pawn);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SquareState Piece(this SquareState p) => p & SquareState.Pieces;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SameColor(this SquareState self, SquareState other) => (self & SquareState.Colors) == (other & SquareState.Colors);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAttack(this SquareState self, SquareState other) => other.IsOccupied() && !self.SameColor(other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Direction(this SquareState self) => self.IsWhite() ? 1 : self.IsBlack() ? -1 : throw new InvalidOperationException("Square is neither black nor White");

        public static string AsString(this SquareState self) => pcs.TryGetValue(self, out var rep) ? rep.ToString() : "-";
        public static SquareState AsPiece(this char self)
        {
            var tuple = pcs.SingleOrDefault(t => t.Value == self, new KeyValuePair<SquareState, char>(SquareState.Invalid, char.MinValue));
            return tuple.Key;
        }
    }
}
