using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Models.Pgn
{
    public class MoveText: IEnumerable<PgnMove>, IReadOnlyList<PgnMove>
    {
        private List<PgnMove> _moves;

        public MoveText(IEnumerable<PgnMove> moves, string result)
        {
            _moves = moves.ToList();
            Result = result;
        }

        public string Result { get; }

        public PgnMove this[int index] => _moves[index];

        public int Count => _moves.Count;

        public IEnumerator<PgnMove> GetEnumerator() => _moves.GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => _moves.GetEnumerator();
    }
}
