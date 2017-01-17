namespace NUnit.Framework
{
    using System;

    /// <summary>
    /// Class DeploymentItemAttribute.
    /// </summary>
    /// <remarks>
    /// This attribute emulates the Microsoft Test DeploymentItemAttribute for NUnit fixtures. The attribute
    /// alone does not deploy any of the files given. To actually deploy all the files before any of the test
    /// cases in your fixture are executed, you should decorate your test methods with the DeploymentItemAttribute,
    /// then in the TestFixtureSetup, execute a method to cause NUnit to do the deployment.
    /// <para>See the example for a template on how to implement deployments. See the documentation for the
    /// individual methods on how the deployment works.</para>
    ///
    /// <code language="csharp">
    /// [TestFixture]
    /// public class NUnitExtensionsTest {
    ///     [TestFixtureSetUp]
    ///     public void SetUpFixture() {
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
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public class DeploymentItemAttribute : Attribute
    {
        /// <summary>
        /// NUnit replacement for Microsoft.VisualStudio.TestTools.UnitTesting.DeploymentItemAttribute
        /// Marks a method to be relevant for a unit-test and copies it to deployment-directory for
        /// this unit-test.
        /// </summary>
        /// <param name="path">The relative or absolute path to the file or directory to deploy.
        /// The path is relative to the build output directory. If the path is a directory, that directory
        /// is also deployed into the current directory.</param>
        /// <remarks>
        /// Marks the method as containing files to deploy, that will be deployed with a call to
        /// <seealso cref="Deploy.ItemsWithAttribute(object)"/>.
        /// </remarks>
        public DeploymentItemAttribute(string path) : this(path, null) { }

        /// <summary>
        /// NUnit replacement for Microsoft.VisualStudio.TestTools.UnitTesting.DeploymentItemAttribute
        /// Marks a method to be relevant for a unit-test and copies it to deployment-directory for this unit-test.
        /// </summary>
        /// <param name="path">The relative or absolute path to the file or directory to deploy.
        /// The path is relative to the build output directory. If the path is a directory, that directory
        /// is also deployed into the <paramref name="outputDirectory"/>. If you want to deploy everything
        /// within the directory, but not the directory itself, append a directory slash path to this
        /// path parameter.</param>
        /// <param name="outputDirectory">The path of the directory to which the items are to be copied.
        /// It can be either absolute or relative to the deployment directory.</param>
        /// <remarks>
        /// Marks the method as containing files to deploy, that will be deployed with a call to
        /// <seealso cref="Deploy.ItemsWithAttribute(object)"/>.
        /// </remarks>
        public DeploymentItemAttribute(string path, string outputDirectory)
        {
            Path = path;
            OutputDirectory = outputDirectory;
        }

        /// <summary>
        /// Gets the path of the file or directory to deploy.
        /// </summary>
        /// <value>The path of the file or directory to deploy.</value>
        public string Path { get; private set; }

        /// <summary>
        /// Gets the output directory where to deploy to.
        /// </summary>
        /// <value>The output directory where to deploy to.</value>
        public string OutputDirectory { get; private set; }
    }
}
