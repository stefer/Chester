using System;

namespace Chester.Models
{
    [Flags]
    public enum Castling : byte
    {
        None = 0b00000000,
        WhiteKing = 0b00000001,
        WhiteQueen = 0b00000010,
        BlackKing = 0b00000100,
        BlackQueen = 0b00001000,
    }
}
