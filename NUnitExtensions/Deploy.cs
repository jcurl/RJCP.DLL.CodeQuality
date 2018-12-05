namespace NUnit.Framework
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    /// <summary>
    /// Methods for the deployment of test resources which NUnit doesn't do automatically.
    /// </summary>
    /// <remarks>
    /// These methods are used to control when files should be deployed along side a test case,
    /// as NUnit doesn't have any mechanism to do this, unlike the Microsoft Test Framework.
    /// <para>You can specifically request deployment with <seealso cref="Deploy.Item(string, string)"/>
    /// within your test case.</para>
    /// </remarks>
    public static class Deploy
    {
        private const int DeleteMaxTime = 5000;
        private const int DeletePollInterval = 100;
        private const int DeleteWaitInterval = 250;
        private const int CopyWaitInterval = 250;
        private const int CopyWaitAttempts = 4;

        /// <summary>
        /// In line test case method for deployment of a test resource.
        /// </summary>
        /// <param name="path">The relative or absolute path to the file or directory to deploy.
        /// The path is relative to the build output directory.</param>
        /// <exception cref="FileNotFoundException">A file being deployed isn't found in the source.</exception>
        /// <exception cref="DirectoryNotFoundException">A source directory could not be found (note, destination directories
        /// are created for you, but may occur if there was a file system error).</exception>
        /// <exception cref="UnauthorizedAccessException">The source file or destination path has an access violation.</exception>
        /// <exception cref="PathTooLongException">The source path or destination path name is too long.</exception>
        /// <exception cref="NotSupportedException">Path names have unsupported or invalid characters.</exception>
        /// <exception cref="ArgumentException">Given path doesn't exist.</exception>
        /// <exception cref="IOException">A network error has occurred.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> may not be <see langword="null"/>.</exception>
        /// <remarks>
        /// This method can deploy files within your test case at a time of your choosing.
        /// <para>This method will work for nUnit shadow copy enabled or disabled, as it relies on the test case
        /// assembly <see cref="System.Reflection.Assembly.CodeBase"/> property.</para>
        /// <para>If you have two different files that are being deployed to the same location with the same name, the
        /// results are undefined. Don't do it. Your test cases may pass or fail depending on the position of the moon,
        /// or if your cat just sneezed a few minutes ago. The same applies if your deploying two different directories
        /// to the same location that have different content but where filenames overlap.</para>
        /// </remarks>
        public static void Item(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            Item(path, null);
        }

        /// <summary>
        /// In line test case method for deployment of a test resource.
        /// </summary>
        /// <param name="path">The relative or absolute path to the file or directory to deploy. The path
        /// is relative to the build output directory.</param>
        /// <param name="outputDirectory">The path of the directory to which the items are to be copied.
        /// It can be either absolute or relative to the deployment directory.</param>
        /// <exception cref="FileNotFoundException">A file being deployed isn't found in the source.</exception>
        /// <exception cref="DirectoryNotFoundException">A source directory could not be found (note, destination directories
        /// are created for you, but may occur if there was a file system error).</exception>
        /// <exception cref="UnauthorizedAccessException">The source file or destination path has an access violation.</exception>
        /// <exception cref="PathTooLongException">The source path or destination path name is too long.</exception>
        /// <exception cref="NotSupportedException">Path names have unsupported or invalid characters.</exception>
        /// <exception cref="ArgumentException">Given path doesn't exist.</exception>
        /// <exception cref="IOException">A network error has occurred.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> may not be <see langword="null"/>.</exception>
        /// <remarks>
        /// This method can deploy files within your test case at a time of your choosing.
        /// <para>This method will work for nUnit shadow copy enabled or disabled, as it relies on the test case
        /// assembly <see cref="System.Reflection.Assembly.CodeBase"/> property.</para>
        /// <para>When files and folders are copied to the output directory, they are <i>merged</i> with the content
        /// of the folder that may already exist.</para>
        /// <para>When using this method, ensure that two test cases don't clobber each other. This might occur
        /// if you try to do strange things such as deploying directory A/B in one test case and then deploying the
        /// parent directory A in a different test case.</para>
        /// <para>Before the file is deployed, it is checked if the copy has already been done by comparing the length,
        /// the create time stamp and modify time stamp. If any of these differ, the file is copied. If they're all the same,
        /// no copy occurs. This makes the copy as fast as possible and ensures also that files can't be copied on top
        /// of themselves.</para>
        /// <para>If you have two different files that are being deployed to the same location with the same name, the
        /// results are undefined. Don't do it. Your test cases may pass or fail depending on the position of the moon,
        /// or if your cat just sneezed a few minutes ago. The same applies if your deploying two different directories
        /// to the same location that have different content but where filenames overlap.</para>
        /// </remarks>
        public static void Item(string path, string outputDirectory)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            string itemPath = GetFullPath(path, Environment.CurrentDirectory);

            // Get the target-path where to copy the deployment item to
            string assemblyFileUri = Assembly.GetExecutingAssembly().CodeBase;
            Uri u = new Uri(assemblyFileUri);
            string assemblyFile = u.LocalPath;
            string assemblyPath = Path.GetDirectoryName(assemblyFile);
            string normalizedOutputDir = outputDirectory == null ? string.Empty : outputDirectory;
            string fullOutputDirectory = GetFullPath(normalizedOutputDir, assemblyPath);

            if (Directory.Exists(itemPath)) {
                // If the 'path' is a directory, the root path is the directory name.
                fullOutputDirectory = Path.Combine(fullOutputDirectory, Path.GetFileName(itemPath));
            }

            CreateDirectory(fullOutputDirectory);
            CopyFiles(itemPath, fullOutputDirectory);
        }

        private static void CopyFiles(string path, string outputDirectory)
        {
            if (File.Exists(path)) {
                // Copy the file.
                string fileName = Path.GetFileName(path);
                string fullDestination = Path.Combine(outputDirectory, fileName);
                CopyFile(path, fullDestination);
                return;
            }

            if (Directory.Exists(path)) {
                CopyDirectory(path, outputDirectory);
                return;
            }

            throw new ArgumentException("Given path doesn't exist", nameof(path));
        }

        private static string[] GetFiles(string path)
        {
            return Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
        }

        private static string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
        }

        private static string GetFullPath(string path, string basePath)
        {
            if (path == null) path = string.Empty;
            int platform = (int)Environment.OSVersion.Platform;
            bool onUnix = platform == 4 || platform == 6 || platform == 128;

            string systemNormalizedPath;
            if (!onUnix) {
                // Convert forward slashes to windows paths.
                systemNormalizedPath = path.Replace("/", @"\");
            } else {
                // Convert back slashes to Unix paths
                systemNormalizedPath = path.Replace(@"\", "/");
            }
            string itemPath;
            try {
                itemPath = new Uri(Path.Combine(basePath, systemNormalizedPath)).LocalPath;
            } catch (UriFormatException e) {
                throw new ArgumentException("Invalid path", e);
            }
            return itemPath;
        }

        /// <summary>
        /// Creates the directory if it doesn't exist already.
        /// </summary>
        /// <param name="directory">The directory to create.</param>
        /// <exception cref="AccessViolationException">The <paramref name="directory"/> could not be created as it is in use.</exception>
        /// <exception cref="UnauthorizedAccessException">The <paramref name="directory"/> could not be created due to insufficient permissions.</exception>
        /// <exception cref="PathTooLongException">The <paramref name="directory"/> path name is too long.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="directory"/> has unsupported or invalid characters.</exception>
        /// <exception cref="IOException">The network name is not known.</exception>
        /// <exception cref="ArgumentException">The <paramref name="directory"/> is a zero length string, contains only whitespace, or
        /// contains invalid characters as defined by <see cref="System.IO.Path.GetInvalidPathChars()"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="directory"/> is <see langword="null"/>.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified <paramref name="directory"/> path is invalid (for example, it
        /// is on an unmapped drive).</exception>
        /// <remarks>
        /// If the directory doesn't exist, it will be created. If there is a file in the place of the
        /// directory, the file will be first deleted and overwritten as a new directory.
        /// <para>In case of an access violation or unauthorized access, the operation is retried up to four times
        /// with a 250ms delay between each attempt.</para>
        /// </remarks>
        public static void CreateDirectory(string directory)
        {
            if (Directory.Exists(directory)) return;

            bool created = false;
            int attempts = CopyWaitAttempts;

            DeleteFile(directory);
            do {
                try {
                    Directory.CreateDirectory(directory);
                    created = true;
                } catch (AccessViolationException) {
                    if (attempts == 0) throw;
                } catch (UnauthorizedAccessException) {
                    // On windows occurs if the file is already open.
                    if (attempts == 0) throw;
                }

                if (!created) {
                    // If the copy failed, it's because it's probably already open somewhere else. So we
                    // wait 250ms and try again.
                    --attempts;
                    Thread.Sleep(CopyWaitInterval);
                }
            } while (!created);
        }

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <exception cref="UnauthorizedAccessException">The caller doesn't have the required permissions; or
        /// <paramref name="fileName" /> is a directory; or <paramref name="fileName" /> is a read-only file.</exception>
        /// <exception cref="IOException">File cannot be deleted.</exception>
        /// <exception cref="ArgumentException">The <paramref name="fileName" /> contains one or more of the invalid
        /// characters defined in <see cref="Path.GetInvalidPathChars()" />.</exception>
        /// <exception cref="PathTooLongException">The <paramref name="fileName" /> is longer than the system
        /// defined maximum length.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="fileName" /> is <see langword="null" />.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified <paramref name="fileName" /> path is invalid (for example, it
        /// is on an unmapped drive).</exception>
        /// <exception cref="NotSupportedException">The <paramref name="fileName" /> has unsupported or invalid characters.</exception>
        /// <exception cref="PlatformNotSupportedException">The platform is not Unix or WinNT. Use File.Delete instead.</exception>
        /// <remarks>
        /// Specify a file name with any relative or absolute path information for the path parameter.
        /// Wildcard characters cannot be included. Relative path information is interpreted as relative to
        /// the current working directory. To obtain the current working directory, see <see cref="Directory.GetCurrentDirectory"/> .
        /// </remarks>
        public static void DeleteFile(string fileName)
        {
            if (Directory.Exists(fileName))
                throw new UnauthorizedAccessException("Can't delete the file, it is a directory");
            if (!File.Exists(fileName)) return;

            if (Platform.IsWinNT()) {
                DeleteFileWindows(fileName);
            } else if (Platform.IsUnix()) {
                DeleteFileUnix(fileName);
            } else {
                throw new PlatformNotSupportedException();
            }
        }

        private static void DeleteFileUnix(string fileName)
        {
            File.Delete(fileName);
            if (!File.Exists(fileName)) return;
            string message = string.Format("File '{0}' couldn't be deleted", fileName);
            throw new IOException(message);
        }

        private static void DeleteFileWindows(string fileName)
        {
            int elapsed;
            int tickCount = Environment.TickCount;
            int deletePollIntervalExp = 5;
            Exception lastException;
            do {
                lastException = null;
                try {
                    File.Delete(fileName);
                } catch (UnauthorizedAccessException ex) {
                    // Occurs on Windows if the file is opened by a process.
                    lastException = ex;
                } catch (IOException ex) {
                    // Occurs on Windows if the file is opened by a process.
                    lastException = ex;
                }
                if (!File.Exists(fileName)) return;
                Thread.Sleep(deletePollIntervalExp);
                if (deletePollIntervalExp < DeletePollInterval) {
                    deletePollIntervalExp = Math.Min(deletePollIntervalExp * 2, DeletePollInterval);
                }
                elapsed = unchecked(Environment.TickCount - tickCount);
            } while (elapsed < DeleteMaxTime);

            if (lastException != null) throw lastException;
            string message = string.Format("File '{0}' couldn't be deleted", fileName);
            throw new IOException(message);
        }

        private static void CopyFile(string source, string destination)
        {
            if (!File.Exists(source)) {
                throw new FileNotFoundException("File not found", source);
            }

            bool copyFinished = false;
            int attempts = CopyWaitAttempts;
            FileInfo itemInfo;
            do {
                itemInfo = new FileInfo(source);

                // Check if we need to copy the file. We only do so if it doesn't exist or if it's
                // different (regardless of why).
                if (File.Exists(destination)) {
                    FileInfo itemPathInBinInfo = new FileInfo(destination);
                    if (itemInfo.Length == itemPathInBinInfo.Length &&
                        itemInfo.LastWriteTime == itemPathInBinInfo.LastWriteTime &&
                        itemInfo.CreationTime == itemPathInBinInfo.CreationTime) return;
                }

                try {
                    File.Copy(source, destination, true);
                    copyFinished = true;
                } catch (AccessViolationException) {
                    if (attempts == 0) throw;
                } catch (UnauthorizedAccessException) {
                    // On windows occurs if the file is already open.
                    if (attempts == 0) throw;
                } catch (DirectoryNotFoundException) {
                    throw;
                }

                if (!copyFinished) {
                    // If the copy failed, it's because it's probably already open and being copied already
                    // from another instance of Deploy. So we wait 250ms and try again. The race
                    // condition occurs because at the time of the check it didn't exist, but between that
                    // and now the copy has started elsewhere.
                    --attempts;
                    if (attempts > 0) System.Threading.Thread.Sleep(CopyWaitInterval);
                }
            } while (!copyFinished && attempts > 0);

            // Allow destination file to be deletable and set the creation time to be identical to the source
            FileAttributes fileAttributes = File.GetAttributes(destination);
            if ((fileAttributes & FileAttributes.ReadOnly) != 0) {
                File.SetAttributes(destination, fileAttributes & ~FileAttributes.ReadOnly);
            }
            File.SetCreationTime(destination, itemInfo.CreationTime);
        }

        private static void CopyDirectory(string sourceDirectory, string destDirectory)
        {
            if (!Directory.Exists(sourceDirectory)) {
                string message = string.Format("Directory '{0}' not found", sourceDirectory);
                throw new DirectoryNotFoundException(message);
            }

            // Copy each individual file
            string[] files = GetFiles(sourceDirectory);
            foreach (string file in files) {
                string destFileName = Path.GetFileName(file);
                string fullDestination = Path.Combine(destDirectory, destFileName);
                CopyFile(file, fullDestination);
            }

            // Copy the subdirectories
            string[] dirs = GetDirectories(sourceDirectory);
            foreach (string dir in dirs) {
                string nextDir = Path.Combine(destDirectory, Path.GetFileName(dir));
                CreateDirectory(nextDir);
                CopyDirectory(dir, nextDir);
            }
        }

        /// <summary>
        /// Deletes the directory with retries.
        /// </summary>
        /// <param name="path">The path of the directory to delete.</param>
        /// <exception cref="UnauthorizedAccessException">The caller doesn't have the required permissions; or
        /// <paramref name="path" /> is a file.</exception>
        /// <exception cref="ArgumentException">The <paramref name="path" /> contains one or more of the invalid
        /// characters defined in <see cref="Path.GetInvalidPathChars()" />.</exception>
        /// <exception cref="PathTooLongException">The <paramref name="path" /> is longer than the system
        /// defined maximum length.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="path" /> is <see langword="null" />.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified <paramref name="path" /> path is invalid (for example, it
        /// is on an unmapped drive).</exception>
        /// <exception cref="NotSupportedException">The <paramref name="path" /> has unsupported or invalid characters.</exception>
        /// <exception cref="PlatformNotSupportedException">The platform is not Unix or WinNT.</exception>
        /// <remarks>
        /// The directory is scanned and each file is individually deleted and waited upon until the file is deleted
        /// before continuing.
        /// </remarks>
        public static void DeleteDirectory(string path)
        {
            if (File.Exists(path))
                throw new UnauthorizedAccessException("Can't delete the directory, it is a file");
            if (!Directory.Exists(path)) return;
            if (!Path.IsPathRooted(path))
                path = Path.Combine(Environment.CurrentDirectory, path);

            DeleteSubDirectory(path);
            DeleteEmptyDirectory(path);
        }

        private static void DeleteSubDirectory(string path)
        {
            string[] files = GetFiles(path);
            foreach (string file in files) {
                DeleteFile(file);
            }

            string[] dirs = GetDirectories(path);
            foreach (string dir in dirs) {
                DeleteSubDirectory(dir);
                DeleteEmptyDirectory(dir);
            }
        }

        private static void DeleteEmptyDirectory(string path)
        {
            if (Platform.IsWinNT()) {
                DeleteEmptyDirectoryWindows(path);
            } else if (Platform.IsUnix()) {
                DeleteEmptyDirectoryUnix(path);
            } else {
                throw new PlatformNotSupportedException();
            }
        }

        private static void DeleteEmptyDirectoryWindows(string path)
        {
            int elapsed;
            int tickCount = Environment.TickCount;
            int deletePollIntervalExp = 5;
            Exception lastException;
            do {
                lastException = null;
                try {
                    Directory.Delete(path);
                } catch (UnauthorizedAccessException ex) {
                    // Occurs on Windows if a file in the directory is open.
                    lastException = ex;
                } catch (IOException ex) {
                    // Occurs on Windows if a file in the directory is open (or on Windows XP someone is enumerating the
                    // directory).
                    lastException = ex;
                }
                if (!Directory.Exists(path)) return;
                Thread.Sleep(deletePollIntervalExp);
                if (deletePollIntervalExp < DeletePollInterval) {
                    deletePollIntervalExp = Math.Min(deletePollIntervalExp * 2, DeletePollInterval);
                }
                elapsed = unchecked(Environment.TickCount - tickCount);
            } while (elapsed < DeleteMaxTime);

            if (lastException != null) throw lastException;
            string message = string.Format("Directory '{0}' couldn't be deleted", path);
            throw new IOException(message);
        }

        private static void DeleteEmptyDirectoryUnix(string path)
        {
            Directory.Delete(path);
            if (!Directory.Exists(path)) return;
            string message = string.Format("Directory '{0}' couldn't be deleted", path);
            throw new IOException(message);
        }
    }
}
