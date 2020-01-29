namespace RJCP.CodeQuality.HelperClasses
{
    internal static class StaticClassTest
    {
        public static int Property { get; set; }

        public static string DoSomething()
        {
            return Property.ToString();
        }
    }
}
