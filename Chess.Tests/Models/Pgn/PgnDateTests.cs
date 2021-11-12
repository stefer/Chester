using Chester.Models.Pgn;
using NUnit.Framework;

namespace Chester.Tests.Models.Pgn
{
    public class PgnDateTests
    {
        [Test] public void ValidFromStringHasValue() => Assert.That(PgnDate.FromString("1906.04.12"), Is.EqualTo(new PgnDate(1906, 4, 12)));

        [Test]
        public void ValidFromStringHasValues()
        {
            var date = PgnDate.FromString("1906.04.12");
            Assert.That(date, Has.Property("Year").EqualTo(new PgnPart(1906)));
            Assert.That(date, Has.Property("Month").EqualTo(new PgnPart(4)));
            Assert.That(date, Has.Property("Day").EqualTo(new PgnPart(12)));
        }

        [Test]
        public void UnknownFromStringHasValues()
        {
            var date = PgnDate.FromString("1906.??.??");
            Assert.That(date, Has.Property("Year").EqualTo(new PgnPart(1906)));
            Assert.That(date, Has.Property("Month").EqualTo(PgnPart.Unknown));
            Assert.That(date, Has.Property("Day").EqualTo(PgnPart.Unknown));
        }

    }
}