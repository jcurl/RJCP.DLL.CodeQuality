namespace NUnit.Framework
{
    using System;
    using System.IO;

    [TestFixture(Category = "NUnitExtensions.Deployment.BaseClass")]
    public class DeployBaseClassTest : NUnitExtensions
    {
        [Test]
        [DeploymentItem("Resources/test1.txt", "BaseClassResources")]
        public void DeploySingleFile()
        {
            Assert.That(File.Exists(Path.Combine("BaseClassResources", "test1.txt")), "File 'BaseClassResoruces/test1.txt' not found");
        }

        [Test]
        [DeploymentItem("Resources/", "BaseClassResources2")]
        public void DeploySingleFile2()
        {
            Assert.That(File.Exists(Path.Combine("BaseClassResources2", "test1.txt")), "File 'BaseClassResoruces2/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine("BaseClassResources2", "test2.txt")), "File 'BaseClassResoruces2test2.txt' not found");
        }

        [Test]
        [DeploymentItem("Resources", "BaseClassResources3")]
        public void DeploySingleFile3()
        {
            Assert.That(File.Exists(Path.Combine("BaseClassResources3", "Resources", "test1.txt")), "File 'BaseClassResoruces3/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine("BaseClassResources3", "Resources", "test2.txt")), "File 'BaseClassResoruces3/Resources/test2.txt' not found");
        }

#if false
        // Activating the below test will cause all test cases in this class to fail
        // with "TestFixtureSetUp failed in NUnitExtensionsBaseClassTest", with the output
        // indicating that this method has a problem.
        [Test]
        [DeploymentItem(null, "BaseClassResources")]
        public void DeployNullFile()
        {
            Assert.That(File.Exists(Path.Combine("BaseClassResources", "test1.txt")), "File 'BaseClassResoruces/test1.txt' not found");
        }
#endif
    }
}
