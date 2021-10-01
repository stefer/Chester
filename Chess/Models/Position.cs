namespace Chess.Models
{
    public record Position
    {
        static readonly int[] ranks = { 1, 2, 3, 4, 5, 6, 7, 8 };
        static readonly char[] files = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };

        public int Rank { get; init; }
        public int File { get; init; }

        public Position(int file, int rank)
        {
            Rank = rank;
            File = file;
        }

        public void Deconstruct(out int file, out int rank) => (rank, file) = (Rank, File);

        public bool Valid => Rank >= 0 && Rank < 8 && File >= 0 && File < 8;

        public Position Move(int df = 0, int dr = 0) => this with { Rank = Rank + dr, File = File + df };

        public override string ToString() => Valid ? $"{files[File]}{ranks[Rank]}" : "--";
        public string FileString => $"{files[File]}";

        public static readonly Position a1 = new(0, 0);
        public static readonly Position a2 = new(0, 1);
        public static readonly Position a3 = new(0, 2);
        public static readonly Position a4 = new(0, 3);
        public static readonly Position a5 = new(0, 4);
        public static readonly Position a6 = new(0, 5);
        public static readonly Position a7 = new(0, 6);
        public static readonly Position a8 = new(0, 7);

        public static readonly Position b1 = new(1, 0);
        public static readonly Position b2 = new(1, 1);
        public static readonly Position b3 = new(1, 2);
        public static readonly Position b4 = new(1, 3);
        public static readonly Position b5 = new(1, 4);
        public static readonly Position b6 = new(1, 5);
        public static readonly Position b7 = new(1, 6);
        public static readonly Position b8 = new(1, 7);

        public static readonly Position c1 = new(2, 0);
        public static readonly Position c2 = new(2, 1);
        public static readonly Position c3 = new(2, 2);
        public static readonly Position c4 = new(2, 3);
        public static readonly Position c5 = new(2, 4);
        public static readonly Position c6 = new(2, 5);
        public static readonly Position c7 = new(2, 6);
        public static readonly Position c8 = new(2, 7);

        public static readonly Position d1 = new(3, 0);
        public static readonly Position d2 = new(3, 1);
        public static readonly Position d3 = new(3, 2);
        public static readonly Position d4 = new(3, 3);
        public static readonly Position d5 = new(3, 4);
        public static readonly Position d6 = new(3, 5);
        public static readonly Position d7 = new(3, 6);
        public static readonly Position d8 = new(3, 7);

        public static readonly Position e1 = new(4, 0);
        public static readonly Position e2 = new(4, 1);
        public static readonly Position e3 = new(4, 2);
        public static readonly Position e4 = new(4, 3);
        public static readonly Position e5 = new(4, 4);
        public static readonly Position e6 = new(4, 5);
        public static readonly Position e7 = new(4, 6);
        public static readonly Position e8 = new(4, 7);

        public static readonly Position f1 = new(5, 0);
        public static readonly Position f2 = new(5, 1);
        public static readonly Position f3 = new(5, 2);
        public static readonly Position f4 = new(5, 3);
        public static readonly Position f5 = new(5, 4);
        public static readonly Position f6 = new(5, 5);
        public static readonly Position f7 = new(5, 6);
        public static readonly Position f8 = new(5, 7);

        public static readonly Position g1 = new(6, 0);
        public static readonly Position g2 = new(6, 1);
        public static readonly Position g3 = new(6, 2);
        public static readonly Position g4 = new(6, 3);
        public static readonly Position g5 = new(6, 4);
        public static readonly Position g6 = new(6, 5);
        public static readonly Position g7 = new(6, 6);
        public static readonly Position g8 = new(6, 7);

        public static readonly Position h1 = new(7, 0);
        public static readonly Position h2 = new(7, 1);
        public static readonly Position h3 = new(7, 2);
        public static readonly Position h4 = new(7, 3);
        public static readonly Position h5 = new(7, 4);
        public static readonly Position h6 = new(7, 5);
        public static readonly Position h7 = new(7, 6);
        public static readonly Position h8 = new(7, 7);

    }
}
