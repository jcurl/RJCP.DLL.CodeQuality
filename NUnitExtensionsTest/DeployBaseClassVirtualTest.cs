namespace NUnit.Framework
{
    using System;
    using System.IO;

    [TestFixture(Category = "NUnitExtensions.Deployment.BaseClass")]
    public class DeployBaseVirtualClassTest : NUnitExtensions
    {
        public override void TestFixtureSetUp()
        {
            Console.WriteLine("In TestFixtureSetup");
            base.TestFixtureSetUp();
        }

        [Test]
        [DeploymentItem("Resources/test1.txt", "BaseClassVResources")]
        public void DeploySingleFileInVirtual()
        {
            Assert.That(File.Exists(Path.Combine("BaseClassResources", "test1.txt")), "File 'BaseClassResoruces/test1.txt' not found");
        }

        [Test]
        [DeploymentItem("Resources/", "BaseClassVResources2")]
        public void DeploySingleFile2InVirtual()
        {
            Assert.That(File.Exists(Path.Combine("BaseClassResources2", "test1.txt")), "File 'BaseClassResoruces2/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine("BaseClassResources2", "test2.txt")), "File 'BaseClassResoruces2test2.txt' not found");
        }

        [Test]
        [DeploymentItem("Resources", "BaseClassVResources3")]
        public void DeploySingleFile3InVirtual()
        {
            Assert.That(File.Exists(Path.Combine("BaseClassResources3", "Resources", "test1.txt")), "File 'BaseClassResoruces3/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine("BaseClassResources3", "Resources", "test2.txt")), "File 'BaseClassResoruces3/Resources/test2.txt' not found");
        }
    }
}
