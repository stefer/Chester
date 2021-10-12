using System.Collections.Generic;

namespace Chess.Models.Pgn
{
    public record PgnResult(string Value)
    {
        public static PgnResult WhiteWin = new("1-0");
        public static PgnResult BlackWin = new("0-1");
        public static PgnResult Draw = new("1/2-1/2");
        public static PgnResult Undecided = new("*");
        public static PgnResult Unknown = new((string)null);
        public static HashSet<PgnResult> pgnResults = new() { WhiteWin, BlackWin, Draw, Undecided };

        public static PgnResult FromString(string value)
        {
            if (pgnResults.TryGetValue(new PgnResult(value), out PgnResult result)) return result;
            return PgnResult.Unknown;
        }
    }
}
