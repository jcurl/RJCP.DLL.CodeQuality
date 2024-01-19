namespace RJCP.CodeQuality.NUnitExtensions
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    internal partial class TestContextAccessor
    {
        private static readonly object s_Lock = new object();
        private static TestContextAccessor s_TestContext;

        public static TestContextAccessor Instance
        {
            get
            {
                if (s_TestContext == null) {
                    lock (s_Lock) {
                        if (s_TestContext == null) {
                            s_TestContext = Create();
                        }
                    }
                }
                return s_TestContext;
            }
        }

        private static TestContextAccessor Create()
        {
            Assembly nUnitAssembly;

            nUnitAssembly = GetNUnitFromStackTrace();
            if (nUnitAssembly != null) return new TestContextAccessor(nUnitAssembly);
            nUnitAssembly = GetNUnitFromAssemblyList();
            if (nUnitAssembly != null) return new TestContextAccessor(nUnitAssembly);

            throw new InvalidOperationException("Can't determine NUnit Test Context. Missing Test attribute or running on a thread?");
        }

        private static Assembly GetNUnitFromStackTrace()
        {
            StackTrace stackTrace = new StackTrace();
            foreach (StackFrame frame in stackTrace.GetFrames()) {
                Type nunitType = FindTestAttribute(frame.GetMethod());
                if (nunitType != null) {
                    return Assembly.GetAssembly(nunitType);
                }
            }
            return null;
        }

        private static Assembly GetNUnitFromAssemblyList()
        {
            AppDomain domain = AppDomain.CurrentDomain;
            foreach (Assembly assembly in domain.GetAssemblies()) {
                Type attrType = assembly.GetType("NUnit.Framework.TestAttribute");
                if (attrType != null) return assembly;
            }
            return null;
        }

        private static Type FindTestAttribute(MethodBase method)
        {
            object[] attrs;

            // Get the Test Fixture
            MemberInfo declaringType = method.DeclaringType;
            attrs = declaringType.GetCustomAttributes(true);
            foreach (object attr in attrs) {
                Type attrType = attr.GetType();
                string attrTypeName = attrType.ToString();
                if (attrTypeName.Equals("NUnit.Framework.TestFixtureAttribute")) {
                    return attrType;
                }
            }

            // Get the Method attributes
            attrs = method.GetCustomAttributes(false);
            foreach (object attr in attrs) {
                Type attrType = attr.GetType();
                string attrTypeName = attrType.ToString();
                if (attrTypeName.Equals("NUnit.Framework.TestAttribute") ||
                    attrTypeName.Equals("NUnit.Framework.TestCaseAttribute")) {
                    return attrType;
                }
            }
            return null;
        }

        private readonly string m_TestDirectory;
        private readonly string m_WorkDirectory;
        private readonly Assembly m_NUnitAssembly;

        private TestContextAccessor(Assembly nUnitAssembly)
        {
            if (nUnitAssembly == null) throw new ArgumentNullException(nameof(nUnitAssembly));
            m_NUnitAssembly = nUnitAssembly;

            PrivateObject currentContext = GetCurrentContext(nUnitAssembly);
            m_TestDirectory = (string)currentContext.GetFieldOrProperty("TestDirectory");
            m_WorkDirectory = (string)currentContext.GetFieldOrProperty("WorkDirectory");

            // Modify the work directory if the user has added relevant details in the configuration
            // - if the current directory should be used, then use it; or
            // - if the Test/Work directory are the same (default), then use a custom path; or
            // - if the user specified force, then use a custom path
            AppConfig.NUnitExtensionsSection settings = AppConfig.NUnitExtensionsSection.Settings;
            if (settings != null) {
                if (settings.Deploy.UseCurrentDirectory) {
                    m_WorkDirectory = Environment.CurrentDirectory;
                } else if (!settings.Deploy.DeployWorkDirectory.Equals(string.Empty)) {
                    if (settings.Deploy.Force ||
                        m_WorkDirectory != null && m_WorkDirectory.Equals(m_TestDirectory)) {

                        if (Path.IsPathRooted(settings.Deploy.DeployWorkDirectory)) {
                            m_WorkDirectory = settings.Deploy.DeployWorkDirectory;
                        } else {
                            m_WorkDirectory = new Uri(Path.Combine(m_WorkDirectory, settings.Deploy.DeployWorkDirectory)).LocalPath;
                        }
                    }
                }
            }

            InitWriteAccessor(currentContext);
        }

        private static PrivateObject GetCurrentContext(Assembly nUnitAssembly)
        {
            PrivateType testContextType = new PrivateType(nUnitAssembly.GetType("NUnit.Framework.TestContext"));
            object currentContext = AccessorBase.GetStaticFieldOrProperty(testContextType, "CurrentContext");
            return new PrivateObject(currentContext);
        }

        private TestAccessor Test
        {
            get
            {
                // The value changes on every test, so we need to get the name on every call.
                PrivateObject currentContext = GetCurrentContext(m_NUnitAssembly);
                object testProperty = currentContext.GetFieldOrProperty("Test");
                if (testProperty == null) return null;

                return new TestAccessor(testProperty);
            }
        }

        public string TestDirectory { get { return m_TestDirectory; } }

        public string WorkDirectory { get { return m_WorkDirectory; } }

        public string TestName { get { return Test.Name; } }

        public string TestFullName { get { return Test.FullName; } }

        #region Write via Reflection
        private Action<ulong> m_WriteUlong;
        private Action<uint> m_WriteUint;
        private Action<string> m_WriteString;
        private Action<float> m_WriteFloat;
        private Action<object> m_WriteObject;
        private Action<decimal> m_WriteDecimal;
        private Action<long> m_WriteLong;
        private Action<int> m_WriteInt;
        private Action<double> m_WriteDouble;
        private Action<char[]> m_WriteCharArray;
        private Action<char> m_WriteChar;
        private Action<bool> m_WriteBool;
        private Action<string, object> m_WriteFormat1;
        private Action<string, object, object> m_WriteFormat2;
        private Action<string, object, object, object> m_WriteFormat3;
        private Action<string, object[]> m_WriteFormatS;

        private Action m_WriteLine;
        private Action<ulong> m_WriteLineUlong;
        private Action<uint> m_WriteLineUint;
        private Action<string> m_WriteLineString;
        private Action<float> m_WriteLineFloat;
        private Action<object> m_WriteLineObject;
        private Action<decimal> m_WriteLineDecimal;
        private Action<long> m_WriteLineLong;
        private Action<int> m_WriteLineInt;
        private Action<double> m_WriteLineDouble;
        private Action<char[]> m_WriteLineCharArray;
        private Action<char> m_WriteLineChar;
        private Action<bool> m_WriteLineBool;
        private Action<string, object> m_WriteLineFormat1;
        private Action<string, object, object> m_WriteLineFormat2;
        private Action<string, object, object, object> m_WriteLineFormat3;
        private Action<string, object[]> m_WriteLineFormatS;

        private void InitWriteAccessor(PrivateObject currentContext)
        {
            m_WriteUlong = GetDelegate<ulong>(currentContext.RealType, nameof(Write));
            m_WriteUint = GetDelegate<uint>(currentContext.RealType, nameof(Write));
            m_WriteString = GetDelegate<string>(currentContext.RealType, nameof(Write));
            m_WriteFloat = GetDelegate<float>(currentContext.RealType, nameof(Write));
            m_WriteObject = GetDelegate<object>(currentContext.RealType, nameof(Write));
            m_WriteDecimal = GetDelegate<decimal>(currentContext.RealType, nameof(Write));
            m_WriteLong = GetDelegate<long>(currentContext.RealType, nameof(Write));
            m_WriteInt = GetDelegate<int>(currentContext.RealType, nameof(Write));
            m_WriteDouble = GetDelegate<double>(currentContext.RealType, nameof(Write));
            m_WriteCharArray = GetDelegate<char[]>(currentContext.RealType, nameof(Write));
            m_WriteChar = GetDelegate<char>(currentContext.RealType, nameof(Write));
            m_WriteBool = GetDelegate<bool>(currentContext.RealType, nameof(Write));

            MethodInfo methodInfoArgs1 = currentContext.RealType.GetMethod(nameof(Write), new Type[] { typeof(string), typeof(object) })
                ?? typeof(WriteConsole).GetMethod(nameof(Write), new Type[] { typeof(string), typeof(object) });
            m_WriteFormat1 = (Action<string, object>)Delegate.CreateDelegate(typeof(Action<string, object>), methodInfoArgs1);

            MethodInfo methodInfoArgs2 = currentContext.RealType.GetMethod(nameof(Write), new Type[] { typeof(string), typeof(object), typeof(object) })
                ?? typeof(WriteConsole).GetMethod(nameof(Write), new Type[] { typeof(string), typeof(object), typeof(object) });
            m_WriteFormat2 = (Action<string, object, object>)Delegate.CreateDelegate(typeof(Action<string, object, object>), methodInfoArgs2);

            MethodInfo methodInfoArgs3 = currentContext.RealType.GetMethod(nameof(Write), new Type[] { typeof(string), typeof(object), typeof(object), typeof(object) })
                ?? typeof(WriteConsole).GetMethod(nameof(Write), new Type[] { typeof(string), typeof(object), typeof(object), typeof(object) });
            m_WriteFormat3 = (Action<string, object, object, object>)Delegate.CreateDelegate(typeof(Action<string, object, object, object>), methodInfoArgs3);

            MethodInfo methodInfoArgsS = currentContext.RealType.GetMethod(nameof(Write), new Type[] { typeof(string), typeof(object[]) })
                ?? typeof(WriteConsole).GetMethod(nameof(Write), new Type[] { typeof(string), typeof(object[]) });
            m_WriteFormatS = (Action<string, object[]>)Delegate.CreateDelegate(typeof(Action<string, object[]>), methodInfoArgsS);

            m_WriteLineUlong = GetDelegate<ulong>(currentContext.RealType, nameof(WriteLine));
            m_WriteLineUint = GetDelegate<uint>(currentContext.RealType, nameof(WriteLine));
            m_WriteLineString = GetDelegate<string>(currentContext.RealType, nameof(WriteLine));
            m_WriteLineFloat = GetDelegate<float>(currentContext.RealType, nameof(WriteLine));
            m_WriteLineObject = GetDelegate<object>(currentContext.RealType, nameof(WriteLine));
            m_WriteLineDecimal = GetDelegate<decimal>(currentContext.RealType, nameof(WriteLine));
            m_WriteLineLong = GetDelegate<long>(currentContext.RealType, nameof(WriteLine));
            m_WriteLineInt = GetDelegate<int>(currentContext.RealType, nameof(WriteLine));
            m_WriteLineDouble = GetDelegate<double>(currentContext.RealType, nameof(WriteLine));
            m_WriteLineCharArray = GetDelegate<char[]>(currentContext.RealType, nameof(WriteLine));
            m_WriteLineChar = GetDelegate<char>(currentContext.RealType, nameof(WriteLine));
            m_WriteLineBool = GetDelegate<bool>(currentContext.RealType, nameof(WriteLine));

            MethodInfo methodInfoLineArgs1 = currentContext.RealType.GetMethod(nameof(WriteLine), new Type[] { typeof(string), typeof(object) })
                ?? typeof(WriteConsole).GetMethod(nameof(WriteLine), new Type[] { typeof(string), typeof(object) });
            m_WriteLineFormat1 = (Action<string, object>)Delegate.CreateDelegate(typeof(Action<string, object>), methodInfoLineArgs1);

            MethodInfo methodInfoLineArgs2 = currentContext.RealType.GetMethod(nameof(WriteLine), new Type[] { typeof(string), typeof(object), typeof(object) })
                ?? typeof(WriteConsole).GetMethod(nameof(WriteLine), new Type[] { typeof(string), typeof(object), typeof(object) });
            m_WriteLineFormat2 = (Action<string, object, object>)Delegate.CreateDelegate(typeof(Action<string, object, object>), methodInfoLineArgs2);

            MethodInfo methodInfoLineArgs3 = currentContext.RealType.GetMethod(nameof(WriteLine), new Type[] { typeof(string), typeof(object), typeof(object), typeof(object) })
                ?? typeof(WriteConsole).GetMethod(nameof(WriteLine), new Type[] { typeof(string), typeof(object), typeof(object), typeof(object) });
            m_WriteLineFormat3 = (Action<string, object, object, object>)Delegate.CreateDelegate(typeof(Action<string, object, object, object>), methodInfoLineArgs3);

            MethodInfo methodInfoLineArgsS = currentContext.RealType.GetMethod(nameof(WriteLine), new Type[] { typeof(string), typeof(object[]) })
                ?? typeof(WriteConsole).GetMethod(nameof(WriteLine), new Type[] { typeof(string), typeof(object[]) });
            m_WriteLineFormatS = (Action<string, object[]>)Delegate.CreateDelegate(typeof(Action<string, object[]>), methodInfoLineArgsS);

#if NET462_OR_GREATER || NETSTANDARD
            MethodInfo methodInfoLine = currentContext.RealType.GetMethod(nameof(WriteLine), Array.Empty<Type>())
                ?? typeof(WriteConsole).GetMethod(nameof(WriteLine), Array.Empty<Type>());
#else
            MethodInfo methodInfoLine = currentContext.RealType.GetMethod(nameof(WriteLine), new Type[] { })
                ?? typeof(WriteConsole).GetMethod(nameof(WriteLine), new Type[] { });
#endif
            m_WriteLine = (Action)Delegate.CreateDelegate(typeof(Action), methodInfoLine);
        }

        private static Action<T> GetDelegate<T>(Type type, string methodName)
        {
            MethodInfo methodInfo = type.GetMethod(methodName, new Type[] { typeof(T) })
                ?? typeof(WriteConsole).GetMethod(methodName, new Type[] { typeof(T) });
            return (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), methodInfo);
        }

        public void Write(ulong value) { m_WriteUlong(value); }

        public void Write(uint value) { m_WriteUint(value); }

        public void Write(string value) { m_WriteString(value); }

        public void Write(float value) { m_WriteFloat(value); }

        public void Write(object value) { m_WriteObject(value); }

        public void Write(decimal value) { m_WriteDecimal(value); }

        public void Write(long value) { m_WriteLong(value); }

        public void Write(int value) { m_WriteInt(value); }

        public void Write(double value) { m_WriteDouble(value); }

        public void Write(char[] value) { m_WriteCharArray(value); }

        public void Write(char value) { m_WriteChar(value); }

        public void Write(bool value) { m_WriteBool(value); }

        public void Write(string format, object arg0) { m_WriteFormat1(format, arg0); }

        public void Write(string format, object arg0, object arg1) { m_WriteFormat2(format, arg0, arg1); }

        public void Write(string format, object arg0, object arg1, object arg2) { m_WriteFormat3(format, arg0, arg1, arg2); }

        public void Write(string format, params object[] args) { m_WriteFormatS(format, args); }

        public void WriteLine() { m_WriteLine(); }

        public void WriteLine(ulong value) { m_WriteLineUlong(value); }

        public void WriteLine(uint value) { m_WriteLineUint(value); }

        public void WriteLine(string value) { m_WriteLineString(value); }

        public void WriteLine(float value) { m_WriteLineFloat(value); }

        public void WriteLine(object value) { m_WriteLineObject(value); }

        public void WriteLine(decimal value) { m_WriteLineDecimal(value); }

        public void WriteLine(long value) { m_WriteLineLong(value); }

        public void WriteLine(int value) { m_WriteLineInt(value); }

        public void WriteLine(double value) { m_WriteLineDouble(value); }

        public void WriteLine(char[] value) { m_WriteLineCharArray(value); }

        public void WriteLine(char value) { m_WriteLineChar(value); }

        public void WriteLine(bool value) { m_WriteLineBool(value); }

        public void WriteLine(string format, object arg0) { m_WriteLineFormat1(format, arg0); }

        public void WriteLine(string format, object arg0, object arg1) { m_WriteLineFormat2(format, arg0, arg1); }

        public void WriteLine(string format, object arg0, object arg1, object arg2) { m_WriteLineFormat3(format, arg0, arg1, arg2); }

        public void WriteLine(string format, params object[] args) { m_WriteLineFormatS(format, args); }
        #endregion
    }
}
