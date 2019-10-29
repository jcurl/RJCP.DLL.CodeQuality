namespace NUnit.Framework.HelperClasses
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Used for testing the <see cref="AccessorBase"/> class functionality for a non-generic object.
    /// </summary>
    public class ClassTestAccessor : AccessorBase
    {
        public int Capacity { get { return (int)GetFieldOrProperty("Capacity"); } }

        public ClassTestAccessor(int length)
            : base("NUnitExtensionsTest", "NUnit.Framework.HelperClasses.ClassTest", new object[] { length })
        {
            BindingFlags |= BindingFlags.NonPublic;
        }
    }
}
