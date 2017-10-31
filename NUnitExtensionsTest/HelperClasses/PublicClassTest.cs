using System;

namespace NUnit.Framework
{
    /// <summary>
    /// Used for testing the <see cref="PrivateType"/> class functionality.
    /// </summary>
    public class PublicClassTest
    {
        static PublicClassTest()
        {
            MyStaticProperty = 111;
        }

#pragma warning disable CS0414
        private static string s_MyStatic = "static";
#pragma warning restore CS0414

        private static int MyStaticProperty { get; set; }

        private static int TestPrivateStaticMethod()
        {
            return 5;
        }

        private static int TestIncArg(int x)
        {
            return x + 1;
        }

        private static float TestIncArg(float x)
        {
            return x + 1.0f;
        }

        protected static T GetDescription<T>(int arg) where T : class
        {
            return arg.ToString() as T;
        }
    }
}
