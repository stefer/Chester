using System;

namespace Chess
{
    public class Evaluation
    {
        public static int CheckMate = 20000;

        public Evaluation(int value, Move move)
        {
            Value = value;
            Move = move;
        }

        public int Value { get; init; }
        public Move Move { get; init; }

        public bool IsCheckMate() => Math.Abs(Value) >= CheckMate;

        override public string ToString() => $"{Move}:{Value}";
    }
}
