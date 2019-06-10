namespace NUnit.Framework
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    internal class TestContextAccessor
    {
        public static TestContextAccessor GetTestContext()
        {
            StackTrace stackTrace = new StackTrace();
            foreach (StackFrame frame in stackTrace.GetFrames()) {
                Type nunitType = FindTestAttribute(frame.GetMethod());
                if (nunitType != null) {
                    Assembly nUnitAssembly = Assembly.GetAssembly(nunitType);
                    return new TestContextAccessor(nUnitAssembly);
                }
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
        }

        private PrivateObject GetCurrentContext(Assembly nUnitAssembly)
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
    }
}
