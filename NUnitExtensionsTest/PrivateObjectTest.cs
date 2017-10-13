namespace NUnit.Framework
{
    using System;
    using System.Reflection;
    using HelperClasses;

    [TestFixture(Category = "NUnitExtensions.PrivateObject")]
    public class PrivateObjectTest
    {
        [Test]
        public void NullAssemblyName()
        {
            Assert.That(() => { new PrivateObject(null, "NUnit.Framework.InternalClassTest"); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void EmptyAssemblyName()
        {
            Assert.That(() => { new PrivateObject(string.Empty, "NUnit.Framework.InternalClassTest"); }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void NullClassName()
        {
            Assert.That(() => { new PrivateObject("NUnitExtensionsTest", null); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void EmptyClassName()
        {
            Assert.That(() => { new PrivateObject("NUnitExtensionsTest", string.Empty); }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void NullType()
        {
            Assert.That(() => { new PrivateObject(null); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NullArgs()
        {
            Assert.That(() => { new PrivateObject(typeof(ObjectClassTest), null); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void InstanceFromType()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            Assert.NotNull(privateObject.Target);
        }

        [Test]
        public void InstanceFromAssemblyAndTypeName()
        {
            PrivateObject privateObject = new PrivateObject("NUnitExtensionsTest", "NUnit.Framework.ObjectClassTest", 7);
            Assert.NotNull(privateObject.Target);
        }

        [Test]
        public void SetTargetNull()
        {
            PrivateObject privateObject = new PrivateObject("NUnitExtensionsTest", "NUnit.Framework.ObjectClassTest", 7);
            Assert.NotNull(privateObject.Target);
            Assert.That(() => { privateObject.Target = null; }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SetAnotherTarget()
        {
            PrivateObject privateObject = new PrivateObject("NUnitExtensionsTest", "NUnit.Framework.ObjectClassTest", 7);
            Assert.NotNull(privateObject.Target);
            ObjectClassTest testInstance = new ObjectClassTest(33);
            privateObject.Target = testInstance;
            Assert.That(privateObject.Target, Is.SameAs(testInstance));
        }

        [Test]
        public void CallPrivateMethod()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);

            int value = (int)privateObject.GetFieldOrProperty("m_Value", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(value, Is.EqualTo(7));

            privateObject.Invoke("DoubleProperty", BindingFlags.NonPublic | BindingFlags.Instance);

            value = (int)privateObject.GetFieldOrProperty("m_Value", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(value, Is.EqualTo(49));
        }

        [Test]
        public void CallInexistentMethod()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);

            Assert.That(() => {
                privateObject.Invoke("XYZ", BindingFlags.NonPublic | BindingFlags.Instance);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void CallPublicMethod()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);

            int value = (int)privateObject.GetFieldOrProperty("m_Value", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(value, Is.EqualTo(7));

            privateObject.Invoke("AddToProperty", BindingFlags.Public | BindingFlags.Instance, 6);

            value = (int)privateObject.GetFieldOrProperty("m_Value", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(value, Is.EqualTo(13));
        }

        [Test]
        public void CallOverloadedMethod()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);

            int result = (int)privateObject.Invoke("Method", bindingFlags: BindingFlags.Public | BindingFlags.Instance, parameterTypes: new Type[0], args: new object[0]);
            Assert.That(result, Is.EqualTo(2));

            result = (int)privateObject.Invoke("Method", bindingFlags: BindingFlags.NonPublic | BindingFlags.Instance, parameterTypes: new Type[] { typeof(int)}, args: new object[] { 8 });
            Assert.That(result, Is.EqualTo(7));
        }

        [Test]
        public void SetAndGetPrivateField()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);

            int value = (int)privateObject.GetFieldOrProperty("m_Value", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
            Assert.That(value, Is.EqualTo(7));

            privateObject.SetFieldOrProperty("m_Value", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField, 9);

            value = (int)privateObject.GetFieldOrProperty("m_Value", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
            Assert.That(value, Is.EqualTo(9));
        }

        [Test]
        public void SetAndGetPrivateProperty()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);

            privateObject.SetFieldOrProperty("Prop", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, 9);

            int value = (int)privateObject.GetFieldOrProperty("Prop", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            Assert.That(value, Is.EqualTo(9));
        }

        [Test]
        public void SetInexistentProperty()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.SetFieldOrProperty("InexistentProp", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, 9);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void GetInexistentProperty()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.GetFieldOrProperty("InexistentProp", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void SetInexistentField()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.SetFieldOrProperty("m_InexistentField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField, 9);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void GetInexistentField()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.GetFieldOrProperty("m_InexistentField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void GenericTypes()
        {
            Type genericType = typeof(ObjectGenericClassTest<object, string>);
            PrivateObject privateObject = new PrivateObject("NUnitExtensionsTest", genericType.GetGenericTypeDefinition().FullName, new[] { typeof(int), typeof(string) }, 9, "abc");

            int itemValue = (int)privateObject.GetFieldOrProperty("m_Item", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(itemValue, Is.EqualTo(9));

            string elementValue = (string)privateObject.GetFieldOrProperty("m_Element", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(elementValue, Is.EqualTo("abc"));
        }

        [Test]
        public void DifferentTypeAndArgCount()
        {
            Type genericType = typeof(ObjectGenericClassTest<object, string>);
            PrivateObject privateObject = new PrivateObject("NUnitExtensionsTest", genericType.GetGenericTypeDefinition().FullName, new[] { typeof(object), typeof(string) }, 9, "abc", 100);

            Assert.That((privateObject.Target as ObjectGenericClassTest<object, string>).Value, Is.EqualTo(100));
        }

        [Test]
        public void InvalidTypeName()
        {
            Assert.That(() => {
                new PrivateObject("NUnitExtensionsTest", "abc_xyz", new[] { typeof(object), typeof(string) }, 9, "xyz");
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void GenericTypesNullAssemblyName() {
            Assert.That(() => {
                new PrivateObject(null, "abc_xyz", new[] { typeof(object), typeof(string) }, 9, "xyz");
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void GenericTypesNullTypeName()
        {
            Assert.That(() => {
                new PrivateObject("NUnitExtensionsTest", null, new[] { typeof(object), typeof(string) }, 9, "xyz");
            }, Throws.TypeOf<ArgumentNullException>());
        }
    }
}
