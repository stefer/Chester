using Chess.Models;
using Chess.Models.Pgn;
using NUnit.Framework;

namespace Chess.Tests.Models.Pgn
{
    public class MoveTextReaderTests
    {
        [Test]
        public void Kalle()
        {
            var moveText = @"
1.c4 g6 2.d4 Bg7 3.e4 d6 4.Nc3 c5 5.dxc5 Bxc3+ 6.bxc3 dxc5 7.Bd3 Nc6 8.f4 Qa5
9.Ne2 Be6 10.f5 O-O-O 11.fxe6 Ne5 12.exf7 Nf6 13.O-O Nxd3 14.Bh6 Ne5 15.Qb3 Nxf7  1-0
";
            var sut = new MoveTextReader(moveText);

            var moves = sut.ReadAll();

            Assert.That(moves.Count, Is.EqualTo(15));
            Assert.That(moves.Result, Is.EqualTo("1-0"));
        }

        [Test]
        public void HalfMove_OnlyFile()
        {
            var sut = new MoveTextReader("dxc5");

            var result = sut.ReadAll();

            Assert.That(result.Count, Is.EqualTo(1));
            var move = result[0];

            Assert.That(move.Seq, Is.EqualTo(1));
            Assert.That(move.Black, Is.Null);
            Assert.That(move.White.Color, Is.EqualTo(Color.White));
            Assert.That(move.White.Piece, Is.EqualTo(PgnPiece.Pawn));
            Assert.That(move.White.Type.HasFlag(PgnMoveType.Take));
            Assert.That(move.White.From.ToString(), Is.EqualTo("d"));
            Assert.That(move.White.To.ToString(), Is.EqualTo("c5"));

            Assert.That(move.ToString(), Is.EqualTo("1. dxc5"));

        }

        [Test]
        [TestCase("e4")]
        [TestCase(" e4 ")]
        [TestCase("1. e4")]
        [TestCase("1. e4 ")]
        [TestCase("1 . e4 ")]
        [TestCase("1.e4 ")]
        public void PawnHalfMove(string pgn)
        {
            var sut = new MoveTextReader(pgn);

            var result = sut.ReadAll();

            Assert.That(result.Count, Is.EqualTo(1));
            var move = result[0];

            Assert.That(move.Seq, Is.EqualTo(1));
            Assert.That(move.Black, Is.Null);
            Assert.That(move.White.Color, Is.EqualTo(Color.White));
            Assert.That(move.White.Piece, Is.EqualTo(PgnPiece.Pawn));
            Assert.That(move.White.From.Valid, Is.True);
            Assert.That(move.White.From.File, Is.EqualTo(4));
            Assert.That(move.White.From.Rank, Is.EqualTo(3));
            Assert.That(move.White.From.ToString(), Is.EqualTo("e4"));
            Assert.That(move.ToString(), Is.EqualTo("1. e4"));
        }
    }
}