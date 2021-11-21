using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chester.Models.Pgn;
using Chester.Openings;
using Chester.Parsers;
using Chester.Models;
using NUnit.Framework;

namespace Chester.Tests.Openings
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

            var alts = sut.GetAlternatives(Moves("1. e4 e5 2. Nf3").AsPlies());

            Assert.That(alts.Count(), Is.EqualTo(1));
            Assert.That(alts.Single().Move, Is.EqualTo(new PgnPly(Color.Black, PgnPiece.Knight, null, "c6")));
            Assert.That(alts.Single().Results, Is.EqualTo(new OpeningResults(1, 0, 0)));
        }

        [Test]
        public void SingleLineToEndAndComment()
        {
            var pgns = new[]
            {
                new PgnGame(new Dictionary<string, string> { { "Result", "1-0"} }, Moves("1. e4 e5 2. Nf3 Nc6 3. Bb5 a6 { This opening is called the Ruy Lopez.}"))
            };

            var sut = new OpeningSearch(pgns);

            var alts = sut.GetAlternatives(Moves("e4 e5 Nf3 Nc6 Bb5").AsPlies());

            Assert.That(alts.Count(), Is.EqualTo(1));
            Assert.That(alts.Single().Move, Is.EqualTo(new PgnPly(Color.Black, PgnPiece.Pawn, null, "a6")));
            Assert.That(alts.Single().Move.Comment, Is.EqualTo("This opening is called the Ruy Lopez."));
            Assert.That(alts.Single().Pgn, Is.Not.Null);
            Assert.That(alts.Single().Pgn.Result, Is.EqualTo(PgnResult.WhiteWin));
            Assert.That(alts.Single().Results, Is.EqualTo(new OpeningResults(1, 0, 0)));
        }

        [Test]
        public void MultipleLines_SameBranch_OneAlternative()
        {
            var pgns = new[]
            {
                new PgnGame(new Dictionary<string, string> { { "Result", "1-0"} }, Moves("1.d4 g6 2.e4 Bg7 3.c4 d6 4.Nc3 e5 5.d5 f5 6.exf5 gxf5 7.Qh5+ Kf8 8.Nf3 Nf6")),
                new PgnGame(new Dictionary<string, string> { { "Result", "0-1"} }, Moves("1.d4 g6 2.e4 Bg7 3.c4 d6 4.Nc3 Nf6 5.Bg5 O-O 6.Qd2 c5 7.d5 Ne8 8.Bd3 Nd7"))
            };

            var sut = new OpeningSearch(pgns);

            var alts = sut.GetAlternatives(Moves("1.d4 g6 2.e4 Bg7").AsPlies());

            Assert.That(alts.Count(), Is.EqualTo(1));
            Assert.That(alts.Single().Move, Is.EqualTo(new PgnPly(Color.White, PgnPiece.Pawn, null, "c4")));
            Assert.That(alts.Single().Results, Is.EqualTo(new OpeningResults(1, 1, 0)));
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

            var alts = sut.GetAlternatives(Moves("1.d4 g6 2.e4 Bg7 3.c4 d6 4.Nc3").AsPlies());

            Assert.That(alts.Count(), Is.EqualTo(2));
            Assert.That(alts.First().Move, Is.EqualTo(new PgnPly(Color.Black, PgnPiece.Pawn, null, "e5")));
            Assert.That(alts.Skip(1).First().Move, Is.EqualTo(new PgnPly(Color.Black, PgnPiece.Knight, null, "f6")));
            Assert.That(alts.First().Results, Is.EqualTo(new OpeningResults(1, 0, 0)));
            Assert.That(alts.Skip(1).First().Results, Is.EqualTo(new OpeningResults(0, 1, 0)));
        }

        [Test]
        public void PgnFile_Parsed_HasAlternatives()
        {
            var pgns = PgnParser.Parse(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Openings", "CenterGame-Danish.pgn")));

            var sut = new OpeningSearch(pgns);

            var alts = sut.GetAlternatives(Moves("1.e4 e5").AsPlies());

            Assert.That(alts.Count(), Is.EqualTo(2));
            Assert.That(alts.First().Move, Is.EqualTo(new PgnPly(Color.White, PgnPiece.Pawn, null, "d4")));
            Assert.That(alts.Skip(1).First().Move, Is.EqualTo(new PgnPly(Color.White, PgnPiece.Knight, null, "f3")));
            Assert.That(alts.First().Results, Is.EqualTo(new OpeningResults(2261, 1900, 1061)));
        }

        private static MoveText Moves(string moveText)
        {
            var reader = new MoveTextReader(moveText);
            return reader.ReadAll();
        }
    }
}
