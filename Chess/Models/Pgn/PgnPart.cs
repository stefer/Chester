namespace Chess.Models.Pgn
{
    public record PgnPart(int? Value)
    {
        public static readonly PgnPart Unknown = new(null);

        public override string ToString() => Value == Unknown.Value ? "??" : Value.ToString();

        public static implicit operator PgnPart(string value) => FromString(value);
        public static implicit operator PgnPart(int value) => new(value);

        public static PgnPart FromString(string part)
        {
            if (int.TryParse(part, out int value)) return new PgnPart(value);
            return Unknown;
        }
    }
}
