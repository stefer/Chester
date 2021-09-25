namespace Chess.Models
{
    public enum Color
    {
        White = 1,
        Black = -1
    }

    public static class ColorExtensions
    {
        public static Color Other(this Color self) => self == Color.White ? Color.Black : Color.White;
    }
}
