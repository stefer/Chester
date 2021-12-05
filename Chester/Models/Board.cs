using Chester.Models.Pgn;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Chester.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "I want some with small caps")]
public class Board
{
    private const SquareState K = SquareState.White | SquareState.King;
    private const SquareState Q = SquareState.White | SquareState.Queen;
    private const SquareState T = SquareState.White | SquareState.Rook;
    private const SquareState B = SquareState.White | SquareState.Bishop;
    private const SquareState N = SquareState.White | SquareState.Knight;
    private const SquareState P = SquareState.White | SquareState.Pawn;
    private const SquareState k = SquareState.Black | SquareState.King;
    private const SquareState q = SquareState.Black | SquareState.Queen;
    private const SquareState t = SquareState.Black | SquareState.Rook;
    private const SquareState b = SquareState.Black | SquareState.Bishop;
    private const SquareState n = SquareState.Black | SquareState.Knight;
    private const SquareState p = SquareState.Black | SquareState.Pawn;
    private const int Ranks = 8;
    private const int Files = 8;

    private static readonly SquareState[] Emptysquares = new SquareState[Ranks * Files]
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

    private readonly SquareState[] _squares = new SquareState[Ranks * Files]
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

    private readonly Stack<Move> _line = new();

    public Board() { }

    public Board(SquareState[] squares)
    {
        _squares = (SquareState[])squares.Clone();
    }

    public Board(Board board) : this(board._squares)
    {
        _line = new Stack<Move>(board._line);
    }

    public static Position Pos(int idx) => Position.Create(idx % Files, idx / Files);

    public static int Index(Position p)
    {
        if (!p.Valid) throw new InvalidOperationException("Cannot index invalid position");
        return p.Rank * Files + p.File;
    }

    public SquareState At(Position p) => p.Valid ? _squares[Index(p)] : SquareState.Invalid;

    public void Set(Position p, SquareState state) => _squares[Index(p)] = state;

    public IEnumerable<Move> MovesFor(Color c)
    {
        Func<SquareState, bool> byColor = c == Color.White ? PieceExtensions.IsWhite : PieceExtensions.IsBlack;
        var pieces = _squares
            .Select((square, from) => (from, square))
            .Where(x => byColor(x.square)).ToList();

        foreach (var (from, square) in pieces)
        {
            var moves = ValidMoves(Pos(from), square).ToList();
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
            var isStartPos = from.Rank == (square.IsWhite() ? 1 : 6);
            var isOneUpFree = At(oneUp).IsFree();
            if (isOneUpFree) yield return new Move(square, from, oneUp);
            if (isStartPos && isOneUpFree && At(twoUp).IsFree()) yield return new Move(square, from, twoUp);

            var leftUp = from.Move(1, direction);
            var rightUp = from.Move(-1, direction);
            if (square.IsAttack(At(leftUp))) yield return new Move(square, from, leftUp, MoveType.Capture);
            if (square.IsAttack(At(rightUp))) yield return new Move(square, from, rightUp, MoveType.Capture);

            // TODO: En Passant
            // TODO: Promote
        }
        else if (square.IsKnight())
        {
            var valids = new (int r, int f)[] { (1, 2), (2, 1), (-1, 2), (-2, 1), (1, -2), (2, -1), (-1, -2), (-2, -1) };
            foreach (var (r, f) in valids)
            {
                var p = from.Move(f, r);
                var to = At(p);
                var isAttack = square.IsAttack(to);
                if (to.IsFree() || isAttack) yield return new Move(square, from, p, isAttack ? MoveType.Capture : MoveType.Normal);
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
                    if (to.IsFree() || isAttack) yield return new Move(square, from, p, isAttack ? MoveType.Capture : MoveType.Normal);
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
                    if (to.IsFree() || isAttack) yield return new Move(square, from, p, isAttack ? MoveType.Capture : MoveType.Normal);
                    if (isAttack || square.SameColor(to)) break;
                    p = p.Move(f, r);
                }
            }
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
                    if (to.IsFree() || isAttack) yield return new Move(square, from, p, isAttack ? MoveType.Capture : MoveType.Normal);
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
                if (to.IsFree() || isAttack) yield return new Move(square, from, p, isAttack ? MoveType.Capture : MoveType.Normal);
            }

            var rookKingSide = from.With(f: 7);
            var rookQueenSide = from.With(f: 0);
            if (!HasMoved(square) && At(from.With(f: 5)).IsFree() && At(from.With(f: 6)).IsFree() && At(rookKingSide).IsRook())
                yield return new Move(square, from, from.With(f: 6), MoveType.CastleKingSide);
            if (!HasMoved(square) && At(from.With(f: 3)).IsFree() && At(from.With(f: 2)).IsFree() && At(from.With(f: 1)).IsFree() && At(rookQueenSide).IsRook())
                yield return new Move(square, from, from.With(f: 3), MoveType.CastleQueenSide);
        }
        else
        {
            throw new InvalidOperationException($"Cannot generate moves for {square.AsString()}");
        }
    }

    private bool HasMoved(SquareState square) => Line.Any(x => (x.FromSquare & (SquareState.Pieces | SquareState.Colors)) == (square & (SquareState.Pieces | SquareState.Colors)));

    public void MakeMoves(MoveText moveText)
    {
        foreach (var move in moveText.AsPlies())
        {
            var from = move.From?.ToModel();
            var to = move.To.ToModel();

            if (from == null)
            {
                // Movetext does not include from position if it obvious from game
                // We need to calculate that in that case
                var validMoves = MovesFor(move.Color);
                var fromMove = validMoves.FirstOrDefault(x => x.To == to && x.FromSquare.Piece() == move.Piece.ToModel());
                from = fromMove?.From;

                if (from == null)
                    throw new InvalidOperationException($"Cannot determine from position ply {move} in movetext {moveText}");
            }

            var fromState = At(from);

            var gameMove = new Move(fromState, from, to, move.Type.ToModel());
            MakeMove(gameMove);
        }
    }

    public SavedMove MakeMove(Move m)
    {
        var fromSquare = At(m.From);
        Debug.Assert(fromSquare == m.FromSquare);

        SavedMove sm = new(m, _squares[Index(m.To)]);

        if (m.MoveType.HasFlag(MoveType.CastleKingSide))
        {
            _squares[Index(m.To)] = fromSquare;
            _squares[Index(m.From)] = SquareState.Free;
            _squares[Index(m.From.With(f: 5))] = At(m.From.With(f: 7));
            _squares[Index(m.From.With(f: 7))] = SquareState.Free;
        }
        if (m.MoveType.HasFlag(MoveType.CastleQueenSide))
        {
            _squares[Index(m.To)] = fromSquare;
            _squares[Index(m.From)] = SquareState.Free;
            _squares[Index(m.From.With(f: 4))] = At(m.From.With(f: 0));
            _squares[Index(m.From.With(f: 0))] = SquareState.Free;
        }
        else
        {
            _squares[Index(m.To)] = fromSquare;
            _squares[Index(m.From)] = SquareState.Free;
        }

        _line.Push(m);
        return sm;
    }

    public void TakeBack(SavedMove sm)
    {
        var m = sm.Move;
        if (sm.Move.MoveType.HasFlag(MoveType.CastleKingSide))
        {
            _squares[Index(m.To)] = SquareState.Free;
            _squares[Index(m.From)] = m.FromSquare;
            _squares[Index(m.From.With(f: 7))] = At(m.From.With(f: 5));
            _squares[Index(m.From.With(f: 5))] = SquareState.Free;
        }
        if (sm.Move.MoveType.HasFlag(MoveType.CastleQueenSide))
        {
            _squares[Index(m.To)] = SquareState.Free;
            _squares[Index(m.From)] = m.FromSquare;
            _squares[Index(m.From.With(f: 0))] = At(m.From.With(f: 4));
            _squares[Index(m.From.With(f: 4))] = SquareState.Free;
        }
        else
        {
            _squares[Index(m.To)] = sm.ToSquare;
            _squares[Index(m.From)] = m.FromSquare;
        }
        _line.Pop();
    }

    public IEnumerable<Move> Line => _line.Reverse().ToList();

    public IEnumerable<Piece> Pieces => Search(s => s.IsOccupied());

    public IEnumerable<Piece> Search(Func<SquareState, bool> predicate) => _squares
            .Select((square, from) => new Piece(Pos(from), square))
            .Where(p => predicate(p.SquareState));

    public Board Clone() => new(this);

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine("  abcdefgh");
        for (var r = 7; r >= 0; r--)
        {
            sb.Append($"{r + 1}|");
            for (var f = 0; f < 8; f++)
            {
                sb.Append(_squares[r * Files + f].AsString());
            }
            sb.AppendLine($"|{r + 1}");
        }
        sb.AppendLine("  abcdefgh");
        return sb.ToString();
    }
}
