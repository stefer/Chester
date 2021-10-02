﻿namespace Chess.Models
{
    public record PgnPosition
    {
        static readonly int[] ranks = { 1, 2, 3, 4, 5, 6, 7, 8 };
        static readonly char[] files = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };

        public int Rank { get; init; }
        public int File { get; init; }

        public PgnPosition(int file, int rank)
        {
            Rank = rank;
            File = file;
        }

        public void Deconstruct(out int file, out int rank) => (rank, file) = (Rank, File);

        public bool InValid => !ValidRank && !ValidFile;
        public bool ValidRank => Rank >= 0 && Rank < 8;
        public bool ValidFile => File >= 0 && File < 8;

        public override string ToString()
        {
            string file = ValidFile ? files[File].ToString() : string.Empty;
            string rank = ValidRank ? ranks[Rank].ToString() : string.Empty;

            return $"{file}{rank}";
        }
    }
}
