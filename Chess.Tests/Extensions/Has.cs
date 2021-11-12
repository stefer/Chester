using System;

namespace Chester.Tests.Extensions
{
    public class Has : NUnit.Framework.Has
    {
        public static EnumHasFlagConstraint<T> Flag<T>(T expected) where T : Enum
        {
            return new EnumHasFlagConstraint<T>(expected);
        }
    }
}
