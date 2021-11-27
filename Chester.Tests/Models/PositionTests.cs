using Chester.Models;
using NUnit.Framework;

namespace Chester.Tests.Models;

public class PositionTests
{
    [TestCase(0, 0, ExpectedResult = "a1")]
    [TestCase(0, 1, ExpectedResult = "a2")]
    [TestCase(1, 1, ExpectedResult = "b2")]
    [TestCase(2, 2, ExpectedResult = "c3")]
    [TestCase(3, 3, ExpectedResult = "d4")]
    [TestCase(4, 4, ExpectedResult = "e5")]
    [TestCase(5, 5, ExpectedResult = "f6")]
    [TestCase(6, 6, ExpectedResult = "g7")]
    [TestCase(7, 7, ExpectedResult = "h8")]
    public string ToString(int file, int rank) => Position.Create(file, rank).ToString();

    [Test]
    public void Equals()
    {
        Assert.That(Position.Create(0, 0), Is.EqualTo(Position.FromString("a1")));
        Assert.That(Position.Create(0, 1), Is.EqualTo(Position.FromString("a2")));
        Assert.That(Position.Create(1, 2), Is.EqualTo(Position.FromString("b3")));
        Assert.That(Position.Create(7, 7), Is.EqualTo(Position.FromString("h8")));
    }
}
