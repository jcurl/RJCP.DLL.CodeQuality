namespace NUnitExtensionsTest.NUnit3
{
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
    }
}
