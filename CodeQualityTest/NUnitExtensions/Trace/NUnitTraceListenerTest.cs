namespace RJCP.CodeQuality.NUnitExtensions.Trace
{
    using System.Diagnostics;
    using NUnit.Framework;

    [TestFixture]
    public class NUnitTraceListenerTest
    {
        [Test]
        public void LogTraceListenerWithTraceSource()
        {
            TraceSource source = new TraceSource("RJCP.NUnitTraceListenerTest");

#if NET6_0_OR_GREATER
            // On .NET Core, tracing doesn't work because it doesn't read the configuration file. We have to instantiate
            // it explicitly.
            source.Listeners.Add(new NUnitTraceListener());
            source.Switch.Level = SourceLevels.All;
#endif

            source.TraceEvent(TraceEventType.Verbose, 0, "Verbose message");
            source.TraceEvent(TraceEventType.Information, 0, "Information message");
            source.TraceEvent(TraceEventType.Warning, 0, "Warning message");
            source.TraceEvent(TraceEventType.Error, 0, "Error message");
            source.TraceEvent(TraceEventType.Critical, 0, "Critical message");
        }

        [Test]
        public void LogTraceListener()
        {
            TraceListener listener = new NUnitTraceListener();
            listener.Fail("Fail Message");
            listener.Fail("Fail Message", "Detailed Message");
            listener.WriteLine("----");
            listener.Write("Write(Message)");
            listener.WriteLine("----");
            listener.WriteLine("Now WriteLine(Message)");
            listener.WriteLine("----");
            listener.Write((object)"Write(String Object)");
            listener.WriteLine("----");
            listener.Write("Write(Message, Category)", "Category");
            listener.WriteLine("----");
            listener.Write((object)"Write(String Object, Category)", "Category");
            listener.WriteLine("----");
            listener.Write("This is a Write() on\nmultiline");
            listener.WriteLine("----");
            listener.WriteLine("This is a WriteLine()\nmultiline");
            listener.WriteLine("----");
            listener.Write("This is a Write(\\n)\n");
            listener.WriteLine("----");
        }
    }
}
