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
