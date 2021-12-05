using System;

namespace Chester.Models;

public record SavedMove(Move Move, SquareState ToSquare) { };

[Flags]
public enum MoveType : ushort
{
    Normal          = 0b0000000000,
    Capture            = 0b0000000001,
    EnPassant       = 0b0000000010,
    
    PromotionQueen  = 0b0000000100,
    PromotionRook   = 0b0000001000,
    PromotionBishop = 0b0000010000,
    PromotionKnight = 0b0000100000,
    PromotionMask   = 0b0000111100,
    
    CastleQueenSide = 0b0001000000,
    CastleKingSide  = 0b0010000000,
    CastleMask      = 0b0011000000,

    Check           = 0b0100000000,
    CheckMate       = 0b1000000000,
}


public record Move
{
    public SquareState FromSquare { get; init; }
    public Position From { get; init; }
    public Position To { get; init; }
    public MoveType MoveType { get; init; }

    public Move(SquareState fromSquare, Position from, Position to, MoveType type = MoveType.Normal)
    {
        if (fromSquare.IsInvalid()) throw new ArgumentException("Cannot make move from invalid square", nameof(fromSquare));
        if (!from.Valid) throw new ArgumentException("Cannot make move from invalid position", nameof(from));
        if (!to.Valid) throw new ArgumentException("Cannot make move to invalid position", nameof(to));

        if (type.HasFlag(MoveType.CastleKingSide) && !((to.Rank == 0 || to.Rank == 7) && to.File == 6))
            throw new ArgumentException("Invalid castling move", nameof(to));
        if (type.HasFlag(MoveType.CastleQueenSide) && !((to.Rank == 0 || to.Rank == 7) && (to.File == 2)))
            throw new ArgumentException("Invalid castling move", nameof(to));

        FromSquare = fromSquare;
        From = from;
        To = to;
        MoveType = type;
    }

    public void Deconstruct(out SquareState fromSquare, out Position from, out Position to, out MoveType moveType) =>
        (fromSquare, from, to, moveType) = (FromSquare, From, To, MoveType);

    public override string ToString()
    {
        if ((MoveType & MoveType.CastleMask) != 0)
        {
            return (MoveType & MoveType.CastleMask) switch
            {
                MoveType.CastleKingSide => "O-O",
                MoveType.CastleQueenSide => "O-O-O",
                _ => throw new InvalidOperationException($"Invalid castling move type {MoveType}"),
            };
        }
        if (FromSquare.IsPawn() && MoveType.HasFlag(MoveType.Capture)) return $"{From.FileString}x{To}";
        if (FromSquare.IsPawn()) return $"{To}";

        return $"{FromSquare.AsString()}{From}{(MoveType.HasFlag(MoveType.Capture) ? 'x' : string.Empty)}{To}";
    }

    public string ToStringLong() => $"{From}{To}";
}
