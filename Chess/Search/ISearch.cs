using Chess.Models;

namespace Chess.Search
{
    public interface ISearch
    {
        Evaluation Search(Board board, Color nextToMove);
    }
}
