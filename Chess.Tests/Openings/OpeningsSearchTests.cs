using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chess.Models;
using Chess.Models.Pgn;
using Chess.Openings;
using Chess.Parsers;
using NUnit.Framework;

namespace Chess.Tests.Openings
{
    internal class OpeningsSearchTests
    {

        [Test]
        public void SingleLine()
        {
            var pgns = new[]
            {
                new PgnGame(new Dictionary<string, string> { { "Result", "1-0"} }, Moves("1. e4 e5 2. Nf3 Nc6 3. Bb5 a6 { This opening is called the Ruy Lopez.}"))
            };

            var sut = new OpeningSearch(pgns);

            var alts = sut.GetAlternatives(Moves("1. e4 e5 2. Nf3").AsHalfMoves());

            Assert.That(alts.Count(), Is.EqualTo(1));
            Assert.That(alts.Single().Move, Is.EqualTo(new PgnHalfMove(Color.Black, PgnPiece.Knight, null, "c6")));
        }

        [Test]
        public void SingleLineToEndAndComment()
        {
            var pgns = new[]
            {
                new PgnGame(new Dictionary<string, string> { { "Result", "1-0"} }, Moves("1. e4 e5 2. Nf3 Nc6 3. Bb5 a6 { This opening is called the Ruy Lopez.}"))
            };

            var sut = new OpeningSearch(pgns);

            var alts = sut.GetAlternatives(Moves("e4 e5 Nf3 Nc6 Bb5").AsHalfMoves());

            Assert.That(alts.Count(), Is.EqualTo(1));
            Assert.That(alts.Single().Move, Is.EqualTo(new PgnHalfMove(Color.Black, PgnPiece.Pawn, null, "a6")));
            Assert.That(alts.Single().Move.Comment, Is.EqualTo("This opening is called the Ruy Lopez."));
            Assert.That(alts.Single().Pgn, Is.Not.Null);
            Assert.That(alts.Single().Pgn.Result, Is.EqualTo(PgnResult.WhiteWin));
        }

        [Test]
        public void MultipleLines_SameBranch_OneAlternative()
        {
            var pgns = new[]
            {
                new PgnGame(new Dictionary<string, string> { { "Result", "1-0"} }, Moves("1.d4 g6 2.e4 Bg7 3.c4 d6 4.Nc3 e5 5.d5 f5 6.exf5 gxf5 7.Qh5+ Kf8 8.Nf3 Nf6")),
                new PgnGame(new Dictionary<string, string> { { "Result", "0-1"} }, Moves("1.e4 g6 2.d4 Bg7 3.c4 d6 4.Nc3 Nf6 5.Bg5 O-O 6.Qd2 c5 7.d5 Ne8 8.Bd3 Nd7"))
            };

            var sut = new OpeningSearch(pgns);

            var alts = sut.GetAlternatives(Moves("1.d4 g6 2.e4 Bg7").AsHalfMoves());

            Assert.That(alts.Count(), Is.EqualTo(1));
            Assert.That(alts.Single().Move, Is.EqualTo(new PgnHalfMove(Color.White, PgnPiece.Pawn, null, "c4")));
        }

        [Test]
        public void MultipleLines_Branching_TwoAlternatives()
        {
            var pgns = new[]
            {
                new PgnGame(new Dictionary<string, string> { { "Result", "1-0"} }, Moves("1.d4 g6 2.e4 Bg7 3.c4 d6 4.Nc3 e5 5.d5 f5 6.exf5 gxf5 7.Qh5+ Kf8 8.Nf3 Nf6")),
                new PgnGame(new Dictionary<string, string> { { "Result", "0-1"} }, Moves("1.d4 g6 2.e4 Bg7 3.c4 d6 4.Nc3 Nf6 5.Bg5 O-O 6.Qd2 c5 7.d5 Ne8 8.Bd3 Nd7"))
            };

            var sut = new OpeningSearch(pgns);

            var alts = sut.GetAlternatives(Moves("1.d4 g6 2.e4 Bg7 3.c4 d6 4.Nc3").AsHalfMoves());

            Assert.That(alts.Count(), Is.EqualTo(2));
            Assert.That(alts.First().Move, Is.EqualTo(new PgnHalfMove(Color.Black, PgnPiece.Pawn, null, "e5")));
            Assert.That(alts.Skip(1).First().Move, Is.EqualTo(new PgnHalfMove(Color.Black, PgnPiece.Knight, null, "f6")));
        }

        [Test]
        public void PgnFile_Parsed_HasAlternatives()
        {
            var pgns = PgnParser.Parse(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Openings", "CenterGame-Danish.pgn")));

            var sut = new OpeningSearch(pgns);

            var alts = sut.GetAlternatives(Moves("1.e4 e5").AsHalfMoves());

            Assert.That(alts.Count(), Is.EqualTo(2));
            Assert.That(alts.First().Move, Is.EqualTo(new PgnHalfMove(Color.White, PgnPiece.Pawn, null, "d4")));
            Assert.That(alts.Skip(1).First().Move, Is.EqualTo(new PgnHalfMove(Color.White, PgnPiece.Knight, null, "f3")));
        }


        private static MoveText Moves(string moveText)
        {
            var reader = new MoveTextReader(moveText);
            return reader.ReadAll();
        }
    }
}
