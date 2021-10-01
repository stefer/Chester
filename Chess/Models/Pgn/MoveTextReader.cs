using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Models.Pgn
{
    public class MoveTextReader
    {
        private ReadOnlyMemory<char> _content;
        private ReadOnlyMemory<char> _current;

        public MoveTextReader(string input): this(input.AsMemory()) { }

        public MoveTextReader(ReadOnlyMemory<char> input)
        {
            _content = input;
            _current = input;
        }

        /// <summary>
        /// Read movetext string like
        /// 1.c4 g6 2.d4 Bg7 3.e4 d6 4.Nc3 c5 5.dxc5 Bxc3+ 6.bxc3 dxc5 7.Bd3 Nc6 8.f4 Qa5
        /// 9.Ne2 Be6 10.f5 O-O-O 11.fxe6 Ne5 12.exf7 Nf6 13.O-O Nxd3 14.Bh6 Ne5 15.Qb3 Nxf7  1-0
        /// </summary>
        /// <returns></returns>
        public MoveText ReadAll()
        {
            List<PgnMove> moves = new();
            int sequence = 1;

            while (Move(sequence++, out PgnMove move)) moves.Add(move);

            WhiteSpace();
            Result(out string result);
            WhiteSpace();

            return new MoveText(moves, result);
        }

        /// <summary>
        /// Parse a result, like "1/2-1/2", 1-0
        /// </summary>
        /// <param name="result"></param>
        private void Result(out string result)
        {
            result = "*";
            if (_current.IsEmpty) return;
            var span = _current.Span;
            if (span[0] == '*') return;

            int move = 0;
            while (move < span.Length && !char.IsWhiteSpace(span[move])) move++;

            result = span[0..move].ToString();
        }

        /// <summary>
        /// Read Move like 11.fxe6 Ne5
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        private bool Move(int sequence, out PgnMove move)
        {
            move = null;
            if (_current.Length < 2) return false;

            WhiteSpace();
            Sequence();
            WhiteSpace();
            if (!HalfMove(Color.White, out PgnHalfMove white)) return false;
            WhiteSpace();
            Sequence();
            WhiteSpace();
            HalfMove(Color.Black, out PgnHalfMove black);

            move = new PgnMove(sequence, white, black);

            return true;
        }

        /// <summary>
        /// Read Half Move, like d4, Qa6xb7#, fxg1=Q+
        /// </summary>
        /// <param name="color"></param>
        /// <param name="white"></param>
        /// <returns></returns>
        private bool HalfMove(Color color, out PgnHalfMove white)
        {
            white = null;
            Piece(out PgnPiece piece);
            if (!Position(out PgnPosition from)) return false;
            Take(out PgnMoveType take);
            Position(out PgnPosition to);
            Promotion(out PgnMoveType promotion);
            Check(out PgnMoveType check);

            white = new PgnHalfMove(color, piece, from, to, take | promotion | check);
            return true;
        }

        static readonly char[] ranks = { '1', '2', '3', '4', '5', '6', '7', '8' };
        static readonly char[] files = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };

        private bool Position(out PgnPosition position)
        {
            var span = _current.Span;
            var file = -1;
            var rank = -1;
            var moved = 0;
            
            if (!span.IsEmpty && files.Contains(span[moved]))
            {
                file = _current.Span[moved] - 'a';
                moved++;
            }

            if (!span.IsEmpty && ranks.Contains(_current.Span[moved]))
            {
                rank = _current.Span[moved] - '1';
                moved++;
            }

            position = new PgnPosition(file, rank);

            _current = _current[moved..];
            return moved > 0;
        }

        private readonly Dictionary<char, PgnPiece> PgnPieces = new()
        {
            ['K'] = PgnPiece.King,
            ['Q'] = PgnPiece.Queen,
            ['R'] = PgnPiece.Rook,
            ['B'] = PgnPiece.Bishop,
            ['N'] = PgnPiece.Knight,
            ['P'] = PgnPiece.Pawn
        };

        private void Piece(out PgnPiece piece)
        {
            piece = PgnPiece.Pawn;

            if (_current.IsEmpty) return;

            if (PgnPieces.TryGetValue(char.ToUpper(_current.Span[0]), out PgnPiece piece2))
            {
                piece = piece2;
                _current = _current[1..];
            }

            return;
        }

        private readonly Dictionary<char, PgnMoveType> Promotions = new()
        {
            ['Q'] = PgnMoveType.PromotionQueen,
            ['R'] = PgnMoveType.PromotionRook,
            ['B'] = PgnMoveType.PromotionBishop,
            ['N'] = PgnMoveType.PromotionKnight,
        };

        private void Promotion(out PgnMoveType promotion)
        {
            promotion = PgnMoveType.Normal;
            var span = _current.Span;
            if (!_current.IsEmpty && span[0] == '=')
            {
                if (span.Length < 2 || !Promotions.TryGetValue(char.ToUpper(span[1]), out PgnMoveType value))
                    throw new PgnParseError($"Expected promotion to known piece at {span.ToString()}");
                promotion = value;
                _current = _current[2..];
            }
        }

        private void Take(out PgnMoveType take)
        {
            take = PgnMoveType.Normal;
            if (!_current.IsEmpty && _current.Span[0] == 'x')
            {
                _current = _current[1..];
                take = PgnMoveType.Take;
            }
        }

        private void Check(out PgnMoveType check)
        {
            check = PgnMoveType.Normal;

            if (_current.IsEmpty)
            {
                return;
            }
            if (_current.Span[0] == '#')
            {
                _current = _current[1..];
                check = PgnMoveType.CheckMate;
            } 
            else if (_current.Span[0] == '+')
            {
                _current = _current[1..];
                check = PgnMoveType.Check;
            }
        }

        private void WhiteSpace()
        {
            var span = _current.Span;
            int i = 0;
            while (i < span.Length && Char.IsWhiteSpace(span[i])) i++;

            _current = _current[i..];
        }

        private void Sequence()
        {
            Number();
            WhiteSpace();
            var span = _current.Span;
            int i = 0;
            while (i < span.Length && span[i] == '.') i++;

            _current = _current[i..];
        }

        private void Number()
        {
            var span = _current.Span;
            int i = 0;
            while (i < span.Length && Char.IsNumber(span[i])) i++;

            _current = _current[i..];
        }
    }
}
