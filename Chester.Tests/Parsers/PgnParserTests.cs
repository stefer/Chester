using Chester.Models.Pgn;
using Chester.Parsers;
using NUnit.Framework;
using System.Linq;

namespace Chester.Tests.Parsers;

public class PgnParserTests
{

    public class SingleEntry
    {

        private static readonly string Single = @"
[Event ""F/S Return Match""]
[Site ""Belgrade, Serbia JUG""]
[Date ""1992.11.04""]
[Round ""29""]
[White ""Fischer, Robert J.""]
[Black ""Spassky, Boris V.""]
[Result ""1/2-1/2""]
[Undef ""Kalle på spången!""]

1. e4 e5 2. Nf3 Nc6 3. Bb5 a6 { This opening is called the Ruy Lopez.}
4. Ba4 Nf6 5. O-O Be7 6. Re1 b5 7. Bb3 d6 8. c3 O-O 9. h3 Nb8 10. d4 Nbd7
11. c4 c6 12. cxb5 axb5 13. Nc3 Bb7 14. Bg5 b4 15. Nb1 h6 16. Bh4 c5 17. dxe5
Nxe4 18. Bxe7 Qxe7 19. exd6 Qf6 20. Nbd2 Nxd6 21. Nc4 Nxc4 22. Bxc4 Nb6
23. Ne5 Rae8 24. Bxf7+ Rxf7 25. Nxf7 Rxe1+ 26. Qxe1 Kxf7 27. Qe3 Qg5 28. Qxg5
hxg5 29. b3 Ke6 30. a3 Kd6 31. axb4 cxb4 32. Ra5 Nd5 33. f3 Bc8 34. Kf2 Bf5
35. Ra7 g6 36. Ra6+ Kc5 37. Ke1 Nf4 38. g3 Nxh3 39. Kd2 Kb5 40. Rd6 Kc5 41. Ra6
Nf2 42. g4 Bd3 43. Re6 1/2-1/2
";

        [Test]
        public void AttributesAreRead()
        {
            var sut = PgnParser.Parse(Single).Single();

            Assert.That(sut.Attributes["Event"], Is.EqualTo("F/S Return Match"));
            Assert.That(sut.Attributes["Site"], Is.EqualTo("Belgrade, Serbia JUG"));
            Assert.That(sut.Attributes["Date"], Is.EqualTo("1992.11.04"));
            Assert.That(sut.Attributes["Round"], Is.EqualTo("29"));
            Assert.That(sut.Attributes["White"], Is.EqualTo("Fischer, Robert J."));
            Assert.That(sut.Attributes["Black"], Is.EqualTo("Spassky, Boris V."));
            Assert.That(sut.Attributes["Result"], Is.EqualTo("1/2-1/2"));
            Assert.That(sut.Attributes["Undef"], Is.EqualTo("Kalle på spången!"));
        }

        [Test]
        public void AttributesAreReadAndDefaultPropertiesSet()
        {
            var sut = PgnParser.Parse(Single).Single();

            Assert.That(sut.Event, Is.EqualTo("F/S Return Match"));
            Assert.That(sut.Site, Is.EqualTo("Belgrade, Serbia JUG"));
            Assert.That(sut.Date, Is.EqualTo(new PgnDate(1992, 11, 04)));
            Assert.That(sut.Round, Is.EqualTo(29));
            Assert.That(sut.White, Is.EqualTo("Fischer, Robert J."));
            Assert.That(sut.Black, Is.EqualTo("Spassky, Boris V."));
            Assert.That(sut.Result, Is.EqualTo(PgnResult.Draw));
        }

        [Test]
        public void MoveTextIsRead()
        {
            var sut = PgnParser.Parse(Single).Single();

            Assert.That(sut.Moves, Is.Not.Empty);
            Assert.That(sut.Moves[0].ToString(), Is.EqualTo("1. e4 e5"));
            Assert.That(sut.Moves[42].ToString(), Is.EqualTo("43. Re6"));
            Assert.That(sut.Result, Is.EqualTo(PgnResult.Draw));
        }
    }

    public class MultipleEntries
    {
        private static readonly string Multiple = @"
[Event ""Match 1""]
[Site ""Belgrade, Serbia JUG""]
[Date ""1992.11.04""]
[Round ""1""]
[White ""Fischer, Robert J.""]
[Black ""Spassky, Boris V.""]
[Result ""1/2-1/2""]

1. e4 e5 2. Nf3 Nc6 3. Bb5 a6 1/2-1/2

[Event ""Match 2""]
[Site ""Belgrade, Serbia JUG""]
[Date ""1992.11.04""]
[Round ""2""]
[White ""Spassky, Boris V.""]
[Black ""Fischer, Robert J.""]
[Result ""1/2-1/2""]

1. e5 e4 2. Nf3 Nc6 3. Bb5 a6 *

";

        [Test]
        public void Parse()
        {
            var pgns = PgnParser.Parse(Multiple).ToList();

            Assert.That(pgns, Has.Count.EqualTo(2));

            Assert.That(pgns[0].Event, Is.EqualTo("Match 1"));
            Assert.That(pgns[0].Round, Is.EqualTo(1));
            Assert.That(pgns[0].White, Is.EqualTo("Fischer, Robert J."));
            Assert.That(pgns[0].Moves[0].ToString(), Is.EqualTo("1. e4 e5"));

            Assert.That(pgns[1].Event, Is.EqualTo("Match 2"));
            Assert.That(pgns[1].Round, Is.EqualTo(2));
            Assert.That(pgns[1].White, Is.EqualTo("Spassky, Boris V."));
            Assert.That(pgns[1].Moves[0].ToString(), Is.EqualTo("1. e5 e4"));
        }
    }
}
