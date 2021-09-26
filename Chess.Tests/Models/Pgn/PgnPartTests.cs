using Chess.Models.Pgn;
using NUnit.Framework;

namespace Chess.Tests.Models.Pgn
{
    public class PgnPartTests
    {
        [Test] public void UnknowHasNoValue() => Assert.That(PgnPart.Unknown.Value, Is.Null);
        [Test] public void UnknownIsFormatted() => Assert.That(PgnPart.Unknown.ToString(), Is.EqualTo("??"));
        [Test] public void UnknownFromStringIsUnknown() => Assert.That(PgnPart.FromString("??"), Is.EqualTo(PgnPart.Unknown));
        [Test] public void UnknownIsConverted() => Assert.That((PgnPart)"??", Is.EqualTo(PgnPart.Unknown));
        [Test] public void AnyInvalidFromStringIsUnknown() => Assert.That(PgnPart.FromString("KKK"), Is.EqualTo(PgnPart.Unknown));
        [Test] public void ValidFromStringHasValue() => Assert.That(PgnPart.FromString("13"), Is.EqualTo(new PgnPart(13)));
        [Test] public void ValidIsFormatted() => Assert.That(new PgnPart(13).ToString(), Is.EqualTo("13"));
        [Test] public void ValidIsConvertedString() => Assert.That((PgnPart)"13", Is.EqualTo(new PgnPart(13)));
        [Test] public void ValidIsConvertedInt() => Assert.That((PgnPart)13, Is.EqualTo(new PgnPart(13)));
    }
}