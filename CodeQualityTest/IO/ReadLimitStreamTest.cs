namespace RJCP.CodeQuality.IO
{
    using System;
    using System.Threading;
    using NUnit.Framework;

    [TestFixture]
    public class ReadLimitStreamTest
    {
        [Test]
        public void SimpleStreamWrapper()
        {
            using (SimpleStream s = new())
            using (ReadLimitStream r = new(s)) {
                Assert.That(r.CanRead, Is.True);
                Assert.That(r.CanWrite, Is.True);
                Assert.That(r.CanSeek, Is.True);
                Assert.That(r.CanTimeout, Is.True);
                Assert.That(r.Length, Is.EqualTo(0));
                Assert.That(r.Position, Is.EqualTo(0));
                Assert.That(r.WriteTimeout, Is.EqualTo(Timeout.Infinite));
                Assert.That(r.ReadTimeout, Is.EqualTo(Timeout.Infinite));
            }
        }

        private static byte[] GetRandomBytes(int length)
        {
            Random rnd = new();
            byte[] initBuffer = new byte[length];
            rnd.NextBytes(initBuffer);
            return initBuffer;
        }

        [Test]
        public void ReadFixedByteBuffer()
        {
            byte[] initBuffer = GetRandomBytes(65536);

            // No max read size is given, so it is the maximum possible.
            using (ReadLimitStream r = new(initBuffer)) {
                Assert.That(r.Position, Is.EqualTo(0));
                Assert.That(r.Length, Is.EqualTo(initBuffer.Length));

                int length = 0;
                byte[] buffer = new byte[initBuffer.Length];
                while (length < initBuffer.Length) {
                    int read = r.Read(buffer, length, buffer.Length - length);
                    length += read;
                }
                Assert.That(buffer, Is.EqualTo(initBuffer));
            }
        }

        [Test]
        public void ReadFixedByteBufferMaxLength()
        {
            byte[] initBuffer = GetRandomBytes(65536);

            // No max read size is given, so it is the maximum possible.
            using (ReadLimitStream r = new(initBuffer, 128)) {
                Assert.That(r.Position, Is.EqualTo(0));
                Assert.That(r.Length, Is.EqualTo(initBuffer.Length));

                int length = 0;
                byte[] buffer = new byte[initBuffer.Length];
                while (length < initBuffer.Length) {
                    int read = r.Read(buffer, length, buffer.Length - length);
                    Assert.That(read, Is.GreaterThanOrEqualTo(1).And.LessThanOrEqualTo(128));
                    length += read;
                }
                Assert.That(buffer, Is.EqualTo(initBuffer));
            }
        }

        [Test]
        public void ReadFixedByteBufferLengthRange()
        {
            byte[] initBuffer = GetRandomBytes(4194304);

            // No max read size is given, so it is the maximum possible.
            using (ReadLimitStream r = new(initBuffer, 64, 128)) {
                Assert.That(r.Position, Is.EqualTo(0));
                Assert.That(r.Length, Is.EqualTo(initBuffer.Length));

                int length = 0;
                byte[] buffer = new byte[initBuffer.Length];
                while (length < initBuffer.Length) {
                    int read = r.Read(buffer, length, buffer.Length - length);
                    Assert.That(read, Is.GreaterThanOrEqualTo(Math.Min(buffer.Length - length, 64)).And.LessThanOrEqualTo(128));
                    length += read;
                }
                Assert.That(buffer, Is.EqualTo(initBuffer));
            }
        }

        [Test]
        public void ReadFixedByteBufferLengthSequence()
        {
            byte[] initBuffer = GetRandomBytes(65536);
            int[] sequence = new int[] { 100, 200 };
            // No max read size is given, so it is the maximum possible.
            using (ReadLimitStream r = new(initBuffer, sequence)) {
                Assert.That(r.Position, Is.EqualTo(0));
                Assert.That(r.Length, Is.EqualTo(initBuffer.Length));

                int length = 0;
                byte[] buffer = new byte[initBuffer.Length];
                while (length < initBuffer.Length) {
                    int read = r.Read(buffer, length, buffer.Length - length);

                    int expectedRead = Math.Min(200, buffer.Length - length);
                    Assert.That(read, Is.EqualTo(100).Or.EqualTo(200).Or.EqualTo(expectedRead));
                    length += read;
                }
                Assert.That(buffer, Is.EqualTo(initBuffer));
            }
        }

        [Test]
        public void ReadSimpleStream([Values(128, 65536, 4194300, 4194304)] int sourceLength)
        {
            using (SimpleStream s = new())
            using (ReadLimitStream r = new(s)) {
                s.SetLength(sourceLength);

                byte[] buffer = new byte[65536];
                int length = 0;
                while (length < sourceLength) {
                    int read = r.Read(buffer, 0, buffer.Length);

                    int expectedRead = Math.Min(buffer.Length, sourceLength - length);
                    Assert.That(read, Is.EqualTo(expectedRead));
                    length += read;
                }
            }
        }

        [Test]
        public void DisposeNoOwnStream1()
        {
            SimpleStream s = new();
            ReadLimitStream r = new(s);
            Assert.That(s.IsDisposed, Is.False);
            Assert.That(r.IsDisposed, Is.False);
            r.Dispose();
            Assert.That(s.IsDisposed, Is.False);
            Assert.That(r.IsDisposed, Is.True);
        }

        [Test]
        public void DisposeOwnStream1()
        {
            SimpleStream s = new();
            ReadLimitStream r = new(s, true);
            Assert.That(s.IsDisposed, Is.False);
            Assert.That(r.IsDisposed, Is.False);
            r.Dispose();
            Assert.That(s.IsDisposed, Is.True);
            Assert.That(r.IsDisposed, Is.True);
        }

        [Test]
        public void DisposeNoOwnStream2()
        {
            SimpleStream s = new();
            ReadLimitStream r = new(s, 10);
            Assert.That(s.IsDisposed, Is.False);
            Assert.That(r.IsDisposed, Is.False);
            r.Dispose();
            Assert.That(s.IsDisposed, Is.False);
            Assert.That(r.IsDisposed, Is.True);
        }

        [Test]
        public void DisposeOwnStream2()
        {
            SimpleStream s = new();
            ReadLimitStream r = new(s, 10, true);
            Assert.That(s.IsDisposed, Is.False);
            Assert.That(r.IsDisposed, Is.False);
            r.Dispose();
            Assert.That(s.IsDisposed, Is.True);
            Assert.That(r.IsDisposed, Is.True);
        }

        [Test]
        public void DisposeNoOwnStream3()
        {
            SimpleStream s = new();
            ReadLimitStream r = new(s, 10, 20);
            Assert.That(s.IsDisposed, Is.False);
            Assert.That(r.IsDisposed, Is.False);
            r.Dispose();
            Assert.That(s.IsDisposed, Is.False);
            Assert.That(r.IsDisposed, Is.True);
        }

        [Test]
        public void DisposeOwnStream3()
        {
            SimpleStream s = new();
            ReadLimitStream r = new(s, 10, 20, true);
            Assert.That(s.IsDisposed, Is.False);
            Assert.That(r.IsDisposed, Is.False);
            r.Dispose();
            Assert.That(s.IsDisposed, Is.True);
            Assert.That(r.IsDisposed, Is.True);
        }

        [Test]
        public void DisposeNoOwnStream4()
        {
            SimpleStream s = new();
            ReadLimitStream r = new(s, new int[] { 10 });
            Assert.That(s.IsDisposed, Is.False);
            Assert.That(r.IsDisposed, Is.False);
            r.Dispose();
            Assert.That(s.IsDisposed, Is.False);
            Assert.That(r.IsDisposed, Is.True);
        }

        [Test]
        public void DisposeOwnStream4()
        {
            SimpleStream s = new();
            ReadLimitStream r = new(s, new int[] { 10 }, true);
            Assert.That(s.IsDisposed, Is.False);
            Assert.That(r.IsDisposed, Is.False);
            r.Dispose();
            Assert.That(s.IsDisposed, Is.True);
            Assert.That(r.IsDisposed, Is.True);
        }
    }
}
