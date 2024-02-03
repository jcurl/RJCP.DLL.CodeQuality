namespace RJCP.CodeQuality.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

#if NET6_0_OR_GREATER || NET452_OR_GREATER
    using System.Threading.Tasks;
#endif

    /// <summary>
    /// Limit the number of bytes read to a predefined value or random.
    /// </summary>
    /// <remarks>
    /// It is very useful to test decoders by limiting reads to arbitrarily small numbers, where small is usually much
    /// smaller than the usual packet size. This stream wraps another stream which will limit those sizes which make
    /// fuzzing of boundary condition errors much more probable.
    /// <para>
    /// This <see cref="Stream"/> will manage the underlying stream, disposing of this object automatically disposes the
    /// underlying stream. This simplifies lifetime management.
    /// </para>
    /// </remarks>
    public class ReadLimitStream : Stream
    {
        private readonly Random m_Rnd;
        private readonly Stream m_Stream;
        private readonly int m_MinReadLength;
        private readonly int m_MaxReadLength;
        private readonly bool m_OwnsStream;
        private IEnumerable<int> m_LengthSequence;
        private IEnumerator<int> m_LengthEnumerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadLimitStream"/> class.
        /// </summary>
        /// <param name="buffer">
        /// The array buffer that contains the preinitialized data to read via a memory stream.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <remarks>No random read lengths are made, this method simply wraps the stream.</remarks>
        public ReadLimitStream(byte[] buffer)
            : this(buffer, 0) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadLimitStream"/> class.
        /// </summary>
        /// <param name="buffer">
        /// The array buffer that contains the preinitialized data to read via a memory stream.
        /// </param>
        /// <param name="readLength">
        /// The maximum length of a read. If zero, then no limitation of reads to smaller chunks are made. Otherwise
        /// reads are limited from 1..readLength bytes.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        public ReadLimitStream(byte[] buffer, int readLength)
            : this(new MemoryStream(buffer), MinRead(readLength), MaxRead(readLength), true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadLimitStream"/> class.
        /// </summary>
        /// <param name="buffer">
        /// The array buffer that contains the preinitialized data to read via a memory stream.
        /// </param>
        /// <param name="minReadLength">The minimum read length. Must be at least 1 byte.</param>
        /// <param name="maxReadLength">
        /// The maximum read length. Must be greater than the <paramref name="minReadLength"/>.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="minReadLength"/> must be 1 or greater.
        /// <para>- or -</para>
        /// <paramref name="maxReadLength"/> must be greater than <paramref name="minReadLength"/>.
        /// </exception>
        ///
        public ReadLimitStream(byte[] buffer, int minReadLength, int maxReadLength)
            : this(new MemoryStream(buffer), minReadLength, maxReadLength, true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadLimitStream"/> class.
        /// </summary>
        /// <param name="buffer">
        /// The array buffer that contains the preinitialized data to read via a memory stream.
        /// </param>
        /// <param name="lengthSequence">
        /// Defines the sequences that are returned when reading data. Can be used to test boundary conditions for
        /// streams that are being read.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="buffer"/> is <see langword="null"/>
        /// <para>- or -</para>
        /// <paramref name="lengthSequence"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="lengthSequence"/> is of zero length.</exception>
        public ReadLimitStream(byte[] buffer, IEnumerable<int> lengthSequence)
            : this(new MemoryStream(buffer), lengthSequence, true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadLimitStream"/> class.
        /// </summary>
        /// <param name="stream">The stream to wrap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// No random read lengths are made, this method simply wraps the stream. The stream provided will need to be
        /// disposed of after this object is disposed.
        /// </remarks>
        public ReadLimitStream(Stream stream)
            : this(stream, 0, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadLimitStream"/> class.
        /// </summary>
        /// <param name="stream">The stream to wrap.</param>
        /// <param name="ownsStream">If this class should dispose the stream when this object is disposed.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// No random read lengths are made, this method simply wraps the stream. If this object <paramref
        /// name="ownsStream"/>, then it will also be disposed when this object is disposed.
        /// </remarks>
        public ReadLimitStream(Stream stream, bool ownsStream)
            : this(stream, 0, ownsStream) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadLimitStream"/> class.
        /// </summary>
        /// <param name="stream">The stream to wrap.</param>
        /// <param name="readLength">
        /// The maximum length of a read. If zero, then no limitation of reads to smaller chunks are made. Otherwise
        /// reads are limited from 1..readLength bytes.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Limits the length of each read to be a random number between 1 and <paramref name="readLength"/>. The stream
        /// provided will need to be disposed of after this object is disposed.
        /// </remarks>
        public ReadLimitStream(Stream stream, int readLength)
            : this(stream, MinRead(readLength), MaxRead(readLength), false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadLimitStream"/> class.
        /// </summary>
        /// <param name="stream">The stream to wrap.</param>
        /// <param name="readLength">
        /// The maximum length of a read. If zero, then no limitation of reads to smaller chunks are made. Otherwise
        /// reads are limited from 1..readLength bytes.
        /// </param>
        /// <param name="ownsStream">If this class should dispose the stream when this object is disposed.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Limits the length of each read to be a random number between 1 and <paramref name="readLength"/>. If this
        /// object <paramref name="ownsStream"/>, then it will also be disposed when this object is disposed.
        /// </remarks>
        public ReadLimitStream(Stream stream, int readLength, bool ownsStream)
            : this(stream, MinRead(readLength), MaxRead(readLength), ownsStream) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadLimitStream"/> class.
        /// </summary>
        /// <param name="stream">The stream to wrap.</param>
        /// <param name="minReadLength">The minimum read length. Must be at least 1 byte.</param>
        /// <param name="maxReadLength">
        /// The maximum read length. Must be greater than the <paramref name="minReadLength"/>.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="minReadLength"/> must be 1 or greater.
        /// <para>- or -</para>
        /// <paramref name="maxReadLength"/> must be greater than <paramref name="minReadLength"/>.
        /// </exception>
        /// <remarks>
        /// Limits the length of each read to be a random number between <paramref name="minReadLength"/> and <paramref
        /// name="maxReadLength"/>. The stream provided will need to be disposed of after this object is disposed.
        /// </remarks>
        public ReadLimitStream(Stream stream, int minReadLength, int maxReadLength)
            : this(stream, minReadLength, maxReadLength, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadLimitStream"/> class.
        /// </summary>
        /// <param name="stream">The stream to wrap.</param>
        /// <param name="minReadLength">The minimum read length. Must be at least 1 byte.</param>
        /// <param name="maxReadLength">
        /// The maximum read length. Must be greater than the <paramref name="minReadLength"/>.
        /// </param>
        /// <param name="ownsStream">If this class should dispose the stream when this object is disposed.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="minReadLength"/> must be 1 or greater.
        /// <para>- or -</para>
        /// <paramref name="maxReadLength"/> must be greater than <paramref name="minReadLength"/>.
        /// </exception>
        /// <remarks>
        /// Limits the length of each read to be a random number between <paramref name="minReadLength"/> and <paramref
        /// name="maxReadLength"/>. If this object <paramref name="ownsStream"/>, then it will also be disposed when
        /// this object is disposed.
        /// </remarks>
        public ReadLimitStream(Stream stream, int minReadLength, int maxReadLength, bool ownsStream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (minReadLength < 1) throw new ArgumentOutOfRangeException(nameof(minReadLength), "Must be 1 or greater");
            if (maxReadLength < minReadLength) throw new ArgumentOutOfRangeException(nameof(maxReadLength), $"Must be {minReadLength} or greater");

            if (stream is MemoryStream memStream) {
                // A small optimization for reading tests, we set the beginning of the stream.
                memStream.Seek(0, SeekOrigin.Begin);
            }

            m_Stream = stream;
            m_MinReadLength = minReadLength;
            m_MaxReadLength = maxReadLength;
            m_OwnsStream = ownsStream;

            if (m_MinReadLength != m_MaxReadLength) {
                m_Rnd = new Random();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadLimitStream"/> class.
        /// </summary>
        /// <param name="stream">The stream to wrap.</param>
        /// <param name="sequenceLength">
        /// Defines the sequences that are returned when reading data. Can be used to test boundary conditions for
        /// streams that are being read.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <see langword="null"/>
        /// <para>- or -</para>
        /// <paramref name="sequenceLength"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="sequenceLength"/> is of zero length.</exception>
        /// <remarks>
        /// Wraps around the <paramref name="stream"/>, iterating over <paramref name="sequenceLength"/> repeatedly. The
        /// stream provided will need to be disposed of after this object is disposed.
        /// </remarks>
        public ReadLimitStream(Stream stream, IEnumerable<int> sequenceLength)
            : this(stream, sequenceLength, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadLimitStream"/> class.
        /// </summary>
        /// <param name="stream">The stream to wrap.</param>
        /// <param name="sequenceLength">
        /// Defines the sequences that are returned when reading data. Can be used to test boundary conditions for
        /// streams that are being read.
        /// </param>
        /// <param name="ownsStream">If this class should dispose the stream when this object is disposed.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <see langword="null"/>
        /// <para>- or -</para>
        /// <paramref name="sequenceLength"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="sequenceLength"/> is of zero length.</exception>
        /// <remarks>
        /// Wraps around the <paramref name="stream"/>, iterating over <paramref name="sequenceLength"/> repeatedly. If
        /// this object <paramref name="ownsStream"/>, then it will also be disposed when this object is disposed.
        /// </remarks>
        public ReadLimitStream(Stream stream, IEnumerable<int> sequenceLength, bool ownsStream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (sequenceLength == null) throw new ArgumentNullException(nameof(sequenceLength));

            if (stream is MemoryStream memStream) {
                // A small optimization for reading tests, we set the beginning of the stream.
                memStream.Seek(0, SeekOrigin.Begin);
            }

            m_Stream = stream;
            m_LengthSequence = sequenceLength;
            m_OwnsStream = ownsStream;
        }

        private static int MinRead(int readLength)
        {
            return readLength > 0 ? 1 : int.MaxValue;
        }

        private static int MaxRead(int readLength)
        {
            return readLength > 0 ? readLength : int.MaxValue;
        }

        private int GetCount(int maxCount)
        {
            int count;
            if (m_LengthSequence != null) {
                if (m_LengthEnumerator == null) {
                    m_LengthEnumerator = m_LengthSequence.GetEnumerator();
                    if (!m_LengthEnumerator.MoveNext()) {
                        // The sequence is empty, so we ignore it.
                        m_LengthSequence = null;
                        m_LengthEnumerator = null;
                    }
                }

                if (m_LengthEnumerator != null) {
                    count = m_LengthEnumerator.Current;
                    if (!m_LengthEnumerator.MoveNext()) {
                        m_LengthEnumerator = null;
                    }
                    if (count < 1) count = 1;
                    return Math.Min(count, maxCount);
                }
            }

            int readMax = Math.Min(maxCount, m_MaxReadLength);
            int readMin = Math.Min(maxCount, m_MinReadLength);
            if (readMin == readMax) return maxCount;
            return m_Rnd.Next(readMin, readMax + 1);
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <value>Is <see langword="true"/> if this instance can read; otherwise, <see langword="false"/>.</value>
        public override bool CanRead { get { return !IsDisposed && m_Stream.CanRead; } }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <value>Is <see langword="true"/> if this instance can seek; otherwise, <see langword="false"/>.</value>
        public override bool CanSeek { get { return !IsDisposed && m_Stream.CanSeek; } }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <value>Is <see langword="true"/> if this instance can write; otherwise, <see langword="false"/>.</value>
        public override bool CanWrite { get { return !IsDisposed && m_Stream.CanWrite; } }

        /// <summary>
        /// Gets a value that determines whether the current stream can time out.
        /// </summary>
        /// <value><see langword="true"/> if this instance can timeout; otherwise, <see langword="false"/>.</value>
        public override bool CanTimeout { get { return !IsDisposed && m_Stream.CanTimeout; } }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        /// <value>The length.</value>
        public override long Length { get { return m_Stream.Length; } }

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        /// <value>The position.</value>
        public override long Position
        {
            get { return m_Stream.Position; }
            set { m_Stream.Position = value; }
        }

        /// <summary>
        /// Gets or sets a value, in milliseconds, that determines how long the stream will attempt to read before
        /// timing out.
        /// </summary>
        /// <value>The read timeout.</value>
        public override int ReadTimeout
        {
            get { return m_Stream.ReadTimeout; }
            set { m_Stream.ReadTimeout = value; }
        }

        /// <summary>
        /// Gets or sets a value, in milliseconds, that determines how long the stream will attempt to write before
        /// timing out.
        /// </summary>
        /// <value>The write timeout.</value>
        public override int WriteTimeout
        {
            get { return m_Stream.WriteTimeout; }
            set { m_Stream.WriteTimeout = value; }
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            m_Stream.Flush();
        }

#if NET6_0_OR_GREATER || NET462_OR_GREATER
        /// <summary>
        /// Clears all buffers for this stream asynchronously and causes any buffered data to be written to the
        /// underlying device.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <returns>Task.</returns>
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return m_Stream.FlushAsync(cancellationToken);
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
        public override int Read(byte[] buffer, int offset, int count)
        {
            return m_Stream.Read(buffer, offset, GetCount(count));
        }

#if NET6_0_OR_GREATER
        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number
        /// of bytes read.
        /// </summary>
        /// <param name="buffer">
        /// A region of memory. When this method returns, the contents of this region are replaced by the bytes read
        /// from the current source.
        /// </param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes allocated in the
        /// buffer if that many bytes are not currently available, or zero (0) if the end of the stream has been
        /// reached.
        /// </returns>
        public override int Read(Span<byte> buffer)
        {
            int count = GetCount(buffer.Length);
            return m_Stream.Read(buffer[..count]);
        }
#endif

#if NET6_0_OR_GREATER || NET462_OR_GREATER
        /// <summary>
        /// Asynchronously reads a sequence of bytes from the current stream, advances the position within the stream by
        /// the number of bytes read, and monitors cancellation requests.
        /// </summary>
        /// <param name="buffer">The buffer to write the data into.</param>
        /// <param name="offset">
        /// The byte offset in <paramref name="buffer"/> at which to begin writing data from the stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is
        /// <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The value of the result
        /// parameter contains the total number of bytes read into the buffer. The result value can be less than the
        /// number of bytes requested if the number of bytes currently available is less than the requested number, or
        /// it can be 0 (zero) if the end of the stream has been reached.
        /// </returns>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return m_Stream.ReadAsync(buffer, offset, GetCount(count), cancellationToken);
        }
#endif

#if NET6_0_OR_GREATER
        /// <summary>
        /// Asynchronously reads a sequence of bytes from the current stream, advances the position within the stream by
        /// the number of bytes read, and monitors cancellation requests.
        /// </summary>
        /// <param name="buffer">The region of memory to write the data into.</param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The value of its
        /// <see cref="ValueTask{TResult}.Result"/> property contains the total number of bytes read into the buffer.
        /// The result value can be less than the number of bytes allocated in the buffer if that many bytes are not
        /// currently available, or it can be 0 (zero) if the end of the stream has been reached.
        /// </returns>
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            int count = GetCount(buffer.Length);
            return m_Stream.ReadAsync(buffer[..count], cancellationToken);
        }
#endif

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
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return m_Stream.BeginRead(buffer, offset, GetCount(count), callback, state);
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
        public override int EndRead(IAsyncResult asyncResult)
        {
            return m_Stream.EndRead(asyncResult);
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
        /// <param name="origin">
        /// A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.
        /// </param>
        /// <returns>The new position within the current stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return m_Stream.Seek(offset, origin);
        }

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        public override void SetLength(long value)
        {
            m_Stream.SetLength(value);
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
        public override void Write(byte[] buffer, int offset, int count)
        {
            m_Stream.Write(buffer, offset, count);
        }

#if NET6_0_OR_GREATER
        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the
        /// number of bytes written.
        /// </summary>
        /// <param name="buffer">
        /// A region of memory. This method copies the contents of this region to the current stream.
        /// </param>
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            m_Stream.Write(buffer);
        }
#endif

#if NET6_0_OR_GREATER || NET462_OR_GREATER
        /// <summary>
        /// Asynchronously writes a sequence of bytes to the current stream, advances the current position within this
        /// stream by the number of bytes written, and monitors cancellation requests.
        /// </summary>
        /// <param name="buffer">The buffer to write data from.</param>
        /// <param name="offset">
        /// The zero-based byte offset in <paramref name="buffer"/> from which to begin copying bytes to the stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to write.</param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return m_Stream.WriteAsync(buffer, offset, count, cancellationToken);
        }
#endif

#if NET6_0_OR_GREATER
        /// <summary>
        /// Asynchronously writes a sequence of bytes to the current stream, advances the current position within this
        /// stream by the number of bytes written, and monitors cancellation requests.
        /// </summary>
        /// <param name="buffer">The region of memory to write data from.</param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return m_Stream.WriteAsync(buffer, cancellationToken);
        }
#endif

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
        /// <returns>
        /// An <see cref="IAsyncResult"/> that represents the asynchronous write, which could still be pending.
        /// </returns>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return m_Stream.BeginWrite(buffer, offset, count, callback, state);
        }

        /// <summary>
        /// Ends an asynchronous write operation.
        /// </summary>
        /// <param name="asyncResult">A reference to the outstanding asynchronous I/O request.</param>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            m_Stream.EndWrite(asyncResult);
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
