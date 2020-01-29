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
        /// The use scratch directory for the current directory (Default setting when using <see cref="None"/>).
        /// </summary>
        UseScratchDir = 0,

        /// <summary>
        /// The keep current directory, do not change it.
        /// </summary>
        KeepCurrentDir = 1,

        /// <summary>
        /// The use deploy directory for the current directory.
        /// </summary>
        UseDeployDir = 2,

        /// <summary>
        /// Create the scratch pad directory (Default setting when using <see cref="None"/>).
        /// </summary>
        CreateScratch = 0,

        /// <summary>
        /// Don't create the scratch pad directory. Combine with <see cref="UseDeployDir"/> to change the directory.
        /// </summary>
        NoScratch = 0x10
    }
}
