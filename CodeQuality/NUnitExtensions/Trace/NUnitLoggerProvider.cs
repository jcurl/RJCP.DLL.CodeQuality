namespace RJCP.CodeQuality.NUnitExtensions.Trace
{
    using System;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// An NUnit Logger Provider for .NET Core and Logging.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class NUnitLoggerProvider : ILoggerProvider
    {
        private readonly LogLevel m_MinLevel;
        private readonly DateTimeOffset? m_LogStart;

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitLoggerProvider"/> class.
        /// </summary>
        public NUnitLoggerProvider() : this(LogLevel.Trace) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitLoggerProvider"/> class.
        /// </summary>
        /// <param name="minLevel">The minimum level.</param>
        public NUnitLoggerProvider(LogLevel minLevel)
        {
            m_MinLevel = minLevel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitLoggerProvider"/> class.
        /// </summary>
        /// <param name="minLevel">The minimum log level.</param>
        /// <param name="logStart">The time logging starts.</param>
        public NUnitLoggerProvider(LogLevel minLevel, DateTimeOffset logStart)
        {
            m_MinLevel = minLevel;
            m_LogStart = logStart;
        }

        /// <summary>
        /// Creates a new <see cref="NUnitLogger"/>.
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <returns>A new <see cref="NUnitLogger"/>.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            if (!m_LogStart.HasValue)
                return new NUnitLogger(categoryName, m_MinLevel);

            return new NUnitLogger(categoryName, m_MinLevel, m_LogStart);
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            /* Nothing to dispose of */
        }
    }
}