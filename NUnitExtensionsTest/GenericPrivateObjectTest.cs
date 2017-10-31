namespace NUnit.Framework
{
    using System;
    using System.Reflection;
    using HelperClasses;

    [TestFixture(Category = "NUnitExtensions.GenericPrivateObject")]
    public class GenericPrivateObjectTest
    {
        [Test]
        public void GenericTypes()
        {
            PrivateObject privateObject = new GenericPrivateObject("NUnitExtensionsTest", "NUnit.Framework.HelperClasses.ObjectGenericClassTest`2",
                new Type[] { typeof(int), typeof(string) },
                9, "abc");

            int itemValue = (int)privateObject.GetFieldOrProperty("m_Item", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(itemValue, Is.EqualTo(9));

            string elementValue = (string)privateObject.GetFieldOrProperty("m_Element", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(elementValue, Is.EqualTo("abc"));
        }

        [Test]
        public void PrivateCtorFromGenericTypes()
        {
            Type genericType = typeof(ObjectGenericClassTest<object, string>);
            PrivateObject privateObject = new GenericPrivateObject("NUnitExtensionsTest", genericType.GetGenericTypeDefinition().FullName, new[] { typeof(int), typeof(string) }, "abc");

            Assert.NotNull(privateObject.Target);
        }

        [Test]
        public void GenericTypesNullAssemblyName()
        {
            Assert.That(() => {
                new GenericPrivateObject(null, "abc_xyz", new[] { typeof(object), typeof(string) }, 9, "xyz");
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void GenericTypesNullTypeName()
        {
            Assert.That(() => {
                new GenericPrivateObject("NUnitExtensionsTest", null, new[] { typeof(object), typeof(string) }, 9, "xyz");
            }, Throws.TypeOf<ArgumentNullException>());
        }
    }
}
