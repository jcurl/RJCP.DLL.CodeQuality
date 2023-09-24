namespace RJCP.CodeQuality.IO
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A write-only stream, that writes to oblivion, or wraps another stream.
    /// </summary>
    public class WriteOnlyStream : Stream
    {
        private readonly Stream m_Stream;
        private readonly bool m_OwnsStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteOnlyStream"/> class.
        /// </summary>
        /// <remarks>This constructor is equivalent to writing to <c>NUL:</c>.</remarks>
        public WriteOnlyStream() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteOnlyStream"/> class.
        /// </summary>
        /// <param name="stream">The stream to wrap that is only writable.</param>
        public WriteOnlyStream(Stream stream) : this(stream, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteOnlyStream"/> class.
        /// </summary>
        /// <param name="stream">The stream to wrap that is only writable.</param>
        /// <param name="ownsStream">
        /// Instructs this class to dispose the stream when this stream itself is disposed.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        public WriteOnlyStream(Stream stream, bool ownsStream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            m_Stream = stream;
            m_OwnsStream = ownsStream;
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <value><see langword="true"/> if this instance can read; otherwise, <see langword="false"/>.</value>
        /// <remarks>This stream is not readable, so always returns <see langword="false"/>.</remarks>
        public override bool CanRead { get { return false; } }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <value><see langword="true"/> if this instance can seek; otherwise, <see langword="false"/>.</value>
        /// <remarks>This stream is not readable, so always returns <see langword="false"/>.</remarks>
        public override bool CanSeek { get { return false; } }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <value><see langword="true"/> if this instance can write; otherwise, <see langword="false"/>.</value>
        public override bool CanWrite { get { return m_Stream == null || m_Stream.CanWrite; } }

        /// <summary>
        /// Gets a value that determines whether the current stream can time out.
        /// </summary>
        /// <value><see langword="true"/> if this instance can timeout; otherwise, <see langword="false"/>.</value>
        public override bool CanTimeout { get { return m_Stream != null && m_Stream.CanTimeout; } }

        /// <summary>
        /// Gets or sets a value, in miliseconds, that determines how long the stream will attempt to read before timing
        /// out.
        /// </summary>
        /// <value>The read timeout.</value>
        /// <exception cref="InvalidOperationException">Setting the position in the stream is not supported.</exception>
        public override int ReadTimeout
        {
            get { throw new InvalidOperationException(); }
            set { throw new InvalidOperationException(); }
        }

        /// <summary>
        /// Gets or sets a value, in milliseconds, that determines how long the stream will attempt to write before
        /// timing out.
        /// </summary>
        /// <value>The write timeout in milliseconds.</value>
        public override int WriteTimeout
        {
            get
            {
                if (m_Stream != null) return m_Stream.WriteTimeout;
                throw new InvalidOperationException();
            }

            set
            {
                if (m_Stream != null) {
                    m_Stream.WriteTimeout = value;
                } else {
                    throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        /// <value>The length of the stream, or zero if no stream was provided in the constructor.</value>
        public override long Length { get { return m_Stream == null ? 0 : m_Stream.Length; } }

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        /// <value>The position in the stream.</value>
        /// <exception cref="NotSupportedException">Setting the position in the stream is not supported.</exception>
        public override long Position
        {
            get { return m_Stream == null ? 0 : m_Stream.Position; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets a reference to the base stream, if there is one.
        /// </summary>
        /// <value>The base stream, or <see langword="null"/> if there is none.</value>
        public Stream BaseStream
        {
            get { return m_Stream; }
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            if (m_Stream != null) {
                m_Stream.Flush();
                return;
            }

            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
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
            if (m_Stream != null)
                return m_Stream.FlushAsync(cancellationToken);

            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            return Task.CompletedTask;
        }
#endif

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number
        /// of bytes read.
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
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that
        /// many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="NotSupportedException">Reading from the stream is not supported.</exception>
        /// <remarks>This stream is only writable, so it always throws <see cref="NotSupportedException"/>.</remarks>
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
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
        /// <exception cref="NotSupportedException">Reading from the stream is not supported.</exception>
        /// <remarks>This stream is only writable, so it always throws <see cref="NotSupportedException"/>.</remarks>
        public override int Read(Span<byte> buffer)
        {
            throw new NotSupportedException();
        }
#endif

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
        /// <exception cref="NotSupportedException">Reading from the stream is not supported.</exception>
        /// <remarks>This stream is only writable, so it always throws <see cref="NotSupportedException"/>.</remarks>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
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
        /// <exception cref="NotSupportedException">Reading from the stream is not supported.</exception>
        /// <remarks>This stream is only writable, so it always throws <see cref="NotSupportedException"/>.</remarks>
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }
#endif

        /// <summary>
        /// Reads a byte from the stream and advances the position within the stream by one byte, or returns -1 if at
        /// the end of the stream.
        /// </summary>
        /// <returns>The unsigned byte cast to an Int32, or -1 if at the end of the stream.</returns>
        /// <exception cref="NotSupportedException">Reading from the stream is not supported.</exception>
        /// <remarks>This stream is only writable, so it always throws <see cref="NotSupportedException"/>.</remarks>
        public override int ReadByte()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Begins an asynchronous read operation.
        /// </summary>
        /// <param name="buffer">The buffer to read the data into.</param>
        /// <param name="offset">
        /// The byte offset in <paramref name="buffer"/> at which to begin writing data read from the stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <param name="callback">An optional asynchronous callback, to be called when the read is complete.</param>
        /// <param name="state">
        /// A user-provided object that distinguishes this particular asynchronous read request from other requests.
        /// </param>
        /// <returns>
        /// An <see cref="IAsyncResult"/> that represents the asynchronous read, which could still be pending.
        /// </returns>
        /// <exception cref="NotSupportedException">Reading from the stream is not supported.</exception>
        /// <remarks>This stream is only writable, so it always throws <see cref="NotSupportedException"/>.</remarks>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Waits for the pending asynchronous read to complete.
        /// </summary>
        /// <param name="asyncResult">The reference to the pending asynchronous request to finish.</param>
        /// <returns>
        /// The number of bytes read from the stream, between zero (0) and the number of bytes you requested. Streams
        /// return zero (0) only at the end of the stream, otherwise, they should block until at least one byte is
        /// available.
        /// </returns>
        /// <exception cref="NotSupportedException">Reading from the stream is not supported.</exception>
        /// <remarks>This stream is only writable, so it always throws <see cref="NotSupportedException"/>.</remarks>
        public override int EndRead(IAsyncResult asyncResult)
        {
            throw new NotSupportedException();
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
        /// <exception cref="NotSupportedException">Reading from the stream is not supported.</exception>
        /// <remarks>This stream is only writable, so it always throws <see cref="NotSupportedException"/>.</remarks>
        public override void CopyTo(Stream destination, int bufferSize)
        {
            throw new NotSupportedException();
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
        /// <exception cref="NotSupportedException">Reading from the stream is not supported.</exception>
        /// <remarks>This stream is only writable, so it always throws <see cref="NotSupportedException"/>.</remarks>
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
#endif

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
        /// <param name="origin">
        /// A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.
        /// </param>
        /// <returns>The new position within the current stream.</returns>
        /// <exception cref="NotSupportedException">This stream does not support seeking.</exception>
        /// <remarks>
        /// This stream is only writable, seeking is not supported, so it always throws <see
        /// cref="NotSupportedException"/>.
        /// </remarks>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="NotSupportedException">
        /// This stream does not support seeking or setting the position.
        /// </exception>
        /// <remarks>
        /// This stream doesn't support setting the length, so it always throws <see cref="NotSupportedException"/>.
        /// </remarks>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
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
        /// <exception cref="ArgumentException">
        /// The <paramref name="offset"/> and <paramref name="count"/> would exceed the boundaries of the array.
        /// </exception>
        /// <exception cref="ObjectDisposedException">This object has been disposed of.</exception>
        /// <remarks>If a stream is provided, then any exceptions are propagated.</remarks>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (m_Stream != null) {
                m_Stream.Write(buffer, offset, count);
                return;
            }

            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), "may not be negative");
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "may not be negative");
            if (offset > buffer.Length - count) throw new ArgumentException("The offset and count would exceed the boundaries of the array");
        }

#if NETSTANDARD
        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the
        /// number of bytes written.
        /// </summary>
        /// <param name="buffer">Writes the bytes to the current stream.</param>
        /// <exception cref="ObjectDisposedException">This object has been disposed of.</exception>
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            if (m_Stream != null) {
                m_Stream.Write(buffer);
                return;
            }

            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
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
        /// <exception cref="ObjectDisposedException">This object has been disposed of.</exception>
        /// <returns>A task indicating when the write operation is complete.</returns>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (m_Stream != null) return m_Stream.WriteAsync(buffer, offset, count, cancellationToken);

            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), "may not be negative");
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "may not be negative");
            if (offset > buffer.Length - count) throw new ArgumentException("The offset and count would exceed the boundaries of the array");

            cancellationToken.ThrowIfCancellationRequested();
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
        /// <exception cref="ObjectDisposedException">This object has been disposed of.</exception>
        /// <returns>A <see cref="ValueTask"/> indicating when the write operation is complete.</returns>
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (m_Stream != null) return m_Stream.WriteAsync(buffer, cancellationToken);

            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            cancellationToken.ThrowIfCancellationRequested();
            return new ValueTask();
        }
#endif

        /// <summary>
        /// Writes a byte to the current position in the stream and advances the position within the stream by one byte.
        /// </summary>
        /// <param name="value">The byte to write to the stream.</param>
        /// <exception cref="ObjectDisposedException">This object has been disposed of.</exception>
        public override void WriteByte(byte value)
        {
            if (m_Stream != null) {
                m_Stream.WriteByte(value);
                return;
            }

            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
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
        /// <exception cref="ObjectDisposedException">This object has been disposed of.</exception>
        /// <returns>An IAsyncResult that represents the asynchronous write, which could still be pending.</returns>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            if (m_Stream != null) return m_Stream.BeginWrite(buffer, offset, count, callback, state);

            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
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
        /// <exception cref="ObjectDisposedException">This object has been disposed of.</exception>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            if (m_Stream != null) {
                m_Stream.EndWrite(asyncResult);
                return;
            }

            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
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

            if (disposing && m_Stream != null && m_OwnsStream) {
                m_Stream.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
