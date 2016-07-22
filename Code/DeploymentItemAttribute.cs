namespace NUnit.Framework
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Class DeploymentItemAttribute.
    /// </summary>
    /// <remarks>
    /// Takes a compatible MSTest type DeploymentItem attribute for usage with nUnit. This code was taken from
    /// http://stackoverflow.com/questions/9378276/nunit-deploymentitem.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public class DeploymentItemAttribute : Attribute
    {
        /// <summary>
        /// NUnit replacement for Microsoft.VisualStudio.TestTools.UnitTesting.DeploymentItemAttribute
        /// Marks an item to be relevant for a unit-test and copies it to deployment-directory for this unit-test.
        /// </summary>
        /// <param name="path">The relative or absolute path to the file or directory to deploy. The path is relative to the build output directory.</param>
        /// <param name="outputDirectory">The path of the directory to which the items are to be copied. It can be either absolute or relative to the deployment directory.</param>
        public DeploymentItemAttribute(string path, string outputDirectory = null)
        {
            // Escape input-path to correct back-slashes for Windows
            string filePath = path.Replace("/", "\\");

            DirectoryInfo environmentDir = new DirectoryInfo(Environment.CurrentDirectory);

            // Get the full path and name of the deployment item
            string itemPath = new Uri(Path.Combine(environmentDir.FullName, filePath)).LocalPath;
            string itemName = Path.GetFileName(itemPath);

            // Get the target-path where to copy the deployment item to
            string binFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string itemPathInBin;
            if (string.IsNullOrEmpty(outputDirectory)) {
                itemPathInBin = new Uri(Path.Combine(binFolderPath, itemName)).LocalPath;
            } else if (!string.IsNullOrEmpty(Path.GetPathRoot(outputDirectory))) {
                itemPathInBin = new Uri(Path.Combine(outputDirectory, itemName)).LocalPath;
            } else {
                itemPathInBin = new Uri(Path.Combine(binFolderPath, outputDirectory, itemName)).LocalPath;
            }

            try {
                if (File.Exists(itemPath)) {
                    string parentFolderPathInBin = new DirectoryInfo(itemPathInBin).Parent.FullName;

                    if (!File.Exists(itemPathInBin)) {
                        if (!Directory.Exists(parentFolderPathInBin)) {
                            Directory.CreateDirectory(parentFolderPathInBin);
                        }
                    }

                    if (!CopyFile(itemPath, itemPathInBin)) {
                        Console.WriteLine("Could not copy from {0} to {1}", itemPath, itemPathInBin);
                    }
                } else if (Directory.Exists(itemPath)) {
                    if (Directory.Exists(itemPathInBin)) {
                        Directory.Delete(itemPathInBin, true);
                    }

                    Directory.CreateDirectory(itemPathInBin);
                    foreach (string dirPath in Directory.GetDirectories(itemPath, "*", SearchOption.AllDirectories)) {
                        Directory.CreateDirectory(dirPath.Replace(itemPath, itemPathInBin));
                    }

                    foreach (string sourcePath in Directory.GetFiles(itemPath, "*.*", SearchOption.AllDirectories)) {
                        string destinationPath = sourcePath.Replace(itemPath, itemPathInBin);
                        if (!CopyFile(sourcePath, destinationPath)) {
                            Console.WriteLine("Could not copy from {0} to {1}", itemPath, itemPathInBin);
                        }
                    }
                } else {
                    Console.WriteLine("Warning: Deployment item does not exist - \"" + itemPath + "\"");
                }
            } catch (System.Exception ex) {
                Console.WriteLine("Exception during deployment: {0}", ex.ToString());
                throw;
            }
        }

        private static bool CopyFile(string source, string destination)
        {
            if (!File.Exists(source)) return false;

            FileInfo itemInfo = new FileInfo(source);

            // Check if we need to copy the file. We only do so if it doesn't exist or if it's
            // different (regardless of why).
            if (File.Exists(destination)) {
                FileInfo itemPathInBinInfo = new FileInfo(destination);
                if (itemInfo.Length == itemPathInBinInfo.Length &&
                    itemInfo.LastWriteTime == itemPathInBinInfo.LastWriteTime &&
                    itemInfo.CreationTime == itemPathInBinInfo.CreationTime) return true;
            }

            File.Copy(source, destination, true);

            // Allow destination file to be deletable and set the creation time to be identical to the source
            FileAttributes fileAttributes = File.GetAttributes(destination);
            if ((fileAttributes & FileAttributes.ReadOnly) != 0) {
                File.SetAttributes(destination, fileAttributes & ~FileAttributes.ReadOnly);
            }
            File.SetCreationTime(destination, itemInfo.CreationTime);
            return true;
        }
    }
}
