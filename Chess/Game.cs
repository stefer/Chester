namespace Chess
{
    public class Game
    {
        public Board Board { get; init; } = new Board();
        public Evaluator _evaluator = new Evaluator();

        public Evaluation Evaluate(Move m)
        {
            var evalBoard = Board.Clone();
            evalBoard.MakeMove(m);
            return _evaluator.Evaluate(evalBoard, m, m.FromSquare.IsWhite() ? Color.White : Color.Black);
        }
    }
}
