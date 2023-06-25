namespace RJCP.CodeQuality.HelperClasses
{
    using System;

    public class ClassExceptionCtorTestAccessor : AccessorBase
    {
        private const string AssemblyName = AccessorTest.AssemblyName;
        private const string TypeName = "RJCP.CodeQuality.HelperClasses.ClassExceptionCtorTest";

        public ClassExceptionCtorTestAccessor()
            : base(AssemblyName, TypeName) { }

        public ClassExceptionCtorTestAccessor(int value)
            : base(AssemblyName, TypeName, new[] { typeof(int) }, new object[] { value }) { }

        // A dummy ctor, so we can use the same constructor without
        // providing the parameterTypes.
        public ClassExceptionCtorTestAccessor(bool temp, int value)
            : base(AssemblyName, TypeName, null, new object[] { value }) { }

        public string Property
        {
            get { return (string)GetFieldOrProperty(nameof(Property)); }
            set { SetFieldOrProperty(nameof(Property), value); }
        }
    }

    public class ClassExceptionCtorTestAccessor<T> : AccessorBase
    {
        private const string AssemblyName = AccessorTest.AssemblyName;
        private const string TypeName = "RJCP.CodeQuality.HelperClasses.ClassExceptionCtorTest`1";

        public ClassExceptionCtorTestAccessor()
            : base(AssemblyName, TypeName, new Type[0], new object[0], new Type[] { typeof(T) }) { }

        public ClassExceptionCtorTestAccessor(T value, int mode)
            : base(AssemblyName, TypeName, new Type[] { typeof(T), typeof(int) }, new object[] { value, mode }, new Type[] { typeof(T) }) { }

        public string Property
        {
            get { return (string)GetFieldOrProperty(nameof(Property)); }
            set { SetFieldOrProperty(nameof(Property), value); }
        }
    }
}
