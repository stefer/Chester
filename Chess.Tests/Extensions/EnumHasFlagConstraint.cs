using NUnit.Framework.Constraints;
using System;

namespace Chester.Tests.Extensions
{
    public class EnumHasFlagConstraint<T> : Constraint where T : Enum
    {
        readonly T _expected;

        public EnumHasFlagConstraint(T expected)
        {
            _expected = expected;
            Description = $"EnumHasFlag {expected}";
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if (typeof(TActual) != typeof(T))
                return new ConstraintResult(this, actual, ConstraintStatus.Error);

            T aEnum = (T)(object)actual;

            if (aEnum.HasFlag(_expected))
                return new ConstraintResult(this, actual, ConstraintStatus.Success);
            else
                return new ConstraintResult(this, actual, ConstraintStatus.Failure);
        }
    }
}
