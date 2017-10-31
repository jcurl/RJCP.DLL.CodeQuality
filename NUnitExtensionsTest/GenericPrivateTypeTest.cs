namespace NUnit.Framework
{
    using System;
    using HelperClasses;

    [TestFixture(Category = "NUnitExtensions.GenericPrivateType")]
    public class GenericPrivateTypeTest
    {
        [TestCase(typeof(GenericClassTest<int>), "System.Int32")]
        [TestCase(typeof(GenericClassTest<object>), "System.Object")]
        [TestCase(typeof(GenericClassTest<GenericPrivateType>), "NUnit.Framework.GenericPrivateType")]
        public void GenericType(Type type, string typeName)
        {
            PrivateType genericPrivateType = new PrivateType(type);
            Assert.That((string)genericPrivateType.InvokeStatic("GenericTypeName"), Is.EqualTo(typeName));
        }

        [TestCase("NUnitExtensionsTest", "NUnit.Framework.HelperClasses.GenericClassTest`1", typeof(GenericPrivateType), "NUnit.Framework.GenericPrivateType")]
        [TestCase("NUnitExtensionsTest", "NUnit.Framework.HelperClasses.GenericClassTest`1", typeof(int), "System.Int32")]
        [TestCase("NUnitExtensionsTest", "NUnit.Framework.HelperClasses.GenericClassTest`1", typeof(object), "System.Object")]
        public void GenericType(string assemblyName, string typeName, Type typeArgument, string genericTypeName)
        {
            PrivateType genericPrivateType = new GenericPrivateType(assemblyName, typeName, typeArgument);
            Assert.That((string)genericPrivateType.InvokeStatic("GenericTypeName"), Is.EqualTo(genericTypeName));
        }

        [Test]
        public void AssemblyNotFound()
        {
            Assert.That(() => { new GenericPrivateType("foo", "type", typeof(int)); },
                Throws.InstanceOf<System.IO.FileNotFoundException>());
        }

        [Test]
        public void TypeNotFound()
        {
            Assert.That(() => { new GenericPrivateType("NUnitExtensionsTest", "NUnit.Framework.GenericPrivateType", typeof(int)); },
                Throws.InstanceOf<TypeLoadException>());
        }
    }
}
