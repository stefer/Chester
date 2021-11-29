using Chester.Models;
using Chester.Parsers;
using NUnit.Framework;
using System.Linq;

namespace Chester.Tests.Models;

using Chester.Tests.Extensions;

public class BoardTests
{
    [Test]
    public void Position_Converts() => Assert.Multiple(() => {
        Assert.That(Board.Pos(0), Is.EqualTo(Position.FromString("a1")));
        Assert.That(Board.Pos(1), Is.EqualTo(Position.FromString("b1")));
        Assert.That(Board.Pos(7), Is.EqualTo(Position.FromString("h1")));
        Assert.That(Board.Pos(8), Is.EqualTo(Position.FromString("a2")));
        Assert.That(Board.Pos(63), Is.EqualTo(Position.FromString("h8")));
        Assert.That(Board.Pos(64).Valid, Is.False);
        Assert.That(Board.Pos(-1).Valid, Is.False);
    });

    [Test]
    public void Index_Converts() => Assert.Multiple(() => {
        Assert.That(Board.Index(Position.FromString("a1")), Is.EqualTo(0));
        Assert.That(Board.Index(Position.FromString("b1")), Is.EqualTo(1));
        Assert.That(Board.Index(Position.FromString("h1")), Is.EqualTo(7));
        Assert.That(Board.Index(Position.FromString("a2")), Is.EqualTo(8));
        Assert.That(Board.Index(Position.FromString("a8")), Is.EqualTo(56));
        Assert.That(Board.Index(Position.FromString("h8")), Is.EqualTo(63));
    });

    [Test]
    public void Move_SavesLine()
    {
        Board board = new();

        board.MakeMove(new Move(board.At("e2"), "e2", "e4"));
        var moves = board.Line;
        Assert.That(moves.Select(x => x.ToStringLong()), Is.EqualTo(new string[] { "e2e4" }));

        var move = board.MakeMove(new Move(board.At("c7"), "c7", "c6"));
        moves = board.Line;
        Assert.That(moves.Select(x => x.ToStringLong()), Is.EqualTo(new string[] { "e2e4", "c7c6" }));
    }

    [Test]
    public void TakeBack_SavesLine()
    {
        Board board = new();

        board.MakeMove(new Move(board.At("e2"), "e2", "e4"));
        Assert.That(board.Line.Select(x => x.ToStringLong()), Is.EqualTo(new string[] { "e2e4" }));

        var move = board.MakeMove(new Move(board.At("c7"), "c7", "c6"));
        Assert.That(board.Line.Select(x => x.ToStringLong()), Is.EqualTo(new string[] { "e2e4", "c7c6" }));

        board.TakeBack(move);
        Assert.That(board.Line.Select(x => x.ToStringLong()), Is.EqualTo(new string[] { "e2e4" }));
    }

    [Test]
    public void Moves_Include_Castling()
    {
        var line = "1. e4 e5 2. Bc4 d6 3. Nf3 Nf6";

        Board board = new();
        Play(board, line);

        var moves = board.MovesFor(Color.White);
        var castling = moves.Single(x => x.MoveType.HasFlag(MoveType.CastleKingSide));
        Assert.That(castling.ToString(), Is.EqualTo("O-O"));
        Assert.That(castling.ToStringLong(), Is.EqualTo("e1g1"));
        Assert.That(castling.From.File, Is.EqualTo(4));
        Assert.That(castling.To.File, Is.EqualTo(6));
    }

    [Test]
    public void Move_Does_Castling()
    {
        var line = "1. e4 e5 2. Bc4 d6 3. Nf3 Nf6";

        Board board = new();
        Play(board, line);

        Assert.That(board.At("e1"), Has.Flag(SquareState.King));
        Assert.That(board.At("f1"), Is.EqualTo(SquareState.Free));
        Assert.That(board.At("g1"), Is.EqualTo(SquareState.Free));
        Assert.That(board.At("h1"), Has.Flag(SquareState.Rook));

        board.MakeMove(new Move(board.At("e1"), "e1", "g1", MoveType.CastleKingSide));

        Assert.That(board.At("e1"), Is.EqualTo(SquareState.Free));
        Assert.That(board.At("f1"), Has.Flag(SquareState.Rook));
        Assert.That(board.At("g1"), Has.Flag(SquareState.King));
        Assert.That(board.At("h1"), Is.EqualTo(SquareState.Free));
    }

    [Test]
    public void TakeBack_Castling_Resets()
    {
        var line = "1. e4 e5 2. Bc4 d6 3. Nf3 Nf6";

        Board board = new();
        Play(board, line);

        var move = board.MakeMove(new Move(board.At("e1"), "e1", "g1", MoveType.CastleKingSide));

        board.TakeBack(move);

        Assert.That(board.At("e1"), Has.Flag(SquareState.King));
        Assert.That(board.At("f1"), Is.EqualTo(SquareState.Free));
        Assert.That(board.At("g1"), Is.EqualTo(SquareState.Free));
        Assert.That(board.At("h1"), Has.Flag(SquareState.Rook));
    }

    [Test]
    public void InvalidMove()
    {
        Board board = new();

        Play(board, "e2e4 c7c6 Qd1d3");

        var moves = board.MovesFor(Color.Black).ToList();

        Assert.That(moves.Select(x => x.ToStringLong()), Does.Not.Contain("b8b2"));
    }

    private static void Play(Board board, string moveText)
    {
        var reader = new MoveTextReader(moveText);
        var moves = reader.ReadAll();
        board.MakeMoves(moves);
    }
}
