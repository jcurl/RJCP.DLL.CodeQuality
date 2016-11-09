namespace NUnit.Framework
{
    using System;

    [TestFixture]
    public class PrivateTypeTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        [Category("NUnitExtensions.PrivateType")]
        public void NullAssemblyName()
        {
            new PrivateType(null, "NUnit.Framework.InternalClassTest");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        [Category("NUnitExtensions.PrivateType")]
        public void EmptyAssemblyName()
        {
            new PrivateType(string.Empty, "NUnit.Framework.InternalClassTest");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        [Category("NUnitExtensions.PrivateType")]
        public void NullClassName()
        {
            new PrivateType("NUnitExtensionsTest", null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        [Category("NUnitExtensions.PrivateType")]
        public void EmptyClassName()
        {
            new PrivateType("NUnitExtensionsTest", string.Empty);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        [Category("NUnitExtensions.PrivateType")]
        public void NullType()
        {
            new PrivateType(null);
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
