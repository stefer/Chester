using Chess.Evaluations;
using Chess.Models;
using Chess.Search;

namespace Chess
{
    public class Game
    {
        private Board Board { get; init; } = new Board();
        public Color NextToMove { get; private set; } = Color.White;
        private readonly ISearch _search = new ParallellAlphaBetaMinMaxSearch(new SearchOptions(5), new Evaluator());

        public Game() { }

        public Game(Board board, Color nextToMove)
        {
            Board = board;
            NextToMove = nextToMove;
        }

        public SquareState At(Position p) => Board.At(p);

        public void MakeMove(Move move)
        {
            Board.MakeMove(move);
            NextToMove = NextToMove.Other();
        }

        public Evaluation Search()
        {
            return _search.Search(Board, NextToMove);
        }

        public override string ToString()
        {
            return Board.ToString();
        }
    }
}
