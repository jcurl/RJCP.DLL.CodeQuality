namespace NUnit.Framework
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Methods for the deployment of test resources which NUnit doesn't do automatically.
    /// </summary>
    /// <remarks>
    /// These methods are identical to those found in the class <see cref="DeploymentItemAttribute"/>.
    /// The biggest difference is that you use these methods within the definition of your test case,
    /// instead of assigning them as an attribute to your test case. Disadvantages of using these
    /// instead of attributes are the incompatibility with MSTest code (although you could just as well
    /// include this library with an MSTest with no issues). Biggest advantage is that the file is
    /// only copied when executed explicitly by your test case and not when nUnit or any other software
    /// iterates over your classes (and therefore avoiding a surprise for other generic code that might
    /// reflect your test cases and not expect files to be copied at the same time).
    /// </remarks>
    public static class Deploy
    {
        /// <summary>
        /// In line test case method for deployment of a test resource.
        /// </summary>
        /// <remarks>
        /// This method is compatible with the attribute <see cref="DeploymentItemAttribute(string)"/>, but can be executed in the test
        /// case body.
        /// </remarks>
        /// <param name="path">The relative or absolute path to the file or directory to deploy.
        /// The path is relative to the build output directory.</param>
        public static void Item(string path)
        {
            Item(path, null);
        }

        /// <summary>
        /// In line test case method for deployment of a test resource.
        /// </summary>
        /// <remarks>
        /// This method is compatible with the attribute <see cref="DeploymentItemAttribute(string, string)"/>, but can be executed in the test
        /// case body.
        /// </remarks>
        /// <param name="path">The relative or absolute path to the file or directory to deploy. The path is relative to the build output directory.</param>
        /// <param name="outputDirectory">The path of the directory to which the items are to be copied. It can be either absolute or relative to the deployment directory.</param>
        public static void Item(string path, string outputDirectory)
        {
            // Escape input-path to correct back-slashes for Windows
            string filePath = path.Replace("/", "\\");

            DirectoryInfo environmentDir = new DirectoryInfo(Environment.CurrentDirectory);

            // Get the full path and name of the deployment item
            string itemPath = GetPath(environmentDir.FullName, filePath);
            string itemName = Path.GetFileName(itemPath);

            // Get the target-path where to copy the deployment item to
            string binFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);

            // Trim any spaces. It's not a good idea if the user provided spaces in the directory name before or after.
            // Most OSes and/or people get confused otherwise.
            string normalizedDir = outputDirectory == null ? string.Empty : outputDirectory.Trim();
            string itemPathInBin;
            if (string.IsNullOrEmpty(normalizedDir)) {
                itemPathInBin = GetPath(binFolderPath, itemName);
            } else if (!string.IsNullOrEmpty(Path.GetPathRoot(normalizedDir))) {
                itemPathInBin = GetPath(normalizedDir, itemName);
            } else {
                itemPathInBin = GetPath(binFolderPath, normalizedDir, itemName);
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

        private static string GetPath(params string[] paths)
        {
            int platform = (int)Environment.OSVersion.Platform;
            bool onUnix = platform == 4 || platform == 6 || platform == 128;

            // See http://www.mono-project.com/docs/faq/technical/#how-to-detect-the-execution-platform
            if (!onUnix) {
                return new Uri(Path.Combine(paths)).LocalPath;
            }

            // For Linux we have to remove the "file:" from the begining of the absolute path, otherwise,
            // after calling Path.Combine, the "file://<root_folder>" part shall be removed and the
            // resulting combined path shall be invalid.
            for (int index = 0; index < paths.Length; index++) {
                if (paths[index].StartsWith("file:")) {
                    paths[index] = paths[index].Replace("file:", string.Empty);
                }
            }

            return new Uri(Path.Combine(paths)).LocalPath;
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

            if (!File.Exists(destination)) {
                return false;
            }

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
