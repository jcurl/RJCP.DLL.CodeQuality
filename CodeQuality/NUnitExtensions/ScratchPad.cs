namespace RJCP.CodeQuality.NUnitExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// A scratch pad for a temporary directory location based on the test name.
    /// </summary>
    /// <remarks>
    /// The <see cref="ScratchPad"/> can simplify integration tests using the file system, by managing the current
    /// directory, and providing a folder in the <see cref="Deploy.WorkDirectory"/> where a test case can execute. Hence
    /// the name <c>ScratchPad</c> as it is a temporary place where a test case can prepare its test files and execute.
    /// <para>
    /// The <see cref="ScratchPad"/> is an <see cref="IDisposable"/> object, which when disposed, restores the test case
    /// settings modified by the <see cref="ScratchPad"/> while keeping the results of the files written. In C#, it is
    /// then most convenient in conjunction with the <c>using</c> statement which can reduce the amount of boilerplate
    /// code needed to write a test case significantly (up to 50% or more for simple test cases).
    /// </para>
    /// <para>
    /// To create a temporary directory based on the name of the test case, and execute code from within that
    /// directory, create an instance using <see cref="ScratchPad()"/> with the default options:
    /// <code language="csharp"><![CDATA[
    /// [Test]
    /// public void MyTestCase() {
    ///   using (Deploy.ScratchPad()) {
    ///     MyObject o = new MyObject();
    ///     o.Save("relative.txt");
    ///   }
    /// }
    /// ]]></code>
    /// This sample code creates a directory called <c>MyTestCase</c> with in <see cref="Deploy.WorkDirectory"/> (based
    /// on the test name of the current test case, obtained via reflection from
    /// <c>NUnit.Framework.TestContext.CurrentContext.Test.Name</c>.
    /// </para>
    /// <para>
    /// If your test case should not change the current directory, or the test case will set the
    /// <see cref="Environment.CurrentDirectory"/> itself to something different, but still need to make a folder
    /// within the <see cref="Deploy.WorkDirectory"/>, use the constructor <see cref="ScratchPad(ScratchOptions)"/>
    /// with the option <see cref="ScratchOptions.KeepCurrentDir"/>.
    /// <code language="csharp"><![CDATA[
    /// [Test]
    /// public void MyTestCase() {
    ///   using (ScratchPad scratch = Deploy.ScratchPad(ScratchOptions.KeepCurrentDir)) {
    ///     MyObject o = new MyObject();
    ///     string path = Path.Combine(scratch.Path, "absolute.txt");
    ///     o.Save(path);
    ///   }
    /// }
    /// ]]></code>
    /// </para>
    /// <para>
    /// Similarly, you can still create the <see cref="ScratchPad"/> and set the directory to
    /// <see cref="Deploy.WorkDirectory"/> immediately using the <see cref="ScratchOptions.UseDeployDir"/> option.
    /// The property <see cref="Path"/> will still be the full path of the scratch pad.
    /// <code language="csharp"><![CDATA[
    /// [Test]
    /// public void MyTestCase() {
    ///   using (ScratchPad scratch = Deploy.ScratchPad(ScratchOptions.UseDeployDir)) {
    ///     MyObject o = new MyObject();
    ///     string path = Path.Combine(scratch.Path, "absolute.txt");
    ///     o.Save(path);
    ///   }
    /// }
    /// ]]></code>
    /// </para>
    /// <para>
    /// The <see cref="ScratchPad"/> will always restore the current directory when <see cref="ScratchPad.Dispose()"/>
    /// is called, which makes it ideal to use with the <c>using</c> statement. If you don't need to create a scratch
    /// directory, you can use the option <see cref="ScratchOptions.NoScratch"/> (the default is
    /// <see cref="ScratchOptions.CreateScratch"/>). Then you can change the <see cref="Environment.CurrentDirectory"/>
    /// within the test case, or you can still set the directory to the <see cref="Deploy.WorkDirectory"/> by combining
    /// with the option <see cref="ScratchOptions.UseDeployDir"/> (the option <see cref="ScratchOptions.UseScratchDir"/>
    /// will be ignored).
    /// <code language="csharp"><![CDATA[
    /// [Test]
    /// public void MyTestCase() {
    ///   using (ScratchPad scratch = Deploy.ScratchPad(ScratchOptions.NoScratch)) {
    ///     Environment.CurrentDirectory = Deploy.TestDirectory;
    ///     MyObject o = new MyObject();
    ///     o.Load("relativefile.txt");
    ///   }
    /// }
    /// ]]></code>
    /// </para>
    /// </remarks>
    public class ScratchPad : IDisposable
    {
        // The flags are made up of bitfield for options
        //
        //   xxx 0000 - UseScratchDir
        //   xxx 0001 - KeepCurrentDir
        //   xxx 0010 - UseDeployDir
        //
        //   000 xxxx - CreateScratch
        //   001 xxxx - NoScratch
        //   010 xxxx - CreateOnMissing
        private const ScratchOptions ScratchChDirMask = (ScratchOptions)0x0F;
        private const ScratchOptions ScratchMkDirMask = (ScratchOptions)0x70;

        private static readonly Dictionary<string, string> s_NameMapping = new();
        private static readonly HashSet<string> s_NamesUsed = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ScratchPad"/> class.
        /// </summary>
        /// <remarks>
        /// If another test case has the same test name, a new name will be generated, based on the full name of the test
        /// case.
        /// </remarks>
        public ScratchPad() : this(ScratchOptions.None) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScratchPad"/> class.
        /// </summary>
        /// <param name="options">Configure the options for the <see cref="ScratchPad"/>.</param>
        /// <remarks>
        /// If another test case has the same test name, a new name will be generated, based on the full name of the test
        /// case.
        /// </remarks>
        public ScratchPad(ScratchOptions options)
        {
            string testFullName = Deploy.TestFullName;
            string testName = SanitizeName(Deploy.TestName);

            string dirName;
            if (s_NamesUsed.Add(testName)) {
                s_NameMapping.Add(testFullName, testName);
                dirName = testName;
            } else {
                if (!s_NameMapping.TryGetValue(testFullName, out dirName)) {
                    dirName = string.Format("{0}-{1:X8}", testName, testFullName.GetHashCode());
                    s_NameMapping.Add(testFullName, dirName);
                }
            }

            Initialize(dirName, options);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScratchPad"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/> is empty.</exception>
        public ScratchPad(string name) : this(name, ScratchOptions.None) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScratchPad"/> class.
        /// </summary>
        /// <param name="name">The name. It must be suitable to create a folder (it's not checked for correctness).</param>
        /// <param name="options">Configure the options for the <see cref="ScratchPad"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/> is empty.</exception>
        public ScratchPad(string name, ScratchOptions options)
        {
            ThrowHelper.ThrowIfNull(name);
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Name is empty", nameof(name));
            Initialize(name, options);
        }

        private void Initialize(string name, ScratchOptions options)
        {
            RelativePath = name;
            Path = System.IO.Path.Combine(Deploy.WorkDirectory, RelativePath);

            m_OriginalCurrentDir = Environment.CurrentDirectory;
            CreateScratchPad(RelativePath, options);
        }

        /// <summary>
        /// Gets the path of the scratch area, relative to <see cref="Deploy.WorkDirectory"/>.
        /// </summary>
        /// <value>The path of the scratch area, relative to <see cref="Deploy.WorkDirectory"/>.</value>
        public string RelativePath { get; private set; }

        /// <summary>
        /// Gets the absolute path of the scratch area.
        /// </summary>
        /// <value>The absolute path of the scratch area.</value>
        public string Path { get; private set; }

        private static string SanitizeName(string name)
        {
            if (name is null) return string.Empty;

            int nameLength = name.Length;
            StringBuilder sb = null;

            int pos = 0;
            int endPos = -1;
            for (int i = 0; i < nameLength; i++) {
                if (IsValidChar(name[i])) continue;
                sb ??= new StringBuilder(nameLength);
                int l = i - pos;
                if (l > 0) {
                    sb.Append(name, pos, l);
                    endPos = pos + l;
                    sb.Append('_');
                }
                pos = i + 1;
            }
            if (sb is null) return name;

            if (endPos != -1) return sb.ToString(0, endPos);
            return sb.ToString();
        }

        private static bool IsValidChar(char ch)
        {
            return (ch is >= 'a' and <= 'z' or
                >= 'A' and <= 'Z' or
                >= '0' and <= '9' or
                '_' or
                '.');
        }

        private string m_OriginalCurrentDir;

        private static void CreateScratchPad(string dirName, ScratchOptions options)
        {
            ScratchOptions chdirOptions = options & ScratchChDirMask;
            ScratchOptions mkdirOptions = options & ScratchMkDirMask;
            switch (mkdirOptions) {
            case ScratchOptions.CreateScratch:
                Deploy.DeleteDirectory(dirName);
                Deploy.CreateDirectory(dirName);
                SetScratchPadDir(dirName, chdirOptions);
                break;
            case ScratchOptions.NoScratch:
                if (chdirOptions == ScratchOptions.UseDeployDir) {
                    // We ignore the chdirOptions here, as there is no scratch directory
                    Environment.CurrentDirectory = Deploy.WorkDirectory;
                }
                break;
            case ScratchOptions.CreateOnMissing:
                if (!System.IO.Directory.Exists(dirName)) {
                    if (System.IO.File.Exists(dirName)) {
                        Deploy.DeleteDirectory(dirName);
                    }
                    Deploy.CreateDirectory(dirName);
                }
                SetScratchPadDir(dirName, chdirOptions);
                break;
            }
        }

        private static void SetScratchPadDir(string dirName, ScratchOptions chdirOptions)
        {
            switch (chdirOptions) {
            case ScratchOptions.UseScratchDir:
                Environment.CurrentDirectory = System.IO.Path.Combine(Deploy.WorkDirectory, dirName);
                break;
            case ScratchOptions.KeepCurrentDir:
                break;
            case ScratchOptions.UseDeployDir:
                Environment.CurrentDirectory = Deploy.WorkDirectory;
                break;
            default:
                break;
            }
        }

        private void Restore()
        {
            Environment.CurrentDirectory = m_OriginalCurrentDir;
        }

        /// <summary>
        /// Copy a file or directory to the scratch pad directory.
        /// </summary>
        /// <param name="path">
        /// The relative or absolute path to the file or directory to deploy. The path is relative to the build output
        /// directory.
        /// </param>
        /// <exception cref="System.IO.FileNotFoundException">
        /// A file being deployed isn't found in the source.
        /// </exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        /// A source directory could not be found (note, destination directories are created for you, but may occur if
        /// there was a file system error).
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The source file or destination path has an access violation.
        /// </exception>
        /// <exception cref="System.IO.PathTooLongException">
        /// The source path or destination path name is too long.
        /// </exception>
        /// <exception cref="NotSupportedException">Path names have unsupported or invalid characters.</exception>
        /// <exception cref="ArgumentException">Given path doesn't exist.</exception>
        /// <exception cref="System.IO.IOException">A network error has occurred.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method is a wrapper for <see cref="Deploy.Item(string)"/> with the destination directory relative to
        /// <see cref="Path"/>.
        /// <para>
        /// Files are copied from the <paramref name="path"/> relative to the <see cref="Deploy.TestDirectory"/>, to the
        /// <see cref="Path"/>.
        /// </para>
        /// <para>
        /// If you have two different files that are being deployed to the same location with the same name, the results
        /// are undefined. Don't do it. Your test cases may pass or fail depending on the position of the moon, or if
        /// your cat just sneezed a few minutes ago. The same applies if your deploying two different directories to the
        /// same location that have different content but where filenames overlap.
        /// </para>
        /// </remarks>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        /// There is a configuration error in the applications configuration file. Check the description provided in the
        /// <see cref="System.Configuration.ConfigurationErrorsException"/> on exactly what went wrong.
        /// </exception>
        public void DeployItem(string path)
        {
            Deploy.Item(path, RelativePath);
        }

        /// <summary>
        /// Copy a file or directory to a subdirectory in the scratch pad directory.
        /// </summary>
        /// <param name="path">
        /// The relative or absolute path to the file or directory to deploy. The path is relative to the build output
        /// directory.
        /// </param>
        /// <param name="outputDirectory">
        /// The path of the directory to which the items are to be copied. It can be either absolute or relative to the
        /// deployment directory. If it's relative, it's copied relative to <see cref="RelativePath"/>.
        /// </param>
        /// <exception cref="System.IO.FileNotFoundException">
        /// A file being deployed isn't found in the source.
        /// </exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        /// A source directory could not be found (note, destination directories are created for you, but may occur if
        /// there was a file system error).
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The source file or destination path has an access violation.
        /// </exception>
        /// <exception cref="System.IO.PathTooLongException">
        /// The source path or destination path name is too long.
        /// </exception>
        /// <exception cref="NotSupportedException">Path names have unsupported or invalid characters.</exception>
        /// <exception cref="ArgumentException">Given path doesn't exist.</exception>
        /// <exception cref="System.IO.IOException">A network error has occurred.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method is a wrapper for <see cref="Deploy.Item(string)"/> with the destination directory relative to
        /// <see cref="Path"/>.
        /// <para>
        /// Files are copied from the <paramref name="path"/> relative to the <see cref="Deploy.TestDirectory"/>, to the
        /// <paramref name="outputDirectory"/> relative to <see cref="Path"/>.
        /// </para>
        /// <para>
        /// When files and folders are copied to the output directory, they are <i>merged</i> with the content of the
        /// folder that may already exist.
        /// </para>
        /// <para>
        /// When using this method, ensure that two test cases don't clobber each other. This might occur if you try to
        /// do strange things such as deploying directory A/B in one test case and then deploying the parent directory A
        /// in a different test case.
        /// </para>
        /// <para>
        /// Before the file is deployed, it is checked if the copy has already been done by comparing the length, the
        /// create time stamp and modify time stamp. If any of these differ, the file is copied. If they're all the
        /// same, no copy occurs. This makes the copy as fast as possible and ensures also that files can't be copied on
        /// top of themselves.
        /// </para>
        /// <para>
        /// If you have two different files that are being deployed to the same location with the same name, the results
        /// are undefined. Don't do it. Your test cases may pass or fail depending on the position of the moon, or if
        /// your cat just sneezed a few minutes ago. The same applies if your deploying two different directories to the
        /// same location that have different content but where filenames overlap.
        /// </para>
        /// </remarks>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        /// There is a configuration error in the applications configuration file. Check the description provided in the
        /// <see cref="System.Configuration.ConfigurationErrorsException"/> on exactly what went wrong.
        /// </exception>
        public void DeployItem(string path, string outputDirectory)
        {
            string newPath = System.IO.Path.Combine(RelativePath, outputDirectory);
            Deploy.Item(path, newPath);
        }

        /// <summary>
        /// Deploys the empty file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is empty.</exception>
        public void DeployEmptyFile(string path)
        {
            ThrowHelper.ThrowIfNull(path);
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("Empty file name", nameof(path));

            string newPath = System.IO.Path.Combine(RelativePath, path);
            Deploy.EmptyFile(newPath);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed and unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release
        /// only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) {
                Restore();
            }
        }
    }
}
