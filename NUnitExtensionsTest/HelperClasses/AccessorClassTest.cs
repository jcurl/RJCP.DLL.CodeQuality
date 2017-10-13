namespace NUnit.Framework.HelperClasses
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Used for testing the <see cref="AccessorBase"/> class functionality for a non-generic object.
    /// </summary>
    public class AccessorClassTest : AccessorBase
    {
        public int Capacity { get { return (int)GetFieldOrProperty("Capacity"); } }

        public AccessorClassTest(int length)
            : base("NUnitExtensionsTest", "NUnit.Framework.HelperClasses.ClassTest", new object[] { length })
        {
            BindingFlags = BindingFlags | BindingFlags.NonPublic;
        }
    }
}
