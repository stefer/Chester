using Chess.Models.Pgn;

namespace Chess.Models
{
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
    }
}
