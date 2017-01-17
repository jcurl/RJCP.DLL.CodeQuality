namespace NUnit.Framework
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Methods for the deployment of test resources which NUnit doesn't do automatically.
    /// </summary>
    /// <remarks>
    /// These methods are used to control when files should be deployed along side a test case,
    /// as NUnit doesn't have any mechanism to do this, unlike the Microsoft Test Framework.
    /// <para>You can decorate the test methods with the <seealso cref="DeploymentItemAttribute"/>,
    /// or specifically request deployment with <seealso cref="Deploy.Item(string, string)"/>
    /// within your test case. When using the attribute, you still need to add code to your test
    /// case to request deployment with <seealso cref="Deploy.ItemsWithAttribute(object)"/>.</para>
    /// </remarks>
    public static class Deploy
    {
        /// <summary>
        /// Deploy files for all methods containing the attribute <see cref="DeploymentItemAttribute"/> for the given class.
        /// </summary>
        /// <param name="testClass">The test class to deploy files for.</param>
        /// <exception cref="ArgumentNullException"><paramref name="testClass"/> may not be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Error when deploying test artifacts. The errors are printed to the
        /// console.</exception>
        /// <remarks>
        /// You need to ensure that the methods for the class you're referencing are decorated with the
        /// <see cref="DeploymentItemAttribute"/>. See the example below.
        ///
        /// <code language="csharp">
        /// [TestFixture]
        /// public class NUnitExtensionsTest {
        ///     [TestFixtureSetUp]
        ///     public void TestFixtureSetUp() {
        ///         Deploy.ItemsWithAttribute(this);
        ///     }
        ///
        ///     [Test]
        ///     [DeploymentItem("test.txt")]
        ///     public void MyTest() {
        ///         Assert.That(File.Exists("test.txt"));
        ///     }
        /// }
        /// </code>
        /// <para>During deployment, as many files are deployed as possible. However, if there are errors when
        /// deploying all the files, the exception <see cref="ArgumentException"/> will be raised.</para>
        /// </remarks>
        public static void ItemsWithAttribute(object testClass)
        {
            if (testClass == null) throw new ArgumentNullException("testClass");
            ItemsWithAttribute(testClass.GetType());
        }

        /// <summary>
        /// Deploy files for all methods containing the attribute <see cref="DeploymentItemAttribute"/> for the given class.
        /// </summary>
        /// <param name="testClass">The test class to deploy files for.</param>
        /// <exception cref="ArgumentNullException"><paramref name="testClass"/> may not be <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Error when deploying test artifacts. The errors are printed to the standard
        /// console.</exception>
        /// <remarks>
        /// You need to ensure that the methods for the class you're referencing are decorated with the
        /// <see cref="DeploymentItemAttribute"/>. See the example below.
        ///
        /// <code language="csharp">
        /// [TestFixture]
        /// public class NUnitExtensionsTest {
        ///     [TestFixtureSetUp]
        ///     public void TestFixtureSetUp() {
        ///         Deploy.ItemsWithAttribute(typeof(this));
        ///     }
        ///
        ///     [Test]
        ///     [DeploymentItem("test.txt")]
        ///     public void MyTest() {
        ///         Assert.That(File.Exists("test.txt"));
        ///     }
        /// }
        /// </code>
        /// <para>During deployment, as many files are deployed as possible. However, if there are errors when
        /// deploying all the files, the exception <see cref="ArgumentException"/> will be raised.</para>
        /// </remarks>
        public static void ItemsWithAttribute(Type testClass)
        {
            bool exceptionFound = false;
            if (testClass == null) throw new ArgumentNullException("testClass");
            foreach (MethodInfo method in testClass.GetMethods(BindingFlags.Public | BindingFlags.Instance)) {
                exceptionFound |= DeployMember(method);
            }
            
            if (exceptionFound) {
                throw new ArgumentException("Error deploying test artifacts");
            }
        }

        private static bool DeployMember(MethodInfo method)
        {
            IEnumerable<DeploymentItemAttribute> attributes = GetAttribute<DeploymentItemAttribute>(method);
            if (attributes == null) return false;

            bool exceptionFound = false;
            foreach (DeploymentItemAttribute attribute in attributes) {
                try {
                    Item(attribute.Path, attribute.OutputDirectory);
                } catch (Exception e) {
                    if (!ReportException(e, method.DeclaringType.Name, method.Name, attribute.Path, attribute.OutputDirectory)) {
                        throw;
                    } else {
                        exceptionFound = true;
                    }
                }
            }
            return exceptionFound;
        }

        private static bool ReportException(Exception e, string className, string methodName, string sourcePath, string destDir)
        {
            Console.WriteLine("Exception deploying class: {0}; method: {1}", className, methodName);
            Console.WriteLine("  Copying from '{0}' to '{1}'", sourcePath, destDir);

            if (e is FileNotFoundException) {
                Console.WriteLine("File not found in test case deployment: {0}", e.Message);
            } else if (e is DirectoryNotFoundException) {
                Console.WriteLine("Directory not found in test case deployment: {0}", e.Message);
            } else if (e is UnauthorizedAccessException) {
                Console.WriteLine("Access denied in test case deployment: {0}", e.Message);
            } else if (e is PathTooLongException) {
                Console.WriteLine("Path too long in test case deployment: {0}", e.Message);
            } else if (e is NotSupportedException) {
                Console.WriteLine("Invalid characters in path in test case deployment: {0}", e.Message);
            } else if (e is ArgumentNullException) {
                Console.WriteLine("Path is null in test case deployment: {0}", e.Message);
            } else if (e is ArgumentException) {
                Console.WriteLine("Argument exception in test case deployment: {0}", e.Message);
            } else if (e is IOException) {
                Console.WriteLine("I/O Exception in test case deployment: {0}", e.Message);
            } else {
                Console.WriteLine("Unhandled Exception: {0}", e.ToString());
                return false;
            }

            return true;
        }

        private static IEnumerable<T> GetAttribute<T>(ICustomAttributeProvider provider) where T : Attribute
        {
            return provider.GetCustomAttributes(typeof(T), false).OfType<T>().ToArray();
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
        /// <exception cref="ArgumentNullException"><paramref name="path"/> may not be <see langword="null"/>.</exception>
        /// <remarks>
        /// This method can deploy files within your test case at a time of your choosing.
        /// <para>This attribute will work for nUnit shadow copy enabled or disabled, as it relies on the test case
        /// assembly <see cref="System.Reflection.Assembly.CodeBase"/> property.</para>
        /// <para>If you have two different files that are being deployed to the same location with the same name, the
        /// results are undefined. Don't do it. Your test cases may pass or fail depending on the position of the moon,
        /// or if your cat just sneezed a few minutes ago. The same applies if your deploying two different directories
        /// to the same location that have different content but where filenames overlap.</para>
        /// </remarks>
        public static void Item(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
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
        /// <para>This attribute will work for nUnit shadow copy enabled or disabled, as it relies on the test case
        /// assembly <see cref="System.Reflection.Assembly.CodeBase"/> property.</para>
        /// <para>When files and folders are copied to the output directory, they are <i>merged</i> with the content
        /// of the folder that may already exist.</para>
        /// <para>When using this attribute, ensure that two test cases don't clobber each other. This might occur
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
            if (path == null) throw new ArgumentNullException("path");
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
            int platform = (int)Environment.OSVersion.Platform;
            bool onUnix = platform == 4 || platform == 6 || platform == 128;

            if (File.Exists(path)) {
                // Copy the file.
                string fileName = Path.GetFileName(path);
                string fullDestination = Path.Combine(outputDirectory, fileName);
                if (!CopyFile(path, fullDestination)) {
                    Console.WriteLine("CopyFiles: Couldn't copy {0} to {1}", path, fullDestination);
                }
            } else if (Directory.Exists(path)) {
                // Get the files and directories and copy.
                string[] files = GetFiles(path);
                HashSet<string> copiedFiles = new HashSet<string>();

                // Copy the files from this directory to the destination.
                foreach (string file in files) {
                    string destFileName = Path.GetFileName(file);
                    string fullDestination = Path.Combine(outputDirectory, destFileName);
                    if (!CopyFile(file, fullDestination)) {
                        Console.WriteLine("CopyFiles: Couldn't copy {0} to {1}", file, fullDestination);
                    }
                    if (!onUnix) destFileName = destFileName.ToLowerInvariant();
                    copiedFiles.Add(destFileName);
                }

                // Copy files for all sub-directories
                string[] dirs = GetDirectories(path);
                foreach (string dir in dirs) {
                    string nextDir = Path.Combine(outputDirectory, Path.GetFileName(dir));
                    CreateDirectory(nextDir);
                    CopyFiles(dir, nextDir);
                }
            } else {
                throw new ArgumentException("Given path doesn't exist", "path");
            }
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

            if (!onUnix) {
                // Convert forward slashes to windows paths.
                string windowsPath = path.Replace("/", "\\");
                string itemPath;
                try {
                    itemPath = new Uri(Path.Combine(basePath, windowsPath)).LocalPath;
                } catch (UriFormatException e) {
                    throw new ArgumentException("Invalid path", e);
                }
                return itemPath;
            } else {
                string itemPath = Path.Combine(basePath, path);
                if (!Path.IsPathRooted(itemPath)) {
                    throw new ArgumentException("Invalid path - not rooted");
                }
                return itemPath;
            }
        }

        private static bool CreateDirectory(string directory)
        {
            if (Directory.Exists(directory)) return true;

            if (File.Exists(directory)) {
                File.Delete(directory);
            }

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
                } catch (DirectoryNotFoundException) {
                    return false;
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
