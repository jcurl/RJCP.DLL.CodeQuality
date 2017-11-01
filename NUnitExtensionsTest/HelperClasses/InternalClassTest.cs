namespace NUnit.Framework.HelperClasses
{
    using System;

    /// <summary>
    /// Used for testing the <see cref="PrivateType"/> class functionality.
    /// </summary>
    internal class InternalClassTest : InternalClassBase
    {
        static InternalClassTest()
        {
            MyStaticProperty = 111;
        }

#pragma warning disable CS0414
        private static string s_MyStatic = "static";
#pragma warning restore CS0414

        private static int MyStaticProperty { get; set; }

        private static int TestStaticMethod()
        {
            return 1;
        }

        private static int TestIncArg(int x)
        {
            return x + 1;
        }

        private static float TestIncArg(float arg)
        {
            return arg + 1.0f;
        }

        protected static T GetDescription<T>(int arg) where T : class
        {
            return arg.ToString() as T;
        }

        private static void ThrowEx()
        {
            throw new InvalidOperationException();
        }
    }
}
