namespace NUnit.Framework
{
    using System;
    using System.IO;

    [TestFixture(Category = "NUnitExtensions.Deployment.BackPath")]
    public class DeployBackPathTest
    {
        [Test]
        [Repeat(100)]
        public void DeployFileInLine()
        {
            Tools.DeleteFile("test1.txt");

            Deploy.Item(@"Resources\test1.txt");
            Assert.That(File.Exists("test1.txt"));
        }

        [Test]
        [Repeat(100)]
        public void DeployFolderInLineWithTrailingSlash1()
        {
            Tools.DeleteDirectory("folder2");
            Deploy.Item("Resources", @"folder2\");

            Assert.That(File.Exists(Path.Combine("folder2", "Resources", "test1.txt")), "File 'folder2/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine("folder2", "Resources", "test2.txt")), "File 'folder2/Resources/test2.txt' not found");
        }

        [Test]
        [Repeat(100)]
        public void DeployFolderInLineWithTrailingSlash2()
        {
            Tools.DeleteDirectory("folder2");
            Deploy.Item(@"Resources\", "folder2");

            Assert.That(File.Exists(Path.Combine("folder2", "test1.txt")), "File 'folder2/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine("folder2", "test2.txt")), "File 'folder2/Resources/test2.txt' not found");
        }

        [Test]
        [Repeat(100)]
        public void DeployFolderInLineWithTrailingSlash3()
        {
            Tools.DeleteDirectory("folder2");
            Deploy.Item(@"Resources\", @"folder2\");

            Assert.That(File.Exists(Path.Combine("folder2", "test1.txt")), "File 'folder2/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine("folder2", "test2.txt")), "File 'folder2/Resources/test2.txt' not found");
        }

        [Test]
        [Repeat(100)]
        public void DeployToSourceInLine()
        {
            Deploy.Item(@"Resources\test1.txt", "Resources");
            Assert.That(File.Exists(Path.Combine("Resources", "test1.txt")), "File 'folder/Resources/test1.txt' not found");
        }

        [Test]
        [Repeat(100)]
        public void DeployItemToAbsolutePath()
        {
            string currentDirectory = Environment.CurrentDirectory;
            Deploy.Item(@"Resources\test1.txt", currentDirectory);

            string file = Path.Combine(currentDirectory, "test1.txt");
            Assert.That(File.Exists(file), "File '{0}' not found", file);
        }

        [Test]
        [Repeat(100)]
        public void DeployItemToAbsolutePath2()
        {
            string currentDirectory = Path.Combine(Environment.CurrentDirectory, "sub");
            Deploy.Item(@"Resources\test1.txt", currentDirectory);

            string file = Path.Combine(currentDirectory, "test1.txt");
            Assert.That(File.Exists(file), "File '{0}' not found", file);
        }
    }
}
