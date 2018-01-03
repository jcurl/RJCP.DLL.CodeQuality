namespace NUnit.Framework
{
    using System;
    using HelperClasses;

    [TestFixture(Category = "NUnitExtensions.PrivateType.Generic")]
    public class GenericPrivateTypeTest
    {
        [TestCase(typeof(GenericClassTest<int>), "System.Int32")]
        [TestCase(typeof(GenericClassTest<object>), "System.Object")]
        [TestCase(typeof(GenericClassTest<PrivateType>), "NUnit.Framework.PrivateType")]
        public void GenericType(Type type, string typeName)
        {
            PrivateType genericPrivateType = new PrivateType(type);
            Assert.That((string)genericPrivateType.InvokeStatic("GenericTypeName"), Is.EqualTo(typeName));
        }

        [TestCase("NUnitExtensionsTest", "NUnit.Framework.HelperClasses.GenericClassTest`1", typeof(PrivateType), "NUnit.Framework.PrivateType")]
        [TestCase("NUnitExtensionsTest", "NUnit.Framework.HelperClasses.GenericClassTest`1", typeof(int), "System.Int32")]
        [TestCase("NUnitExtensionsTest", "NUnit.Framework.HelperClasses.GenericClassTest`1", typeof(object), "System.Object")]
        public void GenericType(string assemblyName, string typeName, Type typeArgument, string genericTypeName)
        {
            PrivateType genericPrivateType = new PrivateType(assemblyName, typeName, new Type[] { typeArgument });
            Assert.That((string)genericPrivateType.InvokeStatic("GenericTypeName"), Is.EqualTo(genericTypeName));
        }

        [Test]
        public void AssemblyNotFound()
        {
            Assert.That(() => { new PrivateType("foo", "type", new Type[] { typeof(int) }); },
                Throws.InstanceOf<System.IO.FileNotFoundException>());
        }

        [Test]
        public void TypeNotFound()
        {
            Assert.That(() => { new PrivateType("NUnitExtensionsTest", "NUnit.Framework.GenericPrivateType", new Type[] { typeof(int) }); },
                Throws.InstanceOf<TypeLoadException>());
        }
    }
}
