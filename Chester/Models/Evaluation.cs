using System;

namespace Chester.Models
{
    public class Evaluation
    {
        public const int CheckMate = 20000;

        public Evaluation(int value, Move move)
        {
            Value = value;
            Move = move;
        }

        public int Value { get; init; }
        public Move Move { get; init; }

        public bool IsCheckMate() => Math.Abs(Value) >= CheckMate;

        public override string ToString() => $"{Move}:{Value}";
    }
}
