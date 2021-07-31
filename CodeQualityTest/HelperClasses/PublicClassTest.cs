﻿namespace RJCP.CodeQuality.HelperClasses
{
    /// <summary>
    /// Used for testing the <see cref="PrivateType"/> class functionality.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Test case uses reflection")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0052:Remove unread private members", Justification = "Test case uses reflection")]
    public class PublicClassTest
    {
        static PublicClassTest()
        {
            MyStaticProperty = 111;
        }

#pragma warning disable CS0414  // Assigned a value never used (obtained via reflection)
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
