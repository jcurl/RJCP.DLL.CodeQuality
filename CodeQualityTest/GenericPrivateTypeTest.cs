namespace RJCP.CodeQuality
{
    using System;
    using HelperClasses;
    using NUnit.Framework;

    [TestFixture(Category = "RJCP.CodeQuality.PrivateType")]
    public class GenericPrivateTypeTest
    {
        [TestCase(typeof(GenericClassTest<int>), "System.Int32")]
        [TestCase(typeof(GenericClassTest<object>), "System.Object")]
        [TestCase(typeof(GenericClassTest<PrivateType>), "RJCP.CodeQuality.PrivateType")]
        public void GenericType(Type type, string typeName)
        {
            PrivateType genericPrivateType = new PrivateType(type);
            Assert.That((string)genericPrivateType.InvokeStatic("GenericTypeName"), Is.EqualTo(typeName));
        }

        [TestCase("RJCP.CodeQuality.HelperClasses.GenericClassTest`1", typeof(PrivateType), "RJCP.CodeQuality.PrivateType")]
        [TestCase("RJCP.CodeQuality.HelperClasses.GenericClassTest`1", typeof(int), "System.Int32")]
        [TestCase("RJCP.CodeQuality.HelperClasses.GenericClassTest`1", typeof(object), "System.Object")]
        public void GenericType(string typeName, Type typeArgument, string genericTypeName)
        {
            PrivateType genericPrivateType = new PrivateType(AccessorTest.AssemblyName, typeName, new Type[] { typeArgument });
            Assert.That((string)genericPrivateType.InvokeStatic("GenericTypeName"), Is.EqualTo(genericTypeName));
        }

        [Test]
        public void AssemblyNotFound()
        {
            Assert.That(() => { _ = new PrivateType("foo", "type", new Type[] { typeof(int) }); },
                Throws.InstanceOf<System.IO.FileNotFoundException>());
        }

        [Test]
        public void TypeNotFound()
        {
            Assert.That(() => { _ = new PrivateType(AccessorTest.AssemblyName, "RJCP.CodeQuality.GenericPrivateType", new Type[] { typeof(int) }); },
                Throws.InstanceOf<TypeLoadException>());
        }
    }
}
