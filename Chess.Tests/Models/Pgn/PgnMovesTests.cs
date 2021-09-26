using NUnit.Framework;

namespace Chess.Tests.Models.Pgn
{
    public class PgnMovesTests
    {
        [Test]
        public void Kalle()
        {
            var moveText = @"
1. e4 e5 2. Nf3
White attacks the black e-pawn.
2... Nc6
Black defends and develops simultaneously.
3. Bb5
White plays the Ruy Lopez.
3... a6
Black elects Morphy's Defence.
";
        }
    }
}