using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Chess
{
    public class Board
    {
        private const SquareState X = SquareState.Invalid;
        private const SquareState K = SquareState.White | SquareState.King;
        private const SquareState Q = SquareState.White | SquareState.Queen;
        private const SquareState T = SquareState.White | SquareState.Tower;
        private const SquareState B = SquareState.White | SquareState.Bishop;
        private const SquareState N = SquareState.White | SquareState.Knight;
        private const SquareState P = SquareState.White | SquareState.Pawn;
        private const SquareState k = SquareState.King;
        private const SquareState q = SquareState.Queen;
        private const SquareState t = SquareState.Tower;
        private const SquareState b = SquareState.Bishop;
        private const SquareState n = SquareState.Knight;
        private const SquareState p = SquareState.Pawn;
        private const int Ranks = 8;
        private const int Files = 8;

        private SquareState[] _squares = new SquareState[Ranks * Files]
        {
            T, B, N, Q, K, N, B, T,
            P, P, P, P, P, P, P, P,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            p, p, p, p, p, p, p, p,
            t, b, n, q, k, n, b, t,
        };

        public Board() { }

        public Board(Board board)
        {
            _squares = (SquareState[])board._squares.Clone();
        }

        private int Index(Position p)
        {
            if (!p.Valid) throw new InvalidOperationException("Cannot index invalid position");
            return p.Rank * Files + p.File;
        }

        public SquareState At(Position p)
        {
            return p.Valid ? _squares[Index(p)] : SquareState.Invalid;
        }

        public IEnumerable<Move> MovesFor(Color c)
        {
            Func<SquareState, bool> byColor = c == Color.White ? PieceExtensions.IsWhite : PieceExtensions.IsBlack;
            var pieces = _squares
                .Select((square, from) => (Position(from), square))
                .Where(x => byColor(x.square));

            foreach(var (from, square) in pieces)
            {
                var moves = ValidMoves(from, square);
                foreach(var move in moves)
                {
                    yield return move;
                }
            }
        }

        private static Position Position(int idx)
        {
            return new Position(idx / Files, idx % Files);
        }

        private IEnumerable<Move> ValidMoves(Position from, SquareState square)
        {
            var direction = square.Direction();
            if (square.IsPawn())
            {
                var oneUp = from.Move(direction, 0);
                var twoUp = from.Move(direction*2, 0);
                if (At(oneUp).IsFree()) yield return new Move(square, from, oneUp);
                if (!square.HasMoved() && At(twoUp).IsFree()) yield return new Move(square, from, twoUp);

                var leftUp = from.Move(direction, 1);
                var rightUp = from.Move(direction, -1);
                if (square.IsAttack(At(leftUp))) yield return new Move(square, from, leftUp, true);
                if (square.IsAttack(At(rightUp))) yield return new Move(square, from, rightUp, true);

                // TODO: En Passant
            }
            else if (square.IsKnight())
            {
                var valids = new (int r, int f)[] { (1, 2), (2, 1), (-1, 2), (-2, 1), (1, -2), (2, -1) };
                foreach(var (r, f) in valids)
                {
                    var p = from.Move(r, f);
                    var to = At(p);
                    var isAttack = square.IsAttack(to);
                    if (to.IsFree() || isAttack) yield return new Move(square, from, p, isAttack);
                }
            }
            else if (square.IsBishop())
            {
                var dirs = new (int r, int f)[] { (-1, 1), (1, 1), (1, -1), (-1, -1) };
                foreach(var(r, f) in dirs)
                {
                    var p = from.Move(r, f);
                    while(p.Valid)
                    {
                        var to = At(p);
                        var isAttack = square.IsAttack(to);
                        if (to.IsFree() || square.IsAttack(to)) yield return new Move(square, from, p, isAttack);
                        if (isAttack || square.SameColor(to)) break;
                        p = p.Move(r, f);
                    }
                }
            }
        }

        public void MakeMove(Move m)
        {
            var fromSquare = At(m.From);
            Debug.Assert(fromSquare == m.FromSquare);

            _squares[Index(m.To)] = fromSquare | SquareState.Moved;
            _squares[Index(m.From)] = SquareState.Free;
        }

        public IEnumerable<Piece> Search(Func<SquareState, bool> predicate)
        {
            return _squares
                .Select((square, from) => new Piece(Position(from), square))
                .Where(p => predicate(p.SquareState));
        }

        public Board Clone()
        {
            return new Board(this);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("  abcdefgh");
            for (int r = 7; r >= 0; r--)
            {
                sb.Append($"{r+1}|");
                for (int f = 0; f < 8; f++)
                {
                    sb.Append(_squares[r * Files + f].AsString());
                }
                sb.AppendLine($"|{r+1}");
            }
            sb.AppendLine("  abcdefgh");
            return sb.ToString();
        }
    }
}
