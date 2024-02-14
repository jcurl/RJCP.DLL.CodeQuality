namespace RJCP.CodeQuality.NUnitExtensions
{
    using System;
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class ScratchPad2ndTest
    {
        [Test]
        public void ScratchPadDefaultNameUsedTwice()
        {
            // The name of this test is important. It should have the same name as another test case in a different
            // class to test name resolution conflict.

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
    }
}
