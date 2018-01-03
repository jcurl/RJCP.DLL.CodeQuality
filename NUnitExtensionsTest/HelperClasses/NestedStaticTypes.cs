namespace NUnit.Framework.HelperClasses
{
    using System;

    internal static class NestedStaticTypes
    {
        internal static class NestedStaticType
        {
            public static int NestedMethod()
            {
                return 42;
            }
        }
    }

    internal class NestedTypes1
    {
        public int MethodA() { return 42; }

        internal class NestedType
        {
            public int MethodB() { return 64; }
        }
    }
}
