namespace RJCP.CodeQuality.AppConfig
{
    using System.Configuration;

#if NET6_0_OR_GREATER
    using System.Diagnostics;
#endif

    internal class NUnitExtensionsSection : ConfigurationSection
    {
        private static readonly object s_SettingsLock = new object();
        private static NUnitExtensionsSection s_Settings;

#if NETFRAMEWORK
        public static NUnitExtensionsSection Settings
        {
            get
            {
                if (s_Settings == null) {
                    lock (s_SettingsLock) {
                        if (s_Settings == null) {
                            s_Settings = ConfigurationManager.GetSection("NUnitExtensions") as NUnitExtensionsSection;
                        }
                    }
                }
                return s_Settings;
            }
        }
#else
        public static NUnitExtensionsSection Settings
        {
            get
            {
                if (s_Settings == null) {
                    lock (s_SettingsLock) {
                        if (s_Settings == null) {
                            // NetStandard / .NET Core doesn't support loading configurations automatically. Further,
                            // when running test cases often the assembly name is the TestHost, which is not the same
                            // as the app configuration path, e.g. MyLibraryTest.dll.config.

                            // We haven't loaded this configuration, so go through the stack trace, looking for the
                            // assemblies that are used, identifying the first configuration file found. If there are
                            // multiple configuration files, the first (deepest) in the call stack is used for this and
                            // all subsequent calls. Normally there should be only one configuration file.

                            StackTrace stackTrace = new StackTrace();
                            string workingAssembly = null;
                            foreach (StackFrame frame in stackTrace.GetFrames()) {
                                string frameAssembly = frame.GetMethod().ReflectedType.Assembly.Location;
                                if (frameAssembly != null && frameAssembly != workingAssembly) {
                                    string configFileName = string.Format("{0}.config", frameAssembly);
                                    if (System.IO.File.Exists(configFileName)) {
                                        Configuration appConfig = ConfigurationManager.OpenExeConfiguration(frameAssembly);
                                        s_Settings = appConfig.GetSection("NUnitExtensions") as NUnitExtensionsSection;
                                        return s_Settings;
                                    }
                                    workingAssembly = frameAssembly;
                                }
                            }

                            s_Settings = ConfigurationManager.GetSection("NUnitExtensions") as NUnitExtensionsSection;
                        }
                    }
                }
                return s_Settings;
            }
        }
#endif

        [ConfigurationProperty("deploy")]
        public Deploy Deploy
        {
            get { return (Deploy)this["deploy"]; }
        }
    }
}
