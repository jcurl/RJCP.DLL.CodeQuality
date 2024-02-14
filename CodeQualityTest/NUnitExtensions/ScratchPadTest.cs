namespace RJCP.CodeQuality.NUnitExtensions
{
    using System;
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class ScratchPadTest
    {
        [Test]
        public void ScratchPadDefault()
        {
            string originalPath = Environment.CurrentDirectory;
            using (ScratchPad scratch = new()) {
                // Default is to use the same name for the directory as the test case name, and to change the directory
                // to be in the scratch folder.
                Assert.That(Directory.Exists(scratch.Path), Is.True);
                Assert.That(scratch.RelativePath, Is.EqualTo(Deploy.TestName));
                Assert.That(Environment.CurrentDirectory, Is.EqualTo(scratch.Path));
            }

            Assert.That(Environment.CurrentDirectory, Is.EqualTo(originalPath));
        }

        [Test]
        public void ScratchPadDefaultNameUsedTwice()
        {
            string originalPath = Environment.CurrentDirectory;
            using (ScratchPad scratch = new()) {
                // Default is to use the same name for the directory as the test case name, and to change the directory
                // to be in the scratch folder.
                Assert.That(Directory.Exists(scratch.Path), Is.True);
                Assert.That(scratch.RelativePath, Is.Not.Null.Or.Empty);
                Assert.That(Environment.CurrentDirectory, Is.EqualTo(scratch.Path));
            }

            Assert.That(Environment.CurrentDirectory, Is.EqualTo(originalPath));
        }

        [Test]
        public void ScratchPadCreateUseDeployDir()
        {
            string originalPath = Environment.CurrentDirectory;
            using (ScratchPad scratch = new(ScratchOptions.UseDeployDir)) {
                Assert.That(Directory.Exists(scratch.Path), Is.True);
                Assert.That(scratch.RelativePath, Is.EqualTo(Deploy.TestName));

                // The 'work' directory is part of the App.Config
                Assert.That(Environment.CurrentDirectory,
                    Is.EqualTo(Path.Combine(Deploy.TestDirectory, "work")));
            }

            Assert.That(Environment.CurrentDirectory, Is.EqualTo(originalPath));
        }

        [Test]
        public void ScratchPadCreateUseCurrentDir()
        {
            string originalPath = Environment.CurrentDirectory;
            using (ScratchPad scratch = new(ScratchOptions.KeepCurrentDir)) {
                Assert.That(Directory.Exists(scratch.Path), Is.True);
                Assert.That(scratch.RelativePath, Is.EqualTo(Deploy.TestName));

                // The 'work' directory is part of the App.Config
                Assert.That(Environment.CurrentDirectory, Is.EqualTo(originalPath));
            }

            Assert.That(Environment.CurrentDirectory, Is.EqualTo(originalPath));
        }

        [TestCase(ScratchOptions.NoScratch, TestName = "ScratchPadNoScratch")]
        [TestCase(ScratchOptions.NoScratch | ScratchOptions.UseScratchDir, TestName = "ScratchPadNoScratch")]
        public void ScratchPadNoScratch(ScratchOptions options)
        {
            string originalPath = Environment.CurrentDirectory;
            using (ScratchPad scratch = new(ScratchOptions.NoScratch)) {
                // In this case, the scratch isn't made. Therefore, don't move to that directory, even if requested to
                // use the scratch directory.
                Assert.That(Directory.Exists(scratch.Path), Is.False);
                Assert.That(scratch.RelativePath, Is.EqualTo("ScratchPadNoScratch"));
                Assert.That(Environment.CurrentDirectory, Is.EqualTo(originalPath));
            }
        }

        [Test]
        public void ScratchPadNoScratchDeployDir()
        {
            using (ScratchPad scratch = new(ScratchOptions.NoScratch | ScratchOptions.UseDeployDir)) {
                // In this case, the scratch isn't made. Therefore, don't move to that directory.
                Assert.That(Directory.Exists(scratch.Path), Is.False);
                Assert.That(scratch.RelativePath, Is.EqualTo(Deploy.TestName));

                // The 'work' directory is part of the App.Config
                Assert.That(Environment.CurrentDirectory,
                    Is.EqualTo(Path.Combine(Deploy.TestDirectory, "work")));
            }
        }

        [Test]
        public void ScratchPadUseTwice()
        {
            using (ScratchPad scratch = new()) {
                Assert.That(Directory.Exists(scratch.Path), Is.True);
                Assert.That(scratch.RelativePath, Is.EqualTo(Deploy.TestName));
                Assert.That(Environment.CurrentDirectory, Is.EqualTo(scratch.Path));
            }

            using (ScratchPad scratch = new()) {
                Assert.That(Directory.Exists(scratch.Path), Is.True);
                Assert.That(scratch.RelativePath, Is.EqualTo(Deploy.TestName));
                Assert.That(Environment.CurrentDirectory, Is.EqualTo(scratch.Path));
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        public void ScratchPadTestWithParams(int param)
        {
            string originalPath = Environment.CurrentDirectory;
            using (ScratchPad scratch = new()) {
                // Default is to use the same name for the directory as the test case name, and to change the directory
                // to be in the scratch folder.
                Assert.That(Directory.Exists(scratch.Path), Is.True);
                Assert.That(scratch.RelativePath, Is.Not.Null.Or.Empty);
                Assert.That(Environment.CurrentDirectory, Is.EqualTo(scratch.Path));
            }

            Assert.That(Environment.CurrentDirectory, Is.EqualTo(originalPath));
        }

        [Test]
        public void ScratchPadCreateEnsureContentsDeleted()
        {
            string path;

            using (ScratchPad scratch = new()) {
                path = scratch.Path;
                using (Stream fs = new FileStream("test.txt", FileMode.CreateNew, FileAccess.ReadWrite)) {
                    fs.Write(new byte[] { 0x40 }, 0, 1);
                }

                Assert.That(File.Exists("test.txt"), Is.True);
            }

            using (ScratchPad scratch = new()) {
                // Directory is deleted first, and the path should remain the same
                Assert.That(scratch.Path, Is.EqualTo(path));
                Assert.That(File.Exists("test.txt"), Is.False);
            }
        }

        [Test]
        public void ScratchPadCreateOnMissing()
        {
            string path;

            using (ScratchPad scratch = new()) {
                path = scratch.Path;
                using (Stream fs = new FileStream("test.txt", FileMode.CreateNew, FileAccess.ReadWrite)) {
                    fs.Write(new byte[] { 0x40 }, 0, 1);
                }

                Assert.That(File.Exists("test.txt"), Is.True);
            }

            using (ScratchPad scratch = new(ScratchOptions.CreateOnMissing)) {
                // Directory is deleted first, and the path should remain the same
                Assert.That(scratch.Path, Is.EqualTo(path));
                Assert.That(File.Exists("test.txt"), Is.True);
            }
        }

        [Test]
        public void ScratchPadCreateOnMissingNested()
        {
            using (ScratchPad scratch = new()) {
                using (Stream fs = new FileStream("test.txt", FileMode.CreateNew, FileAccess.ReadWrite)) {
                    fs.Write(new byte[] { 0x40 }, 0, 1);
                }

                Assert.That(File.Exists("test.txt"), Is.True);

                using (ScratchPad scratch2 = new(ScratchOptions.CreateOnMissing)) {
                    // Directory is deleted first, and the path should remain the same
                    Assert.That(scratch2.Path, Is.EqualTo(scratch.Path));
                    Assert.That(File.Exists("test.txt"), Is.True);
                }
            }
        }

        [Test]
        public void ScratchPadEmptyFile()
        {
            using (ScratchPad scratch = new()) {
                scratch.DeployEmptyFile("test.txt");
                Assert.That(File.Exists("test.txt"));
                FileInfo info = new("test.txt");
                Assert.That(info.Length, Is.EqualTo(0));
            }
        }
    }
}
