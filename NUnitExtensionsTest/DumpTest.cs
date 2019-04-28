﻿namespace NUnit.Framework
{
    using System;
    using System.IO;
    using System.Threading;

    [TestFixture(Category = "NUnitExtensions.Dump")]
    public class DumpTest
    {
        private static void CheckFile(string fileName)
        {
            string fullPath = Path.Combine(Environment.CurrentDirectory, fileName);

            int pollCounter = 0;
            while (pollCounter < 5) {
                if (File.Exists(fullPath)) return;
                Thread.Sleep(100);
                pollCounter++;
            }
            Assert.Fail("No minidump '{0}' was created", fileName);
        }

        [Platform(Include = "Win32NT")]
        [TestCase(DumpType.MiniDump, "minidump.dmp", TestName = "MiniDump_Windows_MiniDump")]
        [TestCase(DumpType.FullHeap, "fulldump.dmp", TestName = "MiniDump_Windows_FullDump")]
        public void MiniDump_Windows(DumpType dumpType, string fileName)
        {
            Deploy.CreateDirectory("Dumps");
            string dumpName = Path.Combine(Deploy.WorkDirectory, "Dumps", fileName);

            Assert.That(Dump.MiniDump(dumpName, dumpType), Is.True);
            CheckFile(dumpName);
        }

        [Test]
        [Platform(Include = "Win32NT")]
        public void MiniDump_Windows_DefaultDump()
        {
            Deploy.CreateDirectory("Dumps");
            string dumpName = Path.Combine(Deploy.WorkDirectory, "Dumps", "defaultdump.dmp");

            Assert.That(Dump.MiniDump(dumpName), Is.True);
            CheckFile(dumpName);
        }

        [Platform(Include = "Win32NT")]
        [TestCase(DumpType.MiniDump, "minidumpexception.dmp", TestName = "MiniDumpException_Windows_MiniDump")]
        [TestCase(DumpType.FullHeap, "fulldumpexception.dmp", TestName = "MiniDump_WindowsException_FullDump")]
        public void MiniDumpException_Windows(DumpType dumpType, string fileName)
        {
            Deploy.CreateDirectory("Dumps");
            string dumpName = Path.Combine(Deploy.WorkDirectory, "Dumps", fileName);

            Exception exception = null;
            bool result;
            try {
                throw new InvalidOperationException("Test Throw");
            } catch (InvalidOperationException ex) {
                exception = ex;
                result = Dump.MiniDump(dumpName, dumpType);
            }
            Assert.That(result, Is.True);
            CheckFile(dumpName);
        }

        [Test]
        [Platform(Include = "Win32NT")]
        public void MiniDumpException_Windows_DefaultDump()
        {
            Deploy.CreateDirectory("Dumps");
            string dumpName = Path.Combine(Deploy.WorkDirectory, "Dumps", "defaultdumpexception.dmp");

            Exception exception = null;
            bool result;
            try {
                throw new InvalidOperationException("Test Throw");
            } catch (InvalidOperationException ex) {
                exception = ex;
                result = Dump.MiniDump(dumpName);
            }
            Assert.That(result, Is.True);
            CheckFile(dumpName);
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
