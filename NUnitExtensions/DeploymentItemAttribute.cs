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
    /// Takes a compatible MSTest type DeploymentItem attribute for usage with nUnit. You specify this for the test
    /// case, or the test fixture. As NUnit reflects over the assembly looking for test csaes, it will reflect over
    /// this attribute also causing the copy to occur. Therefore, the deployment occurs during test case discovery
    /// regardless if your test case is being executed or not.
    /// <para>This attribute will work for nUnit shadow copy enabled or disabled, as it relies on the test case
    /// assembly <see cref="Assembly.CodeBase"/> property.</para>
    /// <para>When using this attribute, ensure that two test cases don't clobber each other. This might occur
    /// if you try to do strange things such as deploying directory A/B in one test case and then deploying the
    /// parent directory A in a different test case.</para>
    /// <para>Before the file is deployed, it is checked if the copy has already been done by comparing the length,
    /// the create timestamp and modify timestamp. If any of these differ, the file is copied. If they're all the same,
    /// no copy occurs. This makes the copy as fast as possible and ensures also that files can't be copied on top
    /// of themselves.</para>
    /// <para>When deploying directories, the files are <b>merged</b> with the destination directory.</para>
    /// <para>If you have two different files that are being deployed to the same location as the same name, the
    /// results are undefined. Don't do it. Your test cases may pass or fail depending on the position of the moon,
    /// or if your cat just sneezed a few minutes ago. The same applies if your deploying two different directories
    /// to the same location that have different content but where filenames overlap.</para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public class DeploymentItemAttribute : Attribute
    {
        /// <summary>
        /// NUnit replacement for Microsoft.VisualStudio.TestTools.UnitTesting.DeploymentItemAttribute
        /// Marks an item to be relevant for a unit-test and copies it to deployment-directory for this unit-test.
        /// </summary>
        /// <param name="path">The relative or absolute path to the file or directory to deploy. The path is relative to the build output directory.</param>
        public DeploymentItemAttribute(string path) : this(path, null) { }

        /// <summary>
        /// NUnit replacement for Microsoft.VisualStudio.TestTools.UnitTesting.DeploymentItemAttribute
        /// Marks an item to be relevant for a unit-test and copies it to deployment-directory for this unit-test.
        /// </summary>
        /// <param name="path">The relative or absolute path to the file or directory to deploy. The path is relative to the build output directory.</param>
        /// <param name="outputDirectory">The path of the directory to which the items are to be copied. It can be either absolute or relative to the deployment directory.</param>
        public DeploymentItemAttribute(string path, string outputDirectory)
        {
            // Escape input-path to correct back-slashes for Windows
            string filePath = path.Replace("/", "\\");

            DirectoryInfo environmentDir = new DirectoryInfo(Environment.CurrentDirectory);

            // Get the full path and name of the deployment item
            string itemPath = new Uri(Path.Combine(environmentDir.FullName, filePath)).LocalPath;
            string itemName = Path.GetFileName(itemPath);

            // Get the target-path where to copy the deployment item to
            string binFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);

            // Trim any spaces. It's not a good idea if the user provided spaces in the directory name before or after.
            // Most OSes and/or people get confused otherwise.
            string normalizedDir = outputDirectory == null ? string.Empty : outputDirectory.Trim();
            string itemPathInBin;
            if (string.IsNullOrEmpty(normalizedDir)) {
                itemPathInBin = new Uri(Path.Combine(binFolderPath, itemName)).LocalPath;
            } else if (!string.IsNullOrEmpty(Path.GetPathRoot(normalizedDir))) {
                itemPathInBin = new Uri(Path.Combine(normalizedDir, itemName)).LocalPath;
            } else {
                itemPathInBin = new Uri(Path.Combine(binFolderPath, normalizedDir, itemName)).LocalPath;
            }

            if (File.Exists(itemPath)) {
                string parentFolderPathInBin = new DirectoryInfo(itemPathInBin).Parent.FullName;

                if (!File.Exists(itemPathInBin)) {
                    if (!CreateDirectory(parentFolderPathInBin)) {
                        Console.WriteLine("Could not create directory at {0}", itemPathInBin);
                    }
                }

                if (!CopyFile(itemPath, itemPathInBin)) {
                    Console.WriteLine("Could not copy from {0} to {1}", itemPath, itemPathInBin);
                }
            } else if (Directory.Exists(itemPath)) {
                if (!CreateDirectory(itemPathInBin)) {
                    Console.WriteLine("Could not create directory at {0}", itemPathInBin);
                }
                foreach (string dirPath in Directory.GetDirectories(itemPath, "*", SearchOption.AllDirectories)) {
                    string destinationDir = dirPath.Replace(itemPath, itemPathInBin);
                    if (!CreateDirectory(destinationDir)) {
                        Console.WriteLine("Could not create directory at {0}", destinationDir);
                    }
                }

                foreach (string sourcePath in Directory.GetFiles(itemPath, "*.*", SearchOption.AllDirectories)) {
                    string destinationPath = sourcePath.Replace(itemPath, itemPathInBin);
                    if (!CopyFile(sourcePath, destinationPath)) {
                        Console.WriteLine("Could not copy from {0} to {1}", itemPath, itemPathInBin);
                    }
                }
            }
        }

        private static bool CreateDirectory(string directory)
        {
            if (Directory.Exists(directory)) return true;

            // Creates all directories and subdirectories in the specified path. So we don't need to create each
            // individual directory.
            try {
                Directory.CreateDirectory(directory);
            } catch (UnauthorizedAccessException) {
                // Permissions problem, couldn't create the directory...
                return false;
            }
            return true;
        }

        private static bool CopyFile(string source, string destination)
        {
            if (!File.Exists(source)) return false;

            bool copyFinished = false;
            int attempts = 4;  // Retry every 250ms meaning maximum timeout of 1s.
            FileInfo itemInfo;
            do {
                itemInfo = new FileInfo(source);

                // Check if we need to copy the file. We only do so if it doesn't exist or if it's
                // different (regardless of why).
                if (File.Exists(destination)) {
                    FileInfo itemPathInBinInfo = new FileInfo(destination);
                    if (itemInfo.Length == itemPathInBinInfo.Length &&
                        itemInfo.LastWriteTime == itemPathInBinInfo.LastWriteTime &&
                        itemInfo.CreationTime == itemPathInBinInfo.CreationTime) return true;
                }

                bool fail = false;
                try {
                    File.Copy(source, destination, true);
                    copyFinished = true;
                } catch (AccessViolationException) {
                    fail = true;
                } catch (UnauthorizedAccessException) {
                    // On windows occurs if the file is already open.
                    fail = true;
                }
                if (fail) {
                    // If the copy failed, it's because it's probably already open and being copied already
                    // from another instance of DeployItemAttribute. So we wait 250ms and try again. The race
                    // condition occurs because at the time of the check it didn't exist, but between that
                    // and now the copy has started elsewhere.
                    --attempts;
                    if (attempts > 0) System.Threading.Thread.Sleep(250);
                }
            } while (!copyFinished && attempts > 0);

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
