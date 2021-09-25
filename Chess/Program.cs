using Chess.Models;
using System;
using System.Linq;

namespace Chess
{
    class Program
    {
        static void Main(string[] args)
        {

            var game= new Game();

            Console.WriteLine(game);

            while(true)
            {
                var evaluation = game.Search();

                if (Math.Abs(evaluation.Value) >= Evaluation.CheckMate) break;

                Console.WriteLine($"{game.NextToMove} made move {evaluation.Move} with value {evaluation.Value}");

                game.MakeMove(evaluation.Move);

                Console.WriteLine(game);
                Console.WriteLine();

                Console.ReadKey();
            }

            Console.Beep();
        }
    }
}
