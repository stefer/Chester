using System.Collections.Generic;

namespace Chester.Models.Pgn;

public record PgnResult(string Value)
{
    public static readonly PgnResult WhiteWin = new("1-0");
    public static readonly PgnResult BlackWin = new("0-1");
    public static readonly PgnResult Draw = new("1/2-1/2");
    public static readonly PgnResult Undecided = new("*");
    public static readonly PgnResult Unknown = new((string)null);
    public static readonly HashSet<PgnResult> pgnResults = new() { WhiteWin, BlackWin, Draw, Undecided };

    public static PgnResult FromString(string value)
    {
        if (pgnResults.TryGetValue(new PgnResult(value), out var result)) return result;
        return Unknown;
    }
}
