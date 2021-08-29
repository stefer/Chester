using System;
using System.Linq;

namespace Chess
{
    class Program
    {
        static void Main(string[] args)
        {

            Game game= new Game();
            var b = game.Board;
            var rand = new Random();

            Console.WriteLine(b);

            var color = Color.White;
            while(true)
            {
                var moves = b.MovesFor(color).ToList();
                // Console.WriteLine(string.Join(",", moves));

                if (moves.Count == 0) break;

                var evaluations = moves.Select(x => game.Evaluate(x));

                var eval = evaluations.Aggregate(evaluations.First(), (acc, x) => (int)color * x.Value > (int)color * acc.Value ? x : acc);
                Console.WriteLine($"Making move {eval.Move} with value {eval.Value}");

                b.MakeMove(eval.Move);

                Console.ReadKey();

                Console.WriteLine(b);
                Console.WriteLine();
                color = color == Color.White ? Color.Black : Color.White;
            }

            Console.Beep();
        }
    }
}
