namespace RJCP.CodeQuality.IO
{
    using System;
    using System.IO;

#if NETSTANDARD || NET462_OR_GREATER
    using System.Threading.Tasks;
#endif

#if NETFRAMEWORK
    using System.Linq;
#endif

    /// <summary>
    /// Extensions for streams.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Reads the stream contents into an array.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="stream"/> is not seekable.
        /// <para>- or -</para>
        /// <paramref name="stream"/> is not readable.
        /// <para>- or -</para>
        /// <paramref name="stream"/> length too long.
        /// </exception>
        /// <returns>An array containing the contents of the stream.</returns>
        /// <remarks>
        /// This method allocates a memory buffer of the stream length. If while reading the <see
        /// cref="Stream.Read(byte[], int, int)"/> returns zero, a subset of the array is returned (which in .NET
        /// Framework requires a copy, so up to twice the memory would be needed).
        /// </remarks>
        public static byte[] ReadStream(this Stream stream)
        {
            if (!stream.CanRead) throw new InvalidOperationException("Stream must be readable");
            if (!stream.CanSeek) throw new InvalidOperationException("Stream must be seekable");
            if (stream.Length > int.MaxValue) throw new InvalidOperationException("Stream length too long");

            if (stream is MemoryStream mstream) {
                return mstream.ToArray();
            }

            int streamLen = (int)stream.Length;
            byte[] buffer = new byte[streamLen];
            stream.Seek(0, SeekOrigin.Begin);
            int pos = 0;

#if NETSTANDARD
            Span<byte> memBuff = buffer.AsSpan();
            while (pos < streamLen) {
                int read = stream.Read(memBuff[pos..]);
                if (read == 0) return memBuff[0..pos].ToArray();
                pos += read;
            }
#else
            while (pos < streamLen) {
                int read = stream.Read(buffer, pos, streamLen - pos);
                if (read == 0) return buffer.Take(pos).ToArray();
                pos += read;
            }
#endif
            return buffer;
        }

#if NETSTANDARD || NET462_OR_GREATER
        /// <summary>
        /// Reads the stream contents into an array.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="stream"/> is not seekable.
        /// <para>- or -</para>
        /// <paramref name="stream"/> is not readable.
        /// <para>- or -</para>
        /// <paramref name="stream"/> length too long.
        /// </exception>
        /// <returns>An array containing the contents of the stream.</returns>
        /// <remarks>
        /// This method allocates a memory buffer of the stream length. If while reading the <see
        /// cref="Stream.ReadAsync(byte[], int, int)"/> returns zero, a subset of the array is returned (which in .NET
        /// Framework requires a copy, so up to twice the memory would be needed).
        /// </remarks>
        public static async Task<byte[]> ReadStreamAsync(this Stream stream)
        {
            if (!stream.CanRead) throw new InvalidOperationException("Stream must be readable");
            if (!stream.CanSeek) throw new InvalidOperationException("Stream must be seekable");
            if (stream.Length > int.MaxValue) throw new InvalidOperationException("Stream length too long");

            if (stream is MemoryStream mstream) {
                return mstream.ToArray();
            }

            int streamLen = (int)stream.Length;
            byte[] buffer = new byte[streamLen];
            stream.Seek(0, SeekOrigin.Begin);
            int pos = 0;

#if NETSTANDARD
            Memory<byte> memBuff = buffer.AsMemory();
            while (pos < streamLen) {
                int read = await stream.ReadAsync(memBuff[pos..]);
                if (read == 0) return memBuff[0..pos].ToArray();
                pos += read;
            }
#else
            while (pos < streamLen) {
                int read = await stream.ReadAsync(buffer, pos, streamLen - pos);
                if (read == 0) return buffer.Take(pos).ToArray();
                pos += read;
            }
#endif
            return buffer;
        }
#endif
    }
}
