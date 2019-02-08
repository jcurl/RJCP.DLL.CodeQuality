namespace NUnit.Framework
{
    using System;
    using System.IO;
    using System.Threading;

    [TestFixture(Category = "NUnitExtensions.Dump")]
    public class DumpTest
    {
        [Test]
        [Platform(Include ="Win32NT")]
        public void MiniDump_Windows()
        {
            string fullPath = Path.Combine(Environment.CurrentDirectory, "Minidump.dmp");

            Assert.That(Dump.MiniDump("Minidump.dmp"), Is.True);

            int pollCounter = 0;
            while (pollCounter < 5) {
                if (File.Exists(fullPath)) return;
                Thread.Sleep(100);
                pollCounter++;
            }
            Assert.Fail("No minidump 'Minidump.dmp' was created");
        }

        [Test]
        [Platform(Include = "Win32NT")]
        public void MiniDumpFull_Windows()
        {
            string fullPath = Path.Combine(Environment.CurrentDirectory, "Minidumpfull.dmp");

            Assert.That(Dump.MiniDump("Minidumpfull.dmp", DumpType.FullHeap), Is.True);

            int pollCounter = 0;
            while (pollCounter < 5) {
                if (File.Exists(fullPath)) return;
                Thread.Sleep(100);
                pollCounter++;
            }
            Assert.Fail("No minidump 'Minidumpfull.dmp' was created");
        }
        [Test]
        [Platform(Include = "Win32NT")]
        public void MiniDumpException_Windows()
        {
            string fullPath = Path.Combine(Environment.CurrentDirectory, "MinidumpException.dmp");

            Exception exception = null;
            bool result;
            try {
                throw new InvalidOperationException("Test Throw");
            } catch (InvalidOperationException ex) {
                exception = ex;
                result = Dump.MiniDump("MinidumpException.dmp");
            }
            Assert.That(result, Is.True);

            int pollCounter = 0;
            while (pollCounter < 5) {
                if (File.Exists(fullPath)) return;
                Thread.Sleep(100);
                pollCounter++;
            }
            Assert.Fail("No minidump 'MinidumpException.dmp' was created");
        }

        [Test]
        [Platform(Include = "Win32NT")]
        public void MiniDumpFullException_Windows()
        {
            string fullPath = Path.Combine(Environment.CurrentDirectory, "MinidumpfullException.dmp");

            Exception exception = null;
            bool result;
            try {
                throw new InvalidOperationException("Test Throw");
            } catch (InvalidOperationException ex) {
                exception = ex;
                result = Dump.MiniDump("MinidumpfullException.dmp", DumpType.FullHeap);
            }
            Assert.That(result, Is.True);

            int pollCounter = 0;
            while (pollCounter < 5) {
                if (File.Exists(fullPath)) return;
                Thread.Sleep(100);
                pollCounter++;
            }
            Assert.Fail("No minidump 'MinidumpfullException.dmp' was created");
        }

        [Test]
        [Platform(Exclude = "Win32NT")]
        public void MiniDump_Linux()
        {
            // Runs also on Linux, just that no file will be created. We don't test for that, because we just don't want
            // it to crash.
            Dump.MiniDump("MinidumpLinux.dmp");
        }
    }
}
