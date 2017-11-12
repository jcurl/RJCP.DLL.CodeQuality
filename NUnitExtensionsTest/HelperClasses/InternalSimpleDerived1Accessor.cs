namespace NUnit.Framework.HelperClasses
{
    using System;

    internal class InternalSimpleDerived1Accessor : InternalSimpleBase1Accessor
    {
        private const string AssemblyName = "NUnitExtensionsTest";
        private const string TypeName = "NUnit.Framework.HelperClasses.InternalSimpleDerived1";

        public InternalSimpleDerived1Accessor(int value, string description)
            : base(AssemblyName, TypeName, new[] {typeof(int), typeof(string)}, new object[] { value, description }) { }

        public string Description {
            get { return (string)GetFieldOrProperty("Description"); }
            set { SetFieldOrProperty("Description", value); }
        }

        public override string DoSomething()
        {
            return (string)Invoke("DoSomething");
        }
    }
}
