namespace RJCP.CodeQuality.NUnitExtensions
{
    internal class TestAccessor : AccessorBase
    {
        public TestAccessor(object testObject)
            : base(new PrivateObject(testObject)) { }

        public string Name { get { return (string)GetFieldOrProperty(nameof(Name)); } }

        public string FullName { get { return (string)GetFieldOrProperty(nameof(FullName)); } }
    }
}
