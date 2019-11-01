namespace NUnit.Framework.HelperClasses
{
    public class ClassExceptionCtorTestAccessor2 : AccessorBase
    {
        private const string AssemblyName = "NUnitExtensionsTest";
        private const string TypeName = "NUnit.Framework.HelperClasses.ClassExceptionCtorTest";
        private readonly static PrivateType AccType = new PrivateType(AssemblyName, TypeName);

        public ClassExceptionCtorTestAccessor2()
            : base(AccType) { }

        public ClassExceptionCtorTestAccessor2(int value)
            : base(AccType, new[] { typeof(int) }, new object[] { value }) { }

        // A dummy ctor, so we can use the same constructor without
        // providing the parameterTypes.
        public ClassExceptionCtorTestAccessor2(bool temp, int value)
            : base(AccType, null, new object[] { value }) { }

        public string Property
        {
            get { return (string)GetFieldOrProperty(nameof(Property)); }
            set { SetFieldOrProperty(nameof(Property), value); }
        }

        public static int Property2
        {
            get { return (int)GetStaticFieldOrProperty(AccType, nameof(Property2)); }
            set { SetStaticFieldOrProperty(AccType, nameof(Property2), value); }
        }
    }
}
