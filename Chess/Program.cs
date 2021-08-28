using System;
using System.Linq;

namespace Chess
{
    class Program
    {
        static void Main(string[] args)
        {

            Board b = new Board();
            var rand = new Random();

            Console.WriteLine(b);

            var color = Color.White;
            while(true)
            {
                var moves = b.MovesFor(color).ToList();
                // Console.WriteLine(string.Join(",", moves));

                if (moves.Count == 0) break;

                var move = moves[rand.Next(0, moves.Count)];
                Console.WriteLine($"Making move {move}");

                Console.ReadKey();

                b.MakeMove(move);

                Console.WriteLine(b);
                color = color == Color.White ? Color.Black : Color.White;
            }

            Console.Beep();
        }
    }
}
