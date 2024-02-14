namespace RJCP.CodeQuality.NUnitExtensions.Trace
{
    using System;
    using System.Globalization;
    using System.Text;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The NUnit Logger that writes to the TestContext.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class NUnitLogger : ILogger
    {
        private static readonly string[] NewLineChars = new[] { Environment.NewLine };
        private readonly string m_Category;
        private readonly LogLevel m_MinLogLevel;
        private readonly DateTimeOffset? m_LogStart;

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitLogger"/> class.
        /// </summary>
        /// <param name="category">Name of the category.</param>
        /// <param name="minLogLevel">The minimum log level.</param>
        public NUnitLogger(string category, LogLevel minLogLevel)
        {
            m_Category = category;
            m_MinLogLevel = minLogLevel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitLogger"/> class.
        /// </summary>
        /// <param name="category">Name of the category.</param>
        /// <param name="minLogLevel">The minimum log level.</param>
        /// <param name="logStart">The time logging starts.</param>
        public NUnitLogger(string category, LogLevel minLogLevel, DateTimeOffset? logStart)
        {
            m_Category = category;
            m_MinLogLevel = minLogLevel;
            m_LogStart = logStart;
        }

        /// <summary>
        /// Logs at the specified log level.
        /// </summary>
        /// <typeparam name="TState">The type of the t state.</typeparam>
        /// <param name="logLevel">The log level.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="state">The state.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="formatter">The formatter.</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) {
                return;
            }

            // Buffer the message into a single string in order to avoid shearing the message when running across multiple threads.
            StringBuilder messageBuilder = new();

            string timeStamp;
            if (m_LogStart.HasValue) {
                timeStamp = string.Format("{0}s", (DateTimeOffset.UtcNow - m_LogStart.Value).TotalSeconds.ToString("###.000", CultureInfo.InvariantCulture));
            } else {
                timeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            }

            string linePrefix = $"[{timeStamp}] {m_Category} {logLevel}: ";
            string[] lines = formatter(state, exception).Split(NewLineChars, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length == 0) {
                messageBuilder.AppendLine(linePrefix);
            } else {
                foreach (string line in lines) {
                    messageBuilder.Append(linePrefix).AppendLine(line);
                }
            }

            if (exception is not null) {
                lines = exception.ToString().Split(NewLineChars, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines) {
                    messageBuilder.Append(linePrefix).AppendLine(line);
                }
            }

            // Remove the last line-break, because ITestOutputHelper only has WriteLine.
            string message = messageBuilder.ToString();
            if (message.EndsWith(Environment.NewLine, StringComparison.Ordinal)) {
                message = message.Substring(0, message.Length - Environment.NewLine.Length);
            }

            try {
                TestContextAccessor.Instance.WriteLine(message);
            } catch (Exception) {
                // We could fail because we're on a background thread and our captured ITestOutputHelper is
                // busted (if the test "completed" before the background thread fired).
                // So, ignore this. There isn't really anything we can do but hope the
                // caller has additional loggers registered
            }
        }

        /// <summary>
        /// Determines whether the specified log level is enabled.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns>
        /// Is <see langword="true"/> if the specified log level is enabled; otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= m_MinLogLevel;
        }

        /// <summary>
        /// Begins a new scope for logging.
        /// </summary>
        /// <typeparam name="TState">The type of the t state.</typeparam>
        /// <param name="state">The state.</param>
        /// <returns>An object to manage the scope.</returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            return default;
        }
    }
}
