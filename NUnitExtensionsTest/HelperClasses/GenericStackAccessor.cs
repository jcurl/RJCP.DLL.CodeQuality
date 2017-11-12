namespace NUnit.Framework.HelperClasses
{
    using System;

    public class GenericStackAccessor<T> : AccessorBase
    {
        private const string AssemblyName = "NUnitExtensionsTest";
        private const string TypeName = "NUnit.Framework.HelperClasses.GenericStack`1";

        public GenericStackAccessor()
            : base(AssemblyName, TypeName, new Type[] { }, new object[] { }, new Type[] {typeof(T)}) { }

        public void Push(T item)
        {
            Invoke("Push", item);
        }

        public T Pop()
        {
            return (T)Invoke("Pop");
        }
    }
}
