namespace NUnitExtensionsTest.NUnit3
{
    using System;
    using NUnit.Framework;

    [TestFixture(Category = "NUnitExtensions.Deployment.NUnit3")]
    public class DeployTest
    {
        [Test]
        public void DeployTestItem()
        {
            string workDirectory = TestContext.CurrentContext.TestDirectory;
            string testDirectory = TestContext.CurrentContext.WorkDirectory;

            Assert.That(Deploy.TestDirectory, Is.EqualTo(testDirectory));
            Assert.That(Deploy.WorkDirectory, Is.EqualTo(workDirectory));
        }

        [TestCase("Name")]
        public void DeployTestCaseItem(string name)
        {
            string workDirectory = TestContext.CurrentContext.TestDirectory;
            string testDirectory = TestContext.CurrentContext.WorkDirectory;

            Assert.That(Deploy.TestDirectory, Is.EqualTo(testDirectory));
            Assert.That(Deploy.WorkDirectory, Is.EqualTo(workDirectory));
        }
    }
}
