using Chester.Evaluations;
using Chester.Models;
using Chester.Search;
using Chester.Services;

namespace Chester
{
    public class Game
    {
        private Board Board { get; init; } = new Board();
        public Color NextToMove { get; private set; } = Color.White;
        private readonly ISearch _search;

        public Game(ISearchReporter reporter) 
        {
            _search = new ParallellAlphaBetaMinMaxSearch(new SearchOptions(4), new Evaluator(), reporter);
        }

        public Game(Board board, Color nextToMove, ISearchReporter reporter): this(reporter)
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
