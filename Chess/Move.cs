using System;

namespace Chess
{
    public record Move
    {
        public SquareState FromSquare { get; init; } 
        public Position From { get; init; } 
        public Position To { get; init; }
        public bool Capture { get; init; }

        public Move(SquareState fromSquare, Position from, Position to, bool capture = false)
        {
            if (fromSquare.IsInvalid()) throw new ArgumentException("Cannot make move from invalid square", nameof(fromSquare));
            if (!from.Valid) throw new ArgumentException("Cannot make move from invalid position", nameof(from));
            if (!to.Valid) throw new ArgumentException("Cannot make move to invalid position", nameof(to));
            FromSquare = fromSquare;
            From = from;
            To = to;
            Capture = capture;
        }

        public void Deconstruct(out SquareState fromSquare, out Position from, out Position to, out bool capture) =>
            (fromSquare, from, to, capture) = (FromSquare, From, To, Capture);

        public override string ToString()
        {
            if (FromSquare.IsPawn() && Capture) return $"{From.FileString}{(Capture ? 'x' : string.Empty)}{To}";
            if (FromSquare.IsPawn()) return $"{To}";

            return $"{FromSquare.AsString()}{From}{(Capture ? 'x' : string.Empty)}{To}";
        }
    }
}
