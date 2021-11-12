using System;

namespace Chester.Models.Pgn
{
    public class PgnParseError : Exception
    {
        public PgnParseError(string message) : base(message) { }
    }
}
