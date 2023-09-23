namespace RJCP.CodeQuality.IO
{
    using System;
    using System.IO;
    using System.Threading;
    using NUnit.Framework;
    using System.Diagnostics.CodeAnalysis;

#if NETCOREAPP || NET462_OR_GREATER && !NET40_LEGACY
    using System.Threading.Tasks;
#endif

    [TestFixture]
    [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Suppressions needed for some frameworks")]
    public class SimpleStreamTest
    {
        [Test]
        public void DefaultStream()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(s.CanRead, Is.True);
                Assert.That(s.CanWrite, Is.True);
                Assert.That(s.CanSeek, Is.True);
                Assert.That(s.CanTimeout, Is.True);
                Assert.That(s.Length, Is.EqualTo(0));
                Assert.That(s.Position, Is.EqualTo(0));
                Assert.That(s.WriteTimeout, Is.EqualTo(Timeout.Infinite));
                Assert.That(s.ReadTimeout, Is.EqualTo(Timeout.Infinite));
            }
        }

        [Test]
        public void SetReadTimeout([Values(-1, 0, 100)] int timeout)
        {
            using (SimpleStream s = new SimpleStream()) {
                s.ReadTimeout = timeout;
                Assert.That(s.ReadTimeout, Is.EqualTo(timeout));
            }
        }

        [Test]
        public void SetWriteTimeout([Values(-1, 0, 100)] int timeout)
        {
            using (SimpleStream s = new SimpleStream()) {
                s.WriteTimeout = timeout;
                Assert.That(s.WriteTimeout, Is.EqualTo(timeout));
            }
        }

        [Test]
        public void SetPosition([Values(0, 100)] int length)
        {
            using (SimpleStream s = new SimpleStream()) {
                s.Position = length;
                Assert.That(s.Position, Is.EqualTo(length));
                Assert.That(s.Length, Is.EqualTo(length));
            }
        }

        [Test]
        public void SetPositionNegative()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(() => {
                    s.Position = -1;
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void Flush()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.Flush();
                Assert.That(s.Length, Is.EqualTo(0));
                Assert.That(s.Position, Is.EqualTo(0));
            }
        }

#if NETCOREAPP || NET462_OR_GREATER && !NET40_LEGACY
        [Test]
        public async Task FlushAsync()
        {
            using (SimpleStream s = new SimpleStream()) {
                await s.FlushAsync();
                Assert.That(s.Length, Is.EqualTo(0));
                Assert.That(s.Position, Is.EqualTo(0));
            }
        }
#endif

        [Test]
        public void Read()
        {
            using (SimpleStream s = new SimpleStream()) {
                byte[] buffer = new byte[100];

                s.SetLength(1000);
                int read = s.Read(buffer, 0, buffer.Length);
                Assert.That(read, Is.EqualTo(100));
                Assert.That(s.Length, Is.EqualTo(1000));
                Assert.That(s.Position, Is.EqualTo(100));
            }
        }

        [Test]
        public void ReadExact()
        {
            using (SimpleStream s = new SimpleStream()) {
                byte[] buffer = new byte[100];

                s.SetLength(100);
                int read = s.Read(buffer, 0, buffer.Length);
                Assert.That(read, Is.EqualTo(100));
                Assert.That(s.Length, Is.EqualTo(100));
                Assert.That(s.Position, Is.EqualTo(100));
            }
        }

        [Test]
        public void ReadZero()
        {
            using (SimpleStream s = new SimpleStream()) {
                byte[] buffer = new byte[100];
                int read = s.Read(buffer, 0, buffer.Length);
                Assert.That(read, Is.EqualTo(0));
                Assert.That(s.Length, Is.EqualTo(0));
                Assert.That(s.Position, Is.EqualTo(0));
            }
        }

        [Test]
        public void ReadZeroBytes()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(100);
                byte[] buffer = new byte[100];
                int read = s.Read(buffer, 0, 0);
                Assert.That(read, Is.EqualTo(0));
                Assert.That(s.Length, Is.EqualTo(100));
                Assert.That(s.Position, Is.EqualTo(0));
            }
        }

        [Test]
        public void ReadNullBuffer()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(() => {
                    _ = s.Read(null, 0, 100);
                }, Throws.TypeOf<ArgumentNullException>());
            }
        }

        [Test]
        public void ReadNegativeOffset()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(() => {
                    byte[] buffer = new byte[100];
                    _ = s.Read(buffer, -1, 100);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void ReadNegativeCount()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(() => {
                    byte[] buffer = new byte[100];
                    _ = s.Read(buffer, 50, -1);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void ReadOutOfBounds()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(() => {
                    byte[] buffer = new byte[100];
                    _ = s.Read(buffer, 50, 51);
                }, Throws.TypeOf<ArgumentException>());
            }
        }

        [Test]
        public void ReadMassiveStream()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(long.MaxValue);
                byte[] buffer = new byte[100];
                int read = s.Read(buffer, 0, buffer.Length);
                Assert.That(read, Is.EqualTo(buffer.Length));
                Assert.That(s.Position, Is.EqualTo(100));
                Assert.That(s.Length, Is.EqualTo(long.MaxValue));
            }
        }

        [Test]
        public void ReadEndOfMassiveStream()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(long.MaxValue);
                s.Position = long.MaxValue - 1;
                byte[] buffer = new byte[100];
                int read = s.Read(buffer, 0, buffer.Length);
                Assert.That(read, Is.EqualTo(1));
                Assert.That(s.Position, Is.EqualTo(long.MaxValue));
                Assert.That(s.Length, Is.EqualTo(long.MaxValue));
            }
        }

        [Test]
        public void ReadEndToMassiveStream()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(long.MaxValue);
                s.Position = long.MaxValue - 1;
                byte[] buffer = new byte[1];
                int read = s.Read(buffer, 0, buffer.Length);
                Assert.That(read, Is.EqualTo(1));
                Assert.That(s.Position, Is.EqualTo(long.MaxValue));
                Assert.That(s.Length, Is.EqualTo(long.MaxValue));
            }
        }

        [Test]
        public void ReadByte()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(1000);
                int readbyte = s.ReadByte();
                Assert.That(readbyte, Is.EqualTo(0));
                Assert.That(s.Length, Is.EqualTo(1000));
                Assert.That(s.Position, Is.EqualTo(1));
            }
        }

        [Test]
        public void ReadByteExact()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(100);
                s.Position = 99;
                int readbyte = s.ReadByte();
                Assert.That(readbyte, Is.EqualTo(0));
                Assert.That(s.Length, Is.EqualTo(100));
                Assert.That(s.Position, Is.EqualTo(100));
            }
        }

        [Test]
        public void ReadByteEofEmpty()
        {
            using (SimpleStream s = new SimpleStream()) {
                int readbyte = s.ReadByte();
                Assert.That(readbyte, Is.EqualTo(-1));
                Assert.That(s.Length, Is.EqualTo(0));
                Assert.That(s.Position, Is.EqualTo(0));
            }
        }

        [Test]
        public void ReadByteEndToMassiveStream()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(long.MaxValue);
                s.Position = long.MaxValue - 1;
                int readbyte = s.ReadByte();
                Assert.That(readbyte, Is.EqualTo(0));
                Assert.That(s.Position, Is.EqualTo(long.MaxValue));
                Assert.That(s.Length, Is.EqualTo(long.MaxValue));
            }
        }

        [Test]
        public void ReadByteEndToMassiveStreamEof()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(long.MaxValue);
                s.Position = long.MaxValue;
                int readbyte = s.ReadByte();
                Assert.That(readbyte, Is.EqualTo(-1));
                Assert.That(s.Position, Is.EqualTo(long.MaxValue));
                Assert.That(s.Length, Is.EqualTo(long.MaxValue));
            }
        }

        [Test]
        public void BeginReadEndRead()
        {
            using (SimpleStream s = new SimpleStream()) {
                byte[] buffer = new byte[100];
                s.SetLength(1000);
                IAsyncResult r = s.BeginRead(buffer, 0, buffer.Length, null, null);
                Assert.That(r, Is.Not.Null);
                Assert.That(s.EndRead(r), Is.EqualTo(100));
            }
        }

        [Test]
        public void BeginReadEndReadCallback()
        {
            using (ManualResetEvent e = new ManualResetEvent(false))
            using (SimpleStream s = new SimpleStream()) {
                int read = -1;

                byte[] buffer = new byte[100];
                s.SetLength(1000);
                bool rcNull = true;
                IAsyncResult r = s.BeginRead(buffer, 0, buffer.Length, (rc) => {
                    rcNull = rc == null;
                    if (rc != null) read = s.EndRead(rc);
                    e.Set();
                }, null);

                Assert.That(r, Is.Not.Null);
                Assert.That(e.WaitOne(5000), Is.True); // Ensure the callback is executed
                Assert.That(rcNull, Is.False);
                Assert.That(read, Is.EqualTo(buffer.Length));
            }
        }

        [Test]
        public void BeginReadNullBuffer()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(() => {
                    s.BeginRead(null, 0, 100, null, null);
                }, Throws.TypeOf<ArgumentNullException>());
            }
        }

        [Test]
        public void BeginReadNegativeOffset()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(() => {
                    byte[] buffer = new byte[100];
                    s.BeginRead(buffer, -1, 100, null, null);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void BeginReadNegativeCount()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(() => {
                    byte[] buffer = new byte[100];
                    s.BeginRead(buffer, 50, -1, null, null);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void BeginReadOutOfBounds()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(() => {
                    byte[] buffer = new byte[100];
                    s.BeginRead(buffer, 50, 51, null, null);
                }, Throws.TypeOf<ArgumentException>());
            }
        }

        [Test]
        public void BeginReadNullEnd()
        {
            using (SimpleStream s = new SimpleStream()) {
                byte[] buffer = new byte[100];
                IAsyncResult ia = s.BeginRead(buffer, 0, buffer.Length, null, null);
                Assert.That(ia, Is.Not.Null);
                Assert.That(() => {
                    s.EndRead(null);
                }, Throws.TypeOf<ArgumentNullException>());
                s.EndRead(ia);
            }
        }

#if NETCOREAPP || NET462_OR_GREATER && !NET40_LEGACY
        [Test]
        public async Task ReadAsync()
        {
            using (SimpleStream s = new SimpleStream()) {
                byte[] buffer = new byte[100];

                s.SetLength(1000);
                int read = await s.ReadAsync(buffer, 0, buffer.Length);
                Assert.That(read, Is.EqualTo(100));
                Assert.That(s.Length, Is.EqualTo(1000));
                Assert.That(s.Position, Is.EqualTo(100));
            }
        }

        [Test]
        public async Task ReadAsyncExact()
        {
            using (SimpleStream s = new SimpleStream()) {
                byte[] buffer = new byte[100];

                s.SetLength(100);
                int read = await s.ReadAsync(buffer, 0, buffer.Length);
                Assert.That(read, Is.EqualTo(100));
                Assert.That(s.Length, Is.EqualTo(100));
                Assert.That(s.Position, Is.EqualTo(100));
            }
        }

        [Test]
        public async Task ReadAsyncZero()
        {
            using (SimpleStream s = new SimpleStream()) {
                byte[] buffer = new byte[100];
                int read = await s.ReadAsync(buffer, 0, buffer.Length);
                Assert.That(read, Is.EqualTo(0));
                Assert.That(s.Length, Is.EqualTo(0));
                Assert.That(s.Position, Is.EqualTo(0));
            }
        }

        [Test]
        public async Task ReadAsyncZeroBytes()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(100);
                byte[] buffer = new byte[100];
                int read = await s.ReadAsync(buffer, 0, 0);
                Assert.That(read, Is.EqualTo(0));
                Assert.That(s.Length, Is.EqualTo(100));
                Assert.That(s.Position, Is.EqualTo(0));
            }
        }

        [Test]
        public void ReadAsyncNullBuffer()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(async () => {
                    _ = await s.ReadAsync(null, 0, 100);
                }, Throws.TypeOf<ArgumentNullException>());
            }
        }

        [Test]
        public void ReadAsyncNegativeOffset()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(async () => {
                    byte[] buffer = new byte[100];
                    _ = await s.ReadAsync(buffer, -1, 100);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void ReadAsyncNegativeCount()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(async () => {
                    byte[] buffer = new byte[100];
                    _ = await s.ReadAsync(buffer, 50, -1);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void ReadAsyncOutOfBounds()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(async () => {
                    byte[] buffer = new byte[100];
                    _ = await s.ReadAsync(buffer, 50, 51);
                }, Throws.TypeOf<ArgumentException>());
            }
        }

        [Test]
        public void ReadAsyncCancelled()
        {
            using (var cts = new CancellationTokenSource())
            using (SimpleStream s = new SimpleStream()) {
                cts.Cancel();
                Assert.That(async () => {
                    byte[] buffer = new byte[100];
                    _ = await s.ReadAsync(buffer, 0, buffer.Length, cts.Token);
                }, Throws.TypeOf<OperationCanceledException>());
            }
        }
#endif

#if NETCOREAPP
        [Test]
        public void ReadSpan()
        {
            using (SimpleStream s = new SimpleStream()) {
                Span<byte> buffer = stackalloc byte[100];

                s.SetLength(1000);
                int read = s.Read(buffer);
                Assert.That(read, Is.EqualTo(100));
                Assert.That(s.Length, Is.EqualTo(1000));
                Assert.That(s.Position, Is.EqualTo(100));
            }
        }

        [Test]
        public void ReadSpanExact()
        {
            using (SimpleStream s = new SimpleStream()) {
                Span<byte> buffer = stackalloc byte[100];

                s.SetLength(100);
                int read = s.Read(buffer);
                Assert.That(read, Is.EqualTo(100));
                Assert.That(s.Length, Is.EqualTo(100));
                Assert.That(s.Position, Is.EqualTo(100));
            }
        }

        [Test]
        public void ReadSpanZero()
        {
            using (SimpleStream s = new SimpleStream()) {
                Span<byte> buffer = stackalloc byte[100];
                int read = s.Read(buffer);
                Assert.That(read, Is.EqualTo(0));
                Assert.That(s.Length, Is.EqualTo(0));
                Assert.That(s.Position, Is.EqualTo(0));
            }
        }

        [Test]
        public void ReadSpanZeroBytes()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(100);
                Span<byte> buffer = stackalloc byte[100];
                int read = s.Read(buffer[0..0]);
                Assert.That(read, Is.EqualTo(0));
                Assert.That(s.Length, Is.EqualTo(100));
                Assert.That(s.Position, Is.EqualTo(0));
            }
        }

        [Test]
        public async Task ReadAsyncMem()
        {
            using (SimpleStream s = new SimpleStream()) {
                Memory<byte> buffer = new byte[100];

                s.SetLength(1000);
                int read = await s.ReadAsync(buffer);
                Assert.That(read, Is.EqualTo(100));
                Assert.That(s.Length, Is.EqualTo(1000));
                Assert.That(s.Position, Is.EqualTo(100));
            }
        }

        [Test]
        public async Task ReadAsyncMemExact()
        {
            using (SimpleStream s = new SimpleStream()) {
                Memory<byte> buffer = new byte[100];

                s.SetLength(100);
                int read = await s.ReadAsync(buffer);
                Assert.That(read, Is.EqualTo(100));
                Assert.That(s.Length, Is.EqualTo(100));
                Assert.That(s.Position, Is.EqualTo(100));
            }
        }

        [Test]
        public async Task ReadAsyncMemZero()
        {
            using (SimpleStream s = new SimpleStream()) {
                Memory<byte> buffer = new byte[100];
                int read = await s.ReadAsync(buffer);
                Assert.That(read, Is.EqualTo(0));
                Assert.That(s.Length, Is.EqualTo(0));
                Assert.That(s.Position, Is.EqualTo(0));
            }
        }

        [Test]
        public async Task ReadAsyncMemZeroBytes()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(100);
                Memory<byte> buffer = new byte[100];
                int read = await s.ReadAsync(buffer[0..0]);
                Assert.That(read, Is.EqualTo(0));
                Assert.That(s.Length, Is.EqualTo(100));
                Assert.That(s.Position, Is.EqualTo(0));
            }
        }

        [Test]
        public void ReadAsyncMemCancelled()
        {
            using (var cts = new CancellationTokenSource())
            using (SimpleStream s = new SimpleStream()) {
                cts.Cancel();
                Assert.That(async () => {
                    Memory<byte> buffer = new byte[100];
                    _ = await s.ReadAsync(buffer, cts.Token);
                }, Throws.TypeOf<OperationCanceledException>());
            }
        }
#endif

        [Test]
        public void CopyTo([Values(100, 2097152)] int length)
        {
            using (MemoryStream ms = new MemoryStream(length))
            using (SimpleStream ss = new SimpleStream()) {
                ss.SetLength(length);
                ss.CopyTo(ms);

                Assert.That(ms.Length, Is.EqualTo(length));
                Assert.That(ms.Position, Is.EqualTo(length));

                byte[] buffer = new byte[length];
                ms.Position = 0;
                int r = ms.Read(buffer, 0, buffer.Length);
                Assert.That(r, Is.EqualTo(length));
                // Much faster than `(buffer, Has.All.EqualTo(0))` from 1.6s to 4ms.
                Assert.That(Array.TrueForAll(buffer, b => b == 0), Is.True);
            }
        }

#if NETCOREAPP || NET462_OR_GREATER && !NET40_LEGACY
        [Test]
        public async Task CopyToAsync([Values(100, 2097152)] int length)
        {
            using (MemoryStream ms = new MemoryStream())
            using (SimpleStream ss = new SimpleStream()) {
                ss.SetLength(length);
                await ss.CopyToAsync(ms);

                Assert.That(ms.Length, Is.EqualTo(length));
                Assert.That(ms.Position, Is.EqualTo(length));

                byte[] buffer = new byte[length];
                ms.Position = 0;
                int r = ms.Read(buffer, 0, buffer.Length);
                Assert.That(r, Is.EqualTo(length));
                // Much faster than `(buffer, Has.All.EqualTo(0))` from 1.6s to 4ms.
                Assert.That(Array.TrueForAll(buffer, b => b == 0), Is.True);
            }
        }
#endif

        [Test]
        public void SeekStartEmpty()
        {
            using (SimpleStream s = new SimpleStream()) {
                long pos = s.Seek(100, SeekOrigin.Begin);
                Assert.That(pos, Is.EqualTo(100));
                Assert.That(s.Position, Is.EqualTo(100));
                Assert.That(s.Length, Is.EqualTo(100));
            }
        }

        [Test]
        public void SeekStart()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(1000);
                long pos = s.Seek(100, SeekOrigin.Begin);
                Assert.That(pos, Is.EqualTo(100));
                Assert.That(s.Position, Is.EqualTo(100));
                Assert.That(s.Length, Is.EqualTo(1000));
            }
        }

        [Test]
        public void SeekEndEmpty([Values(1, 100)] int seekEnd)
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(() => {
                    _ = s.Seek(seekEnd, SeekOrigin.End);
                }, Throws.TypeOf<ArgumentException>());
                Assert.That(s.Position, Is.EqualTo(0));
                Assert.That(s.Length, Is.EqualTo(0));
            }
        }

        [Test]
        public void SeekEnd([Values(1, 100)] int seekEnd)
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(1000);
                long pos = s.Seek(seekEnd, SeekOrigin.End);
                Assert.That(pos, Is.EqualTo(1000 - seekEnd));
                Assert.That(s.Position, Is.EqualTo(1000 - seekEnd));
                Assert.That(s.Length, Is.EqualTo(1000));
            }
        }

        [Test]
        public void SeekCurrent()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(1000);
                long pos = s.Seek(100, SeekOrigin.Current);
                Assert.That(pos, Is.EqualTo(100));
                Assert.That(s.Position, Is.EqualTo(100));
                Assert.That(s.Length, Is.EqualTo(1000));
            }
        }

        [Test]
        public void SeekCurrentEmpty()
        {
            using (SimpleStream s = new SimpleStream()) {
                long pos = s.Seek(100, SeekOrigin.Current);
                Assert.That(pos, Is.EqualTo(100));
                Assert.That(s.Position, Is.EqualTo(100));
                Assert.That(s.Length, Is.EqualTo(100));
            }
        }

        [Test]
        public void SeekCurrentMiddle()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(1000);
                s.Position = 500;
                long pos = s.Seek(100, SeekOrigin.Current);
                Assert.That(pos, Is.EqualTo(600));
                Assert.That(s.Position, Is.EqualTo(600));
                Assert.That(s.Length, Is.EqualTo(1000));
            }
        }

        [Test]
        public void SeekCurrentEnd()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(1000);
                s.Position = 1000;
                long pos = s.Seek(100, SeekOrigin.Current);
                Assert.That(pos, Is.EqualTo(1100));
                Assert.That(s.Position, Is.EqualTo(1100));
                Assert.That(s.Length, Is.EqualTo(1100));
            }
        }

        [Test]
        public void SeekCurrentExcessive()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(long.MaxValue);
                s.Position = long.MaxValue - 1;
                Assert.That(() => {
                    _ = s.Seek(100, SeekOrigin.Current);
                }, Throws.TypeOf<IOException>());
                Assert.That(s.Position, Is.EqualTo(long.MaxValue - 1));
                Assert.That(s.Length, Is.EqualTo(long.MaxValue));
            }
        }

        [Test]
        public void SeekInvalidEnum()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(long.MaxValue);
                s.Position = long.MaxValue - 1;
                Assert.That(() => {
                    _ = s.Seek(100, (SeekOrigin)100);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
                Assert.That(s.Position, Is.EqualTo(long.MaxValue - 1));
                Assert.That(s.Length, Is.EqualTo(long.MaxValue));
            }
        }

        [Test]
        public void SetLength()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(1000);
                Assert.That(s.Position, Is.EqualTo(0));
                Assert.That(s.Length, Is.EqualTo(1000));
            }
        }

        [Test]
        public void SetLengthTruncatePosition()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(1000);
                s.Position = 1000;
                Assert.That(s.Position, Is.EqualTo(1000));
                Assert.That(s.Length, Is.EqualTo(1000));

                s.SetLength(100);
                Assert.That(s.Position, Is.EqualTo(100));
                Assert.That(s.Length, Is.EqualTo(100));
            }
        }

        [Test]
        public void Write()
        {
            using (SimpleStream s = new SimpleStream()) {
                byte[] buffer = new byte[100];

                s.SetLength(1000);
                s.Write(buffer, 0, buffer.Length);
                Assert.That(s.Length, Is.EqualTo(1000));
                Assert.That(s.Position, Is.EqualTo(buffer.Length));
            }
        }

        [Test]
        public void WriteExact()
        {
            using (SimpleStream s = new SimpleStream()) {
                byte[] buffer = new byte[100];

                s.SetLength(buffer.Length);
                s.Write(buffer, 0, buffer.Length);
                Assert.That(s.Length, Is.EqualTo(buffer.Length));
                Assert.That(s.Position, Is.EqualTo(buffer.Length));
            }
        }

        [Test]
        public void WriteZeroBytes()
        {
            using (SimpleStream s = new SimpleStream()) {
                byte[] buffer = new byte[100];
                s.Write(buffer, 0, 0);
                Assert.That(s.Length, Is.EqualTo(0));
                Assert.That(s.Position, Is.EqualTo(0));
            }
        }

        [Test]
        public void WriteAtEnd()
        {
            using (SimpleStream s = new SimpleStream()) {
                byte[] buffer = new byte[100];
                s.Write(buffer, 0, buffer.Length);
                Assert.That(s.Length, Is.EqualTo(buffer.Length));
                Assert.That(s.Position, Is.EqualTo(buffer.Length));
            }
        }

        [Test]
        public void WriteNullBuffer()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(() => {
                    s.Write(null, 0, 100);
                }, Throws.TypeOf<ArgumentNullException>());
            }
        }

        [Test]
        public void WriteNegativeOffset()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(() => {
                    byte[] buffer = new byte[100];
                    s.Write(buffer, -1, 100);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void WriteNegativeCount()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(() => {
                    byte[] buffer = new byte[100];
                    s.Write(buffer, 50, -1);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void WriteOutOfBounds()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(() => {
                    byte[] buffer = new byte[100];
                    s.Write(buffer, 50, 51);
                }, Throws.TypeOf<ArgumentException>());
            }
        }

        [Test]
        public void WriteMassiveStream()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(long.MaxValue);
                byte[] buffer = new byte[100];
                s.Write(buffer, 0, buffer.Length);
                Assert.That(s.Position, Is.EqualTo(buffer.Length));
                Assert.That(s.Length, Is.EqualTo(long.MaxValue));
            }
        }

        [Test]
        public void WriteEndOfMassiveStream()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(long.MaxValue);
                s.Position = long.MaxValue - 1;
                Assert.That(() => {
                    byte[] buffer = new byte[100];
                    s.Write(buffer, 0, buffer.Length);
                }, Throws.TypeOf<IOException>());
                Assert.That(s.Position, Is.EqualTo(long.MaxValue - 1));
                Assert.That(s.Length, Is.EqualTo(long.MaxValue));
            }
        }

        [Test]
        public void WriteEndToMassiveStream()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(long.MaxValue - 1);
                s.Position = long.MaxValue - 1;
                byte[] buffer = new byte[1];
                s.Write(buffer, 0, buffer.Length);
                Assert.That(s.Position, Is.EqualTo(long.MaxValue));
                Assert.That(s.Length, Is.EqualTo(long.MaxValue));
            }
        }

        [Test]
        public void WriteByte()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(1000);
                s.WriteByte(0xFF);
                Assert.That(s.Length, Is.EqualTo(1000));
                Assert.That(s.Position, Is.EqualTo(1));
            }
        }

        [Test]
        public void WriteByteExact()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(100);
                s.Position = 99;
                s.WriteByte(0xFF);
                Assert.That(s.Length, Is.EqualTo(100));
                Assert.That(s.Position, Is.EqualTo(100));
            }
        }

        [Test]
        public void WriteByteAtEnd()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(100);
                s.Position = 100;
                s.WriteByte(0xFF);
                Assert.That(s.Length, Is.EqualTo(101));
                Assert.That(s.Position, Is.EqualTo(101));
            }
        }

        [Test]
        public void WriteByteEndToMassiveStream()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(long.MaxValue);
                s.Position = long.MaxValue - 1;
                s.WriteByte(0xFF);
                Assert.That(s.Position, Is.EqualTo(long.MaxValue));
                Assert.That(s.Length, Is.EqualTo(long.MaxValue));
            }
        }

        [Test]
        public void WriteByteEndOfMassiveStream()
        {
            using (SimpleStream s = new SimpleStream()) {
                s.SetLength(long.MaxValue);
                s.Position = long.MaxValue;
                Assert.That(() => {
                    s.WriteByte(0xFF);
                }, Throws.TypeOf<IOException>());
                Assert.That(s.Position, Is.EqualTo(long.MaxValue));
                Assert.That(s.Length, Is.EqualTo(long.MaxValue));
            }
        }

        [Test]
        public void BeginWriteEndWrite()
        {
            using (SimpleStream s = new SimpleStream()) {
                byte[] buffer = new byte[100];
                IAsyncResult r = s.BeginWrite(buffer, 0, buffer.Length, null, null);
                Assert.That(r, Is.Not.Null);
                s.EndWrite(r);
            }
        }

        [Test]
        public void BeginWriteEndWriteCallback()
        {
            using (ManualResetEvent e = new ManualResetEvent(false))
            using (SimpleStream s = new SimpleStream()) {
                byte[] buffer = new byte[100];
                bool rcNull = true;
                IAsyncResult r = s.BeginWrite(buffer, 0, buffer.Length, (rc) => {
                    rcNull = rc == null;
                    if (rc != null) s.EndWrite(rc);
                    e.Set();
                }, null);

                Assert.That(r, Is.Not.Null);
                Assert.That(e.WaitOne(5000), Is.True); // Ensure the callback is executed
                Assert.That(rcNull, Is.False);
            }
        }

        [Test]
        public void BeginWriteNullBuffer()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(() => {
                    s.BeginWrite(null, 0, 100, null, null);
                }, Throws.TypeOf<ArgumentNullException>());
            }
        }

        [Test]
        public void BeginWriteNegativeOffset()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(() => {
                    byte[] buffer = new byte[100];
                    s.BeginWrite(buffer, -1, 100, null, null);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void BeginWriteNegativeCount()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(() => {
                    byte[] buffer = new byte[100];
                    s.BeginWrite(buffer, 50, -1, null, null);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void BeginWriteOutOfBounds()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(() => {
                    byte[] buffer = new byte[100];
                    s.BeginWrite(buffer, 50, 51, null, null);
                }, Throws.TypeOf<ArgumentException>());
            }
        }

        [Test]
        public void BeginWriteNullEnd()
        {
            using (SimpleStream s = new SimpleStream()) {
                byte[] buffer = new byte[100];
                IAsyncResult ia = s.BeginWrite(buffer, 0, buffer.Length, null, null);
                Assert.That(ia, Is.Not.Null);
                Assert.That(() => {
                    s.EndWrite(null);
                }, Throws.TypeOf<ArgumentNullException>());
                s.EndWrite(ia);
            }
        }

#if NETCOREAPP || NET462_OR_GREATER && !NET40_LEGACY
        [Test]
        public async Task WriteAsync()
        {
            using (SimpleStream s = new SimpleStream()) {
                byte[] buffer = new byte[100];

                s.SetLength(1000);
                await s.WriteAsync(buffer, 0, buffer.Length);
                Assert.That(s.Length, Is.EqualTo(1000));
                Assert.That(s.Position, Is.EqualTo(buffer.Length));
            }
        }

        [Test]
        public async Task WriteAsyncExact()
        {
            using (SimpleStream s = new SimpleStream()) {
                byte[] buffer = new byte[100];

                s.SetLength(100);
                await s.WriteAsync(buffer, 0, buffer.Length);
                Assert.That(s.Length, Is.EqualTo(buffer.Length));
                Assert.That(s.Position, Is.EqualTo(buffer.Length));
            }
        }

        [Test]
        public async Task WriteAsyncZeroBytes()
        {
            using (SimpleStream s = new SimpleStream()) {
                byte[] buffer = new byte[100];
                await s.WriteAsync(buffer, 0, 0);
                Assert.That(s.Length, Is.EqualTo(0));
                Assert.That(s.Position, Is.EqualTo(0));
            }
        }

        [Test]
        public async Task WriteAsyncAtEnd()
        {
            using (SimpleStream s = new SimpleStream()) {
                byte[] buffer = new byte[100];
                await s.WriteAsync(buffer, 0, buffer.Length);
                Assert.That(s.Length, Is.EqualTo(buffer.Length));
                Assert.That(s.Position, Is.EqualTo(buffer.Length));
            }
        }

        [Test]
        public void WriteAsyncNullBuffer()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(async () => {
                    await s.WriteAsync(null, 0, 100);
                }, Throws.TypeOf<ArgumentNullException>());
            }
        }

        [Test]
        public void WriteAsyncNegativeOffset()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(async () => {
                    byte[] buffer = new byte[100];
                    await s.WriteAsync(buffer, -1, 100);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void WriteAsyncNegativeCount()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(async () => {
                    byte[] buffer = new byte[100];
                    await s.WriteAsync(buffer, 50, -1);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void WriteAsyncOutOfBounds()
        {
            using (SimpleStream s = new SimpleStream()) {
                Assert.That(async () => {
                    byte[] buffer = new byte[100];
                    await s.WriteAsync(buffer, 50, 51);
                }, Throws.TypeOf<ArgumentException>());
            }
        }

        [Test]
        public void WriteAsyncCancelled()
        {
            using (var cts = new CancellationTokenSource())
            using (SimpleStream s = new SimpleStream()) {
                cts.Cancel();
                Assert.That(async () => {
                    byte[] buffer = new byte[100];
                    await s.WriteAsync(buffer, 0, buffer.Length, cts.Token);
                }, Throws.TypeOf<OperationCanceledException>());
            }
        }
#endif

#if NETCOREAPP
        [Test]
        public void WriteSpan()
        {
            using (SimpleStream s = new SimpleStream()) {
                ReadOnlySpan<byte> buffer = stackalloc byte[100];

                s.SetLength(1000);
                s.Write(buffer);
                Assert.That(s.Length, Is.EqualTo(1000));
                Assert.That(s.Position, Is.EqualTo(100));
            }
        }

        [Test]
        public void WriteSpanExact()
        {
            using (SimpleStream s = new SimpleStream()) {
                ReadOnlySpan<byte> buffer = stackalloc byte[100];

                s.SetLength(100);
                s.Write(buffer);
                Assert.That(s.Length, Is.EqualTo(100));
                Assert.That(s.Position, Is.EqualTo(100));
            }
        }

        [Test]
        public void WriteSpanZeroBytes()
        {
            using (SimpleStream s = new SimpleStream()) {
                ReadOnlySpan<byte> buffer = stackalloc byte[100];
                s.Write(buffer[0..0]);
                Assert.That(s.Length, Is.EqualTo(0));
                Assert.That(s.Position, Is.EqualTo(0));
            }
        }

        [Test]
        public void WriteSpanAtEnd()
        {
            using (SimpleStream s = new SimpleStream()) {
                ReadOnlySpan<byte> buffer = stackalloc byte[100];
                s.Write(buffer);
                Assert.That(s.Length, Is.EqualTo(100));
                Assert.That(s.Position, Is.EqualTo(100));
            }
        }

        [Test]
        public async Task WriteAsyncMem()
        {
            using (SimpleStream s = new SimpleStream()) {
                ReadOnlyMemory<byte> buffer = new byte[100];

                s.SetLength(1000);
                await s.WriteAsync(buffer);
                Assert.That(s.Length, Is.EqualTo(1000));
                Assert.That(s.Position, Is.EqualTo(buffer.Length));
            }
        }

        [Test]
        public async Task WriteAsyncMemExact()
        {
            using (SimpleStream s = new SimpleStream()) {
                ReadOnlyMemory<byte> buffer = new byte[100];

                s.SetLength(100);
                await s.WriteAsync(buffer);
                Assert.That(s.Length, Is.EqualTo(buffer.Length));
                Assert.That(s.Position, Is.EqualTo(buffer.Length));
            }
        }

        [Test]
        public async Task WriteAsyncMemZeroBytes()
        {
            using (SimpleStream s = new SimpleStream()) {
                ReadOnlyMemory<byte> buffer = new byte[100];
                await s.WriteAsync(buffer[0..0]);
                Assert.That(s.Length, Is.EqualTo(0));
                Assert.That(s.Position, Is.EqualTo(0));
            }
        }

        [Test]
        public async Task WriteAsyncMemAtEnd()
        {
            using (SimpleStream s = new SimpleStream()) {
                ReadOnlyMemory<byte> buffer = new byte[100];
                await s.WriteAsync(buffer);
                Assert.That(s.Length, Is.EqualTo(buffer.Length));
                Assert.That(s.Position, Is.EqualTo(buffer.Length));
            }
        }

        [Test]
        public void WriteAsyncMemCancelled()
        {
            using (var cts = new CancellationTokenSource())
            using (SimpleStream s = new SimpleStream()) {
                cts.Cancel();
                Assert.That(async () => {
                    ReadOnlyMemory<byte> buffer = new byte[100];
                    await s.WriteAsync(buffer, cts.Token);
                }, Throws.TypeOf<OperationCanceledException>());
            }
        }
#endif

        // TODO: BeginWrite/EndWrite

        [Test]
        public void Dispose()
        {
            SimpleStream s = new SimpleStream();
            Assert.That(s.IsDisposed, Is.False);

            s.Dispose();
            Assert.That(s.IsDisposed, Is.True);
        }

        [Test]
        public void DisposeTwice()
        {
            SimpleStream s = new SimpleStream();
            Assert.That(s.IsDisposed, Is.False);

            s.Dispose();
            Assert.That(s.IsDisposed, Is.True);

            s.Dispose();
            Assert.That(s.IsDisposed, Is.True);
        }
    }
}
