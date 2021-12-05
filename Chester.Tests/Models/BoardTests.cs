using Chester.Models;
using Chester.Parsers;
using NUnit.Framework;
using System.Linq;

namespace Chester.Tests.Models;

using Chester.Tests.Extensions;

public class BoardTests
{
    [TestCase(0, "a1")]
    [TestCase(1, "b1")]
    [TestCase(7, "h1")]
    [TestCase(8, "a2")]
    [TestCase(63, "h8")]
    public void Position_Converts(int pos, string expected) => Assert.That(Board.Pos(pos), Is.EqualTo(Position.FromString(expected)));

    [TestCase(64)]
    [TestCase(-1)]
    public void Position_Invalid(int pos) => Assert.That(Board.Pos(pos).Valid, Is.False);

    [TestCase("a1", 0)]
    [TestCase("b1", 1)]
    [TestCase("h1", 7)]
    [TestCase("a2", 8)]
    [TestCase("a8", 56)]
    [TestCase("h8", 63)]
    public void Index_Converts(string pos, int expected) => Assert.That(Board.Index(Position.FromString(pos)), Is.EqualTo(expected));

    [Test]
    public void Index_Invalid_Throws() => Assert.That(() => Board.Index(Position.Invalid), Throws.InvalidOperationException);

    [Test]
    public void Move_SavesLine()
    {
        Board board = new();

        board.MakeMove(new Move(board.At("e2"), "e2", "e4"));
        var moves = board.Line;
        Assert.That(moves.Select(x => x.ToStringLong()), Is.EqualTo(new string[] { "e2e4" }));

        var move = board.MakeMove(new Move(board.At("c7"), "c7", "c6"));
        moves = board.Line;
        Assert.That(moves.Select(x => x.ToStringLong()), Is.EqualTo(new string[] { "e2e4", "c7c6" }));
    }

    [Test]
    public void TakeBack_SavesLine()
    {
        Board board = new();

        board.MakeMove(new Move(board.At("e2"), "e2", "e4"));
        Assert.That(board.Line.Select(x => x.ToStringLong()), Is.EqualTo(new string[] { "e2e4" }));

        var move = board.MakeMove(new Move(board.At("c7"), "c7", "c6"));
        Assert.That(board.Line.Select(x => x.ToStringLong()), Is.EqualTo(new string[] { "e2e4", "c7c6" }));

        board.TakeBack(move);
        Assert.That(board.Line.Select(x => x.ToStringLong()), Is.EqualTo(new string[] { "e2e4" }));
    }

    public class Moves
    {
        [Test]
        public void Knight_Black_D4()
        {
            var line = "e2e4 b8c6 g1f3 g8f6 b1c3 d7d6 d2d4 c8g4 f1e2 c6b4 e1g1";
            Board board = new();
            Play(board, line);

            var knightMoves = board.MovesFor(Color.Black)
                .Where(x => x.FromSquare.IsBlack() && x.FromSquare.IsKnight() && x.From.Equals(Position.FromString("b4")))
                .Select(x => x.ToString());

            Assert.That(knightMoves, Is.EquivalentTo(new[] { "nb4xa2", "nb4xc2", "nb4d3", "nb4d5", "nb4c6", "nb4a6" }));
        }
    }

    public class CastlingWhiteKingSide
    {
        [Test]
        public void Moves_Include_Castling()
        {
            var line = "1. e4 e5 2. Bc4 d6 3. Nf3 Nf6";

            Board board = new();
            Play(board, line);

            var moves = board.MovesFor(Color.White);
            var castling = moves.Single(x => x.MoveType.HasFlag(MoveType.CastleKingSide));
            Assert.That(castling.ToString(), Is.EqualTo("O-O"));
            Assert.That(castling.ToStringLong(), Is.EqualTo("e1g1"));
            Assert.That(castling.From.File, Is.EqualTo(4));
            Assert.That(castling.To.File, Is.EqualTo(6));
        }

        [Test]
        public void Move_Does_Castling()
        {
            var line = "1. e4 e5 2. Bc4 d6 3. Nf3 Nf6";

            Board board = new();
            Play(board, line);

            Assert.That(board.At("e1"), Has.Flag(SquareState.King));
            Assert.That(board.At("f1"), Is.EqualTo(SquareState.Free));
            Assert.That(board.At("g1"), Is.EqualTo(SquareState.Free));
            Assert.That(board.At("h1"), Has.Flag(SquareState.Rook));

            board.MakeMove(new Move(board.At("e1"), "e1", "g1", MoveType.CastleKingSide));

            Assert.That(board.At("e1"), Is.EqualTo(SquareState.Free));
            Assert.That(board.At("f1"), Has.Flag(SquareState.Rook));
            Assert.That(board.At("g1"), Has.Flag(SquareState.King));
            Assert.That(board.At("h1"), Is.EqualTo(SquareState.Free));
        }

        [Test]
        public void Move_Does_Castling_FromLine()
        {
            var line = "1. e4 e5 2. Bc4 d6 3. Nf3 Nf6 4. O-O";

            Board board = new();
            Play(board, line);

            Assert.That(board.At("e1"), Is.EqualTo(SquareState.Free));
            Assert.That(board.At("f1"), Has.Flag(SquareState.Rook));
            Assert.That(board.At("g1"), Has.Flag(SquareState.King));
            Assert.That(board.At("h1"), Is.EqualTo(SquareState.Free));
        }


        [Test]
        public void TakeBack_Castling_Resets()
        {
            var line = "1. e4 e5 2. Bc4 d6 3. Nf3 Nf6";

            Board board = new();
            Play(board, line);

            var move = board.MakeMove(new Move(board.At("e1"), "e1", "g1", MoveType.CastleKingSide));

            board.TakeBack(move);

            Assert.That(board.At("e1"), Has.Flag(SquareState.King));
            Assert.That(board.At("f1"), Is.EqualTo(SquareState.Free));
            Assert.That(board.At("g1"), Is.EqualTo(SquareState.Free));
            Assert.That(board.At("h1"), Has.Flag(SquareState.Rook));
        }
    }

    public class CastlingBlackQueenSide
    {
        [Test]
        public void Moves_Include_Castling()
        {
            var line = "e2e4 b8c6 g1f3 g8f6 b1c3 d7d6 d2d4 c8g4 f1e2 c6b4 e1g1 g4f3 e2f3 b4c2 d1c2 d8d7 c1e3";

            Board board = new();
            Play(board, line);

            var moves = board.MovesFor(Color.Black);
            var castling = moves.Single(x => x.MoveType.HasFlag(MoveType.CastleQueenSide));
            Assert.That(castling.ToString(), Is.EqualTo("O-O-O"));
            Assert.That(castling.ToStringLong(), Is.EqualTo("e8c8"));
            Assert.That(castling.From.File, Is.EqualTo(4));
            Assert.That(castling.To.File, Is.EqualTo(2));
        }

        [Test]
        public void Move_Does_Castling()
        {
            var line = "e2e4 b8c6 g1f3 g8f6 b1c3 d7d6 d2d4 c8g4 f1e2 c6b4 e1g1 g4f3 e2f3 b4c2 d1c2 d8d7 c1e3";

            Board board = new();
            Play(board, line);

            Assert.That(board.At("e8"), Has.Flag(SquareState.King));
            Assert.That(board.At("b1"), Is.EqualTo(SquareState.Free));
            Assert.That(board.At("c1"), Is.EqualTo(SquareState.Free));
            Assert.That(board.At("a1"), Has.Flag(SquareState.Rook));

            board.MakeMove(new Move(board.At("e8"), "e8", "c8", MoveType.CastleQueenSide));

            Assert.That(board.At("e8"), Is.EqualTo(SquareState.Free));
            Assert.That(board.At("d8"), Has.Flag(SquareState.Rook));
            Assert.That(board.At("c8"), Has.Flag(SquareState.King));
            Assert.That(board.At("b8"), Is.EqualTo(SquareState.Free));
            Assert.That(board.At("a8"), Is.EqualTo(SquareState.Free));
        }

        [Test]
        public void Move_Does_Castling_FromLine()
        {
            var line = "e2e4 b8c6 g1f3 g8f6 b1c3 d7d6 d2d4 c8g4 f1e2 c6b4 e1g1 g4f3 e2f3 b4c2 d1c2 d8d7 c1e3 O-O-O";

            Board board = new();
            Play(board, line);

            Assert.That(board.At("e8"), Is.EqualTo(SquareState.Free));
            Assert.That(board.At("d8"), Has.Flag(SquareState.Rook));
            Assert.That(board.At("c8"), Has.Flag(SquareState.King));
            Assert.That(board.At("b8"), Is.EqualTo(SquareState.Free));
            Assert.That(board.At("a8"), Is.EqualTo(SquareState.Free));
        }


        [Test]
        public void TakeBack_Castling_Resets()
        {
            var line = "e2e4 b8c6 g1f3 g8f6 b1c3 d7d6 d2d4 c8g4 f1e2 c6b4 e1g1 g4f3 e2f3 b4c2 d1c2 d8d7 c1e3";

            Board board = new();
            Play(board, line);

            var move = board.MakeMove(new Move(board.At("e8"), "e8", "c8", MoveType.CastleQueenSide));

            board.TakeBack(move);

            Assert.That(board.At("e8"), Has.Flag(SquareState.King));
            Assert.That(board.At("b1"), Is.EqualTo(SquareState.Free));
            Assert.That(board.At("c1"), Is.EqualTo(SquareState.Free));
            Assert.That(board.At("a1"), Has.Flag(SquareState.Rook));
        }
    }

    [Test]
    public void InvalidMove()
    {
        Board board = new();

        Play(board, "e2e4 c7c6 Qd1d3");

        var moves = board.MovesFor(Color.Black).ToList();

        Assert.That(moves.Select(x => x.ToStringLong()), Does.Not.Contain("b8b2"));
    }

    [Test]
    public void InvalidAfterCastling()
    {
        Board board = new();

        Play(board, "e2e4 b8c6 g1f3 g8f6 b1c3 d7d6 d2d4 c8g4 f1e2 c6b4 e1g1");

        var moves = board.MovesFor(Color.Black).ToList();

        Assert.That(moves.Select(x => x.ToStringLong()), Does.Not.Contain("b8b2"));
    }

    private static void Play(Board board, string moveText)
    {
        var reader = new MoveTextReader(moveText);
        var moves = reader.ReadAll();
        board.MakeMoves(moves);
    }
}
