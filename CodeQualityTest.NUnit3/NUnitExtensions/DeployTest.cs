namespace RJCP.CodeQuality.NUnitExtensions
{
    using NUnit.Framework;

    [TestFixture(Category = "RJCP.CodeQuality.NUnitExtensions.Deployment")]
    public class DeployTest
    {
        [Test]
        public void DeployTestItem()
        {
            string workDirectory = TestContext.CurrentContext.WorkDirectory;
            string testDirectory = TestContext.CurrentContext.TestDirectory;

            Assert.That(Deploy.TestDirectory, Is.EqualTo(testDirectory));
            Assert.That(Deploy.WorkDirectory, Is.EqualTo(workDirectory));
        }

        [TestCase("Name")]
        public void DeployTestCaseItem(string name)
        {
            string workDirectory = TestContext.CurrentContext.WorkDirectory;
            string testDirectory = TestContext.CurrentContext.TestDirectory;

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
