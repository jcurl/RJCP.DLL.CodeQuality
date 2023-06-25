namespace RJCP.CodeQuality.HelperClasses
{
    public static class NestedStaticTypesAccessor
    {
        private const string AssemblyName = AccessorTest.AssemblyName;
        private const string TypeName = "RJCP.CodeQuality.HelperClasses.NestedStaticTypes";
        private static readonly PrivateType AccType = new PrivateType(AssemblyName, TypeName);

        public static class NestedStaticTypeAccessor
        {
            private static readonly PrivateType AccType =
                NestedStaticTypesAccessor.AccType.GetNestedType("NestedStaticType");

            public static int NestedMethod()
            {
                return (int)AccessorBase.InvokeStatic(AccType, nameof(NestedMethod));
            }
        }
    }

    public class NestedTypes1Accessor : AccessorBase
    {
        private const string AssemblyName = AccessorTest.AssemblyName;
        private const string TypeName = "RJCP.CodeQuality.HelperClasses.NestedTypes1";
        private static readonly PrivateType AccType = new PrivateType(AssemblyName, TypeName);

        public NestedTypes1Accessor() : base(AccType) { }

        public int MethodA()
        {
            return (int)Invoke(nameof(MethodA));
        }

        public class NestedTypeAccessor : AccessorBase
        {
            private static readonly PrivateType AccType =
                NestedTypes1Accessor.AccType.GetNestedType("NestedType");

            public NestedTypeAccessor() : base(AccType) { }

            public int MethodB()
            {
                return (int)Invoke(nameof(MethodB));
            }
        }
    }
}
