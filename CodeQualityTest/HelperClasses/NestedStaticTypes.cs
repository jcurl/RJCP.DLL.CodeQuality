namespace RJCP.CodeQuality.HelperClasses
{
    using System;

    internal static class NestedStaticTypes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "Test Case")]
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "Test Case")]
        internal class NestedType
        {
            public int MethodB() { return 64; }
        }
    }
}
