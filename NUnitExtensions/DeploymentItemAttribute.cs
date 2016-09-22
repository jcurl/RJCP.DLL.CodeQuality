namespace NUnit.Framework
{
    using System;
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
    /// the create time stamp and modify time stamp. If any of these differ, the file is copied. If they're all the same,
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
            Deploy.Item(path, outputDirectory);
        }
    }
}
