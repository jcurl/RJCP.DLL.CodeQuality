namespace RJCP.CodeQuality.NUnitExtensions
{
    using System;

    internal partial class TestContextAccessor
    {
        private static class WriteConsole
        {
            public static void Write(ulong value) { Console.Write(value); }

            public static void Write(uint value) { Console.Write(value); }

            public static void Write(string value) { Console.Write(value); }

            public static void Write(float value) { Console.Write(value); }

            public static void Write(object value) { Console.Write(value); }

            public static void Write(decimal value) { Console.Write(value); }

            public static void Write(long value) { Console.Write(value); }

            public static void Write(int value) { Console.Write(value); }

            public static void Write(double value) { Console.Write(value); }

            public static void Write(char[] value) { Console.Write(value); }

            public static void Write(char value) { Console.Write(value); }

            public static void Write(bool value) { Console.Write(value); }

            public static void Write(string format, object arg0) { Console.Write(format, arg0); }

            public static void Write(string format, object arg0, object arg1) { Console.Write(format, arg0, arg1); }

            public static void Write(string format, object arg0, object arg1, object arg2) { Console.Write(format, arg0, arg1, arg2); }

            public static void Write(string format, params object[] args) { Console.Write(format, args); }

            public static void WriteLine() { Console.WriteLine(); }

            public static void WriteLine(ulong value) { Console.WriteLine(value); }

            public static void WriteLine(uint value) { Console.WriteLine(value); }

            public static void WriteLine(string value) { Console.WriteLine(value); }

            public static void WriteLine(float value) { Console.WriteLine(value); }

            public static void WriteLine(object value) { Console.WriteLine(value); }

            public static void WriteLine(decimal value) { Console.WriteLine(value); }

            public static void WriteLine(long value) { Console.WriteLine(value); }

            public static void WriteLine(int value) { Console.WriteLine(value); }

            public static void WriteLine(double value) { Console.WriteLine(value); }

            public static void WriteLine(char[] value) { Console.WriteLine(value); }

            public static void WriteLine(char value) { Console.WriteLine(value); }

            public static void WriteLine(bool value) { Console.WriteLine(value); }

            public static void WriteLine(string format, object arg0) { Console.WriteLine(format, arg0); }

            public static void WriteLine(string format, object arg0, object arg1) { Console.WriteLine(format, arg0, arg1); }

            public static void WriteLine(string format, object arg0, object arg1, object arg2) { Console.WriteLine(format, arg0, arg1, arg2); }

            public static void WriteLine(string format, params object[] args) { Console.WriteLine(format, args); }
        }
    }
}
