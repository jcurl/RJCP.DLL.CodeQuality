namespace NUnit.Framework
{
    using System;

    /// <summary>
    /// Class NUnitExtensions Base Class.
    /// </summary>
    /// <remarks>
    /// You can derive from the base class <see cref="NUnitExtensions"/> to gain extra functionality in
    /// addition to the standard functionality from NUnit 2.6.4.
    /// <para><b>Automation Deployment of Files:</b></para>
    /// <para>When you derive from this class, it implements a <see cref="NUnit.Framework.TestFixtureSetUpAttribute"/>
    /// that will automatically deploy files that are decorated with the <see cref="DeploymentItemAttribute"/>. You
    /// should still decorate your classes with the <see cref="NUnit.Framework.TestFixtureAttribute"/>.</para>
    /// <code language="csharp">
    /// [TestFixture(Category = "NUnitExtensions.Deployment.BaseClass")]
    /// public class NUnitExtensionsBaseClassTest : NUnitExtensions
    /// {
    ///     [Test]
    ///     [DeploymentItem("Resources/test1.txt", "BaseClassResources")]
    ///     public void DeploySingleFile()
    ///     {
    ///         Assert.That(File.Exists(Path.Combine("BaseClassResources", "test1.txt")), "File 'folder/Resources/test1.txt' not found");
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public abstract class NUnitExtensions
    {
        /// <summary>
        /// Executes the test fixture set up.
        /// </summary>
        /// <remarks>
        /// When implementing your own test case, you will want to override this method and then call the base
        /// if you have extra work to do during the test case fixture set up.
        /// <para>If an exception occurs in this method (during the deployment), NUnit will likely complain
        /// about an exception in the <see cref="TestFixtureSetUpAttribute"/>. Please look at the console output
        /// to identify the error.</para>
        /// </remarks>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Deploy.ItemsWithAttribute(this);
        }
    }
}
