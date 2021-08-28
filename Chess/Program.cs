using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Chess
{
    [Flags]
    public enum SquareState : byte
    {
        Invalid = 0b11111111,
        Free = 0b00000000,
        White = 0b10000000,
        Moved = 0b01000000,

        King   = 0b00100000,
        Queen  = 0b00010000,
        Tower  = 0b00001000,
        Bishop = 0b00000100,
        Knight = 0b00000010,
        Pawn   = 0b00000001,
    }

    public enum Color
    {
        White,
        Black
    }

    public static class PieceExtensions
    {
        static Dictionary<SquareState, char> pcs = new Dictionary<SquareState, char>()
        {
            [SquareState.King] = 'k',
            [SquareState.Queen] = 'q',
            [SquareState.Tower] = 't',
            [SquareState.Bishop] = 'b',
            [SquareState.Knight] = 'n',
            [SquareState.Pawn] = 'p',
            [SquareState.White | SquareState.King] = 'K',
            [SquareState.White | SquareState.Queen] = 'Q',
            [SquareState.White | SquareState.Tower] = 'T',
            [SquareState.White | SquareState.Bishop] = 'B',
            [SquareState.White | SquareState.Knight] = 'N',
            [SquareState.White | SquareState.Pawn] = 'P',
            [SquareState.Free] = ' ',
            [SquareState.Invalid] = 'X',
        };

        public static bool IsFree(this SquareState p) => p == SquareState.Free;
        public static bool IsInvalid(this SquareState p) => p == SquareState.Invalid;
        public static bool IsOccupied(this SquareState p) => !(p.IsFree() || p.IsInvalid());
        public static bool IsWhite(this SquareState p) => p.HasFlag(SquareState.White) && p.IsOccupied();
        public static bool IsBlack(this SquareState p) => !p.HasFlag(SquareState.White) && p.IsOccupied();
        public static bool HasMoved(this SquareState p) => p.HasFlag(SquareState.Moved);

        public static bool IsKing(this SquareState p) => p.HasFlag(SquareState.King);
        public static bool IsQueen(this SquareState p) => p.HasFlag(SquareState.Queen);
        public static bool IsTower(this SquareState p) => p.HasFlag(SquareState.Tower);
        public static bool IsBishop(this SquareState p) => p.HasFlag(SquareState.Bishop);
        public static bool IsKnight(this SquareState p) => p.HasFlag(SquareState.Knight);
        public static bool IsPawn(this SquareState p) => p.HasFlag(SquareState.Pawn);

        public static bool SameColor(this SquareState self, SquareState other) => (self & SquareState.White) == (other & SquareState.White);
        public static bool IsAttack(this SquareState self, SquareState other) => other.IsOccupied() && !self.SameColor(other);

        public static int Direction(this SquareState self) => self.IsWhite() ? 1 : self.IsBlack() ? -1 : throw new InvalidOperationException("Square is neither black nor White");

        public static string AsString(this SquareState self) => pcs.TryGetValue(self & ~SquareState.Moved, out char rep) ? rep.ToString() : "-";
    }

    public record Move
    {
        public SquareState FromSquare { get; init; } 
        public Position From { get; init; } 
        public Position To { get; init; }
        public bool Capture { get; init; }

        public Move(SquareState fromSquare, Position from, Position to, bool capture = false)
        {
            if (fromSquare.IsInvalid()) throw new ArgumentException("Cannot make move from invalid square", nameof(fromSquare));
            if (!from.Valid) throw new ArgumentException("Cannot make move from invalid position", nameof(from));
            if (!to.Valid) throw new ArgumentException("Cannot make move to invalid position", nameof(to));
            FromSquare = fromSquare;
            From = from;
            To = to;
            Capture = capture;
        }

        public void Deconstruct(out SquareState fromSquare, out Position from, out Position to, out bool capture) =>
            (fromSquare, from, to, capture) = (FromSquare, From, To, Capture);

        public override string ToString()
        {
            if (FromSquare.IsPawn() && Capture) return $"{From.FileString}{(Capture ? 'x' : string.Empty)}{To}";
            if (FromSquare.IsPawn()) return $"{To}";

            return $"{FromSquare.AsString()}{From}{(Capture ? 'x' : string.Empty)}{To}";
        }
    }

    public record Position
    {
        static readonly int[] ranks = { 1, 2, 3, 4, 5, 6, 7, 8 };
        static readonly char[] files = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };

        public int Rank { get; init; }
        public int File { get; init; }

        public Position(int rank, int file)
        {
            Rank = rank;
            File = file;
        }

        public void Deconstruct(out int rank, out int file) => (rank, file) = (Rank, File);

        public bool Valid => Rank >= 0 && Rank < 8 && File >= 0 && File < 8;

        public Position Move(int dr = 0, int df = 0) => this with { Rank = Rank + dr, File = File + df };

        public override string ToString() => Valid ? $"{files[File]}{ranks[Rank]}" : "--";
        public string FileString => $"{files[File]}";
    }

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
        private const int Ranks = 10;
        private const int Files = 12;

        private SquareState[] _squares = new SquareState[Ranks * Files]
        {
            X, X, X, X, X, X, X, X, X, X, X, X,
            X, X, T, B, N, Q, K, N, B, T, X, X,
            X, X, P, P, P, P, P, P, P, P, X, X,
            X, X, 0, 0, 0, 0, 0, 0, 0, 0, X, X,
            X, X, 0, 0, 0, 0, 0, 0, 0, 0, X, X,
            X, X, 0, 0, 0, 0, 0, 0, 0, 0, X, X,
            X, X, 0, 0, 0, 0, 0, 0, 0, 0, X, X,
            X, X, p, p, p, p, p, p, p, p, X, X,
            X, X, t, b, n, q, k, n, b, t, X, X,
            X, X, X, X, X, X, X, X, X, X, X, X,
        };

        private int Index(Position p)
        {
            if (!p.Valid) throw new InvalidOperationException("Cannot index invalid position");
            return (p.Rank + 1) * Files + p.File + 2;
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
            return new Position((idx / Files) - 1, (idx % Files) - 2);
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

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("  abcdefgh");
            for (int r = 8; r > 0; r--)
            {
                sb.Append($"{r}|");
                for (int f = 2; f < 10; f++)
                {
                    sb.Append(_squares[r * Files + f].AsString());
                }
                sb.AppendLine($"|{r}");
            }
            sb.AppendLine("  abcdefgh");
            return sb.ToString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

            Board b = new Board();
            var rand = new Random();

            Console.WriteLine(b);

            var color = Color.White;
            while(true)
            {
                var moves = b.MovesFor(color).ToList();
                // Console.WriteLine(string.Join(",", moves));

                if (moves.Count == 0) break;

                var move = moves[rand.Next(0, moves.Count)];
                Console.WriteLine($"Making move {move}");

                Console.ReadKey();

                b.MakeMove(move);

                Console.WriteLine(b);
                color = color == Color.White ? Color.Black : Color.White;
            }

            Console.Beep();
        }
    }
}
