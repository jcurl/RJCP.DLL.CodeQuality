namespace NUnit.Framework
{
    using System;
    using System.IO;

    [TestFixture(Category = "NUnitExtensions.Deployment.ForwardPath")]
    public class DeployForwardPathTest
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Deploy.ItemsWithAttribute(this);
        }

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

        [Test]
        [DeploymentItem("Resources/test1.txt", "files")]
        [DeploymentItem("Resources/test2.txt", "files")]
        public void DeployTwoItemsToOtherDirectory()
        {
            Assert.That(File.Exists(Path.Combine("files", "test1.txt")), "File 'files/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine("files", "test2.txt")), "File 'files/test2.txt' not found");
        }

        [Test]
        [Repeat(100)]
        public void DeployFileInLine()
        {
            Deploy.DeleteFile("test1.txt");
            Deploy.Item("Resources/test1.txt");
            Assert.That(File.Exists("test1.txt"));
        }

        [Test]
        [Repeat(100)]
        public void DeployFolderInLine()
        {
            Deploy.DeleteDirectory("folder2");
            Deploy.Item("Resources", "folder2");

            Assert.That(File.Exists(Path.Combine("folder2", "Resources", "test1.txt")), "File 'folder2/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine("folder2", "Resources", "test2.txt")), "File 'folder2/Resources/test2.txt' not found");
        }

        [Test]
        [Repeat(100)]
        public void DeployFolderInLineWithTrailingSlash1()
        {
            Deploy.DeleteDirectory("folder2");
            Deploy.Item("Resources", "folder2/");

            Assert.That(File.Exists(Path.Combine("folder2", "Resources", "test1.txt")), "File 'folder2/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine("folder2", "Resources", "test2.txt")), "File 'folder2/Resources/test2.txt' not found");
        }

        [Test]
        [Repeat(100)]
        public void DeployFolderInLineWithTrailingSlash2()
        {
            Deploy.DeleteDirectory("folder2");
            Deploy.Item("Resources/", "folder2");

            Assert.That(File.Exists(Path.Combine("folder2", "test1.txt")), "File 'folder2/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine("folder2", "test2.txt")), "File 'folder2/Resources/test2.txt' not found");
        }

        [Test]
        [Repeat(100)]
        public void DeployFolderInLineWithTrailingSlash3()
        {
            Deploy.DeleteDirectory("folder2");
            Deploy.Item("Resources/", "folder2/");

            Assert.That(File.Exists(Path.Combine("folder2", "test1.txt")), "File 'folder2/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine("folder2", "test2.txt")), "File 'folder2/Resources/test2.txt' not found");
        }

        [Test]
        [Repeat(100)]
        public void DeployToSourceInLine()
        {
            Deploy.Item("Resources/test1.txt", "Resources");
            Assert.That(File.Exists(Path.Combine("Resources", "test1.txt")), "File 'folder/Resources/test1.txt' not found");
        }

        [Test]
        [Repeat(100)]
        public void DeployItemToAbsolutePath()
        {
            string currentDirectory = Environment.CurrentDirectory;
            Deploy.Item("Resources/test1.txt", currentDirectory);

            string file = Path.Combine(currentDirectory, "test1.txt");
            Assert.That(File.Exists(file), "File '{0}' not found", file);
        }

        [Test]
        [Repeat(100)]
        public void DeployItemToAbsolutePath2()
        {
            string currentDirectory = Path.Combine(Environment.CurrentDirectory, "sub");
            Deploy.Item("Resources/test1.txt", currentDirectory);

            string file = Path.Combine(currentDirectory, "test1.txt");
            Assert.That(File.Exists(file), "File '{0}' not found", file);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeployNullItem()
        {
            Deploy.Item(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeployNullItem2()
        {
            Deploy.Item(null, ".");
        }

        [Test]
        [Repeat(100)]
        public void DeleteDirectory()
        {
            Deploy.Item("Resources", "folder3");

            Assert.That(File.Exists(Path.Combine("folder3", "Resources", "test1.txt")), "File 'folder3/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine("folder3", "Resources", "test2.txt")), "File 'folder3/Resources/test2.txt' not found");

            Deploy.DeleteDirectory("folder3");

            Assert.That(!Directory.Exists("folder3"));
        }

        [Test]
        [Repeat(100)]
        public void DeleteTwoDirectories()
        {
            Deploy.Item("Resources", "folder4/test1");
            Deploy.Item("Resources", "folder4/test2");

            Assert.That(File.Exists(Path.Combine("folder4", "test1", "Resources", "test1.txt")), "File 'folder4/Resources/test1/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine("folder4", "test1", "Resources", "test2.txt")), "File 'folder4/Resources/test1/test2.txt' not found");
            Assert.That(File.Exists(Path.Combine("folder4", "test2", "Resources", "test1.txt")), "File 'folder4/Resources/test2/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine("folder4", "test2", "Resources", "test2.txt")), "File 'folder4/Resources/test2/test2.txt' not found");

            Deploy.DeleteDirectory("folder4");

            Assert.That(!Directory.Exists("folder4"));
        }
    }
}
