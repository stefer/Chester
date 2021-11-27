using Chester.Parsers;
using NUnit.Framework;

namespace Chester.Tests.Parsers;

using Chester.Models;
using Chester.Tests.Extensions;

public class FenParserTests
{
    public class ParseStandard
    {
        private Fen _fen;

        [OneTimeSetUp]
        public void SetUp()
        {
            var parser = new FenParser("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

            _fen = parser.Parse();
        }

        [Test]
        public void Sets_Board() => Assert.That(_fen.NextToMove, Is.EqualTo(Color.White));

        [Test]
        public void Sets_NoEnpassant() => Assert.That(_fen.EnPassantTarget.Valid, Is.False);

        [Test]
        public void Sets_HalfMOveClock() => Assert.That(_fen.HalfMoveClock, Is.EqualTo(0));

        [Test]
        public void Initializes_Board()
        {
            Assert.That(_fen.Board.At("a1"), Is.EqualTo(SquareState.Rook | SquareState.White));
            Assert.That(_fen.Board.At("b2"), Is.EqualTo(SquareState.Pawn | SquareState.White));
            Assert.That(_fen.Board.At("h1"), Is.EqualTo(SquareState.Rook | SquareState.White));
            Assert.That(_fen.Board.At("a8"), Is.EqualTo(SquareState.Rook | SquareState.Black));
            Assert.That(_fen.Board.At("b7"), Is.EqualTo(SquareState.Pawn | SquareState.Black));
            Assert.That(_fen.Board.At("h8"), Is.EqualTo(SquareState.Rook | SquareState.Black));
        }

        [Test]
        public void Sets_Castling()
        {
            Assert.That(_fen.Castling, Has.Flag(Castling.WhiteKing));
            Assert.That(_fen.Castling, Has.Flag(Castling.WhiteQueen));
            Assert.That(_fen.Castling, Has.Flag(Castling.BlackKing));
            Assert.That(_fen.Castling, Has.Flag(Castling.BlackQueen));
        }
    }

    [TestCase("w", ExpectedResult = Color.White)]
    [TestCase("b", ExpectedResult = Color.Black)]
    public Color NextToMove(string c)
    {
        var parser = new FenParser($"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR {c} KQkq - 0 1");

        var fen = parser.Parse();
        return fen.NextToMove;
    }

    [TestCase("KQkq", ExpectedResult = Castling.WhiteKing | Castling.WhiteQueen | Castling.BlackKing | Castling.BlackQueen)]
    [TestCase("K", ExpectedResult = Castling.WhiteKing)]
    [TestCase("Q", ExpectedResult = Castling.WhiteQueen)]
    [TestCase("kq", ExpectedResult = Castling.BlackKing | Castling.BlackQueen)]
    [TestCase("-", ExpectedResult = Castling.None)]
    public Castling Castlings(string c)
    {
        var parser = new FenParser($"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w {c} - 0 1");

        var fen = parser.Parse();
        return fen.Castling;
    }

    [Test]
    public void EnPassantTarget_None()
    {
        var parser = new FenParser("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

        var fen = parser.Parse();
        Assert.That(fen.EnPassantTarget.Valid, Is.False);
    }

    [Test]
    public void EnPassantTarget()
    {
        var parser = new FenParser("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq b3 0 1");

        var fen = parser.Parse();
        Assert.That(fen.EnPassantTarget.Valid, Is.True);
        Assert.That(fen.EnPassantTarget.File, Is.EqualTo(1));
        Assert.That(fen.EnPassantTarget.Rank, Is.EqualTo(2));
    }

    [Test]
    public void HalfMoveClock()
    {
        var parser = new FenParser("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 34 1");

        var fen = parser.Parse();
        Assert.That(fen.HalfMoveClock, Is.EqualTo(34));
    }

    [Test]
    public void FullMoveNumber()
    {
        var parser = new FenParser("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 54");

        var fen = parser.Parse();
        Assert.That(fen.FullMoveNumber, Is.EqualTo(54));
    }

    /// <summary>
    /// 1.e4 c5 2.Nf3
    /// </summary>
    [Test]
    public void Board_When_Moved()
    {
        var parser = new FenParser("rnbqkbnr/pp1ppppp/8/2p5/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2");

        var fen = parser.Parse();

        Assert.That(fen.Board.At("e2"), Is.EqualTo(SquareState.Free));
        Assert.That(fen.Board.At("e4"), Is.EqualTo(SquareState.Pawn | SquareState.White));
        Assert.That(fen.Board.At("c7"), Is.EqualTo(SquareState.Free));
        Assert.That(fen.Board.At("c5"), Is.EqualTo(SquareState.Pawn | SquareState.Black));
        Assert.That(fen.Board.At("g1"), Is.EqualTo(SquareState.Free));
        Assert.That(fen.Board.At("f3"), Is.EqualTo(SquareState.Knight | SquareState.White));
    }
}
