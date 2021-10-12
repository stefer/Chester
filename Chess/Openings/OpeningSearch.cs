using Chess.Models.Pgn;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Chess.Openings
{
    public record OpeningResults(int WinsWhite, int WinsBlack, int Draws)
    {
        public static OpeningResults operator +(OpeningResults left, OpeningResults right) => 
            new(left.WinsWhite + right.WinsWhite, right.WinsBlack + left.WinsBlack, right.Draws + left.Draws);
    }

    [DebuggerDisplay("Alt {Move}")]
    public class OpeningAlternative
    {
        public OpeningAlternative(PgnHalfMove move)
        {
            Move = move;
        }

        public PgnHalfMove Move { get; init; }
        public PgnGame Pgn { get; private set; }
        public OpeningResults Results { get; private set; }

        internal void SetPgn(PgnGame pgn)
        {
            Pgn = pgn;
        }

        internal void SetResult(OpeningResults result)
        {
            Results = result;
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

                return (x.From, x.To, x.Piece).Equals((y.From, y.To, y.Piece));
            }

            public int GetHashCode([DisallowNull] PgnHalfMove obj)
            {
                return (obj.From, obj.To, obj.Piece).GetHashCode();
            }
        }

        [DebuggerDisplay("Node {Value} ({_alternatives.Count})")]
        private class Node
        {
            private Dictionary<PgnHalfMove, Node> _alternatives = new(new HalfMoveComparer());

            public Node()
            {
                Value = new OpeningAlternative(PgnHalfMove.None);
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

            public void SetPgn(PgnGame pgn)
            {
                Value.SetPgn(pgn);
            }
        }

        private Node _root;

        public OpeningSearch(IEnumerable<PgnGame> pgns)
        {
            Build(pgns);
            Calculate(_root);
        }

        private void Calculate(Node node)
        {
            OpeningResults result = new(0, 0, 0);
            if (node?.Value.Pgn != null)
            {
                result += GetResult(node.Value.Pgn.Result);
            }
            foreach(var subnode in node.Alternatives)
            {
                Calculate(subnode);
                result += subnode.Value.Results;
            }
            node?.Value.SetResult(result);
        }

        private OpeningResults GetResult(PgnResult result)
        {
            if (result == PgnResult.WhiteWin) return new OpeningResults(1, 0, 0);
            if (result == PgnResult.BlackWin) return new OpeningResults(0, 1, 0);
            if (result == PgnResult.Draw) return new OpeningResults(0, 0, 1);
            return new OpeningResults(0, 0, 0);
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

        private void Build(IEnumerable<PgnGame> pgns)
        {
            _root = new Node();

            foreach(PgnGame pgn in pgns)
            {
                BuildNode(pgn);
            }
        }

        private void BuildNode(PgnGame pgn)
        {
            var edge = BuildTree(_root, pgn.Moves.AsHalfMoves());
            edge.SetPgn(pgn);
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
