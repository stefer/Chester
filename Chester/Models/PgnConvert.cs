using Chester.Models.Pgn;

namespace Chester.Models;

public static class PgnConvert
{
    public static Position ToModel(this PgnPosition pos) => Position.Create(pos.File, pos.Rank);
    public static SquareState ToModel(this PgnPiece piece) => piece switch
    {
        PgnPiece.Pawn => SquareState.Pawn,
        PgnPiece.King => SquareState.King,
        PgnPiece.Queen => SquareState.Queen,
        PgnPiece.Rook => SquareState.Rook,
        PgnPiece.Bishop => SquareState.Bishop,
        PgnPiece.Knight => SquareState.Knight,
        _ => throw new System.NotImplementedException(),
    };

    public static MoveType ToModel(this PgnMoveType moveType)
    {
        var type = MoveType.Normal;
        if (moveType.HasFlag(PgnMoveType.Take)) type |= MoveType.Capture;
        if (moveType.HasFlag(PgnMoveType.EnPassant)) type |= MoveType.EnPassant;
        if (moveType.HasFlag(PgnMoveType.CastleQueenSide)) type |= MoveType.CastleQueenSide;
        if (moveType.HasFlag(PgnMoveType.CastleKingSide)) type |= MoveType.CastleKingSide;
        if (moveType.HasFlag(PgnMoveType.PromotionBishop)) type |= MoveType.PromotionBishop;
        if (moveType.HasFlag(PgnMoveType.PromotionKnight)) type |= MoveType.PromotionKnight;
        if (moveType.HasFlag(PgnMoveType.PromotionQueen)) type |= MoveType.PromotionQueen;
        if (moveType.HasFlag(PgnMoveType.PromotionRook)) type |= MoveType.PromotionRook;
        if (moveType.HasFlag(PgnMoveType.Check)) type |= MoveType.Check;
        if (moveType.HasFlag(PgnMoveType.CheckMate)) type |= MoveType.CheckMate;

        return type;
    }
}
