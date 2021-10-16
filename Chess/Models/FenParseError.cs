using System;

namespace Chess.Models
{
    public class FenParseError : Exception
    {
        public FenParseError(string message) : base(message) { }
    }
}
