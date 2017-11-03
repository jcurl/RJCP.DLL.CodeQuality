namespace NUnit.Framework.HelperClasses
{
    using System;

    public class ClassExceptionCtorTestAccessor : AccessorBase
    {
        private const string AssemblyName = "NUnitExtensionsTest";
        private const string TypeName = "NUnit.Framework.HelperClasses.ClassExceptionCtorTest";

        public ClassExceptionCtorTestAccessor()
            : base(AssemblyName, TypeName) { }

        public ClassExceptionCtorTestAccessor(int value)
            : base(AssemblyName, TypeName, new[] { typeof(int) }, new object[] { value }) { }

        // A dummy ctor, so we can use the same constructor without
        // providing the parameterTypes.
        public ClassExceptionCtorTestAccessor(bool temp, int value)
            : base(AssemblyName, TypeName, null, new object[] { value }) { }
    }
}
