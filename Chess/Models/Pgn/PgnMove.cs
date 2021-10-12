using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Chess.Models.Pgn
{
    public enum PgnPiece
    {
        King,
        Queen,
        Rook,
        Bishop,
        Knight,
        Pawn
    }

    public enum PgnMoveType: ushort
    {
        Normal          = 0b0000000000,
        Take            = 0b0000000001,
        EnPassant       = 0b0000000010,
        PromotionQueen  = 0b0000000100, 
        PromotionRook   = 0b0000001000,
        PromotionBishop = 0b0000010000,
        PromotionKnight = 0b0000100000,
        PromotionMask   = 0b0000111100,
        CastleQueenSide = 0b0001000000,
        CastleKingSide  = 0b0010000000,
        Check           = 0b0100000000,
        CheckMate       = 0b1000000000,

    }

    public record PgnHalfMove(Color Color, PgnPiece Piece, PgnPosition From, PgnPosition To = null, PgnMoveType Type = PgnMoveType.Normal, string Comment = null)
    {
        public static PgnHalfMove None = new((Color) (-1), (PgnPiece) (-1), null);

        private static readonly Dictionary<PgnPiece, string> PgnPieces = new()
        {
            [PgnPiece.King] = "K",
            [PgnPiece.Queen] = "Q",
            [PgnPiece.Rook] = "R",
            [PgnPiece.Bishop] = "B",
            [PgnPiece.Knight] = "N",
            [PgnPiece.Pawn] = ""
        };

        private static readonly Dictionary<PgnMoveType, char> Promotions = new()
        {
            [PgnMoveType.PromotionQueen] = 'Q',
            [PgnMoveType.PromotionRook] = 'R',
            [PgnMoveType.PromotionBishop] = 'B',
            [PgnMoveType.PromotionKnight] = 'N',
        };

        public override string ToString()
        {
            // examples : d4, Qa6xb7#, fxg1=Q+, O-O-O

            var sb = new StringBuilder();
            if (Type.HasFlag(PgnMoveType.CastleKingSide))
                sb.Append("O-O");
            else if (Type.HasFlag(PgnMoveType.CastleQueenSide))
                sb.Append("O-O-O");
            else
            {
                sb.Append(PgnPieces[Piece]);
                sb.Append(From);
                if (Type.HasFlag(PgnMoveType.Take)) sb.Append('x');
                if (To != null && !To.InValid) sb.Append(To);
                var promotion = Type & PgnMoveType.PromotionMask;
                if (promotion != PgnMoveType.Normal) sb.Append('=').Append(Promotions[promotion]);
            }

            if (Type.HasFlag(PgnMoveType.CheckMate)) sb.Append('#');
            if (Type.HasFlag(PgnMoveType.Check)) sb.Append('+');
            if (Comment != null) sb.Append(" ").Append('{').Append(Comment).Append('}');

            return sb.ToString();
        }

        public virtual bool Equals(PgnHalfMove other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;

            return (Color, Piece, From, To, To).Equals((other.Color, other.Piece, other.From, other.To, other.To));
        }

        public override int GetHashCode()
        {
            return (Color, Piece, From, To, To).GetHashCode();
        }
    }

    public record PgnMove(int Seq, PgnHalfMove White, PgnHalfMove Black, string Comment = null)
    {
        public override string ToString()
        {
            if (Black != null)
                return $"{Seq}. {White} {Black}";

            return $"{Seq}. {White}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<PgnHalfMove> HalfMoves()
        {
            yield return White;
            if (Black != null) yield return Black;
        }

        public virtual bool Equals(PgnMove other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;

            return (Seq, White, Black).Equals((other.Seq, other.White, other.Black));
        }

        public override int GetHashCode()
        {
            return (Seq, White, Black).GetHashCode();
        }
    }
}
