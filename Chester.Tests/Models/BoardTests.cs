using Chester.Parsers;
using Chester.Models;
using NUnit.Framework;
using System.Linq;

namespace Chester.Tests.Models
{
    public class BoardTests
    {
        [Test]
        public void Position_Converts()
        {
            Assert.Multiple(() =>
            {
                Assert.That(Board.Pos(0), Is.EqualTo(Position.FromString("a1")));
                Assert.That(Board.Pos(1), Is.EqualTo(Position.FromString("b1")));
                Assert.That(Board.Pos(7), Is.EqualTo(Position.FromString("h1")));
                Assert.That(Board.Pos(8), Is.EqualTo(Position.FromString("a2")));
                Assert.That(Board.Pos(63), Is.EqualTo(Position.FromString("h8")));
                Assert.That(Board.Pos(64).Valid, Is.False);
                Assert.That(Board.Pos(-1).Valid, Is.False);
            });
        }

        [Test]
        public void Index_Converts()
        {
            Assert.Multiple(() =>
            {
                Assert.That(Board.Index(Position.FromString("a1")), Is.EqualTo(0));
                Assert.That(Board.Index(Position.FromString("b1")), Is.EqualTo(1));
                Assert.That(Board.Index(Position.FromString("h1")), Is.EqualTo(7));
                Assert.That(Board.Index(Position.FromString("a2")), Is.EqualTo(8));
                Assert.That(Board.Index(Position.FromString("a8")), Is.EqualTo(56));
                Assert.That(Board.Index(Position.FromString("h8")), Is.EqualTo(63));
            });
        }

        [Test]
        public void InvalidMove()
        {
            Board board = new Board();

            Play(board, "e2e4 c7c6 Qd1d3");

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
}
