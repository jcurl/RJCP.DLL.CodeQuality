namespace RJCP.CodeQuality.HelperClasses
{
    internal class InternalSimpleDerived1Accessor : InternalSimpleBase1Accessor
    {
        private const string AssemblyName = AccessorTest.AssemblyName;
        private const string TypeName = "RJCP.CodeQuality.HelperClasses.InternalSimpleDerived1";

        public InternalSimpleDerived1Accessor(int value, string description)
            : base(AssemblyName, TypeName, new[] { typeof(int), typeof(string) }, new object[] { value, description }) { }

        public string Description
        {
            get { return (string)GetFieldOrProperty(nameof(Description)); }
            set { SetFieldOrProperty(nameof(Description), value); }
        }

        public override string DoSomething()
        {
            return (string)Invoke(nameof(DoSomething));
        }
    }
}
