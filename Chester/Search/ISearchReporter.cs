using Chester.Models;
using System.Collections.Generic;

namespace Chester.Search
{
    public interface ISearchReporter
    {
        public void Reset();
        public void CurrentMove(Move move, long moveNumber, int score);
        public void BestLine(int depth, int score, IEnumerable<Move> bestLine);
        public void NodeVisited();
    }
}
