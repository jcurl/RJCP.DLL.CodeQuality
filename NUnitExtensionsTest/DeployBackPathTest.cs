namespace NUnit.Framework
{
    using System.IO;

    [TestFixture(Category = "NUnitExtensions.Deployment.BackPath")]
    public class DeployBackPathTest
    {
        [Test]
        [Repeat(100)]
        public void DeployFileInLine()
        {
            Deploy.DeleteFile("test1.txt");
            Deploy.Item(@"Resources\test1.txt");
            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "test1.txt")));
        }

        [Test]
        [Repeat(100)]
        public void DeployFolderInLineWithTrailingSlash1()
        {
            Deploy.DeleteDirectory("folder2");
            Deploy.Item("Resources", @"folder2\");

            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "folder2", "Resources", "test1.txt")), "File 'folder2/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "folder2", "Resources", "test2.txt")), "File 'folder2/Resources/test2.txt' not found");
        }

        [Test]
        [Repeat(100)]
        public void DeployFolderInLineWithTrailingSlash2()
        {
            Deploy.DeleteDirectory("folder2");
            Deploy.Item(@"Resources\", "folder2");

            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "folder2", "test1.txt")), "File 'folder2/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "folder2", "test2.txt")), "File 'folder2/Resources/test2.txt' not found");
        }

        [Test]
        [Repeat(100)]
        public void DeployFolderInLineWithTrailingSlash3()
        {
            Deploy.DeleteDirectory("folder2");
            Deploy.Item(@"Resources\", @"folder2\");

            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "folder2", "test1.txt")), "File 'folder2/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "folder2", "test2.txt")), "File 'folder2/Resources/test2.txt' not found");
        }

        [Test]
        [Repeat(100)]
        public void DeployToSourceInLine()
        {
            Deploy.Item(@"Resources\test1.txt", "Resources");
            Assert.That(File.Exists(Path.Combine(Deploy.WorkDirectory, "Resources", "test1.txt")), "File 'folder/Resources/test1.txt' not found");
        }

        [Test]
        [Repeat(100)]
        public void DeployItemToAbsolutePath()
        {
            Deploy.Item(@"Resources\test1.txt", Deploy.WorkDirectory);

            string file = Path.Combine(Deploy.WorkDirectory, "test1.txt");
            Assert.That(File.Exists(file), "File '{0}' not found", file);
        }

        [Test]
        [Repeat(100)]
        public void DeployItemToAbsolutePath2()
        {
            string subDirectory = Path.Combine(Deploy.WorkDirectory, "work-foo");
            Deploy.Item(@"Resources\test1.txt", subDirectory);

            string file = Path.Combine(subDirectory, "test1.txt");
            Assert.That(File.Exists(file), "File '{0}' not found", file);
        }
    }
}
