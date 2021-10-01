
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
                Assert.That(Board.Position(0), Is.EqualTo(Position.a1));
                Assert.That(Board.Position(1), Is.EqualTo(Position.b1));
                Assert.That(Board.Position(7), Is.EqualTo(Position.h1));
                Assert.That(Board.Position(8), Is.EqualTo(Position.a2));
                Assert.That(Board.Position(63), Is.EqualTo(Position.h8));
                Assert.That(Board.Position(64).Valid, Is.False);
                Assert.That(Board.Position(-1).Valid, Is.False);
            });
        }

        [Test]
        public void Index_Converts()
        {
            Assert.Multiple(() => {
                Assert.That(Board.Index(Position.a1), Is.EqualTo(0));
                Assert.That(Board.Index(Position.b1), Is.EqualTo(1));
                Assert.That(Board.Index(Position.h1), Is.EqualTo(7));
                Assert.That(Board.Index(Position.a2), Is.EqualTo(8));
                Assert.That(Board.Index(Position.a8), Is.EqualTo(56));
                Assert.That(Board.Index(Position.h8), Is.EqualTo(63));
            });
        }
    }
}
