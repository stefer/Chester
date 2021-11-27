using System;

namespace Chester.Services;

public class GameError : Exception
{
    public GameError(string message) : base(message) { }
}
