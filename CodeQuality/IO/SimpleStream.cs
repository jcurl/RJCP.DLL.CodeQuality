namespace RJCP.CodeQuality.IO
{
    using System;
    using System.IO;
    using System.Threading;

#if NETSTANDARD || NET462_OR_GREATER
    using System.Threading.Tasks;
#endif

    /// <summary>
    /// A very simple stream that does almost nothing, that is also easy to mock.
    /// </summary>
    public class SimpleStream : Stream
    {
        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <value><see langword="true"/> if this instance can read; otherwise, <see langword="false"/>.</value>
        public override bool CanRead { get { return true; } }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <value><see langword="true"/> if this instance can seek; otherwise, <see langword="false"/>.</value>
        public override bool CanSeek { get { return true; } }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <value><see langword="true"/> if this instance can write; otherwise, <see langword="false"/>.</value>
        public override bool CanWrite { get { return true; } }

        /// <summary>
        /// Gets a value that determines whether the current stream can time out.
        /// </summary>
        /// <value><see langword="true"/> if this instance can timeout; otherwise, <see langword="false"/>.</value>
        /// <remarks>
        /// This stream supports timeouts.
        /// </remarks>
        public override bool CanTimeout { get { return true; } }

        /// <summary>
        /// Gets or sets a value, in milliseconds, that determines how long the stream will attempt to write before
        /// timing out.
        /// </summary>
        /// <value>The write timeout.</value>
        public override int WriteTimeout { get; set; } = Timeout.Infinite;

        /// <summary>
        /// Gets or sets a value, in milliseconds, that determines how long the stream will attempt to read before
        /// timing out.
        /// </summary>
        /// <value>The read timeout.</value>
        public override int ReadTimeout { get; set; } = Timeout.Infinite;

        private long m_Length;

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        /// <value>The length of the stream, or zero if no stream was provided in the constructor.</value>
        public override long Length { get { return m_Length; } }

        private long m_Position;

        /// <summary>
        /// Gets or sets the position of the stream.
        /// </summary>
        /// <value>The position of the stream.</value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Setting the position of the stream would exceed the length of the stream.
        /// </exception>
        /// <remarks>
        /// When updating the position, if it exceeds the current length, would extend the length of the stream.
        /// </remarks>
        public override long Position
        {
            get { return m_Position; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                m_Position = value;
                if (m_Position > m_Length) m_Length = m_Position;
            }
        }

        /// <summary>
        /// Flushes the stream.
        /// </summary>
        /// <remarks>Flushing the stream has no operation.</remarks>
        public override void Flush()
        {
            /* Nothing to do */
        }

#if NETSTANDARD || NET462_OR_GREATER
        /// <summary>
        /// Clears all buffers asynchronously for this stream and causes any buffered data to be written to the
        /// underlying device.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <returns>Task.</returns>
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
#endif

        /// <summary>
        /// Reads the specified buffer from the stream.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the
        /// current stream.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current
        /// stream.
        /// </param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <returns>The amount of bytes read.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> may not be negative;
        /// <para>- or -</para>
        /// &gt; <paramref name="count"/> may not be negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="offset"/> and <paramref name="count"/> would exceed the boundaries of the array.
        /// </exception>
        /// <remarks>Updates the position, without actually writing anything.</remarks>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), "may not be negative");
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "may not be negative");
            if (offset > buffer.Length - count) throw new ArgumentException("The offset and count would exceed the boundaries of the array");

            return ReadInternal(count);
        }

#if NETSTANDARD
        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number
        /// of bytes read.
        /// </summary>
        /// <param name="buffer">The buffer to read into.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that
        /// many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <remarks>Updates the position, without actually writing anything.</remarks>
        public override int Read(Span<byte> buffer)
        {
            return ReadInternal(buffer.Length);
        }
#endif

        private int ReadInternal(int count)
        {
            int maxRead;
            if (m_Length - m_Position > int.MaxValue) {
                maxRead = count;
            } else {
                maxRead = Math.Min(count, (int)(m_Length - m_Position));
            }

            m_Position += maxRead;
            return maxRead;
        }

#if NETSTANDARD || NET462_OR_GREATER
        /// <summary>
        /// Reads a sequence of bytes asynchronously from the current stream and advances the position within the stream
        /// by the number of bytes read.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. When this method returns, the buffer contains the specified byte array with the values
        /// between <paramref name="offset"/> and ( <paramref name="offset"/> + <paramref name="count"/> - 1) replaced
        /// by the bytes read from the current source.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the
        /// current stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <param name="cancellationToken">
        /// The cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that
        /// many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> may not be negative;
        /// <para>- or -</para>
        /// &gt; <paramref name="count"/> may not be negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="offset"/> and <paramref name="count"/> would exceed the boundaries of the array.
        /// </exception>
        /// <exception cref="OperationCanceledException">The Operation has been cancelled.</exception>
        /// <remarks>Updates the position, without actually writing anything.</remarks>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), "may not be negative");
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "may not be negative");
            if (offset > buffer.Length - count) throw new ArgumentException("The offset and count would exceed the boundaries of the array");
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(ReadInternal(count));
        }
#endif

#if NETSTANDARD
        /// <summary>
        /// Reads a sequence of bytes asynchronously from the current stream and advances the position within the stream
        /// by the number of bytes read.
        /// </summary>
        /// <param name="buffer">The buffer to read into.</param>
        /// <param name="cancellationToken">
        /// The cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that
        /// many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="OperationCanceledException">The Operation has been cancelled.</exception>
        /// <remarks>Updates the position, without actually writing anything.</remarks>
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return new ValueTask<int>(ReadInternal(buffer.Length));
        }
#endif

        /// <summary>
        /// Reads a byte from the stream and advances the position within the stream by one byte, or returns -1 if at
        /// the end of the stream.
        /// </summary>
        /// <returns>The unsigned byte cast to an Int32, or -1 if at the end of the stream.</returns>
        public override int ReadByte()
        {
            int read = ReadInternal(1);
            return read == 0 ? -1 : 0;
        }

        /// <summary>
        /// Begins an asynchronous read operation.
        /// </summary>
        /// <param name="buffer">The buffer to read data to.</param>
        /// <param name="offset">The byte offset in <paramref name="buffer"/> from which to begin reading.</param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <param name="callback">An optional asynchronous callback, to be called when the write is complete.</param>
        /// <param name="state">
        /// A user-provided object that distinguishes this particular asynchronous write request from other requests.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> may not be negative;
        /// <para>- or -</para>
        /// &gt; <paramref name="count"/> may not be negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="offset"/> and <paramref name="count"/> would exceed the boundaries of the array.
        /// </exception>
        /// <returns>An IAsyncResult that represents the asynchronous write, which could still be pending.</returns>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), "may not be negative");
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "may not be negative");
            if (offset > buffer.Length - count) throw new ArgumentException("The offset and count would exceed the boundaries of the array");

            IAsyncResult result = new CompletedAsync<int>(state, ReadInternal(count));
            if (callback != null) callback(result);
            return result;
        }

        /// <summary>
        /// Ends an asynchronous write operation.
        /// </summary>
        /// <param name="asyncResult">A reference to the outstanding asynchronous I/O request.</param>
        public override int EndRead(IAsyncResult asyncResult)
        {
            if (asyncResult == null) throw new ArgumentNullException(nameof(asyncResult));
            if (!(asyncResult is CompletedAsync<int> readAsync))
                throw new ArgumentException("Invalid async result", nameof(asyncResult));
            CompletedAsync.End(asyncResult);
            return readAsync.Result;
        }

#if NETSTANDARD
        /// <summary>
        /// Reads all the bytes from the current stream and writes them to a destination stream, using a specified
        /// buffer size.
        /// </summary>
        /// <param name="destination">The stream that will contain the contents of the current stream.</param>
        /// <param name="bufferSize">
        /// The size of the buffer. This value must be greater than zero. The default size is 4096.
        /// </param>
        /// <remarks>
        /// Copies all zeroes to the <paramref name="destination"/> stream for the stream <see cref="Length"/>.
        /// </remarks>
        public override void CopyTo(Stream destination, int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];
            int written = 0;
            while (written < m_Length) {
                int write = (int)Math.Min(bufferSize, m_Length - written);
                destination.Write(buffer, 0, write);
                written += write;
            }
        }

        /// <summary>
        /// Reads all the bytes asynchronously from the current stream and writes them to a destination stream, using a
        /// specified buffer size.
        /// </summary>
        /// <param name="destination">The stream that will contain the contents of the current stream.</param>
        /// <param name="bufferSize">
        /// The size of the buffer. This value must be greater than zero. The default size is 4096.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <returns>
        /// The number of bytes read from the stream, between zero (0) and the number of bytes you requested. Streams
        /// return zero (0) only at the end of the stream, otherwise, they should block until at least one byte is
        /// available.
        /// </returns>
        /// <remarks>
        /// Copies all zeroes to the <paramref name="destination"/> stream for the stream <see cref="Length"/>.
        /// </remarks>
        public override async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            ReadOnlyMemory<byte> buffer = new byte[bufferSize];
            int written = 0;
            while (written < m_Length) {
                int write = (int)Math.Min(bufferSize, m_Length - written);
                await destination.WriteAsync(buffer[0..write], cancellationToken);
                written += write;
            }
        }
#endif

        /// <summary>
        /// Seeks to the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="origin">The origin.</param>
        /// <returns>The new position in the stream.</returns>
        /// <exception cref="ArgumentException"><paramref name="offset"/> would exceed beyond end of file;</exception>
        /// <exception cref="ArgumentOutOfRangeException">Unknown seek <paramref name="origin"/>.</exception>
        /// <exception cref="IOException"><paramref name="offset"/> would exceed beyond beginning of file;</exception>
        /// <remarks>Updates the position of the stream.</remarks>
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin) {
            case SeekOrigin.Begin:
                m_Position = offset;
                if (m_Position > m_Length) m_Length = m_Position;
                break;
            case SeekOrigin.Current:
                if (long.MaxValue - m_Position < offset) {
                    throw new IOException("Offset would exceed beyond end of file");
                }
                m_Position += offset;
                if (m_Position > m_Length) m_Length = m_Position;
                break;
            case SeekOrigin.End:
                if (m_Length < offset) {
                    throw new ArgumentException("Offset would exceed beyond beginning of file", nameof(offset));
                }
                m_Position = m_Length - offset;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(origin), "Unknown seek origin");
            }
            return m_Position;
        }

        /// <summary>
        /// Sets the length of the stream.
        /// </summary>
        /// <param name="value">The new length of the stream.</param>
        /// <remarks>Updates the length of the stream. If shortened, the <see cref="Position"/> is updated.</remarks>
        public override void SetLength(long value)
        {
            m_Length = value;
            if (m_Position > m_Length) m_Position = m_Length;
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the
        /// number of bytes written.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the
        /// current stream.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current
        /// stream.
        /// </param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> may not be negative;
        /// <para>- or -</para>
        /// &gt; <paramref name="count"/> may not be negative.
        /// </exception>
        /// <exception cref="IOException">
        /// The <paramref name="offset"/> and <paramref name="count"/> would exceed the boundaries of the array.
        /// </exception>
        /// <remarks>Updates the position, without actually writing anything.</remarks>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), "may not be negative");
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "may not be negative");
            if (offset > buffer.Length - count) throw new ArgumentException("The offset and count would exceed the boundaries of the array");

            WriteInternal(count);
        }

        private void WriteInternal(int count)
        {
            if (long.MaxValue - m_Position < count)
                throw new IOException("Writing to the stream would exceed maximum length");

            m_Position += count;
            if (m_Position > m_Length) m_Length = m_Position;
        }

#if NETSTANDARD
        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the
        /// number of bytes written.
        /// </summary>
        /// <param name="buffer">Writes the bytes to the current stream.</param>
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            WriteInternal(buffer.Length);
        }
#endif

#if NETSTANDARD || NET462_OR_GREATER
        /// <summary>
        /// Writes a sequence of bytes asynchronously to the current stream and advances the current position within
        /// this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the
        /// current stream.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current
        /// stream.
        /// </param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <param name="cancellationToken">
        /// The cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> may not be negative;
        /// <para>- or -</para>
        /// &gt; <paramref name="count"/> may not be negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="offset"/> and <paramref name="count"/> would exceed the boundaries of the array.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// The <paramref name="cancellationToken"/> was cancelled.
        /// </exception>
        /// <returns>A task indicating when the write operation is complete.</returns>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), "may not be negative");
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "may not be negative");
            if (offset > buffer.Length - count) throw new ArgumentException("The offset and count would exceed the boundaries of the array");
            cancellationToken.ThrowIfCancellationRequested();

            WriteInternal(count);
            return Task.CompletedTask;
        }
#endif

#if NETSTANDARD
        /// <summary>
        /// Writes a sequence of bytes asynchronously to the current stream and advances the current position within
        /// this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">Writes the bytes to the current stream.</param>
        /// <param name="cancellationToken">
        /// The cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <exception cref="OperationCanceledException">
        /// The <paramref name="cancellationToken"/> was cancelled.
        /// </exception>
        /// <returns>A <see cref="ValueTask"/> indicating when the write operation is complete.</returns>
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            WriteInternal(buffer.Length);
            return new ValueTask();
        }
#endif

        /// <summary>
        /// Writes a byte to the current position in the stream and advances the position within the stream by one byte.
        /// </summary>
        /// <param name="value">The byte to write to the stream.</param>
        public override void WriteByte(byte value)
        {
            WriteInternal(1);
        }

        /// <summary>
        /// Begins an asynchronous write operation.
        /// </summary>
        /// <param name="buffer">The buffer to write data from.</param>
        /// <param name="offset">The byte offset in <paramref name="buffer"/> from which to begin writing.</param>
        /// <param name="count">The maximum number of bytes to write.</param>
        /// <param name="callback">An optional asynchronous callback, to be called when the write is complete.</param>
        /// <param name="state">
        /// A user-provided object that distinguishes this particular asynchronous write request from other requests.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> may not be negative;
        /// <para>- or -</para>
        /// &gt; <paramref name="count"/> may not be negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="offset"/> and <paramref name="count"/> would exceed the boundaries of the array.
        /// </exception>
        /// <returns>An IAsyncResult that represents the asynchronous write, which could still be pending.</returns>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), "may not be negative");
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "may not be negative");
            if (offset > buffer.Length - count) throw new ArgumentException("The offset and count would exceed the boundaries of the array");

            IAsyncResult result = new CompletedAsync(state);
            if (callback != null) callback(result);
            return result;
        }

        /// <summary>
        /// Ends an asynchronous write operation.
        /// </summary>
        /// <param name="asyncResult">A reference to the outstanding asynchronous I/O request.</param>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            if (asyncResult == null) throw new ArgumentNullException(nameof(asyncResult));
            CompletedAsync.End(asyncResult);
        }

        private int m_IsDisposed = 0;

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value><see langword="true"/> if this instance is disposed; otherwise, <see langword="false"/>.</value>
        public bool IsDisposed
        {
            get { return m_IsDisposed != 0; }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="Stream"/> and optionally releases the managed
        /// resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release
        /// only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (Interlocked.CompareExchange(ref m_IsDisposed, 1, 0) != 0)
                return;

            base.Dispose(disposing);
        }
    }
}
