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
    public class WriteOnlyStreamTest
    {
        private static WriteOnlyStream GetStream(bool withStream)
        {
            if (withStream)
                return new WriteOnlyStream(new MemoryStream());
            return new WriteOnlyStream();
        }

        [Test]
        public void NullBaseStream()
        {
            Assert.That(() => {
                _ = new WriteOnlyStream(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NullBaseStream([Values(false, true)] bool ownsStream)
        {
            Assert.That(() => {
                _ = new WriteOnlyStream(null, ownsStream);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void DefaultStream()
        {
            using (WriteOnlyStream s = new WriteOnlyStream()) {
                Assert.That(s.CanRead, Is.False);
                Assert.That(s.CanWrite, Is.True);
                Assert.That(s.CanSeek, Is.False);
                Assert.That(s.CanTimeout, Is.False);
                Assert.That(s.Length, Is.EqualTo(0));
            }
        }

        [Test]
        public void MemoryStream()
        {
            using (WriteOnlyStream s = GetStream(true)) {
                Assert.That(s.CanRead, Is.False);
                Assert.That(s.CanWrite, Is.True);
                Assert.That(s.CanSeek, Is.False);
                Assert.That(s.CanTimeout, Is.False);
                Assert.That(s.Length, Is.EqualTo(0));
            }
        }

        [Test]
        public void StreamReadTimeoutGet([Values(false, true)] bool withStream)
        {
            using (WriteOnlyStream s = GetStream(withStream)) {
                Assert.That(() => {
                    _ = s.ReadTimeout;
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public void StreamReadTimeoutSet([Values(false, true)] bool withStream)
        {
            using (WriteOnlyStream s = GetStream(withStream)) {
                Assert.That(() => {
                    s.ReadTimeout = 0;
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public void TestWriteTimeout([Values(0, -1, 100)] int timeout)
        {
            using (SimpleStream ss = new SimpleStream())
            using (WriteOnlyStream s = new WriteOnlyStream(ss)) {
                s.WriteTimeout = timeout;
                Assert.That(s.WriteTimeout, Is.EqualTo(timeout));
                Assert.That(ss.WriteTimeout, Is.EqualTo(timeout));
            }
        }

        [Test]
        public void DefaultStreamWriteTimeoutGet()
        {
            using (WriteOnlyStream s = new WriteOnlyStream()) {
                Assert.That(() => {
                    _ = s.WriteTimeout;
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public void DefaultStreamWriteTimeoutSet()
        {
            using (WriteOnlyStream s = new WriteOnlyStream()) {
                Assert.That(() => {
                    s.WriteTimeout = 0;
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public void DefaultStreamGetPosition()
        {
            using (WriteOnlyStream s = new WriteOnlyStream()) {
                Assert.That(s.Position, Is.EqualTo(0));
            }
        }

        [Test]
        public void MemoryStreamGetPosition()
        {
            using (MemoryStream m = new MemoryStream())
            using (WriteOnlyStream s = new WriteOnlyStream(m)) {
                byte[] buffer = new byte[100];
                m.Write(buffer, 0, buffer.Length);
                Assert.That(s.Position, Is.EqualTo(100));
            }
        }

        [Test]
        public void StreamSetPosition([Values(false, true)] bool withStream)
        {
            using (WriteOnlyStream s = GetStream(withStream)) {
                Assert.That(() => {
                    s.Position = 0;
                }, Throws.TypeOf<NotSupportedException>());
            }
        }

        [Test]
        public void StreamFlush([Values(false, true)] bool withStream)
        {
            using (WriteOnlyStream s = GetStream(withStream)) {
                Assert.That(() => {
                    s.Flush();
                }, Throws.Nothing);
            }
        }

#if NETCOREAPP || NET462_OR_GREATER && !NET40_LEGACY
        [Test]
        public void StreamFlushAsync([Values(false, true)] bool withStream)
        {
            using (WriteOnlyStream s = GetStream(withStream)) {
                Assert.That(async () => {
                    await s.FlushAsync();
                }, Throws.Nothing);
            }
        }
#endif

        [Test]
        public void StreamRead([Values(false, true)] bool withStream)
        {
            using (WriteOnlyStream s = GetStream(withStream)) {
                Assert.That(() => {
                    byte[] buffer = new byte[100];
                    s.Read(buffer, 0, 100);
                }, Throws.TypeOf<NotSupportedException>());
            }
        }

#if NETCOREAPP
        [Test]
        public void StreamReadSpan([Values(false, true)] bool withStream)
        {
            using (WriteOnlyStream s = GetStream(withStream)) {
                Assert.That(() => {
                    Span<byte> buffer = stackalloc byte[100];
                    s.Read(buffer);
                }, Throws.TypeOf<NotSupportedException>());
            }
        }
#endif

#if NETCOREAPP || NET462_OR_GREATER && !NET40_LEGACY
        [Test]
        [SuppressMessage("Performance", "CA1835:Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'", Justification = "TestCase")]
        public void StreamReadAsync([Values(false, true)] bool withStream)
        {
            using (var cancelSource = new CancellationTokenSource())
            using (WriteOnlyStream s = GetStream(withStream)) {
                Assert.That(async () => {
                    byte[] buffer = new byte[100];
                    await s.ReadAsync(buffer, 0, buffer.Length, cancelSource.Token);
                }, Throws.TypeOf<NotSupportedException>());
            }
        }
#endif

#if NETCOREAPP
        [Test]
        public void StreamReadAsyncMemory([Values(false, true)] bool withStream)
        {
            using (var cancelSource = new CancellationTokenSource())
            using (WriteOnlyStream s = GetStream(withStream)) {
                Assert.That(async () => {
                    Memory<byte> buffer = new byte[100];
                    await s.ReadAsync(buffer, cancelSource.Token);
                }, Throws.TypeOf<NotSupportedException>());
            }
        }
#endif

        [Test]
        public void StreamReadByte([Values(false, true)] bool withStream)
        {
            using (WriteOnlyStream s = GetStream(withStream)) {
                Assert.That(() => {
                    _ = s.ReadByte();
                }, Throws.TypeOf<NotSupportedException>());
            }
        }

        [Test]
        public void StreamBeginRead([Values(false, true)] bool withStream)
        {
            using (WriteOnlyStream s = GetStream(withStream)) {
                Assert.That(() => {
                    byte[] buffer = new byte[100];
                    _ = s.BeginRead(buffer, 0, buffer.Length, null, null);
                }, Throws.TypeOf<NotSupportedException>());
            }
        }

        [Test]
        public void StreamEndRead([Values(false, true)] bool withStream)
        {
            using (WriteOnlyStream s = GetStream(withStream)) {
                Assert.That(() => {
                    s.EndRead(null);
                }, Throws.TypeOf<NotSupportedException>());
            }
        }

        [Test]
        public void StreamCopyTo([Values(false, true)] bool withStream)
        {
            using (Stream d = new MemoryStream())
            using (WriteOnlyStream s = GetStream(withStream)) {
                Assert.That(() => {
                    s.CopyTo(d);
                }, Throws.TypeOf<NotSupportedException>());
            }
        }

#if NETCOREAPP || NET462_OR_GREATER && !NET40_LEGACY
        [Test]
        public void StreamCopyToAsync([Values(false, true)] bool withStream)
        {
            using (Stream d = new MemoryStream())
            using (WriteOnlyStream s = GetStream(withStream)) {
                Assert.That(async () => {
                    await s.CopyToAsync(d);
                }, Throws.TypeOf<NotSupportedException>());
            }
        }
#endif

        [Test]
        public void StreamSeek(
            [Values(false, true)] bool withStream,
            [Values(SeekOrigin.Begin, SeekOrigin.Current, SeekOrigin.End)] SeekOrigin seek)
        {
            using (WriteOnlyStream s = GetStream(withStream)) {
                Assert.That(() => {
                    s.Seek(0, seek);
                }, Throws.TypeOf<NotSupportedException>());
            }
        }

        [Test]
        public void StreamSetLength([Values(false, true)] bool withStream)
        {
            using (WriteOnlyStream s = GetStream(withStream)) {
                Assert.That(() => {
                    s.SetLength(100);
                }, Throws.TypeOf<NotSupportedException>());
            }
        }

        [Test]
        public void StreamWrite([Values(false, true)] bool withStream)
        {
            using (WriteOnlyStream s = GetStream(withStream)) {
                byte[] buffer = new byte[100];
                s.Write(buffer, 0, buffer.Length);
                if (withStream) {
                    Assert.That(s.BaseStream.Position, Is.EqualTo(100));
                    Assert.That(s.BaseStream.Length, Is.EqualTo(100));
                    Assert.That(s.Position, Is.EqualTo(100));
                    Assert.That(s.Length, Is.EqualTo(100));
                } else {
                    Assert.That(s.Position, Is.EqualTo(0));
                    Assert.That(s.Length, Is.EqualTo(0));
                }
            }
        }

        [Test]
        public void StreamWriteZeroBytes([Values(false, true)] bool withStream)
        {
            using (WriteOnlyStream s = GetStream(withStream)) {
                byte[] buffer = new byte[100];
                s.Write(buffer, 0, 0);
                if (withStream) {
                    Assert.That(s.BaseStream.Position, Is.EqualTo(0));
                    Assert.That(s.BaseStream.Length, Is.EqualTo(0));
                }
                Assert.That(s.Position, Is.EqualTo(0));
                Assert.That(s.Length, Is.EqualTo(0));
            }
        }

        [Test]
        public void DefaultStreamWriteNullBuffer()
        {
            using (WriteOnlyStream s = new WriteOnlyStream()) {
                Assert.That(() => {
                    s.Write(null, 0, 100);
                }, Throws.TypeOf<ArgumentNullException>());
            }
        }

        [Test]
        public void DefaultStreamWriteNegativeOffset()
        {
            using (WriteOnlyStream s = new WriteOnlyStream()) {
                Assert.That(() => {
                    byte[] buffer = new byte[100];
                    s.Write(buffer, -1, 100);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void DefaultStreamWriteNegativeCount()
        {
            using (WriteOnlyStream s = new WriteOnlyStream()) {
                Assert.That(() => {
                    byte[] buffer = new byte[100];
                    s.Write(buffer, 50, -1);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void DefaultStreamWriteOutOfBounds()
        {
            using (WriteOnlyStream s = new WriteOnlyStream()) {
                Assert.That(() => {
                    byte[] buffer = new byte[100];
                    s.Write(buffer, 50, 51);
                }, Throws.TypeOf<ArgumentException>());
            }
        }

#if NETCOREAPP
        [Test]
        public void StreamWriteSpan([Values(false, true)] bool withStream)
        {
            using (WriteOnlyStream s = GetStream(withStream)) {
                ReadOnlySpan<byte> buffer = stackalloc byte[100];
                s.Write(buffer);
                if (withStream) {
                    Assert.That(s.BaseStream.Position, Is.EqualTo(100));
                    Assert.That(s.BaseStream.Length, Is.EqualTo(100));
                    Assert.That(s.Position, Is.EqualTo(100));
                    Assert.That(s.Length, Is.EqualTo(100));
                } else {
                    Assert.That(s.Position, Is.EqualTo(0));
                    Assert.That(s.Length, Is.EqualTo(0));
                }
            }
        }
#endif

#if NETCOREAPP || NET462_OR_GREATER && !NET40_LEGACY
        [Test]
        [SuppressMessage("Performance", "CA1835:Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'", Justification = "TestCase")]
        public async Task StreamWriteAsync([Values(false, true)] bool withStream)
        {
            using (var cancelSource = new CancellationTokenSource())
            using (WriteOnlyStream s = GetStream(withStream)) {
                byte[] buffer = new byte[100];
                await s.WriteAsync(buffer, 0, buffer.Length, cancelSource.Token);
                if (withStream) {
                    Assert.That(s.BaseStream.Position, Is.EqualTo(100));
                    Assert.That(s.BaseStream.Length, Is.EqualTo(100));
                    Assert.That(s.Position, Is.EqualTo(100));
                    Assert.That(s.Length, Is.EqualTo(100));
                } else {
                    Assert.That(s.Position, Is.EqualTo(0));
                    Assert.That(s.Length, Is.EqualTo(0));
                }
            }
        }

        [Test]
        [SuppressMessage("Performance", "CA1835:Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'", Justification = "TestCase")]
        public void DefaultStreamWriteAsyncCancelled()
        {
            using (var cancelSource = new CancellationTokenSource())
            using (WriteOnlyStream s = new WriteOnlyStream()) {
                cancelSource.Cancel();
                byte[] buffer = new byte[100];
                Assert.That(async () => {
                    await s.WriteAsync(buffer, 0, buffer.Length, cancelSource.Token);
                }, Throws.TypeOf<OperationCanceledException>());
            }
        }

        [Test]
        [SuppressMessage("Performance", "CA1835:Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'", Justification = "TestCase")]
        public void DefaultStreamWriteAsyncNullBuffer()
        {
            using (var cancelSource = new CancellationTokenSource())
            using (WriteOnlyStream s = new WriteOnlyStream()) {
                Assert.That(async () => {
                    await s.WriteAsync(null, 0, 100, cancelSource.Token);
                }, Throws.TypeOf<ArgumentNullException>());
            }
        }

        [Test]
        [SuppressMessage("Performance", "CA1835:Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'", Justification = "TestCase")]
        public void DefaultStreamWriteAsyncNegativeOffset()
        {
            using (var cancelSource = new CancellationTokenSource())
            using (WriteOnlyStream s = new WriteOnlyStream()) {
                Assert.That(async () => {
                    byte[] buffer = new byte[100];
                    await s.WriteAsync(buffer, -1, 100, cancelSource.Token);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        [SuppressMessage("Performance", "CA1835:Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'", Justification = "TestCase")]
        public void DefaultStreamWriteAsyncNegativeCount()
        {
            using (var cancelSource = new CancellationTokenSource())
            using (WriteOnlyStream s = new WriteOnlyStream()) {
                Assert.That(async () => {
                    byte[] buffer = new byte[100];
                    await s.WriteAsync(buffer, 50, -1, cancelSource.Token);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        [SuppressMessage("Performance", "CA1835:Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'", Justification = "TestCase")]
        public void DefaultStreamWriteAsyncOutOfBounds()
        {
            using (var cancelSource = new CancellationTokenSource())
            using (WriteOnlyStream s = new WriteOnlyStream()) {
                Assert.That(async () => {
                    byte[] buffer = new byte[100];
                    await s.WriteAsync(buffer, 50, 51, cancelSource.Token);
                }, Throws.TypeOf<ArgumentException>());
            }
        }
#endif

#if NETCOREAPP
        [Test]
        public async Task StreamWriteAsyncMemory([Values(false, true)] bool withStream)
        {
            using (var cancelSource = new CancellationTokenSource())
            using (WriteOnlyStream s = GetStream(withStream)) {
                ReadOnlyMemory<byte> buffer = new byte[100];
                await s.WriteAsync(buffer, cancelSource.Token);
                if (withStream) {
                    Assert.That(s.BaseStream.Position, Is.EqualTo(100));
                    Assert.That(s.BaseStream.Length, Is.EqualTo(100));
                    Assert.That(s.Position, Is.EqualTo(100));
                    Assert.That(s.Length, Is.EqualTo(100));
                } else {
                    Assert.That(s.Position, Is.EqualTo(0));
                    Assert.That(s.Length, Is.EqualTo(0));
                }
            }
        }

        [Test]
        public void DefaultStreamWriteAsyncMemoryCancelled()
        {
            using (var cancelSource = new CancellationTokenSource())
            using (WriteOnlyStream s = new WriteOnlyStream()) {
                cancelSource.Cancel();
                ReadOnlyMemory<byte> buffer = new byte[100];
                Assert.That(async () => {
                    await s.WriteAsync(buffer, cancelSource.Token);
                }, Throws.TypeOf<OperationCanceledException>());
            }
        }
#endif

        [Test]
        public void StreamWriteByte([Values(false, true)] bool withStream)
        {
            using (WriteOnlyStream s = GetStream(withStream)) {
                s.WriteByte(0x00);
                if (withStream) {
                    Assert.That(s.BaseStream.Position, Is.EqualTo(1));
                    Assert.That(s.BaseStream.Length, Is.EqualTo(1));
                    Assert.That(s.Position, Is.EqualTo(1));
                    Assert.That(s.Length, Is.EqualTo(1));
                } else {
                    Assert.That(s.Position, Is.EqualTo(0));
                    Assert.That(s.Length, Is.EqualTo(0));
                }
            }
        }

        [Test]
        public void StreamBeginWriteEndWrite([Values(false, true)] bool withStream)
        {
            using (WriteOnlyStream s = GetStream(withStream)) {
                byte[] buffer = new byte[100];
                IAsyncResult r = s.BeginWrite(buffer, 0, buffer.Length, null, null);
                Assert.That(r, Is.Not.Null);
                s.EndWrite(r);
            }
        }

        [Test]
        public void StreamBeginWriteEndWriteCallback([Values(false, true)] bool withStream)
        {
            using (ManualResetEvent e = new ManualResetEvent(false))
            using (WriteOnlyStream s = GetStream(withStream)) {
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
        public void DefaultStreamBeginWriteNullBuffer()
        {
            using (WriteOnlyStream s = new WriteOnlyStream()) {
                Assert.That(() => {
                    s.BeginWrite(null, 0, 100, null, null);
                }, Throws.TypeOf<ArgumentNullException>());
            }
        }

        [Test]
        public void DefaultStreamBeginWriteNegativeOffset()
        {
            using (WriteOnlyStream s = new WriteOnlyStream()) {
                Assert.That(() => {
                    byte[] buffer = new byte[100];
                    s.BeginWrite(buffer, -1, 100, null, null);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void DefaultStreamBeginWriteNegativeCount()
        {
            using (WriteOnlyStream s = new WriteOnlyStream()) {
                Assert.That(() => {
                    byte[] buffer = new byte[100];
                    s.BeginWrite(buffer, 50, -1, null, null);
                }, Throws.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void DefaultStreamBeginWriteOutOfBounds()
        {
            using (WriteOnlyStream s = new WriteOnlyStream()) {
                Assert.That(() => {
                    byte[] buffer = new byte[100];
                    s.BeginWrite(buffer, 50, 51, null, null);
                }, Throws.TypeOf<ArgumentException>());
            }
        }

        [Test]
        public void DefaultStreamBeginWriteNullEnd()
        {
            using (WriteOnlyStream s = new WriteOnlyStream()) {
                byte[] buffer = new byte[100];
                IAsyncResult ia = s.BeginWrite(buffer, 0, buffer.Length, null, null);
                Assert.That(ia, Is.Not.Null);
                Assert.That(() => {
                    s.EndWrite(null);
                }, Throws.TypeOf<ArgumentNullException>());
                s.EndWrite(ia);
            }
        }

        [Test]
        public void DisposeTwice()
        {
            WriteOnlyStream s = new WriteOnlyStream();
            Assert.That(() => {
                s.Dispose();
                s.Dispose();
            }, Throws.Nothing);
        }

        [Test]
        public void DisposeOwnedStream()
        {
            SimpleStream ss = new SimpleStream();
            WriteOnlyStream s = new WriteOnlyStream(ss, true);
            Assert.That(ss.IsDisposed, Is.False);
            s.Dispose();
            Assert.That(ss.IsDisposed, Is.True);
        }
    }
}
