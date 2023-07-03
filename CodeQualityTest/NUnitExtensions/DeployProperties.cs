namespace RJCP.CodeQuality.NUnitExtensions
{
    using System;
    using System.IO;
    using System.Threading;
    using NUnit.Framework;

    [TestFixture]
    public class DeployProperties
    {
        [Test]
        public void DeployTestItem()
        {
            string workDirectory = TestContext.CurrentContext.WorkDirectory;
            string testDirectory = TestContext.CurrentContext.TestDirectory;

            Console.WriteLine("NUnit Work Directory: {0}", workDirectory);
            Console.WriteLine("NUnit Test Directory: {0}", testDirectory);
            Console.WriteLine("NUnitExtensions Work Directory: {0}", Deploy.WorkDirectory);
            Console.WriteLine("NUnitExtensions Test Directory: {0}", Deploy.TestDirectory);

            Assert.That(Deploy.TestDirectory, Is.EqualTo(testDirectory));

            if (testDirectory.Equals(workDirectory)) {
                // In case that /work is not provided on the command line and the work/test directory are identical, then
                // NUnitExtensions.Deploy.WorkDirectory is modified as per the configuration to add the 'work' folder.
                Assert.That(Deploy.WorkDirectory, Is.EqualTo(Path.Combine(workDirectory, "work")));
            } else {
                // In case that /work is provided on the command line and they are not identical any more, then don't add
                // the directory.
                Assert.That(Deploy.WorkDirectory, Is.EqualTo(workDirectory));
            }
        }

        [Test]
        public void TestName()
        {
            Assert.That(Deploy.TestName, Is.EqualTo(TestContext.CurrentContext.Test.Name));
        }

        [Test]
        public void TestName2()
        {
            Assert.That(Deploy.TestName, Is.EqualTo(TestContext.CurrentContext.Test.Name));
        }

        [TestCase("Name")]
        public void TestCase(string name)
        {
            Assert.That(Deploy.TestName, Is.EqualTo(TestContext.CurrentContext.Test.Name));
        }

        [Test]
        public void WorkDirectoryOnThread()
        {
            // This test case is intended to run on its own, before the TestContextAccessor is instantiated the first
            // time.

            string workDirectory = null;
            Thread workerThread = new Thread(() => { workDirectory = Deploy.WorkDirectory; });
            workerThread.Start();
            workerThread.Join();

            Assert.That(workDirectory, Is.Not.Null);
        }
    }
}
