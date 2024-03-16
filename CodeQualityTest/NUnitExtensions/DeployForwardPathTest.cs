namespace RJCP.CodeQuality.NUnitExtensions
{
    using System;
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class DeployForwardPathTest
    {
        [Test]
        [Repeat(100)]
        public void DeployFileInLine()
        {
            Deploy.DeleteFile("test1.txt");
            Deploy.Item("Resources/test1.txt");
            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "test1.txt")));
        }

        [Test]
        [Repeat(100)]
        public void DeployFolderInLine()
        {
            Deploy.DeleteDirectory("folder2");
            Deploy.Item("Resources", "folder2");

            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "folder2", "Resources", "test1.txt")), "File 'folder2/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "folder2", "Resources", "test2.txt")), "File 'folder2/Resources/test2.txt' not found");
        }

        [Test]
        [Repeat(100)]
        public void DeployFolderInLineWithTrailingSlash1()
        {
            Deploy.DeleteDirectory("folder2");
            Deploy.Item("Resources", "folder2/");

            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "folder2", "Resources", "test1.txt")), "File 'folder2/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "folder2", "Resources", "test2.txt")), "File 'folder2/Resources/test2.txt' not found");
        }

        [Test]
        [Repeat(100)]
        public void DeployFolderInLineWithTrailingSlash2()
        {
            Deploy.DeleteDirectory("folder2");
            Deploy.Item("Resources/", "folder2");

            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "folder2", "test1.txt")), "File 'folder2/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "folder2", "test2.txt")), "File 'folder2/Resources/test2.txt' not found");
        }

        [Test]
        [Repeat(100)]
        public void DeployFolderInLineWithTrailingSlash3()
        {
            Deploy.DeleteDirectory("folder2");
            Deploy.Item("Resources/", "folder2/");

            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "folder2", "test1.txt")), "File 'folder2/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "folder2", "test2.txt")), "File 'folder2/Resources/test2.txt' not found");
        }

        [Test]
        [Repeat(100)]
        public void DeployToSourceInLine()
        {
            Deploy.Item("Resources/test1.txt", "Resources");
            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "Resources", "test1.txt")), "File 'folder/Resources/test1.txt' not found");
        }

        [Test]
        [Repeat(100)]
        public void DeployItemToAbsolutePath()
        {
            Deploy.Item("Resources/test1.txt", Deploy.WorkDirectory);

            string file = Path.Combine(Deploy.WorkDirectory, "test1.txt");
            Assert.That(File.Exists(file), $"File '{file}' not found");
        }

        [Test]
        [Repeat(100)]
        public void DeployItemToAbsolutePath2()
        {
            string subDirectory = Path.Combine(Deploy.WorkDirectory, "work-foo");
            Deploy.Item("Resources/test1.txt", subDirectory);

            string file = Path.Combine(subDirectory, "test1.txt");
            Assert.That(File.Exists(file), $"File '{file}' not found");
        }

        [Test]
        public void DeployNullItem()
        {
            Assert.That(() => { Deploy.Item(null); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void DeployNullItem2()
        {
            Assert.That(() => { Deploy.Item(null, "."); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [Repeat(100)]
        public void DeleteDirectory()
        {
            Deploy.Item("Resources", "folder3");

            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "folder3", "Resources", "test1.txt")), "File 'folder3/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "folder3", "Resources", "test2.txt")), "File 'folder3/Resources/test2.txt' not found");

            Deploy.DeleteDirectory("folder3");

            Assert.That(!Directory.Exists(Path.Combine(Deploy.WorkDirectory, "folder3")));
        }

        [Test]
        [Repeat(100)]
        public void DeleteTwoDirectories()
        {
            Deploy.Item("Resources", "folder4/test1");
            Deploy.Item("Resources", "folder4/test2");

            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "folder4", "test1", "Resources", "test1.txt")), "File 'folder4/Resources/test1/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "folder4", "test1", "Resources", "test2.txt")), "File 'folder4/Resources/test1/test2.txt' not found");
            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "folder4", "test2", "Resources", "test1.txt")), "File 'folder4/Resources/test2/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "folder4", "test2", "Resources", "test2.txt")), "File 'folder4/Resources/test2/test2.txt' not found");

            Deploy.DeleteDirectory("folder4");

            Assert.That(!Directory.Exists(Path.Combine(Deploy.WorkDirectory, "folder4")));
        }
    }
}
