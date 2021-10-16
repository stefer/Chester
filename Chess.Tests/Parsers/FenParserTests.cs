using Chess.Parsers;
using NUnit.Framework;

namespace Chess.Tests.Parsers
{
    public class FenParserTests
    {
        [Test]
        public void ParseStandard()
        {
            var parser = new FenParser("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

            var fen = parser.Parse();
        }
    }
}
