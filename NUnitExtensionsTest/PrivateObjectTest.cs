namespace NUnit.Framework
{
    using System;
    using System.Reflection;

    [TestFixture]
    public class PrivateObjectTest
    {
        [Test]
        [Category("NUnitExtensions.PrivateObject")]
        public void NullAssemblyName()
        {
            Assert.That(() => { new PrivateObject(null, "NUnit.Framework.InternalClassTest"); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [Category("NUnitExtensions.PrivateObject")]
        public void EmptyAssemblyName()
        {
            Assert.That(() => { new PrivateObject(string.Empty, "NUnit.Framework.InternalClassTest"); }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        [Category("NUnitExtensions.PrivateObject")]
        public void NullClassName()
        {
            Assert.That(() => { new PrivateObject("NUnitExtensionsTest", null); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [Category("NUnitExtensions.PrivateObject")]
        public void EmptyClassName()
        {
            Assert.That(() => { new PrivateObject("NUnitExtensionsTest", string.Empty); }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        [Category("NUnitExtensions.PrivateObject")]
        public void NullType()
        {
            Assert.That(() => { new PrivateObject(null); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [Category("NUnitExtensions.PrivateObject")]
        public void NullArgs()
        {
            Assert.That(() => { new PrivateObject(typeof(ObjectClassTest), null); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [Category("NUnitExtensions.PrivateObject")]
        public void InstanceFromType()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            Assert.NotNull(privateObject.Target);
        }

        [Test]
        [Category("NUnitExtensions.PrivateObject")]
        public void InstanceFromAssemblyAndTypeName()
        {
            PrivateObject privateObject = new PrivateObject("NUnitExtensionsTest", "NUnit.Framework.ObjectClassTest", 7);
            Assert.NotNull(privateObject.Target);
        }

        [Test]
        [Category("NUnitExtensions.PrivateObject")]
        public void SetTargetNull()
        {
            PrivateObject privateObject = new PrivateObject("NUnitExtensionsTest", "NUnit.Framework.ObjectClassTest", 7);
            Assert.NotNull(privateObject.Target);
            Assert.That(() => { privateObject.Target = null; }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [Category("NUnitExtensions.PrivateObject")]
        public void SetAnotherTarget()
        {
            PrivateObject privateObject = new PrivateObject("NUnitExtensionsTest", "NUnit.Framework.ObjectClassTest", 7);
            Assert.NotNull(privateObject.Target);
            ObjectClassTest testInstance = new ObjectClassTest(33);
            privateObject.Target = testInstance;
            Assert.That(privateObject.Target, Is.SameAs(testInstance));
        }

        [Test]
        [Category("NUnitExtensions.PrivateObject")]
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
        [Category("NUnitExtensions.PrivateObject")]
        public void CallInexistentMethod()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);

            Assert.That(() => {
                privateObject.Invoke("XYZ", BindingFlags.NonPublic | BindingFlags.Instance);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        [Category("NUnitExtensions.PrivateObject")]
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
        [Category("NUnitExtensions.PrivateObject")]
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
        [Category("NUnitExtensions.PrivateObject")]
        public void SetAndGetPrivateProperty()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);

            privateObject.SetFieldOrProperty("Prop", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, 9);

            int value = (int)privateObject.GetFieldOrProperty("Prop", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            Assert.That(value, Is.EqualTo(9));
        }

        [Test]
        [Category("NUnitExtensions.PrivateObject")]
        public void SetInexistentProperty()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.SetFieldOrProperty("InexistentProp", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, 9);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        [Category("NUnitExtensions.PrivateObject")]
        public void GetInexistentProperty()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.GetFieldOrProperty("InexistentProp", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        [Category("NUnitExtensions.PrivateObject")]
        public void SetInexistentField()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.SetFieldOrProperty("m_InexistentField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField, 9);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        [Category("NUnitExtensions.PrivateObject")]
        public void GetInexistentField()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.GetFieldOrProperty("m_InexistentField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
            }, Throws.TypeOf<MissingMethodException>());
        }
    }
}
