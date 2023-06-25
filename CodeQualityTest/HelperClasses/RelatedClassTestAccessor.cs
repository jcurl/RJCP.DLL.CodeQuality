namespace RJCP.CodeQuality.HelperClasses
{
    using System;

    public class RelatedClassTestAccessor : AccessorBase
    {
        private const string AssemblyName = AccessorTest.AssemblyName;
        private const string TypeName = "RJCP.CodeQuality.HelperClasses.RelatedClassTest";

        public RelatedClassTestAccessor(PrivateObject obj) : base(obj) { }

        public RelatedClassTestAccessor(int initialValue)
            : base(AssemblyName, TypeName, new Type[] { typeof(int) }, new object[] { initialValue }) { }

        public int Value
        {
            get
            {
                return (int)GetFieldOrProperty(nameof(Value));
            }
        }
    }

    public class RelatedClassTestFactoryAccessor : AccessorBase
    {
        private const string AssemblyName = AccessorTest.AssemblyName;
        private const string TypeName = "RJCP.CodeQuality.HelperClasses.RelatedClassTestFactory";

        public RelatedClassTestFactoryAccessor()
            : base(AssemblyName, TypeName) { }

        public RelatedClassTestAccessor Create()
        {
            object obj = Invoke(nameof(Create));
            return obj == null ? null : new RelatedClassTestAccessor(new PrivateObject(obj));
        }

        public RelatedClassTestAccessor Create_PrivateObjectNull()
        {
            PrivateObject nullPrvObj = null;
            return new RelatedClassTestAccessor(nullPrvObj);
        }
    }
}
