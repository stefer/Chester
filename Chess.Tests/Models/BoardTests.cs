
using Chess.Models;
using NUnit.Framework;

namespace Chess.Tests.Models
{
    public class BoardTests
    {
        [Test]
        public void Position_Converts()
        {
            Assert.Multiple( () => {
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
            Assert.Multiple(() => {
                Assert.That(Board.Index(Position.FromString("a1")), Is.EqualTo(0));
                Assert.That(Board.Index(Position.FromString("b1")), Is.EqualTo(1));
                Assert.That(Board.Index(Position.FromString("h1")), Is.EqualTo(7));
                Assert.That(Board.Index(Position.FromString("a2")), Is.EqualTo(8));
                Assert.That(Board.Index(Position.FromString("a8")), Is.EqualTo(56));
                Assert.That(Board.Index(Position.FromString("h8")), Is.EqualTo(63));
            });
        }
    }
}
