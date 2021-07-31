namespace RJCP.CodeQuality.HelperClasses
{
    using System;

    public class GenericStackAccessor<T> : AccessorBase
    {
        private const string AssemblyName = "RJCP.CodeQualityTest";
        private const string TypeName = "RJCP.CodeQuality.HelperClasses.GenericStack`1";

        public GenericStackAccessor()
            : base(AssemblyName, TypeName, new Type[] { }, new object[] { }, new Type[] { typeof(T) }) { }

        public void Push(T item)
        {
            Invoke(nameof(Push), item);
        }

        public T Pop()
        {
            return (T)Invoke(nameof(Pop));
        }
    }
}
