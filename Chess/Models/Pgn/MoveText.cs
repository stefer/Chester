using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Chester.Models.Pgn
{
    public class MoveText : IEnumerable<PgnMove>, IReadOnlyList<PgnMove>
    {
        private List<PgnMove> _moves;

        public static readonly MoveText Empty = new MoveText(Enumerable.Empty<PgnMove>(), "*");

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<PgnHalfMove> AsHalfMoves()
        {
            List<PgnHalfMove> result = new(_moves.Count * 2);
            foreach (var move in _moves)
            {
                result.Add(move.White);
                if (move.Black is not null) result.Add(move.Black);
            }
            return result;
        }

        override public string ToString()
        {
            const int LineLength = 80;

            var sb = new StringBuilder();
            var lineLength = 0;

            foreach (var move in this)
            {
                var moveStr = move.ToString();

                if (lineLength + moveStr.Length + 1 > LineLength)
                {
                    sb.AppendLine();
                    lineLength = 0;
                }

                sb.Append(moveStr).Append(" ");
                lineLength += moveStr.Length + 1;
            }
            sb.Append(" ");
            sb.Append(Result);

            return sb.ToString();
        }
    }
}
