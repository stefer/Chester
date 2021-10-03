using Chess.Models;
using Chess.Models.Pgn;
using NUnit.Framework;

namespace Chess.Tests.Parsers
{
    using Chess.Parsers;
    using Chess.Tests.Extensions;

    public class MoveTextReaderTests
    {
        [Test]
        public void ReadAll_Complete_ToString()
        {
            var moveText = @"
1.c4 g6 2.d4 Bg7 3.e4 d6 4.Nc3 c5 5.dxc5 Bxc3+ 6.bxc3 dxc5 7.Bd3 Nc6 8.f4 Qa5
9.Ne2 Be6 10.f5 O-O-O 11.fxe6 Ne5 12.exf7 Nf6 13.O-O Nxd3 14.Bh6 Ne5 15.Qb3 Nxf7  1-0
";

            var moveTextExpected = 
@"1. c4 g6 2. d4 Bg7 3. e4 d6 4. Nc3 c5 5. dxc5 Bxc3+ 6. bxc3 dxc5 7. Bd3 Nc6 
8. f4 Qa5 9. Ne2 Be6 10. f5 O-O-O 11. fxe6 Ne5 12. exf7 Nf6 13. O-O Nxd3 
14. Bh6 Ne5 15. Qb3 Nxf7  1-0";
            var sut = new MoveTextReader(moveText);

            var moves = sut.ReadAll();

            Assert.That(moves.Count, Is.EqualTo(15));
            Assert.That(moves.Result, Is.EqualTo("1-0"));
            Assert.That(moves.ToString(), Is.EqualTo(moveTextExpected));
        }

        [Test]
        public void ReadAll_bxc3()
        {
            var moveText = @"6.bxc3 dxc5";
            var sut = new MoveTextReader(moveText);

            var move = sut.ReadAll()[0];

            Assert.That(move.White.Piece, Is.EqualTo(PgnPiece.Pawn));
            Assert.That(move.White.Type, Has.Flag(PgnMoveType.Take));
            Assert.That(move.White.From.ToString(), Is.EqualTo("b"));
            Assert.That(move.White.To.ToString(), Is.EqualTo("c3"));

            Assert.That(move.Black.Piece, Is.EqualTo(PgnPiece.Pawn));
            Assert.That(move.Black.Type, Has.Flag(PgnMoveType.Take));
            Assert.That(move.Black.From.ToString(), Is.EqualTo("d"));
            Assert.That(move.Black.To.ToString(), Is.EqualTo("c5"));
        }

        [Test]
        public void ShortWithResult()
        {
            var moveText = @"1.c4 g6 1-0";
            var sut = new MoveTextReader(moveText);

            var moves = sut.ReadAll();

            Assert.That(moves.Count, Is.EqualTo(1));
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
        public void HalfMove_NoFrom()
        {
            var sut = new MoveTextReader("Bxc3+");

            var result = sut.ReadAll();

            Assert.That(result.Count, Is.EqualTo(1));
            var move = result[0];

            Assert.That(move.Seq, Is.EqualTo(1));
            Assert.That(move.Black, Is.Null);
            Assert.That(move.White.Color, Is.EqualTo(Color.White));
            Assert.That(move.White.Piece, Is.EqualTo(PgnPiece.Bishop));
            Assert.That(move.White.Type.HasFlag(PgnMoveType.Take));
            Assert.That(move.White.From.InValid, Is.True);
            Assert.That(move.White.To.ToString(), Is.EqualTo("c3"));

            Assert.That(move.ToString(), Is.EqualTo("1. Bxc3+"));
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
            Assert.That(move.White.From.InValid, Is.False);
            Assert.That(move.White.From.File, Is.EqualTo(4));
            Assert.That(move.White.From.Rank, Is.EqualTo(3));
            Assert.That(move.White.From.ToString(), Is.EqualTo("e4"));
            Assert.That(move.ToString(), Is.EqualTo("1. e4"));
        }

        [Test]
        public void WhiteKingCastling()
        {
            var sut = new MoveTextReader("O-O");

            var result = sut.ReadAll();

            Assert.That(result.Count, Is.EqualTo(1));
            var move = result[0];

            Assert.That(move.White.Type.HasFlag(PgnMoveType.CastleKingSide));
            Assert.That(move.White.Color, Is.EqualTo(Color.White));
            Assert.That(move.White.Piece, Is.EqualTo(PgnPiece.King));
            Assert.That(move.White.From.InValid, Is.False);
            Assert.That(move.White.From.ToString(), Is.EqualTo("e1"));
            Assert.That(move.White.To.ToString(), Is.EqualTo("g1"));
            Assert.That(move.ToString(), Is.EqualTo("1. O-O"));
        }

        [Test]
        public void BlackKingCastling()
        {
            var sut = new MoveTextReader("e4 O-O");

            var result = sut.ReadAll();

            Assert.That(result.Count, Is.EqualTo(1));
            var move = result[0];

            Assert.That(move.Black.Type.HasFlag(PgnMoveType.CastleKingSide));
            Assert.That(move.Black.Color, Is.EqualTo(Color.Black));
            Assert.That(move.Black.Piece, Is.EqualTo(PgnPiece.King));
            Assert.That(move.Black.From.InValid, Is.False);
            Assert.That(move.Black.From.ToString(), Is.EqualTo("e8"));
            Assert.That(move.Black.To.ToString(), Is.EqualTo("g8"));
            Assert.That(move.ToString(), Is.EqualTo("1. e4 O-O"));
        }

        [Test]
        public void WhiteQueenCastling()
        {
            var sut = new MoveTextReader("O-O-O");

            var result = sut.ReadAll();

            Assert.That(result.Count, Is.EqualTo(1));
            var move = result[0];

            Assert.That(move.White.Type.HasFlag(PgnMoveType.CastleQueenSide));
            Assert.That(move.White.Color, Is.EqualTo(Color.White));
            Assert.That(move.White.Piece, Is.EqualTo(PgnPiece.King));
            Assert.That(move.White.From.InValid, Is.False);
            Assert.That(move.White.From.ToString(), Is.EqualTo("e1"));
            Assert.That(move.White.To.ToString(), Is.EqualTo("c1"));
            Assert.That(move.ToString(), Is.EqualTo("1. O-O-O"));
        }

        [Test]
        public void BlackQueenCastling()
        {
            var sut = new MoveTextReader("e4 O-O-O");

            var result = sut.ReadAll();

            Assert.That(result.Count, Is.EqualTo(1));
            var move = result[0];

            Assert.That(move.Black.Type.HasFlag(PgnMoveType.CastleQueenSide));
            Assert.That(move.Black.Color, Is.EqualTo(Color.Black));
            Assert.That(move.Black.Piece, Is.EqualTo(PgnPiece.King));
            Assert.That(move.Black.From.InValid, Is.False);
            Assert.That(move.Black.From.ToString(), Is.EqualTo("e8"));
            Assert.That(move.Black.To.ToString(), Is.EqualTo("c8"));
            Assert.That(move.ToString(), Is.EqualTo("1. e4 O-O-O"));
        }

        [Test]
        public void InlineBlackComment()
        {
            var sut = new MoveTextReader("1. e4 e5 { This opening is called the Ruy Lopez.} 4.Ba4 Nf6");

            var result = sut.ReadAll();

            Assert.That(result[0].Black.Comment, Is.EqualTo("This opening is called the Ruy Lopez."));
        }

        [Test]
        public void InlineWhiteComment()
        {
            var sut = new MoveTextReader("1. e4 { This opening is called the Ruy Lopez.} e5  4.Ba4 Nf6");

            var result = sut.ReadAll();

            Assert.That(result[0].White.Comment, Is.EqualTo("This opening is called the Ruy Lopez."));
        }

        [Test]
        public void InlineMoveComment()
        {
            var sut = new MoveTextReader("1. { This opening is called the Ruy Lopez.} e4 e5  4.Ba4 Nf6");

            var result = sut.ReadAll();

            Assert.That(result[0].Comment, Is.EqualTo("This opening is called the Ruy Lopez."));
        }

        [Test]
        public void LineComment()
        {
            var sut = new MoveTextReader("1. e4 e5 ; This opening is called the Ruy Lopez. \r\n 4.Ba4 Nf6");

            var result = sut.ReadAll();

            Assert.That(result[0].Black.Comment, Is.EqualTo("This opening is called the Ruy Lopez."));
        }
    }
}