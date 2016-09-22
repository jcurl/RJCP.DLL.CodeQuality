namespace NUnit.Framework
{
    using System;
    using System.IO;

    [TestFixture]
    public class NUnitExtensionsTest
    {
        [Test]
        [Category("NUnitExtensions.Deployment")]
        [DeploymentItem("Resources/test1.txt")]
        public void DeployFile()
        {
            Assert.That(File.Exists("test1.txt"));
        }

        [Test]
        [Category("NUnitExtensions.Deployment")]
        [DeploymentItem("Resources", "folder")]
        public void DeployFolder()
        {
            Assert.That(File.Exists(Path.Combine("folder", "Resources", "test1.txt")), "File 'folder/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine("folder", "Resources", "test2.txt")), "File 'folder/Resources/test2.txt' not found");
        }

        [Test]
        [Category("NUnitExtensions.Deployment")]
        [DeploymentItem("Resources/test1.txt", "Resources")]
        public void DeployToSource()
        {
            Assert.That(File.Exists(Path.Combine("Resources", "test1.txt")), "File 'folder/Resources/test1.txt' not found");
        }

        [Test]
        [Category("NUnitExtensions.Deployment")]
        [DeploymentItem("Resources/test1.txt", "Resources")]
        public void DeployToSourceSecondCopy()
        {
            Assert.That(File.Exists(Path.Combine("Resources", "test1.txt")), "File 'folder/Resources/test1.txt' not found");
        }

        [Test]
        [Category("NUnitExtensions.Deployment")]
        public void DeployFileInLine()
        {
            if (File.Exists("test1.txt")) File.Delete("test1.txt");

            Deploy.Item("Resources/test1.txt");
            Assert.That(File.Exists("test1.txt"));
        }

        [Test]
        [Category("NUnitExtensions.Deployment")]
        public void DeployFolderInLine()
        {
            if (Directory.Exists("folder2")) Directory.Delete("folder2", true);
            Deploy.Item("Resources", "folder2");

            Assert.That(File.Exists(Path.Combine("folder2", "Resources", "test1.txt")), "File 'folder2/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine("folder2", "Resources", "test2.txt")), "File 'folder2/Resources/test2.txt' not found");
        }

        [Test]
        [Category("NUnitExtensions.Deployment")]
        public void DeployToSourceInLine()
        {
            Deploy.Item("Resources/test1.txt", "Resources");
            Assert.That(File.Exists(Path.Combine("Resources", "test1.txt")), "File 'folder/Resources/test1.txt' not found");
        }

        [Test]
        [Category("NUnitExtensions.Deployment")]
        public void DeployItemToAbsolutePath()
        {
            string currentDirectory = Environment.CurrentDirectory;
            Deploy.Item("Resources/test1.txt", currentDirectory);

            string file = Path.Combine(currentDirectory, "test1.txt");
            Assert.That(File.Exists(file), "File '{0}' not found", file);
        }

        [Test]
        [Category("NUnitExtensions.Deployment")]
        public void DeployItemToAbsolutePath2()
        {
            string currentDirectory = Path.Combine(Environment.CurrentDirectory, "sub");
            Deploy.Item("Resources/test1.txt", currentDirectory);

            string file = Path.Combine(currentDirectory, "test1.txt");
            Assert.That(File.Exists(file), "File '{0}' not found", file);
        }
    }
}
