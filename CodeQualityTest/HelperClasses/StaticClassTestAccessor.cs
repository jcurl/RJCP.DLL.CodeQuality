namespace RJCP.CodeQuality.HelperClasses
{
    public static class StaticClassTestAccessor
    {
        private const string AssemblyName = AccessorTest.AssemblyName;
        private const string TypeName = "RJCP.CodeQuality.HelperClasses.StaticClassTest";
        private static readonly PrivateType m_Type = new PrivateType(AssemblyName, TypeName);

        public static int Property
        {
            get { return (int)AccessorBase.GetStaticFieldOrProperty(m_Type, nameof(Property)); }
            set { AccessorBase.SetStaticFieldOrProperty(m_Type, nameof(Property), value); }
        }

        public static string DoSomething()
        {
            return (string)AccessorBase.InvokeStatic(m_Type, nameof(DoSomething));
        }
    }
}
