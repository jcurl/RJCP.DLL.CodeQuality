namespace NUnit.Framework
{
    using System;
    using System.IO;

    [TestFixture(Category = "NUnitExtensions.Deployment")]
    public class NUnitExtensionsTest
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Deploy.ItemsWithAttribute(this);
        }

        private static bool DeleteFile(string path)
        {
            int attempts = 4;
            bool first = true;
            while (attempts > 0 && File.Exists(path)) {
                if (!first) System.Threading.Thread.Sleep(100);
                File.Delete(path);
                --attempts;
                first = false;
            }
            return !File.Exists(path);
        }

        private static bool DeleteDirectory(string path)
        {
            int attempts = 4;
            bool first = true;
            while (attempts > 0 && Directory.Exists(path)) {
                if (!first) System.Threading.Thread.Sleep(100);
                try {
                    Directory.Delete(path, true);
                } catch (DirectoryNotFoundException) {
                    /* We ignore this case, and retry */
                }
                --attempts;
                first = false;
            }
            return !Directory.Exists(path);
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
        public void DeployFileInLine()
        {
            DeleteFile("test1.txt");

            Deploy.Item("Resources/test1.txt");
            Assert.That(File.Exists("test1.txt"));
        }

        [Test]
        public void DeployFolderInLine()
        {
            DeleteDirectory("folder2");
            Deploy.Item("Resources", "folder2");

            Assert.That(File.Exists(Path.Combine("folder2", "Resources", "test1.txt")), "File 'folder2/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine("folder2", "Resources", "test2.txt")), "File 'folder2/Resources/test2.txt' not found");
        }

        [Test]
        public void DeployFolderInLineWithTrailingSlash1()
        {
            DeleteDirectory("folder2");
            Deploy.Item("Resources", "folder2/");

            Assert.That(File.Exists(Path.Combine("folder2", "Resources", "test1.txt")), "File 'folder2/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine("folder2", "Resources", "test2.txt")), "File 'folder2/Resources/test2.txt' not found");
        }

        [Test]
        public void DeployFolderInLineWithTrailingSlash2()
        {
            DeleteDirectory("folder2");
            Deploy.Item("Resources/", "folder2");

            Assert.That(File.Exists(Path.Combine("folder2", "test1.txt")), "File 'folder2/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine("folder2", "test2.txt")), "File 'folder2/Resources/test2.txt' not found");
        }

        [Test]
        public void DeployFolderInLineWithTrailingSlash3()
        {
            DeleteDirectory("folder2");
            Deploy.Item("Resources/", "folder2/");

            Assert.That(File.Exists(Path.Combine("folder2", "test1.txt")), "File 'folder2/Resources/test1.txt' not found");
            Assert.That(File.Exists(Path.Combine("folder2", "test2.txt")), "File 'folder2/Resources/test2.txt' not found");
        }

        [Test]
        public void DeployToSourceInLine()
        {
            Deploy.Item("Resources/test1.txt", "Resources");
            Assert.That(File.Exists(Path.Combine("Resources", "test1.txt")), "File 'folder/Resources/test1.txt' not found");
        }

        [Test]
        public void DeployItemToAbsolutePath()
        {
            string currentDirectory = Environment.CurrentDirectory;
            Deploy.Item("Resources/test1.txt", currentDirectory);

            string file = Path.Combine(currentDirectory, "test1.txt");
            Assert.That(File.Exists(file), "File '{0}' not found", file);
        }

        [Test]
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
    }
}
