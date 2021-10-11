using Chess.Models.Pgn;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Chess.Openings
{
    [DebuggerDisplay("Alt {Move}")]
    public class OpeningAlternative
    {
        public OpeningAlternative(PgnHalfMove move)
        {
            Move = move;
        }

        public PgnHalfMove Move {  get; init; }
        public Pgn Pgn {  get; private set; }
        // Wins for black / white, draws?

        internal void SetPgn(Pgn pgn)
        {
            Pgn = pgn;
        }
    }

    public class OpeningSearch
    {
        private class HalfMoveComparer : IEqualityComparer<PgnHalfMove>
        {
            public bool Equals(PgnHalfMove x, PgnHalfMove y)
            {
                if (x is null && y is null) return true;
                if (x is null || y is null) return false;

                return Get(x).Equals(Get(y));
            }

            public int GetHashCode([DisallowNull] PgnHalfMove obj)
            {
                return Get(obj).GetHashCode();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static (PgnPosition, PgnPosition, PgnPiece, PgnMoveType) Get(PgnHalfMove move) => (move.From, move.To, move.Piece, move.Type);
        }

        [DebuggerDisplay("Node {Value} ({_alternatives.Count})")]
        private class Node
        {
            private Dictionary<PgnHalfMove, Node> _alternatives = new(new HalfMoveComparer());

            public Node()
            {
            }

            public Node(PgnHalfMove move)
            {
                Value = new OpeningAlternative(move);
            }

            public Node(PgnHalfMove move, Node subtree): this(move)
            {
                Add(subtree);
            }

            public OpeningAlternative Value { get; }

            public IEnumerable<Node> Alternatives => _alternatives.Values;

            public void Add(Node subtree)
            {
                if (TryGet(subtree.Value.Move, out Node existing))
                    existing.Add(subtree);
                else
                    _alternatives.Add(subtree.Value.Move, subtree);

            }

            public bool TryGet(PgnHalfMove halfMove, out Node node) => _alternatives.TryGetValue(halfMove, out node);

            public void SetPgn(Pgn pgn)
            {
                Value.SetPgn(pgn);
            }
        }

        private Node _root;

        public OpeningSearch(IEnumerable<Pgn> pgns)
        {
            Build(pgns);
        }

        public IEnumerable<OpeningAlternative> GetAlternatives(IEnumerable<PgnHalfMove> moves)
        {
            var edge = Find(_root, moves);

            if (edge == null) return Enumerable.Empty<OpeningAlternative>();

            return edge.Alternatives.Select(n => n.Value);
        }

        private Node Find(Node current, IEnumerable<PgnHalfMove> moves)
        {
            if (!moves.Any()) return current;

            if (!current.TryGet(moves.First(), out var subtree))
            {
                return null;
            }

            return Find(subtree, moves.Skip(1));
        }

        private void Build(IEnumerable<Pgn> pgns)
        {
            _root = new Node();

            foreach(Pgn pgn in pgns)
            {
                BuildNode(pgn);
            }
        }

        private void BuildNode(Pgn pgn)
        {
            var edge = BuildTree(_root, pgn.Moves.SelectMany(GetHalfMoves));
            edge.SetPgn(pgn);
        }

        private IEnumerable<PgnHalfMove> GetHalfMoves(PgnMove m)
        {
            yield return m.White;
            if (m.Black != null) yield return m.Black;
        }

        private Node BuildTree(Node current, IEnumerable<PgnHalfMove> moves)
        {
            if (!moves.Any()) return current;

            if (!current.TryGet(moves.First(), out var subtree))
            {
                subtree = BuildBranch(moves);
                current.Add(subtree);
            }

            return BuildTree(subtree, moves.Skip(1));
        }

        private Node BuildBranch(IEnumerable<PgnHalfMove> moves)
        {
            if (moves.Count() == 1)
                return new Node(moves.First());

            return new Node(moves.First(), BuildBranch(moves.Skip(1)));
        }
    }
}
