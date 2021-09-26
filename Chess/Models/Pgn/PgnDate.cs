using System;

namespace Chess.Models.Pgn
{
    public record PgnDate(PgnPart Year, PgnPart Month, PgnPart Day)
    {
        public override string ToString() => $"{Year}.{Month}.{Day}";

        public static PgnDate FromString(string date)
        {
            var parts = date.Split('.');

            if (parts.Length != 3) throw new ArgumentException($"Invalid Pgn date: {date}", nameof(date));

            return new PgnDate(PgnPart.FromString(parts[0]), PgnPart.FromString(parts[1]), PgnPart.FromString(parts[2]));
        }
    }
}
