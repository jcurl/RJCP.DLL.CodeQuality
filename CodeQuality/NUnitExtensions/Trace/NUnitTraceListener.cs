namespace RJCP.CodeQuality.NUnitExtensions.Trace
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// A <see cref="TraceListener"/> that writes to NUnit Test Context.
    /// </summary>
    public class NUnitTraceListener : TraceListener
    {
        private static readonly string[] NewLineChars = new[] { "\n" };
        private bool m_OnNewLine = true;

        private void Write(string message, bool newLine)
        {
            string timeStamp = GetTimeStamp();
            string[] lines = message.Split(NewLineChars, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++) {
                bool lastLine = i == lines.Length - 1;
                if (m_OnNewLine) {
                    if (!lastLine || newLine) {
                        TestContextAccessor.Instance.WriteLine($"[{timeStamp}] {lines[i]}");
                        m_OnNewLine = true;
                    } else {
                        TestContextAccessor.Instance.Write($"[{timeStamp}] {lines[i]}");
                        m_OnNewLine = false;
                    }
                } else {
                    if (!lastLine || newLine) {
                        TestContextAccessor.Instance.WriteLine(lines[i]);
                        m_OnNewLine = true;
                    } else {
                        TestContextAccessor.Instance.Write(lines[i]);
                        m_OnNewLine = false;
                    }
                }
            }
        }

        /// <summary>
        /// Writes the specified message to the NUnit Test Context.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public override void Write(string message)
        {
            Write(message, false);
        }

        /// <summary>
        /// Writes a message to the NUnit Test Context, followed by a line terminator.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public override void WriteLine(string message)
        {
            Write(message, true);
        }

        private static string GetTimeStamp()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
        }
    }
}
