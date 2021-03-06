using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Chester.Models;

public record Position
{
    private static readonly int[] _ranks = { 1, 2, 3, 4, 5, 6, 7, 8 };
    private static readonly char[] _files = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };

    public static readonly Position Invalid = new(-1, -1);

    public int Rank { get; init; }
    public int File { get; init; }

    public static readonly Dictionary<(int, int), Position> Positions = new(64);

    static Position()
    {
        for (var f = 0; f < 8; f++)
            for (var r = 0; r < 8; r++)
                Positions[(f, r)] = new Position(f, r);

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Position Create(int file, int rank)
    {
        if (Positions.TryGetValue((file, rank), out var value)) return value;
        return Invalid;
    }

    private Position(int file, int rank)
    {
        Rank = rank;
        File = file;
    }

    public static Position FromString(string position)
    {
        if (string.IsNullOrEmpty(position)) throw new ArgumentNullException(nameof(position));
        if (position.Length != 2) throw new ArgumentException("Must have length 2", nameof(position));
        if (!_files.Contains(position[0])) throw new ArgumentException("File must be a-h", nameof(position));
        if (!_ranks.Contains(position[1] - '0')) throw new ArgumentException("Rank must be 1-8", nameof(position));
        return Create(Array.IndexOf(_files, position[0]), Array.IndexOf(_ranks, position[1] - '0'));
    }

    public void Deconstruct(out int file, out int rank) => (file, rank) = (File, Rank);

    public bool Valid => Rank >= 0 && Rank < 8 && File >= 0 && File < 8;

    public Position Move(int df = 0, int dr = 0) => Create(File + df, Rank + dr);
    public Position With(int? f = null, int? r = null) => Create(f ?? File, r ?? Rank);

    public static implicit operator Position(string pos) => FromString(pos);

    public override string ToString() => Valid ? $"{_files[File]}{_ranks[Rank]}" : "--";
    public string FileString => $"{_files[File]}";

}
