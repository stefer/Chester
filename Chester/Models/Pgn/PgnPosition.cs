using System;
using System.Linq;

namespace Chester.Models.Pgn
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

        public PgnPosition(string position)
        {
            if (string.IsNullOrEmpty(position)) throw new ArgumentNullException(nameof(position));
            if (position.Length != 2) throw new ArgumentException("Must have length 2", nameof(position));
            if (!files.Contains(position[0])) throw new ArgumentException("File must be a-h", nameof(position));
            if (!ranks.Contains(position[1] - '0')) throw new ArgumentException("Rank must be 1-8", nameof(position));
            File = Array.IndexOf(files, position[0]);
            Rank = Array.IndexOf(ranks, position[1] - '0');
        }

        public void Deconstruct(out int file, out int rank) => (file, rank) = (File, Rank);

        public bool InValid => !ValidRank && !ValidFile;
        public bool ValidRank => Rank >= 0 && Rank < 8;
        public bool ValidFile => File >= 0 && File < 8;

        public override string ToString()
        {
            string file = ValidFile ? files[File].ToString() : string.Empty;
            string rank = ValidRank ? ranks[Rank].ToString() : string.Empty;

            return $"{file}{rank}";
        }

        public static implicit operator PgnPosition(string pos) => new(pos);
    }
}
