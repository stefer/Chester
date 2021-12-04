using Chester.Models;
using Chester.Models.Pgn;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chester.Parsers;

/// <summary>
/// Parse a movetext where moves are represented using SAN (Standard Algebraic Notation)
/// </summary>
public class MoveTextReader
{
    private readonly ReadOnlyMemory<char> _content;
    private ReadOnlyMemory<char> _current;

    public MoveTextReader(string input) : this(input.AsMemory()) { }

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
        var sequence = 1;

        while (Move(sequence++, out var move)) moves.Add(move);

        WhiteSpace();
        Result(out var result);

        return new MoveText(moves, result);
    }

    /// <summary>
    /// Parse a result, like *, "1/2-1/2", 1-0, 0-1
    /// </summary>
    /// <param name="result"></param>
    private void Result(out string result)
    {
        result = null;
        if (_current.IsEmpty) return;
        if (!IsResult()) return;

        var span = _current.Span;

        var move = 0;
        while (move < span.Length && !char.IsWhiteSpace(span[move])) move++;

        result = span[0..move].ToString();
        _current = _current[move..];
    }

    private static readonly string[] Results = new string[] { "*", "1/2-1/2", "1-0", "0-1" };

    private bool IsResult()
    {
        var span = _current.Span;
        while (char.IsWhiteSpace(span[0])) span = span[1..];

        foreach (var result in Results)
        {
            if (span.StartsWith(result)) return true;
        }

        return false;
    }

    /// <summary>
    /// Read Move like 11.fxe6 Ne5
    /// Sequence number can repeat, with three dots for black: 11.fxe6 11...Ne5
    /// Comments may in different places
    ///   11. {Comment Move} fxe6 { Comment White } Ne5 { Comment Black }
    /// </summary>
    /// <param name="sequence"></param>
    /// <param name="move"></param>
    /// <returns></returns>
    private bool Move(int sequence, out PgnMove move)
    {
        move = null;
        if (_current.Length < 2) return false;
        if (IsResult()) return false;

        Sequence();
        Comment(out var comment);
        if (!HalfMove(Color.White, out var white)) return false;
        Sequence();
        HalfMove(Color.Black, out var black);

        move = new PgnMove(sequence, white, black, comment);

        return true;
    }

    private void Comment(out string comment)
    {
        comment = null;
        WhiteSpace();
        var span = _current.Span;
        if (span.IsEmpty) return;

        if (span[0] == '{')
        {
            var end = 0;
            while (end < span.Length && span[end] != '}') end++;
            comment = span[1..end].ToString().Trim();
            end = Math.Min(span.Length, end + 1);
            _current = _current[end..];
        }
        if (span[0] == ';')
        {
            var end = 0;
            while (end < span.Length && span[end] != '\n') end++;
            comment = span[1..end].ToString().Trim();
            end = Math.Min(span.Length, end + 1);
            _current = _current[end..];
        }
    }

    /// <summary>
    /// Read Half Move, like d4, Qa6xb7#, fxg1=Q+
    /// </summary>
    /// <param name="color"></param>
    /// <param name="halfMove"></param>
    /// <returns></returns>
    private bool HalfMove(Color color, out PgnPly halfMove)
    {
        halfMove = null;
        WhiteSpace();

        if (Castling(color, out var castling))
        {
            Check(out var check1);
            Comment(out var comment);
            halfMove = castling with { Type = castling.Type | check1, Comment = comment };
            return true;
        }

        Piece(out var piece);
        Position(out var first);
        Take(out var take);
        Position(out var second);
        Promotion(out var promotion);
        Check(out var check);
        Comment(out var comment1);


        if (first.InValid && second.InValid) return false;

        // if only one position is found, it is the to position
        var from = second.InValid ? null : first;
        var to = second.InValid ? first : second;

        halfMove = new PgnPly(color, piece, from, to, take | promotion | check, comment1);
        return true;
    }

    private static readonly List<(Color color, string move, PgnMoveType type)> Castlings = new()
    {
        (Color.White, "e1g1",  PgnMoveType.CastleKingSide),
        (Color.White, "O-O",   PgnMoveType.CastleKingSide),
        (Color.White, "e1c1",  PgnMoveType.CastleQueenSide),
        (Color.White, "O-O-O", PgnMoveType.CastleQueenSide),
        (Color.Black, "e8g8",  PgnMoveType.CastleKingSide),
        (Color.Black, "O-O",   PgnMoveType.CastleKingSide),
        (Color.Black, "e8c8",  PgnMoveType.CastleQueenSide),
        (Color.Black, "O-O-O", PgnMoveType.CastleQueenSide),
    };

    private bool Castling(Color color, out PgnPly castling)
    {
        castling = null;

        var span = _current.Span;
        var start = span.ToString();

        var variant = Castlings.SingleOrDefault(x => x.color == color && start.StartsWith(x.move));

        if (variant == default) return false;

        var queenSide = variant.type == PgnMoveType.CastleQueenSide;
        var kingSide = variant.type == PgnMoveType.CastleKingSide;

        var rank = color == Color.White ? 0 : 7;
        var fromFile = 4;
        var toFile = queenSide ? 2 : 6;

        castling = new PgnPly(
            color,
            PgnPiece.King,
            new PgnPosition(fromFile, rank),
            new PgnPosition(toFile, rank),
            variant.type);

        _current = _current[variant.move.Length..];

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

    private static readonly Dictionary<char, PgnPiece> PgnPieces = new()
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

        if (PgnPieces.TryGetValue(_current.Span[0], out var piece2))
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
            if (span.Length < 2 || !Promotions.TryGetValue(char.ToUpper(span[1]), out var value))
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
        var i = 0;
        while (i < span.Length && char.IsWhiteSpace(span[i])) i++;

        _current = _current[i..];
    }

    private void Sequence()
    {
        WhiteSpace();
        Number();
        WhiteSpace();
        var span = _current.Span;
        var i = 0;
        while (i < span.Length && span[i] == '.') i++;

        _current = _current[i..];
    }

    private void Number()
    {
        var span = _current.Span;
        var i = 0;
        while (i < span.Length && char.IsNumber(span[i])) i++;

        _current = _current[i..];
    }
}
