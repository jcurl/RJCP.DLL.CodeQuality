namespace RJCP.CodeQuality.NUnitExtensions.Trace
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Configuration;

    /// <summary>
    /// NUnit Logger Extensions for adding a <see cref="NUnitLogger"/>.
    /// </summary>
    /// <remarks>
    /// Integrating the <see cref="NUnitLogger"/> into your NUnit project depends on the test case.
    /// <para></para>
    /// <para><b>Using <see cref="ILoggerFactory"/></b>.</para>
    /// <para>
    /// If your test case allows injection of an <see cref="ILoggerFactory"/>, you would construct the logging setup in
    /// your test code, and inject the settings, so that when the code under test gets an <see cref="ILogger"/>, it gets
    /// the correct configuration.
    /// </para>
    /// <code language="csharp"><![CDATA[
    ///ILoggerFactory factory = LoggerFactory.Create(builder => {
    ///  builder
    ///    .AddFilter("Microsoft", LogLevel.Warning)
    ///    .AddFilter("System", LogLevel.Warning)
    ///    .AddFilter("RJCP.IO.DeviceMgr", LogLevel.Debug)
    ///    .AddNUnitLogger();
    ///});
    /// ]]></code>
    /// <para>
    /// You would then have to inject the <see cref="ILoggerFactory"/> into the code somehow that is being tested.
    /// </para>
    /// <para></para>
    /// <para><b>Injecting via <c>LogSource</c></b>.</para>
    /// <para>
    /// If you are using the NuGet package <b>RJCP.Diagnostics.Trace</b>, your code in your project being tested likely
    /// has a method to read and configure its logging. A library performing logging (like <b>RJCP.IO.DeviceMgr</b>)
    /// defines a static property that is used for logging such as:
    /// </para>
    /// <code language="csharp"><![CDATA[
    ///namespace RJCP.IO.DeviceMgr
    ///{
    ///  using RJCP.Diagnostics.Trace;
    ///
    ///  internal static class Log {
    ///    private const string CfgMgrIdentifier = "RJCP.IO.DeviceMgr";
    ///    public static readonly LogSource CfgMgr = new(CfgMgrIdentifier);
    ///  }
    ///}
    /// ]]></code>
    /// <para>
    /// As the test code is responsible for configuring the logging, you initialise the <see cref="NUnitLogger"/> as
    /// follows:
    /// </para>
    /// <para>
    /// Create a file called <c>GlobalLogger.cs</c> in your test project with the following implementation:
    /// </para>
    /// <code language="csharp"><![CDATA[
    ///namespace RJCP
    ///{
    ///    // This file is only for .NET Core
    /// 
    ///    using Microsoft.Extensions.Logging;
    ///    using RJCP.CodeQuality.NUnitExtensions.Trace;
    ///    using RJCP.Diagnostics.Trace;
    /// 
    ///    internal static class GlobalLogger {
    ///        static GlobalLogger() {
    ///            ILoggerFactory factory = LoggerFactory.Create(builder => {
    ///                builder
    ///                    .AddFilter("Microsoft", LogLevel.Warning)
    ///                    .AddFilter("System", LogLevel.Warning)
    ///                    .AddFilter("RJCP.IO.DeviceMgr", LogLevel.Debug)
    ///                    .AddNUnitLogger();
    ///            });
    ///            LogSource.SetLoggerFactory(factory, true);
    ///        }
    /// 
    ///        // Just calling this method will result in the static constructor being executed.
    ///        public static void Initialize() {
    ///            /* Can be empty, reference will initialize static constructor */
    ///        }
    ///    }
    ///}
    /// ]]></code>
    /// <para>
    /// Then create a test fixture that runs, which then initialises the <c>GlobalLogger</c>, injecting the
    /// <see cref="NUnitLogger"/> via the <see cref="AddNUnitLogger(ILoggingBuilder)"/> method:
    /// </para>
    /// <code language="csharp"><![CDATA[
    ///namespace RJCP
    ///{
    ///    // This file is only for .NET Core
    /// 
    ///    using NUnit.Framework;
    /// 
    ///    [SetUpFixture]
    ///    public class TestSetupFixture
    ///    {
    ///        [OneTimeSetUp]
    ///        public void GlobalSetup()
    ///        {
    ///            GlobalLogger.Initialize();
    ///        }
    ///    }
    ///}
    /// ]]></code>
    /// <para>
    /// It works that NUnit first calls into the <c>[SetUpFixture]</c>, which calls the empty method
    /// <c>GlobalLogger.Initialize()</c>. The first time a staticmethod of a static class is called, the static
    /// constructor is run, which injects the factory into the <c>LogSource</c> via the <c>SetLoggerFactory</c>. The
    /// <see langword="true"/> parameter given forces an override of the factory. In this particular example, the
    /// force is not important, but becomes so when injecting into a program, to be described next. Then when the
    /// library code, under test, calls <c>RJCP.IO.DeviceMgr.Log.CfgMgr</c>, it will call into the <c>LogSource</c>
    /// that retrieve the <see cref="NUnitLogger"/> that was injected by the <c>[SetUpFixture]</c>.
    /// </para>
    /// <para></para>
    /// <para><b>Injecting into Application Test Code</b>.</para>
    /// <para>
    /// The third case is where unit tests are testing methods of an application. As it is not a library, the
    /// application provides its own methods using the <c>LogSource</c> reading from a configuration file. Its code
    /// would normally look like:
    /// </para>
    /// <code language="csharp"><![CDATA[
    ///namespace RJCP.MyApp
    ///{
    ///    using Microsoft.Extensions.Configuration;
    ///    using Microsoft.Extensions.Logging;
    ///    using RJCP.Diagnostics.Trace;
    ///
    ///    public static class Log {
    ///        public static LogSource Search { get; }
    ///
    ///        static Log() {
    ///            LogSource.SetLoggerFactory(GetLoggerFactory());
    ///            Search = new LogSource("RJCP.MyApp.Search");
    ///        }
    ///
    ///        private static ILoggerFactory GetLoggerFactory() {
    ///            IConfigurationRoot config = new ConfigurationBuilder()
    ///                .AddJsonFile("myapp.settings.log.json", true, false)
    ///                .Build();
    ///
    ///            return LoggerFactory.Create(builder => {
    ///                builder
    ///                    .AddConfiguration(config.GetSection("Logging"))
    ///                    .AddConsole();
    ///            });
    ///        }
    ///
    ///        public static void Close() {
    ///            Search.Dispose();
    ///        }
    ///    }
    ///}
    ///]]></code>
    /// <para>
    /// It's clear to see here it reads the file <c>myapp.settings.log.json</c>, and it logs to the console. If this
    /// code remains as is, testing functions that log to the sink <c>RJCP.MyApp.Search</c> would log to the console,
    /// and in NUnit test cases, logging usually fails (it works for the first instance, and then the second test case
    /// doesn't show logs any more).
    /// </para>
    /// <para>
    /// The test code for the application would have the same <c>[SetUpFixture]</c> code. The logging initialising
    /// function looks almost idential to the previous case:
    /// </para>
    /// <code language="csharp"><![CDATA[
    ///namespace RJCP
    ///{
    ///    // This file is only for .NET Core
    /// 
    ///    using Microsoft.Extensions.Logging;
    ///    using RJCP.CodeQuality.NUnitExtensions.Trace;
    ///    using RJCP.Diagnostics.Trace;
    /// 
    ///    internal static class GlobalLogger {
    ///        static GlobalLogger() {
    ///            ILoggerFactory factory = LoggerFactory.Create(builder => {
    ///                builder
    ///                    .AddFilter("RJCP.MyApp.Search", LogLevel.Debug)
    ///                    .AddNUnitLogger();
    ///            });
    ///            LogSource.SetLoggerFactory(factory, true);
    ///        }
    /// 
    ///        // Just calling this method will result in the static constructor being executed.
    ///        public static void Initialize() {
    ///            /* Can be empty, reference will initialize static constructor */
    ///        }
    ///    }
    ///}
    /// ]]></code>
    /// <para>
    /// Here, the <c>GlobalLogger</c> initialises the logging for <c>RJCP.MyApp.Search</c>. When the code under test
    /// references the <c>LogSource</c> field <c>RJCP.MyApp.Log.Search</c>, the static constructor of
    /// <c>RJCP.MyApp.Log</c> is run. When it calls <c>LogSource.SetLoggerFactory()</c>, nothing happens (because it
    /// doesn't override), leaving the initialisation from the test case in place. This now allows application code
    /// to log to <see cref="NUnitLogger"/> despite another instance potentially overwriting it.
    /// </para>
    /// </remarks>
    [CLSCompliant(false)]
    public static class NUnitLoggerExtensions
    {
        /// <summary>
        /// Adds the <see cref="NUnitLogger"/> for logging.
        /// </summary>
        /// <param name="builder">The logging builder.</param>
        /// <returns>The <see cref="ILoggingBuilder"/> for fluent construction.</returns>
        public static ILoggingBuilder AddNUnitLogger(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, NUnitLoggerProvider>());
            return builder;
        }
    }
}
