namespace NUnit.Framework
{
    using System;

    /// <summary>
    /// Used for testing the <see cref="PrivateType"/> class functionality.
    /// </summary>
    internal class InternalClassBase
    {
        static InternalClassBase()
        {
            MyBaseStaticProp = 99;
        }

        protected InternalClassBase() { }

        protected static int MyBaseStaticProp { get; set; }

        protected static int MyBaseStaticMethod()
        {
            return 111;
        }

        protected static string MyBaseStaticIntMethod(int arg)
        {
            return arg.ToString();
        }

        protected static T MyBaseStaticGenericMethod<T>(int arg) where T : class
        {
            return arg.ToString() as T;
        }
    }
}
