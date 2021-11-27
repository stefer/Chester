using System;

namespace Chester.Models
{
    [Flags]
    public enum SquareState : byte
    {
        Invalid = 0b11111111,
        Free = 0b00000000,

        White = 0b10000000,
        Black = 0b01000000,
        Colors = 0b11000000,

        King = 0b00100000,
        Queen = 0b00010000,
        Rook = 0b00001000,
        Bishop = 0b00000100,
        Knight = 0b00000010,
        Pawn = 0b00000001,
        Pieces = 0b00111111,
    }
}
