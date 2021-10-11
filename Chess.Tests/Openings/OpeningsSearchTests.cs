using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void TestSingleLine()
        {
            var pgns = new[]
            {
                new Pgn(new Dictionary<string, string> { { "Result", "1-0"} }, Moves("1. e4 e5 2. Nf3 Nc6 3. Bb5 a6 { This opening is called the Ruy Lopez.}"))
            };

            var sut = new OpeningSearch(pgns);

            var alts = sut.GetAlternatives(Moves("1. e4 e5 2. Nf3").AsHalfMoves());

            Assert.That(alts.Count(), Is.EqualTo(1));
            Assert.That(alts.Single().Move, Is.EqualTo(new PgnHalfMove(Color.Black, PgnPiece.Knight, null, "c6")));
        }

        [Test]
        public void TestSingleLineToEndAndComment()
        {
            var pgns = new[]
            {
                new Pgn(new Dictionary<string, string> { { "Result", "1-0"} }, Moves("1. e4 e5 2. Nf3 Nc6 3. Bb5 a6 { This opening is called the Ruy Lopez.}"))
            };

            var sut = new OpeningSearch(pgns);

            var alts = sut.GetAlternatives(Moves("e4 e5 Nf3 Nc6 Bb5").AsHalfMoves());

            Assert.That(alts.Count(), Is.EqualTo(1));
            Assert.That(alts.Single().Move, Is.EqualTo(new PgnHalfMove(Color.Black, PgnPiece.Pawn, null, "a6")));
            Assert.That(alts.Single().Move.Comment, Is.EqualTo("This opening is called the Ruy Lopez."));
            Assert.That(alts.Single().Pgn, Is.Not.Null);
            Assert.That(alts.Single().Pgn.Result, Is.EqualTo("1-0"));
        }


        private MoveText Moves(string moveText)
        {
            var reader = new MoveTextReader(moveText);
            return reader.ReadAll();
        }
    }
}
