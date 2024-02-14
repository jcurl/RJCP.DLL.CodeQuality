namespace RJCP.CodeQuality.NUnitExtensions.Trace
{
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    [TestFixture]
    public class NUnitLoggerTest
    {
        [Test]
        public void CreateNUnitLoggerFromFactory()
        {
            // Should manually check the output, that logging did occur

            ILoggerFactory logFactory = LoggerFactory.Create(builder => {
                builder.AddFilter("RJCP", LogLevel.Debug)
                    .AddNUnitLogger();
            });

            ILogger logger = logFactory.CreateLogger("RJCP.CodeQuality.NUnitExtensions");
            logger.LogDebug("Debug Message");
            logger.LogTrace("Trace Message");
            logger.LogInformation("Info Message");
            logger.LogWarning("Warning Message");
            logger.LogError("Error Message");
            logger.LogCritical("Critical Message");

            ILogger loggerTest = logFactory.CreateLogger("Test");
            loggerTest.LogDebug("Debug Message");
            loggerTest.LogTrace("Trace Message");
            loggerTest.LogInformation("Info Message");
            loggerTest.LogWarning("Warning Message");
            loggerTest.LogError("Error Message");
            loggerTest.LogCritical("Critical Message");
        }

        private static ILoggerFactory s_LogFactory;
        private static readonly object s_Lock = new();

        private static ILoggerFactory LoggerInstance
        {
            get
            {
                if (s_LogFactory is null) {
                    lock (s_Lock) {
                        s_LogFactory ??= LoggerFactory.Create(builder => {
                            builder.AddFilter("RJCP", LogLevel.Debug)
                                .AddNUnitLogger();
                        });
                    }
                }
                return s_LogFactory;
            }
        }

        [Test]
        public void CreateNUnitLogger1()
        {
            // Check that this test case has exactly one log entry

            ILogger logger = LoggerInstance.CreateLogger("RJCP.CodeQuality");
            logger.LogDebug("CreateNUnitLogger1");
        }

        [Test]
        public void CreateNUnitLogger2()
        {
            // Check that this test case has exactly one log entry

            ILogger logger = LoggerInstance.CreateLogger("RJCP.CodeQuality");
            logger.LogDebug("CreateNUnitLogger2");
        }
    }
}
