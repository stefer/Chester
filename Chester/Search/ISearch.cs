using Chester.Models;

namespace Chester.Search
{
    public interface ISearch
    {
        Evaluation Search(Board board, Color nextToMove);
    }
}
