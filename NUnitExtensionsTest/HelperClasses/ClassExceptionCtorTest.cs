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

        public string Property
        {
            get { throw new ObjectDisposedException("exception"); }
            set { throw new InvalidOperationException(); }
        }

        public static int Property2
        {
            get { throw new ObjectDisposedException("exception"); }
            set { throw new ArgumentOutOfRangeException("value"); }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Test case uses reflection")]
    internal class ClassExceptionCtorTest<T>
    {
        public ClassExceptionCtorTest()
        {
            throw new InvalidOperationException();
        }

        public ClassExceptionCtorTest(T value, int mode)
        {
            if (mode == 42)
                throw new NotSupportedException("The meaning of life, the universe and everything");

            if (mode == 43)
                throw new System.Reflection.TargetInvocationException(new InvalidOperationException("Getting Better"));
        }

        public string Property
        {
            get { throw new ObjectDisposedException("exception"); }
            set { throw new InvalidOperationException(); }
        }
    }
}
