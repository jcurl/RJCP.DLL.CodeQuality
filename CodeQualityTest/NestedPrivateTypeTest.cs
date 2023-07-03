namespace RJCP.CodeQuality
{
    using System.Globalization;
    using HelperClasses;
    using NUnit.Framework;

    [TestFixture]
    public class NestedPrivateTypeTest
    {
        [Test]
        public void NestedStaticMethod()
        {
            Assert.That(NestedStaticTypesAccessor.NestedStaticTypeAccessor.NestedMethod(), Is.EqualTo(42));
        }

        [Test]
        public void NestedStaticGenericMethod1()
        {
            Assert.That(NestedStaticGTypes1Accessor.NestedStaticGTypeAccessor<int>.Name(), Is.EqualTo("System.Int32"));
        }

        [Test]
        public void NestedStaticGenericMethod2()
        {
            Assert.That(NestedStaticGTypes2Accessor<int>.NestedStaticGTypeAccessor.Name(), Is.EqualTo("System.Int32"));
        }

        [Test]
        public void NestedStaticGenericMethod3()
        {
            Assert.That(NestedStaticGTypes3Accessor.NestedStaticGTypeAccessor.Name<int>(), Is.EqualTo("System.Int32"));
        }

        [Test]
        public void NestedStaticGenericMethod4()
        {
            Assert.That(NestedStaticGTypes4Accessor<ushort>.NestedStaticGTypeAccessor<ulong>.Name(),
                Is.EqualTo("System.UInt16+System.UInt64"));
        }

        [Test]
        public void NestedStaticGenericMethod5()
        {
            Assert.That(NestedStaticGTypes5Accessor<ushort>.NestedStaticGTypeAccessor<ulong>.Name<uint>(),
                Is.EqualTo("System.UInt16+System.UInt64+System.UInt32"));
        }

        [Test]
        public void NestedStaticGenericMethod6()
        {
            Assert.That(NestedStaticGTypes6Accessor<ushort>.NestedStaticGTypeAccessor<ulong>.Name<uint>(),
                Is.EqualTo("System.UInt16+System.UInt64+System.UInt32"));
        }

        [Test]
        public void NestedMethod1()
        {
            var parent = new NestedTypes1Accessor();
            Assert.That(parent.MethodA, Is.EqualTo(42));

            var nested = new NestedTypes1Accessor.NestedTypeAccessor();
            Assert.That(nested.MethodB, Is.EqualTo(64));
        }

        [Test]
        public void NestedGenericMethod1()
        {
            var parent = new NestedGTypes1<int>(42);
            Assert.That(parent.Value, Is.EqualTo("42"));

            double doubleValue = 3.14;
            var nested = new NestedGTypes1<int>.NestedGType<double>(doubleValue);
            Assert.That(nested.ValueNested, Is.EqualTo(doubleValue.ToString(CultureInfo.CurrentCulture)));
        }
    }
}
