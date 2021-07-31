namespace RJCP.CodeQuality.NUnitExtensions
{
    using System;
    using System.IO;
    using System.Threading;

    /// <summary>
    /// Methods for the deployment of test resources which NUnit doesn't do automatically.
    /// </summary>
    /// <remarks>
    /// These methods are used to control when files should be deployed along side a test case, as NUnit doesn't have any
    /// mechanism to do this.
    /// <para>
    /// You can specifically request deployment with <seealso cref="Deploy.Item(string, string)"/> within your test case.
    /// </para>
    /// <para>
    /// When deploying, files are copied relative from the <see cref="Deploy.TestDirectory"/> to the
    /// <see cref="Deploy.WorkDirectory"/>. These are based on NUnit's <c>TestContext.CurrentContext.TestDirectory</c>
    /// and <c>TestContext.CurrentContext.WorkDirectory</c> respectively, and so may be configurable by the user
    /// executing the test runner.
    /// </para>
    /// <para>
    /// Under Visual Studio, the <see cref="Deploy.TestDirectory"/> and the <see cref="Deploy.WorkDirectory"/> are
    /// usually the same value (for both NUnit 2.6.4 and NUnit 3.11.0). When running using a console runner, the
    /// <see cref="Deploy.TestDirectory"/> is the path where the test assembly is, where
    /// <see cref="Deploy.WorkDirectory"/> is the current directory where the runner is executed and so in this case
    /// differ.
    /// </para>
    /// <para>
    /// One must be careful to not rely on the current directory when running tests. Under NUnit 2.6.4, the current
    /// directory is set to <see cref="Deploy.TestDirectory"/>. Under NUnit 3.11.0, it is changed and is the current
    /// directory when the test runner was executed. This may break test cases when migrating from NUnit 2.6 to NUnit
    /// 3.x.
    /// </para>
    /// <para>
    /// A possibility is therefore provided by this class to allow the user to provide a work directory different to the
    /// test directory. A user can specify an application configuration to provide a path for
    /// <see cref="Deploy.WorkDirectory"/> which is different to <c>TestContext.CurrentContext.WorkDirectory</c>,
    /// especially useful when running unit tests within the Visual Studio environment.
    /// </para>
    /// <para>The following code snippet shows how to append the directory <c>work</c> to the work directory.</para>
    /// <code language="xml"><![CDATA[
    /// <configSections>
    ///   <section name="NUnitExtensions" type="NUnit.Framework.AppConfig.NUnitExtensionsSection, RJCP.NUnitExtensions"/>
    /// </configSections>
    ///
    /// <NUnitExtensions>
    ///   <deploy workDir="work" force="false"/>
    /// </NUnitExtensions>
    /// ]]></code>
    /// <para>
    /// The extension given by the <c><![CDATA[<deploy>]]></c> tag is only used if NUnit's <c>TestContext.CurrentContext</c>
    /// properties <c>TestDirectory</c> and <c>WorkDirectory</c> have the same value, or if the property
    /// <c>force="true"</c> is provided in the tag.
    /// </para>
    /// <para>
    /// As a general guideline, any files in the path given by <see cref="Deploy.TestDirectory"/> should not be modified.
    /// You should first <see cref="Deploy.Item(string)"/> which will copy to the <see cref="Deploy.WorkDirectory"/>.
    /// Only deploy if you intend to modify or create files based on the input during testing. If files don't need to be
    /// created, don't deploy and just reference the file direct with <see cref="Deploy.TestDirectory"/>.
    /// </para>
    /// <para>
    /// When creating files, create the path based on <see cref="Deploy.WorkDirectory"/>. You can create directories with
    /// <see cref="Deploy.CreateDirectory(string)"/>. You can delete files and directories with the
    /// <see cref="Deploy.DeleteDirectory(string)"/> and <see cref="Deploy.DeleteFile(string)"/>. If the paths given to
    /// the APIs are relative, it will be relative to <see cref="Deploy.WorkDirectory"/>.
    /// </para>
    /// <para>
    /// There is a workaround mode for NUnit 2 users to use the current directory for the working directory, even if the
    /// runner has a working directory specified. This option should not be used for new projects, as it will not work
    /// when using NUnit 3 (the current directory is usually read-only and points to the Visual Studio installation for
    /// NUnit 3).
    /// </para>
    /// <code language="xml"><![CDATA[
    /// <configSections>
    ///   <section name="NUnitExtensions" type="NUnit.Framework.AppConfig.NUnitExtensionsSection, RJCP.NUnitExtensions"/>
    /// </configSections>
    ///
    /// <NUnitExtensions>
    ///   <deploy useCwd="true"/>
    /// </NUnitExtensions>
    /// ]]></code>
    /// </remarks>
    public static class Deploy
    {
        private const int DeleteMaxTime = 5000;
        private const int DeletePollInterval = 100;
        private const int CopyWaitInterval = 250;
        private const int CopyWaitAttempts = 4;

        private readonly static object s_TestContextLock = new object();
        private static TestContextAccessor s_TestContextAccessor;

        private static TestContextAccessor TestContext
        {
            get
            {
                if (s_TestContextAccessor == null) {
                    lock (s_TestContextLock) {
                        if (s_TestContextAccessor == null) {
                            s_TestContextAccessor = TestContextAccessor.GetTestContext();
                        }
                    }
                }
                return s_TestContextAccessor;
            }
        }

        /// <summary>
        /// Gets the NUnit test directory.
        /// </summary>
        /// <value>The NUnit test directory.</value>
        /// <remarks>
        /// This property obtains the NUnit test directory, without directly referencing the NUnit framework. Instead, it
        /// uses reflection to determine the version of NUnit through the <c>[Test]</c> or <c>[TestCase]</c> attribute of
        /// the calling method. Thus it retrieves the <c>NUnit.Framework.TestContext.CurrentContext.TestDirectory</c>
        /// which is always compatible with the current version of NUnit. If there is a problem retrieving the directory,
        /// then <see cref="string.Empty"/> is returned.
        /// <para>This property is obtained via reflection on the first usage, and remains cached thereafter.</para>
        /// </remarks>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        /// There is a configuration error in the applications configuration file. Check the description provided in the
        /// <see cref="System.Configuration.ConfigurationErrorsException"/> on exactly what went wrong.
        /// </exception>
        public static string TestDirectory
        {
            get { return TestContext.TestDirectory ?? string.Empty; }
        }

        /// <summary>
        /// Gets the NUnit work directory.
        /// </summary>
        /// <value>The NUnit work directory.</value>
        /// <remarks>
        /// This property obtains the NUnit work directory, without directly referencing the NUnit framework. Instead, it
        /// uses reflection to determine the version of NUnit through the <c>[Test]</c> or <c>[TestCase]</c> attribute of
        /// the calling method. Thus it retrieves the <c>NUnit.Framework.TestContext.CurrentContext.WorkDirectory</c>
        /// which is always compatible with the current version of NUnit. If there is a problem retrieving the directory,
        /// then <see cref="string.Empty"/> is returned.
        /// <para>This property is obtained via reflection on the first usage, and remains cached thereafter.</para>
        /// </remarks>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        /// There is a configuration error in the applications configuration file. Check the description provided in the
        /// <see cref="System.Configuration.ConfigurationErrorsException"/> on exactly what went wrong.
        /// </exception>
        public static string WorkDirectory
        {
            get { return TestContext.WorkDirectory ?? string.Empty; }
        }

        private static string WorkDirectoryAbsolute
        {
            get
            {
                string workDirectory = WorkDirectory;
                if (Path.IsPathRooted(workDirectory)) return workDirectory;
                return Path.Combine(Environment.CurrentDirectory, workDirectory);
            }
        }

        /// <summary>
        /// Gets the name of the test that is currently executing.
        /// </summary>
        /// <value>The name of the test.</value>
        public static string TestName
        {
            get { return TestContext.TestName ?? string.Empty; }
        }

        /// <summary>
        /// Gets the full name of the test.
        /// </summary>
        /// <value>The full name of the test.</value>
        public static string TestFullName
        {
            get { return TestContext.TestFullName ?? string.Empty; }
        }

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
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method can deploy files within your test case at a time of your choosing.
        /// <para>Files are copied from the <paramref name="path"/> relative to the <see cref="TestDirectory"/>,
        /// to the <see cref="WorkDirectory"/>.</para>
        /// <para>If you have two different files that are being deployed to the same location with the same name, the
        /// results are undefined. Don't do it. Your test cases may pass or fail depending on the position of the moon,
        /// or if your cat just sneezed a few minutes ago. The same applies if your deploying two different directories
        /// to the same location that have different content but where filenames overlap.</para>
        /// </remarks>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        /// There is a configuration error in the applications configuration file. Check the description provided in the
        /// <see cref="System.Configuration.ConfigurationErrorsException"/> on exactly what went wrong.
        /// </exception>
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
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method can deploy files within your test case at a time of your choosing.
        /// <para>Files are copied from the <paramref name="path"/> relative to the <see cref="TestDirectory"/>,
        /// to the <paramref name="outputDirectory"/> relative to <see cref="WorkDirectory"/>.</para>
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
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        /// There is a configuration error in the applications configuration file. Check the description provided in the
        /// <see cref="System.Configuration.ConfigurationErrorsException"/> on exactly what went wrong.
        /// </exception>
        public static void Item(string path, string outputDirectory)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            string itemPath = GetFullPath(path, TestDirectory);

            // Get the target-path where to copy the deployment item to
            string normalizedOutputDir = outputDirectory ?? string.Empty;
            string fullOutputDirectory = GetFullPath(normalizedOutputDir, WorkDirectoryAbsolute);

            if (Directory.Exists(itemPath)) {
                // If the 'path' is a directory, the root path is the directory name.
                fullOutputDirectory = Path.Combine(fullOutputDirectory, Path.GetFileName(itemPath));
            }

            CreateDirectory(fullOutputDirectory);
            CopyFiles(itemPath, fullOutputDirectory);
        }

        /// <summary>
        /// Creates a new directory based on the current test name and changes to that directory.
        /// </summary>
        /// <returns>An object that should be disposed to return back to the original state.</returns>
        public static ScratchPad ScratchPad()
        {
            return new ScratchPad();
        }

        /// <summary>
        /// Creates a new directory based on the current test name.
        /// </summary>
        /// <param name="options">Configure the options for the <see cref="NUnitExtensions.ScratchPad()"/>.</param>
        /// <returns>An object that should be disposed to return back to the original state.</returns>
        public static ScratchPad ScratchPad(ScratchOptions options)
        {
            return new ScratchPad(options);
        }

        /// <summary>
        /// Creates a new directory based on the name and changes to that directory.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>An object that should be disposed to return back to the original state.</returns>
        public static ScratchPad ScratchPad(string name)
        {
            return new ScratchPad(name);
        }

        /// <summary>
        /// Creates a new directory based on the current test name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="options">Configure the options for the <see cref="NUnitExtensions.ScratchPad()"/>.</param>
        /// <returns>An object that should be disposed to return back to the original state.</returns>
        public static ScratchPad ScratchPad(string name, ScratchOptions options)
        {
            return new ScratchPad(name, options);
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

            string systemNormalizedPath;
            if (!OSInfo.Platform.IsUnix()) {
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
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        /// There is a configuration error in the applications configuration file. Check the description provided in the
        /// <see cref="System.Configuration.ConfigurationErrorsException"/> on exactly what went wrong.
        /// </exception>
        public static void CreateDirectory(string directory)
        {
            string fullPath = GetFullPath(directory, WorkDirectoryAbsolute);

            if (Directory.Exists(fullPath)) return;

            bool created = false;
            int attempts = CopyWaitAttempts;

            DeleteFile(fullPath);
            do {
                try {
                    Directory.CreateDirectory(fullPath);
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
        /// Deletes a file.
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
        /// Wild card characters cannot be included. Relative path information is interpreted as relative to
        /// the test working directory. To obtain the current working directory, see <see cref="WorkDirectory"/> .
        /// </remarks>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        /// There is a configuration error in the applications configuration file. Check the description provided in the
        /// <see cref="System.Configuration.ConfigurationErrorsException"/> on exactly what went wrong.
        /// </exception>
        public static void DeleteFile(string fileName)
        {
            string fullPath = GetFullPath(fileName, WorkDirectoryAbsolute);

            if (Directory.Exists(fullPath))
                throw new UnauthorizedAccessException("Can't delete the file, it is a directory");
            if (!File.Exists(fullPath)) return;

            if (OSInfo.Platform.IsWinNT()) {
                DeleteFileWindows(fullPath);
            } else if (OSInfo.Platform.IsUnix()) {
                DeleteFileUnix(fullPath);
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
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        /// There is a configuration error in the applications configuration file. Check the description provided in the
        /// <see cref="System.Configuration.ConfigurationErrorsException"/> on exactly what went wrong.
        /// </exception>
        public static void DeleteDirectory(string path)
        {
            string fullPath = GetFullPath(path, WorkDirectoryAbsolute);

            if (File.Exists(fullPath))
                throw new UnauthorizedAccessException("Can't delete the directory, it is a file");
            if (!Directory.Exists(fullPath)) return;

            DeleteSubDirectory(fullPath);
            DeleteEmptyDirectory(fullPath);
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
            if (OSInfo.Platform.IsWinNT()) {
                DeleteEmptyDirectoryWindows(path);
            } else if (OSInfo.Platform.IsUnix()) {
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
