namespace NUnit.Framework
{
    using System;

    [TestFixture]
    public class PrivateTypeTest
    {
        [Test]
        [Category("NUnitExtensions.PrivateType")]
        public void NullAssemblyName()
        {
            Assert.That(() => { new PrivateType(null, "NUnit.Framework.InternalClassTest"); }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        [Category("NUnitExtensions.PrivateType")]
        public void EmptyAssemblyName()
        {
            Assert.That(() => { new PrivateType(string.Empty, "NUnit.Framework.InternalClassTest"); }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        [Category("NUnitExtensions.PrivateType")]
        public void NullClassName()
        {
            Assert.That(() => { new PrivateType("NUnitExtensionsTest", null); }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        [Category("NUnitExtensions.PrivateType")]
        public void EmptyClassName()
        {
            Assert.That(() => { new PrivateType("NUnitExtensionsTest", string.Empty); }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        [Category("NUnitExtensions.PrivateType")]
        public void NullType()
        {
            Assert.That(() => { new PrivateType(null); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [Category("NUnitExtensions.PrivateType")]
        public void InvokeStaticOnTypeFromAssembly()
        {
            PrivateType privType = new PrivateType("NUnitExtensionsTest", "NUnit.Framework.InternalClassTest");
            int result = (int)privType.InvokeStatic("TestStaticMethod", null);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        [Category("NUnitExtensions.PrivateType")]
        public void InvokeStaticFromPublicType()
        {
            PrivateType privType = new PrivateType(typeof(PublicClassTest));
            int result = (int)privType.InvokeStatic("TestPrivateStaticMethod", null);

            Assert.That(result, Is.EqualTo(5));
        }
    }
}
