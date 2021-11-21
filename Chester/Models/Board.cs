using Chester.Models.Pgn;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Chester.Models
{
    public class Board
    {
        private const SquareState K = SquareState.White | SquareState.King;
        private const SquareState Q = SquareState.White | SquareState.Queen;
        private const SquareState T = SquareState.White | SquareState.Rook;
        private const SquareState B = SquareState.White | SquareState.Bishop;
        private const SquareState N = SquareState.White | SquareState.Knight;
        private const SquareState P = SquareState.White | SquareState.Pawn;
        private const SquareState k = SquareState.King;
        private const SquareState q = SquareState.Queen;
        private const SquareState t = SquareState.Rook;
        private const SquareState b = SquareState.Bishop;
        private const SquareState n = SquareState.Knight;
        private const SquareState p = SquareState.Pawn;
        private const int Ranks = 8;
        private const int Files = 8;

        private static SquareState[] Emptysquares = new SquareState[Ranks * Files]
        {
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
        };

        public static Board Empty() => new(Emptysquares);

        private SquareState[] _squares = new SquareState[Ranks * Files]
        {
            T, N, B, Q, K, B, N, T,
            P, P, P, P, P, P, P, P,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            p, p, p, p, p, p, p, p,
            t, n, b, q, k, b, n, t,
        };

        private Stack<Move> _line = new Stack<Move>();

        public Board() { }

        public Board(SquareState[] squares)
        {
            _squares = (SquareState[])squares.Clone();
        }

        public Board(Board board) : this(board._squares) 
        { 
            _line = new Stack<Move>(board._line);
        }

        public static Position Pos(int idx)
        {
            return Position.Create(idx % Files, idx / Files);
        }

        public static int Index(Position p)
        {
            if (!p.Valid) throw new InvalidOperationException("Cannot index invalid position");
            return p.Rank * Files + p.File;
        }

        public SquareState At(Position p)
        {
            return p.Valid ? _squares[Index(p)] : SquareState.Invalid;
        }

        public void Set(Position p, SquareState state)
        {
            _squares[Index(p)] = state;
        }

        public IEnumerable<Move> MovesFor(Color c)
        {
            Func<SquareState, bool> byColor = c == Color.White ? PieceExtensions.IsWhite : PieceExtensions.IsBlack;
            var pieces = _squares
                .Select((square, from) => (from, square))
                .Where(x => byColor(x.square));

            foreach (var (from, square) in pieces)
            {
                var moves = ValidMoves(Pos(from), square);
                foreach (var move in moves)
                {
                    yield return move;
                }
            }
        }

        private IEnumerable<Move> ValidMoves(Position from, SquareState square)
        {
            var direction = square.Direction();
            if (square.IsPawn())
            {
                var oneUp = from.Move(0, direction);
                var twoUp = from.Move(0, direction * 2);
                if (At(oneUp).IsFree()) yield return new Move(square, from, oneUp);
                if (!square.HasMoved() && At(twoUp).IsFree()) yield return new Move(square, from, twoUp);

                var leftUp = from.Move(1, direction);
                var rightUp = from.Move(-1, direction);
                if (square.IsAttack(At(leftUp))) yield return new Move(square, from, leftUp, true);
                if (square.IsAttack(At(rightUp))) yield return new Move(square, from, rightUp, true);

                // TODO: En Passant
            }
            else if (square.IsKnight())
            {
                var valids = new (int r, int f)[] { (1, 2), (2, 1), (-1, 2), (-2, 1), (1, -2), (2, -1), (-1, -2), (-2, -1) };
                foreach (var (r, f) in valids)
                {
                    var p = from.Move(f, r);
                    var to = At(p);
                    var isAttack = square.IsAttack(to);
                    if (to.IsFree() || isAttack) yield return new Move(square, from, p, isAttack);
                }
            }
            else if (square.IsBishop())
            {
                var dirs = new (int r, int f)[] { (-1, 1), (1, 1), (1, -1), (-1, -1) };
                foreach (var (r, f) in dirs)
                {
                    var p = from.Move(f, r);
                    while (p.Valid)
                    {
                        var to = At(p);
                        var isAttack = square.IsAttack(to);
                        if (to.IsFree() || isAttack) yield return new Move(square, from, p, isAttack);
                        if (isAttack || square.SameColor(to)) break;
                        p = p.Move(f, r);
                    }
                }
            }
            else if (square.IsRook())
            {
                var dirs = new (int r, int f)[] { (0, 1), (0, -1), (1, 0), (-1, 0) };
                foreach (var (r, f) in dirs)
                {
                    var p = from.Move(f, r);
                    while (p.Valid)
                    {
                        var to = At(p);
                        var isAttack = square.IsAttack(to);
                        if (to.IsFree() || isAttack) yield return new Move(square, from, p, isAttack);
                        if (isAttack || square.SameColor(to)) break;
                        p = p.Move(f, r);
                    }
                }

                // TODO: Castle
            }
            else if (square.IsQueen())
            {
                var dirs = new (int r, int f)[] { (-1, 1), (1, 1), (1, -1), (-1, -1), (0, 1), (0, -1), (1, 0), (-1, 0) };
                foreach (var (r, f) in dirs)
                {
                    var p = from.Move(f, r);
                    while (p.Valid)
                    {
                        var to = At(p);
                        var isAttack = square.IsAttack(to);
                        if (to.IsFree() || isAttack) yield return new Move(square, from, p, isAttack);
                        if (isAttack || square.SameColor(to)) break;
                        p = p.Move(f, r);
                    }
                }
            }
            else if (square.IsKing())
            {
                var dirs = new (int r, int f)[] { (-1, 1), (1, 1), (1, -1), (-1, -1), (0, 1), (0, -1), (1, 0), (-1, 0) };
                foreach (var (r, f) in dirs)
                {
                    var p = from.Move(f, r);
                    var to = At(p);
                    var isAttack = square.IsAttack(to);
                    if (to.IsFree() || isAttack) yield return new Move(square, from, p, isAttack);
                }

                // TODO: Castle
            }
            else
            {
                throw new InvalidOperationException($"Cannot generate moves for {square.AsString()}");
            }
        }

        public void MakeMoves(MoveText moveText)
        {
            foreach (var move in moveText.AsPlies())
            {
                var from = move.From.ToModel();
                var to = move.To.ToModel();
                var fromState = At(from);
                var toState = At(to);

                var gameMove = new Move(fromState, from, to, fromState.IsAttack(toState));
                MakeMove(gameMove);
            }
        }

        public SavedMove MakeMove(Move m)
        {
            var fromSquare = At(m.From);
            Debug.Assert(fromSquare == m.FromSquare);

            SavedMove sm = new(m, _squares[Index(m.To)]);

            _squares[Index(m.To)] = fromSquare | SquareState.Moved;
            _squares[Index(m.From)] = SquareState.Free;

            _line.Push(m);
            return sm;
        }

        public void TakeBack(SavedMove sm)
        {
            _squares[Index(sm.Move.To)] = sm.ToSquare;
            _squares[Index(sm.Move.From)] = sm.Move.FromSquare;
            _line.Pop();
        }

        public IEnumerable<Move> Line => _line.Reverse().ToList();

        public IEnumerable<Piece> Pieces => Search(s => s.IsOccupied());

        public IEnumerable<Piece> Search(Func<SquareState, bool> predicate)
        {
            return _squares
                .Select((square, from) => new Piece(Pos(from), square))
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
                sb.Append($"{r + 1}|");
                for (int f = 0; f < 8; f++)
                {
                    sb.Append(_squares[r * Files + f].AsString());
                }
                sb.AppendLine($"|{r + 1}");
            }
            sb.AppendLine("  abcdefgh");
            return sb.ToString();
        }
    }
}
