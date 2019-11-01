namespace NUnit.Framework.HelperClasses
{
    public static class StaticClassTestAccessor
    {
        private const string AssemblyName = "NUnitExtensionsTest";
        private const string TypeName = "NUnit.Framework.HelperClasses.StaticClassTest";
        private static readonly PrivateType m_Type = new PrivateType(AssemblyName, TypeName);

        public static int Property {
            get { return (int)AccessorBase.GetStaticFieldOrProperty(m_Type, "Property"); }
            set { AccessorBase.SetStaticFieldOrProperty(m_Type, "Property", value); }
        }

        public static string DoSomething()
        {
            return (string)AccessorBase.InvokeStatic(m_Type, "DoSomething");
        }
    }
}
