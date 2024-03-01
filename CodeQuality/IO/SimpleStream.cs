namespace RJCP.CodeQuality.IO
{
    using System;
    using System.IO;
    using System.Threading;

#if NET6_0_OR_GREATER || NET462_OR_GREATER
    using System.Threading.Tasks;
#endif

    /// <summary>
    /// A very simple stream that does almost nothing, that is also easy to mock.
    /// </summary>
    public class SimpleStream : Stream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleStream"/> class with a default mode of
        /// <see cref="StreamMode.All"/>.
        /// </summary>
        public SimpleStream()
        {
            Mode = StreamMode.All;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleStream"/> class.
        /// </summary>
        /// <param name="mode">The stream operating mode, which is the property <see cref="Mode"/>.</param>
        public SimpleStream(StreamMode mode)
        {
            Mode = mode;
        }

        /// <summary>
        /// Gets or sets the stream operating mode.
        /// </summary>
        /// <value>The stream operating mode.</value>
        /// <remarks>
        /// This is useful to simulate different types of streams.
        /// </remarks>
        public StreamMode Mode { get; set; }

        private bool IsMode(StreamMode mode)
        {
            return !IsDisposed && ((Mode & mode) != 0);
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <value><see langword="true"/> if this instance can read; otherwise, <see langword="false"/>.</value>
        public override bool CanRead { get { return IsMode(StreamMode.Read); } }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <value><see langword="true"/> if this instance can seek; otherwise, <see langword="false"/>.</value>
        public override bool CanSeek { get { return IsMode(StreamMode.Seek); } }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <value><see langword="true"/> if this instance can write; otherwise, <see langword="false"/>.</value>
        public override bool CanWrite { get { return IsMode(StreamMode.Write); } }

        /// <summary>
        /// Gets a value that determines whether the current stream can time out.
        /// </summary>
        /// <value><see langword="true"/> if this instance can timeout; otherwise, <see langword="false"/>.</value>
        /// <remarks>
        /// This stream supports timeouts.
        /// </remarks>
        public override bool CanTimeout { get { return IsMode(StreamMode.Timeout); } }

        private int m_WriteTimeout = Timeout.Infinite;

        /// <summary>
        /// Gets or sets a value, in milliseconds, that determines how long the stream will attempt to write before
        /// timing out.
        /// </summary>
        /// <value>The write timeout.</value>
        /// <exception cref="InvalidOperationException">Write timeout is not supported.</exception>
        public override int WriteTimeout
        {
            get
            {
                if (!IsMode(StreamMode.Timeout))
                    throw new InvalidOperationException("Write timeout not supported");
                return m_WriteTimeout;
            }
            set
            {
                if (!IsMode(StreamMode.Timeout))
                    throw new InvalidOperationException("Write timeout not supported");
                m_WriteTimeout = value;
            }
        }

        private int m_ReadTimeout = Timeout.Infinite;

        /// <summary>
        /// Gets or sets a value, in milliseconds, that determines how long the stream will attempt to read before
        /// timing out.
        /// </summary>
        /// <value>The read timeout.</value>
        /// <exception cref="InvalidOperationException">Read timeout is not supported.</exception>
        public override int ReadTimeout
        {
            get
            {
                if (!IsMode(StreamMode.Timeout))
                    throw new InvalidOperationException("Read timeout not supported");
                return m_ReadTimeout;
            }
            set
            {
                if (!IsMode(StreamMode.Timeout))
                    throw new InvalidOperationException("Read timeout not supported");
                m_ReadTimeout = value;
            }
        }

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
        /// <exception cref="NotSupportedException">Seek is not supported.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        /// <remarks>
        /// When updating the position, if it exceeds the current length, would extend the length of the stream.
        /// </remarks>
        public override long Position
        {
            get { return m_Position; }
            set
            {
                if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
                if (!IsMode(StreamMode.Seek)) throw new NotSupportedException("Seek is not supported");
                ThrowHelper.ThrowIfNegative(value, nameof(Position));

                m_Position = value;
                if (m_Position > m_Length) m_Length = m_Position;
            }
        }

        /// <summary>
        /// Flushes the stream.
        /// </summary>
        /// <exception cref="NotSupportedException">Write is not supported.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        /// <remarks>Flushing the stream has no operation.</remarks>
        public override void Flush()
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            if (!IsMode(StreamMode.Write)) throw new NotSupportedException("Write is not supported");
            /* Nothing to do */
        }

#if NET6_0_OR_GREATER || NET462_OR_GREATER
        /// <summary>
        /// Clears all buffers asynchronously for this stream and causes any buffered data to be written to the
        /// underlying device.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <exception cref="OperationCanceledException">Operation has been cancelled.</exception>
        /// <exception cref="NotSupportedException">Write is not supported.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        /// <returns>Task.</returns>
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            if (!IsMode(StreamMode.Write)) throw new NotSupportedException("Write is not supported");
            cancellationToken.ThrowIfCancellationRequested();
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
        /// <exception cref="NotSupportedException">Read is not supported.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        /// <remarks>Updates the position, without actually writing anything.</remarks>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            if (!IsMode(StreamMode.Read)) throw new NotSupportedException("Read is not supported");
            ThrowHelper.ThrowIfArrayOutOfBounds(buffer, offset, count);

            return ReadInternal(count);
        }

#if NET6_0_OR_GREATER
        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number
        /// of bytes read.
        /// </summary>
        /// <param name="buffer">The buffer to read into.</param>
        /// <exception cref="NotSupportedException">Read is not supported.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that
        /// many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <remarks>Updates the position, without actually writing anything.</remarks>
        public override int Read(Span<byte> buffer)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            if (!IsMode(StreamMode.Read)) throw new NotSupportedException("Read is not supported");
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

#if NET6_0_OR_GREATER || NET462_OR_GREATER
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
        /// <exception cref="NotSupportedException">Read is not supported.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        /// <remarks>Updates the position, without actually writing anything.</remarks>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            if (!IsMode(StreamMode.Read)) throw new NotSupportedException("Read is not supported");
            ThrowHelper.ThrowIfArrayOutOfBounds(buffer, offset, count);
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(ReadInternal(count));
        }
#endif

#if NET6_0_OR_GREATER
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
        /// <exception cref="NotSupportedException">Read is not supported.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        /// <remarks>Updates the position, without actually writing anything.</remarks>
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            if (!IsMode(StreamMode.Read)) throw new NotSupportedException("Read is not supported");
            cancellationToken.ThrowIfCancellationRequested();

            return new ValueTask<int>(ReadInternal(buffer.Length));
        }
#endif

        /// <summary>
        /// Reads a byte from the stream and advances the position within the stream by one byte, or returns -1 if at
        /// the end of the stream.
        /// </summary>
        /// <exception cref="NotSupportedException">Read is not supported.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        /// <returns>The unsigned byte cast to an Int32, or -1 if at the end of the stream.</returns>
        public override int ReadByte()
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            if (!IsMode(StreamMode.Read)) throw new NotSupportedException("Read is not supported");
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
        /// <exception cref="NotSupportedException">Read is not supported.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        /// <returns>An IAsyncResult that represents the asynchronous write, which could still be pending.</returns>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            if (!IsMode(StreamMode.Read)) throw new NotSupportedException("Read is not supported");
            ThrowHelper.ThrowIfArrayOutOfBounds(buffer, offset, count);

            IAsyncResult result = new CompletedAsync<int>(state, ReadInternal(count));
            if (callback is not null) callback(result);
            return result;
        }

        /// <summary>
        /// Ends an asynchronous write operation.
        /// </summary>
        /// <param name="asyncResult">A reference to the outstanding asynchronous I/O request.</param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="asyncResult"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="asyncResult"/> is from a different operation.
        /// </exception>
        /// <exception cref="NotSupportedException">Read is not supported.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        public override int EndRead(IAsyncResult asyncResult)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            if (!IsMode(StreamMode.Read)) throw new NotSupportedException("Read is not supported");
            ThrowHelper.ThrowIfNull(asyncResult);
            if (asyncResult is not CompletedAsync<int> readAsync)
                throw new ArgumentException("Invalid async result", nameof(asyncResult));
            CompletedAsync.End(asyncResult);
            return readAsync.Result;
        }

#if NET6_0_OR_GREATER
        /// <summary>
        /// Reads all the bytes from the current stream and writes them to a destination stream, using a specified
        /// buffer size.
        /// </summary>
        /// <param name="destination">The stream that will contain the contents of the current stream.</param>
        /// <param name="bufferSize">
        /// The size of the buffer. This value must be greater than zero. The default size is 4096.
        /// </param>
        /// <exception cref="NotSupportedException">Read is not supported.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        /// <remarks>
        /// Copies all zeroes to the <paramref name="destination"/> stream for the stream <see cref="Length"/>.
        /// </remarks>
        public override void CopyTo(Stream destination, int bufferSize)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            if (!IsMode(StreamMode.Read)) throw new NotSupportedException("Read is not supported");
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
        /// <exception cref="NotSupportedException">Read is not supported.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
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
            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            if (!IsMode(StreamMode.Read)) throw new NotSupportedException("Read is not supported");
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
        /// <exception cref="ArgumentException"><paramref name="offset"/> would exceed beyond end of file.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Unknown seek <paramref name="origin"/>.</exception>
        /// <exception cref="IOException"><paramref name="offset"/> would exceed beyond beginning of file.</exception>
        /// <exception cref="NotSupportedException">Seek is not supported.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        /// <remarks>Updates the position of the stream.</remarks>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            if (!IsMode(StreamMode.Seek)) throw new NotSupportedException("Seek is not supported");

            switch (origin) {
            case SeekOrigin.Begin:
                ThrowHelper.ThrowIfNegative(offset);
                m_Position = offset;
                if (m_Position > m_Length) m_Length = m_Position;
                break;
            case SeekOrigin.Current:
                ThrowHelper.ThrowIfLessThan(offset, -m_Position);
                ThrowHelper.ThrowIfGreaterThan(offset, long.MaxValue - m_Position);
                m_Position += offset;
                if (m_Position > m_Length) m_Length = m_Position;
                break;
            case SeekOrigin.End:
                ThrowHelper.ThrowIfNegative(offset);
                ThrowHelper.ThrowIfGreaterThan(offset, m_Length);
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
        /// <exception cref="NotSupportedException">Write and Seek are not supported.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        /// <remarks>Updates the length of the stream. If shortened, the <see cref="Position"/> is updated.</remarks>
        public override void SetLength(long value)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            if (!IsMode(StreamMode.Write)) throw new NotSupportedException("Write is not supported");
            if (!IsMode(StreamMode.Seek)) throw new NotSupportedException("Seek is not supported");

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
        /// <exception cref="NotSupportedException">Write is not supported.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        /// <remarks>Updates the position, without actually writing anything.</remarks>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            if (!IsMode(StreamMode.Write)) throw new NotSupportedException("Write is not supported");
            ThrowHelper.ThrowIfArrayOutOfBounds(buffer, offset, count);

            WriteInternal(count);
        }

        private void WriteInternal(int count)
        {
            if (long.MaxValue - m_Position < count)
                throw new IOException("Writing to the stream would exceed maximum length");

            m_Position += count;
            if (m_Position > m_Length) m_Length = m_Position;
        }

#if NET6_0_OR_GREATER
        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the
        /// number of bytes written.
        /// </summary>
        /// <param name="buffer">Writes the bytes to the current stream.</param>
        /// <exception cref="NotSupportedException">Write is not supported.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            if (!IsMode(StreamMode.Write)) throw new NotSupportedException("Write is not supported");
            WriteInternal(buffer.Length);
        }
#endif

#if NET6_0_OR_GREATER || NET462_OR_GREATER
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
        /// <exception cref="NotSupportedException">Write is not supported.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        /// <returns>A task indicating when the write operation is complete.</returns>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            if (!IsMode(StreamMode.Write)) throw new NotSupportedException("Write is not supported");
            ThrowHelper.ThrowIfArrayOutOfBounds(buffer, offset, count);
            cancellationToken.ThrowIfCancellationRequested();

            WriteInternal(count);
            return Task.CompletedTask;
        }
#endif

#if NET6_0_OR_GREATER
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
        /// <exception cref="NotSupportedException">Write is not supported.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        /// <returns>A <see cref="ValueTask"/> indicating when the write operation is complete.</returns>
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            if (!IsMode(StreamMode.Write)) throw new NotSupportedException("Write is not supported");
            cancellationToken.ThrowIfCancellationRequested();
            WriteInternal(buffer.Length);
            return new ValueTask();
        }
#endif

        /// <summary>
        /// Writes a byte to the current position in the stream and advances the position within the stream by one byte.
        /// </summary>
        /// <param name="value">The byte to write to the stream.</param>
        /// <exception cref="NotSupportedException">Write is not supported.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        public override void WriteByte(byte value)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            if (!IsMode(StreamMode.Write)) throw new NotSupportedException("Write is not supported");
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
        /// <exception cref="NotSupportedException">Write is not supported.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        /// <returns>An IAsyncResult that represents the asynchronous write, which could still be pending.</returns>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            if (!IsMode(StreamMode.Write)) throw new NotSupportedException("Write is not supported");
            ThrowHelper.ThrowIfArrayOutOfBounds(buffer, offset, count);

            IAsyncResult result = new CompletedAsync(state);
            if (callback is not null) callback(result);
            return result;
        }

        /// <summary>
        /// Ends an asynchronous write operation.
        /// </summary>
        /// <param name="asyncResult">A reference to the outstanding asynchronous I/O request.</param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="asyncResult"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="asyncResult"/> is from a different operation.
        /// </exception>
        /// <exception cref="NotSupportedException">Write is not supported.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(SimpleStream));
            if (!IsMode(StreamMode.Write)) throw new NotSupportedException("Write is not supported");
            ThrowHelper.ThrowIfNull(asyncResult);
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
