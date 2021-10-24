using System;

namespace Chess.Services
{
    public class GameError : Exception
    {
        public GameError(string message) : base(message) { }
    }
}
