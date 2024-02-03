namespace RJCP.CodeQuality.IO
{
    using System;
    using System.IO;
    using NUnit.Framework;

#if NET6_0_OR_GREATER || !NET40_LEGACY && NET462_OR_GREATER
    using System.Threading.Tasks;
#endif

    [TestFixture]
    public class StreamExtensionsTest
    {
        [Test]
        public void ReadMemoryStream()
        {
            Random r = new Random();
            byte[] rnd = new byte[100];
            r.NextBytes(rnd);

            using (MemoryStream ms = new MemoryStream()) {
                ms.Write(rnd, 0, rnd.Length);
                byte[] data = ms.ReadStream();
                Assert.That(data, Is.EqualTo(rnd));
            }
        }

        [Test]
        public void ReadStream()
        {
            Random r = new Random();
            byte[] rnd = new byte[100];
            r.NextBytes(rnd);

            using (SparseStream ms = new SparseStream()) {
                ms.Write(rnd, 0, rnd.Length);
                byte[] data = ms.ReadStream();
                Assert.That(data, Is.EqualTo(rnd));
            }
        }

        [Test]
        public void ReadStreamWriteOnly()
        {
            using (SimpleStream ws = new SimpleStream()) {
                ws.Mode = StreamMode.Write;
                Assert.That(() => {
                    _ = ws.ReadStream();
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public void ReadStreamNoSeek()
        {
            using (SimpleStream ws = new SimpleStream()) {
                ws.Mode = StreamMode.Write | StreamMode.Read;
                Assert.That(() => {
                    _ = ws.ReadStream();
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public void ReadStreamLarge()
        {
            using (SparseStream ms = new SparseStream()) {
                ms.SetLength((long)int.MaxValue + 100);
                Assert.That(() => {
                    _ = ms.ReadStream();
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

#if NET6_0_OR_GREATER || !NET40_LEGACY && NET462_OR_GREATER
        [Test]
        public async Task ReadMemoryStreamAsync()
        {
            Random r = new Random();
            byte[] rnd = new byte[100];
            r.NextBytes(rnd);

            using (MemoryStream ms = new MemoryStream()) {
                ms.Write(rnd, 0, rnd.Length);
                byte[] data = await ms.ReadStreamAsync();
                Assert.That(data, Is.EqualTo(rnd));
            }
        }

        [Test]
        public async Task ReadStreamAsync()
        {
            Random r = new Random();
            byte[] rnd = new byte[100];
            r.NextBytes(rnd);

            using (SparseStream ms = new SparseStream()) {
                ms.Write(rnd, 0, rnd.Length);
                byte[] data = await ms.ReadStreamAsync();
                Assert.That(data, Is.EqualTo(rnd));
            }
        }

        [Test]
        public void ReadStreamAsyncWriteOnly()
        {
            using (SimpleStream ws = new SimpleStream()) {
                ws.Mode = StreamMode.Write;
                Assert.That(async () => {
                    _ = await ws.ReadStreamAsync();
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public void ReadStreamAsyncNoSeek()
        {
            using (SimpleStream ws = new SimpleStream()) {
                ws.Mode = StreamMode.Write | StreamMode.Read;
                Assert.That(async () => {
                    _ = await ws.ReadStreamAsync();
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public void ReadStreamAsyncLarge()
        {
            using (SparseStream ms = new SparseStream()) {
                ms.SetLength((long)int.MaxValue + 100);
                Assert.That(async () => {
                    _ = await ms.ReadStreamAsync();
                }, Throws.TypeOf<InvalidOperationException>());
            }
        }
#endif
    }
}
