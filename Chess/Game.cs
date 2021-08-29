namespace Chess
{
    public class Game
    {
        public Board Board { get; init; } = new Board();
        public Evaluator _evaluator = new Evaluator();

        public Evaluation Evaluate(Move m)
        {
            var board = Board.Clone();
            board.MakeMove(m);
            return _evaluator.Evaluate(Board, m, m.FromSquare.IsWhite() ? Color.White : Color.Black);
        }
    }
}
