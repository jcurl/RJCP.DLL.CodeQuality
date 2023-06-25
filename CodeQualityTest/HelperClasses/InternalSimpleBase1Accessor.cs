namespace RJCP.CodeQuality.HelperClasses
{
    using System;

    public class InternalSimpleBase1Accessor : AccessorBase
    {
        private const string AssemblyName = AccessorTest.AssemblyName;
        private const string TypeName = "RJCP.CodeQuality.HelperClasses.InternalSimpleBase1";

        protected InternalSimpleBase1Accessor(string assemblyName, string typeName, Type[] parameterArguments, object[] arguments)
            : base(assemblyName, typeName, parameterArguments, arguments) { }

        public InternalSimpleBase1Accessor(int value)
            : base(AssemblyName, TypeName, new[] { typeof(int) }, new object[] { value }) { }

        public int Value
        {
            get { return (int)GetFieldOrProperty(nameof(Value)); }
            set { SetFieldOrProperty(nameof(Value), value); }
        }

        public virtual string DoSomething()
        {
            return (string)Invoke(nameof(DoSomething));
        }
    }
}
