using Chester.Models;
using System;

namespace Chester.Parsers
{
    public class FenParser
    {
        private readonly string _fen;

        public FenParser(string fen)
        {
            _fen = fen;
        }

        public Fen Parse()
        {
            var parts = _fen.Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (parts.Length != 6) throw new FenParseError($"A FEN should have six fields, but {parts.Length} where found - {_fen}");

            var board = ParseBoard(parts[0]);
            var nextToMove = ParseColor(parts[1]);
            var castling = ParseCastling(parts[2]);
            var enPassantTarget = ParsePosition(parts[3]);
            var halfMoveClock = ParseNumber(parts[4]);
            var fullMoveNumber = ParseNumber(parts[5]);

            return new Fen(board, nextToMove, castling, enPassantTarget, halfMoveClock, fullMoveNumber);
        }

        private static Board ParseBoard(string v)
        {
            var board = Board.Empty();
            var rankStrings = v.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (rankStrings.Length != 8) throw new FenParseError($"Expected 8 ranks, but was {rankStrings.Length} in {v}");

            for (var r = 7; r >= 0; r--)
            {
                var rankString = rankStrings[7 - r];
                var f = 0;
                foreach (var c in rankString)
                {
                    if (char.IsDigit(c))
                    {
                        f += c - '0';
                    }
                    else
                    {
                        var piece = c.AsPiece();
                        if (piece.IsInvalid()) throw new FenParseError($"Unknown piece character {c} in {rankString}");

                        board.Set(Position.Create(f, r), piece);
                        f++;
                    }
                }

                if (f != 8) throw new FenParseError($"Wrong number of files in {rankString}");
            }

            return board;
        }

        private static Color ParseColor(string color) => color switch
        {
            "w" => Color.White,
            "b" => Color.Black,
            _ => throw new FenParseError($"Unknown color {color}")
        };

        private static Position ParsePosition(string pos)
        {
            if (pos == "-") return Position.Invalid;

            var position = Position.FromString(pos);
            if (!position.Valid) throw new FenParseError($"Unknown position format {pos}");
            return position;
        }

        private static Castling ParseCastling(string value)
        {
            var castling = Castling.None;
            foreach (var c in value)
                castling |= c switch
                {
                    'K' => Castling.WhiteKing,
                    'Q' => Castling.WhiteQueen,
                    'k' => Castling.BlackKing,
                    'q' => Castling.BlackQueen,
                    '-' => Castling.None,
                    _ => throw new FenParseError($"Unknown castling character {c} in {value}")
                };
            return castling;
        }

        private static int ParseNumber(string value)
        {
            if (!int.TryParse(value, out var number)) throw new FenParseError($"Expected a number {value}");
            return number;
        }
    }
}
