namespace NUnit.Framework.HelperClasses
{
    using System;

    internal static class StaticClassTest
    {
        public static int Property { get; set; }

        public static string DoSomething()
        {
            return Property.ToString();
        }
    }
}
