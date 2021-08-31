namespace Chess.Models
{
    public record Position
    {
        static readonly int[] ranks = { 1, 2, 3, 4, 5, 6, 7, 8 };
        static readonly char[] files = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };

        public int Rank { get; init; }
        public int File { get; init; }

        public Position(int rank, int file)
        {
            Rank = rank;
            File = file;
        }

        public void Deconstruct(out int rank, out int file) => (rank, file) = (Rank, File);

        public bool Valid => Rank >= 0 && Rank < 8 && File >= 0 && File < 8;

        public Position Move(int dr = 0, int df = 0) => this with { Rank = Rank + dr, File = File + df };

        public override string ToString() => Valid ? $"{files[File]}{ranks[Rank]}" : "--";
        public string FileString => $"{files[File]}";
    }
}
