namespace RJCP.CodeQuality.IO
{
    using System;
    using System.IO;

    /// <summary>
    /// Sets the mode for the stream.
    /// </summary>
    [Flags]
    public enum StreamMode
    {
        /// <summary>
        /// No operations are allowed.
        /// </summary>
        None = 0,

        /// <summary>
        /// The stream is readable, so that <see cref="Stream.CanRead"/> is <see langword="true"/>.
        /// </summary>
        Read = 1,

        /// <summary>
        /// The stream is readable, so that <see cref="Stream.CanWrite"/> is <see langword="true"/>.
        /// </summary>
        Write = 2,

        /// <summary>
        /// The stream is readable, so that <see cref="Stream.CanSeek"/> is <see langword="true"/>.
        /// </summary>
        Seek = 4,

        /// <summary>
        /// All modes for reading and writing are active, but no timeouts.
        /// </summary>
        ReadWrite = Read + Write + Seek,

        /// <summary>
        /// The stream is readable, so that <see cref="Stream.CanTimeout"/> is <see langword="true"/>.
        /// </summary>
        Timeout = 8,

        /// <summary>
        /// All modes are active.
        /// </summary>
        All = Read + Write + Seek + Timeout
    }
}
