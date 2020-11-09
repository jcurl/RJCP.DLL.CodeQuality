namespace RJCP.CodeQuality.NUnitExtensions
{
    using System;

    /// <summary>
    /// List of options that can be given to the <see cref="ScratchPad"/> class.
    /// </summary>
    [Flags]
    public enum ScratchOptions
    {
        /// <summary>
        /// No specific options, use the default.
        /// </summary>
        None = 0,

        /// <summary>
        /// Set the current directory to be the newly created scratch directory (Default setting when using
        /// <see cref="None"/>).
        /// </summary>
        UseScratchDir = 0,

        /// <summary>
        /// Keep the current directory, do not change it.
        /// </summary>
        KeepCurrentDir = 1,

        /// <summary>
        /// Set the current directory to be the test deploy directory.
        /// </summary>
        UseDeployDir = 2,

        /// <summary>
        /// Create the scratch pad directory (Default setting when using <see cref="None"/>). If it exists, then remove
        /// the contents of the directory first.
        /// </summary>
        CreateScratch = 0,

        /// <summary>
        /// Don't create the scratch pad directory. The path is still generated if the user wishes to create the
        /// directory themselves. Combine with <see cref="UseDeployDir"/> to change the directory. The usage of
        /// <see cref="UseScratchDir"/> is ignored.
        /// </summary>
        NoScratch = 0x10,

        /// <summary>
        /// Create the scratch pad directory only if it doesn't exist. This keeps the contents of the directory if it
        /// existed prior.
        /// </summary>
        CreateOnMissing = 0x20
    }
}
