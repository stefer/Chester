using System;

namespace Chess.Models.Pgn
{
    public class PgnParseError : Exception
    {
        public PgnParseError(string message): base(message) {}
    }
}
