namespace RJCP.CodeQuality.HelperClasses
{
    using System.Reflection;

    /// <summary>
    /// Used for testing the <see cref="AccessorBase"/> class functionality for a non-generic object.
    /// </summary>
    public class ClassTestAccessor : AccessorBase
    {
        public int Capacity { get { return (int)GetFieldOrProperty(nameof(Capacity)); } }

        public ClassTestAccessor(int length)
            : base("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.ClassTest", new object[] { length })
        {
            BindingFlags |= BindingFlags.NonPublic;
        }
    }
}
