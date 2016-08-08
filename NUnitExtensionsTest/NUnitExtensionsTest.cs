namespace NUnit.Framework
{
    using System;
    using System.IO;

    [TestFixture]
    public class NUnitExtensionsTest
    {
        [Test]
        [DeploymentItem("Resources/test1.txt")]
        public void DeployFile()
        {
            Assert.That(File.Exists("test1.txt"));
        }

        [Test]
        [DeploymentItem("Resources", "folder")]
        public void DeployFolder()
        {
            Assert.That(File.Exists(Path.Combine("folder", "Resources", "test1.txt")), "File 'folder/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine("folder", "Resources", "test2.txt")), "File 'folder/Resources/test2.txt' not found");
        }

        [Test]
        [DeploymentItem("Resources/test1.txt", "Resources")]
        public void DeployToSource()
        {
            Assert.That(File.Exists(Path.Combine("Resources", "test1.txt")), "File 'folder/Resources/test1.txt' not found");
        }

        [Test]
        [DeploymentItem("Resources/test1.txt", "Resources")]
        public void DeployToSourceSecondCopy()
        {
            Assert.That(File.Exists(Path.Combine("Resources", "test1.txt")), "File 'folder/Resources/test1.txt' not found");
        }
    }
}
