namespace Chester.Models
{
    public class Piece
    {
        public Piece(Position position, SquareState squareState)
        {
            Position = position;
            SquareState = squareState;
        }

        public Position Position { get; init; }
        public SquareState SquareState { get; init; }
    }
}
