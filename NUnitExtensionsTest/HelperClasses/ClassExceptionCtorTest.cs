namespace NUnit.Framework.HelperClasses
{
    using System;

    internal class ClassExceptionCtorTest
    {
        public ClassExceptionCtorTest()
        {
            throw new InvalidOperationException();
        }

        public ClassExceptionCtorTest(int value)
        {
            if (value == 42)
                throw new NotSupportedException("The meaning of life, the universe and everything");

            if (value == 43)
                throw new System.Reflection.TargetInvocationException(new InvalidOperationException("Getting Better"));
        }
    }
}
