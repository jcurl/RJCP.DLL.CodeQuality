namespace RJCP.CodeQuality
{
    using System;
    using System.Reflection;
    using HelperClasses;
    using NUnit.Framework;

    [TestFixture]
    public class GenericPrivateObjectTest
    {
        [Test]
        public void GenericTypes()
        {
            PrivateObject privateObject = new PrivateObject(AccessorTest.AssemblyName,
                "RJCP.CodeQuality.HelperClasses.ObjectGenericClassTest`2",
                new Type[] { typeof(int), typeof(string) },   // Constructor signature
                new object[] { 9, "abc" },                    // Values to the constructor
                new Type[] { typeof(int), typeof(string) });  // Type arguments

            int itemValue = (int)privateObject.GetFieldOrProperty("m_Item", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(itemValue, Is.EqualTo(9));

            string elementValue = (string)privateObject.GetFieldOrProperty("m_Element", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(elementValue, Is.EqualTo("abc"));
        }

        [Test]
        public void PrivateCtorFromGenericTypes()
        {
            Type genericType = typeof(ObjectGenericClassTest<object, string>);
            PrivateObject privateObject = new PrivateObject(AccessorTest.AssemblyName,
                genericType.GetGenericTypeDefinition().FullName,
                new Type[] { typeof(string) },                // Constructor signature
                new object[] { "abc" },                       // Values to the constructor
                new[] { typeof(int), typeof(string) });       // Type arguments

            Assert.That(privateObject.Target, Is.Not.Null);
        }

        [Test]
        public void GenericTypesNullAssemblyName()
        {
            Assert.That(() => {
                _ = new PrivateObject(null, "abc_xyz",
                    new[] { typeof(object), typeof(string) },
                    new object[] { 9, "xyz" },
                    new[] { typeof(object), typeof(string) });
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void GenericTypesNullTypeName()
        {
            Assert.That(() => {
                _ = new PrivateObject(AccessorTest.AssemblyName, null,
                    new[] { typeof(object), typeof(string) },
                    new object[] { 9, "xyz" },
                    new[] { typeof(object), typeof(string) });
            }, Throws.TypeOf<ArgumentNullException>());
        }
    }
}
