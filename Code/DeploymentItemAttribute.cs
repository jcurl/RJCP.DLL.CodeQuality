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

            // NUnit uses an obscure ShadowCopyCache directory which can be hard to find, so let's output it so the poor developer can get at it more easily
            Debug.WriteLine("DeploymentItem: Copying " + itemPath + " to " + binFolderPath);

            string itemPathInBin;
            if (string.IsNullOrEmpty(outputDirectory)) {
                itemPathInBin = new Uri(Path.Combine(binFolderPath, itemName)).LocalPath;
            } else if (!string.IsNullOrEmpty(Path.GetPathRoot(outputDirectory))) {
                itemPathInBin = new Uri(Path.Combine(outputDirectory, itemName)).LocalPath;
            } else {
                itemPathInBin = new Uri(Path.Combine(binFolderPath, outputDirectory, itemName)).LocalPath;
            }

            if (File.Exists(itemPath)) {
                string parentFolderPathInBin = new DirectoryInfo(itemPathInBin).Parent.FullName;

                // If the target directory does not exist, create it
                if (!Directory.Exists(parentFolderPathInBin)) {
                    Directory.CreateDirectory(parentFolderPathInBin);
                }

                // copy source-file to the destination
                File.Copy(itemPath, itemPathInBin, true);

                // We must allow the destination file to be deletable
                FileAttributes fileAttributes = File.GetAttributes(itemPathInBin);
                if ((fileAttributes & FileAttributes.ReadOnly) != 0) {
                    File.SetAttributes(itemPathInBin, fileAttributes & ~FileAttributes.ReadOnly);
                }
            } else if (Directory.Exists(itemPath)) {
                if (Directory.Exists(itemPathInBin)) {
                    Directory.Delete(itemPathInBin, true);
                }

                // Create target directory
                Directory.CreateDirectory(itemPathInBin);

                // Now Create all of the sub-directories
                foreach (string dirPath in Directory.GetDirectories(itemPath, "*", SearchOption.AllDirectories)) {
                    Directory.CreateDirectory(dirPath.Replace(itemPath, itemPathInBin));
                }

                // Copy all the files & Replace any files with the same name
                foreach (string sourcePath in Directory.GetFiles(itemPath, "*.*", SearchOption.AllDirectories)) {
                    string destinationPath = sourcePath.Replace(itemPath, itemPathInBin);
                    File.Copy(sourcePath, destinationPath, true);

                    // We must allow the destination file to be deletable
                    FileAttributes fileAttributes = File.GetAttributes(destinationPath);
                    if ((fileAttributes & FileAttributes.ReadOnly) != 0) {
                        File.SetAttributes(destinationPath, fileAttributes & ~FileAttributes.ReadOnly);
                    }
                }
            } else {
                Debug.WriteLine("Warning: Deployment item does not exist - \"" + itemPath + "\"");
            }
        }
    }
}
