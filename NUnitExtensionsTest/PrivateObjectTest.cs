namespace NUnit.Framework
{
    using System;
    using System.Reflection;

    [TestFixture]
    public class PrivateObjectTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        [Category("NUnitExtensions.PrivateObject")]
        public void NullAssemblyName()
        {
            new PrivateObject(null, "NUnit.Framework.InternalClassTest");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        [Category("NUnitExtensions.PrivateObject")]
        public void EmptyAssemblyName()
        {
            new PrivateObject(string.Empty, "NUnit.Framework.InternalClassTest");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        [Category("NUnitExtensions.PrivateObject")]
        public void NullClassName()
        {
            new PrivateObject("NUnitExtensionsTest", null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        [Category("NUnitExtensions.PrivateObject")]
        public void EmptyClassName()
        {
            new PrivateObject("NUnitExtensionsTest", string.Empty);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        [Category("NUnitExtensions.PrivateObject")]
        public void NullType()
        {
            new PrivateObject(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        [Category("NUnitExtensions.PrivateObject")]
        public void NullArgs()
        {
            new PrivateObject(typeof(ObjectClassTest), null);
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
        [ExpectedException(typeof(ArgumentNullException))]
        [Category("NUnitExtensions.PrivateObject")]
        public void SetTargetNull()
        {
            PrivateObject privateObject = new PrivateObject("NUnitExtensionsTest", "NUnit.Framework.ObjectClassTest", 7);
            Assert.NotNull(privateObject.Target);
            privateObject.Target = null;
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
        [ExpectedException(typeof(ArgumentException))]
        [Category("NUnitExtensions.PrivateObject")]
        public void SetInexistentProperty()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            privateObject.SetFieldOrProperty("InexistentProp", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, 9);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        [Category("NUnitExtensions.PrivateObject")]
        public void GetInexistentProperty()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            privateObject.GetFieldOrProperty("InexistentProp", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        [Category("NUnitExtensions.PrivateObject")]
        public void SetInexistentField()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            privateObject.SetFieldOrProperty("m_InexistentField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField, 9);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        [Category("NUnitExtensions.PrivateObject")]
        public void GetInexistentField()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            privateObject.GetFieldOrProperty("m_InexistentField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
        }
    }
}
