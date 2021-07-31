namespace RJCP.CodeQuality
{
    using System;
    using System.Reflection;
    using HelperClasses;
    using NUnit.Framework;

    [TestFixture(Category = "RJCP.CodeQuality.PrivateObject")]

    public class PrivateObjectTest
    {
        private readonly BindingFlags m_BindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;

        [Test]
        public void NullObject()
        {
            object obj = null;
            Assert.That(() => {
                _ = new PrivateObject(obj);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PrivateObject_Object()
        {
            Assert.That(new PrivateObject(new object()), Is.Not.Null);
        }

        [Test]
        public void PrivateObject_MemberToAccess_NullObject()
        {
            object obj = null;

            Assert.That(() => {
                _ = new PrivateObject(obj, "test");
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PrivateObject_MemberToAccess_NullMember()
        {
            object obj = new object();
            string member = null;

            Assert.That(() => {
                _ = new PrivateObject(obj, member);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PrivateObject_MemberToAccess_EmptyMember()
        {
            Assert.That(() => {
                _ = new PrivateObject(new object(), string.Empty);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void PrivateObject_MemberToAccess()
        {
            PrivateObject privateObj = new PrivateObject(new ClassTest(7), "Capacity");
            Assert.That(privateObj.Target, Is.EqualTo(7));
        }

        [Test]
        public void NullArgs()
        {
            Type objectType = typeof(ObjectClassTest);
            object[] args = null;

            Assert.That(new PrivateObject(objectType, args), Is.Not.Null);
        }

        [Test]
        public void InstanceFromType()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            Assert.That(privateObject.Target, Is.Not.Null);
        }

        [Test]
        public void PrivateCtorFromType()
        {
            object[] args = new object[] { "ObjectName" };
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), args);

            Assert.That(privateObject.Target, Is.Not.Null);
        }

        [Test]
        public void PrivateObject_NullType()
        {
            Assert.That(() => {
                _ = new PrivateObject(null, new object[] { });
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PrivateObject_PrivateType_NullType()
        {
            object obj = new object();
            Type type = null;

            Assert.That(() => {
                _ = new PrivateObject(obj, new PrivateType(type));
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PrivateObject_PrivateType()
        {
            object obj = new object();
            Type type = typeof(ObjectClassTest);

            Assert.That(new PrivateObject(obj, new PrivateType(type)), Is.Not.Null);
        }

        [Test]
        public void NullAssemblyName()
        {
            Assert.That(() => { _ = new PrivateObject(null, "NUnit.Framework.HelperClasses.InternalClassTest", new object[0]); },
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void EmptyAssemblyName()
        {
            Assert.That(() => { _ = new PrivateObject(string.Empty, "NUnit.Framework.HelperClasses.InternalClassTest", new object[0]); },
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NullClassName()
        {
            Assert.That(() => { _ = new PrivateObject("RJCP.CodeQualityTest", null, new object[0]); },
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void EmptyClassName()
        {
            Assert.That(() => { _ = new PrivateObject("RJCP.CodeQualityTest", string.Empty, new object[0]); },
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NullTypeWithArgs()
        {
            Assert.That(() => { _ = new PrivateObject(null, 0, 1); },
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void InstanceFromAssembly_WithTypeName()
        {
            PrivateObject privateObject = new PrivateObject("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.ObjectClassTest", 7);
            Assert.That(privateObject.Target, Is.Not.Null);
        }

        [Test]
        public void PrivateCtorFromAssembly_WithType()
        {
            PrivateObject privateObject = new PrivateObject("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.ObjectClassTest", "ObjectName");
            Assert.That(privateObject.Target, Is.Not.Null);
        }

        [Test]
        public void PrivateObjectFromAssembly_WithParameterTypes()
        {
            Type[] parameterTypes = new Type[] { typeof(int) };
            object[] args = new object[] { 7 };

            Assert.That(
                new PrivateObject("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.ObjectClassTest", parameterTypes, args),
                Is.Not.Null);
        }

        [Test]
        public void PrivateObjectFromAssembly_WithParameterTypes_NullAssembly()
        {
            Type[] parameterTypes = new Type[] { typeof(double) };
            object[] args = new object[] { 7 };

            Assert.That(() => {
                _ = new PrivateObject(null, "RJCP.CodeQuality.HelperClasses.ObjectClassTest", parameterTypes, args);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PrivateObjectFromAssembly_WithParameterTypes_NullType()
        {
            Type[] parameterTypes = new Type[] { typeof(double) };
            object[] args = new object[] { 7 };

            Assert.That(() => {
                _ = new PrivateObject("RJCP.CodeQualityTest", null, parameterTypes, args);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PrivateObjectFromAssembly_WithParameterTypes_CtorNotFound()
        {
            Type[] parameterTypes = new Type[] { typeof(double) };
            object[] args = new object[] { 7 };

            Assert.That(() => {
                _ = new PrivateObject("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.ObjectClassTest", parameterTypes, args);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void PrivateObject_ParameterTypes()
        {
            Type[] parameterTypes = new Type[] { typeof(double) };
            object[] args = new object[] { 7 };

            Assert.That(() => {
                _ = new PrivateObject(typeof(ObjectClassTest), parameterTypes, args);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void PrivateObject_ParameterTypes_NullType()
        {
            Type[] parameterTypes = new Type[] { typeof(double) };
            object[] args = new object[] { 7 };

            Assert.That(() => {
                _ = new PrivateObject(null, parameterTypes, args);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PrivateObject_ParameterTypes_CtorNotFound()
        {
            Type type = typeof(ObjectClassTest);
            Type[] parameterTypes = new Type[] { typeof(int) };
            object[] args = new object[] { 7 };

            Assert.That(new PrivateObject(type, parameterTypes, args), Is.Not.Null);
        }

        #region Invoke
        [Test]
        public void InvokeNullName()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.Invoke(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void InvokeNullName_Types()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            privateObject.Invoke("AddToProperty", new Type[] { typeof(int) }, new object[] { 3 });

            int value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(10));
        }

        [Test]
        public void Invoke_Types_Generic()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 3);
            privateObject.Invoke("AddCount",
                new Type[] { typeof(object), typeof(string) },
                new object[] { 7, "ABC" },
                new Type[] { typeof(object) });

            int value = (int)privateObject.GetFieldOrProperty("m_Count", m_BindingFlags);
            string name = (string)privateObject.GetFieldOrProperty("m_Name", m_BindingFlags);

            Assert.That(value, Is.EqualTo(7));
            Assert.That(name, Is.EqualTo("ABC"));
        }

        [Test]
        public void Invoke_BindingFlags_Types()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            privateObject.Invoke("AddToProperty",
                m_BindingFlags,
                new Type[] { typeof(int) },
                new object[] { 3 });

            int value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(10));
        }

        [Test]
        public void Invoke_BindingFlags_Types_Arguments()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            privateObject.Invoke("AddToProperty",
                m_BindingFlags,
                new Type[] { typeof(int) },
                new object[] { 3 },
                new Type[0]);

            int value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(10));
        }
        #endregion

        [Test]
        public void SetTargetNull()
        {
            PrivateObject privateObject = new PrivateObject("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.ObjectClassTest", 7);
            Assert.That(privateObject.Target, Is.Not.Null);
            Assert.That(() => { privateObject.Target = null; }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SetAnotherTarget()
        {
            PrivateObject privateObject = new PrivateObject("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.ObjectClassTest", 7);
            Assert.That(privateObject.Target, Is.Not.Null);
            ObjectClassTest testInstance = new ObjectClassTest(33);
            privateObject.Target = testInstance;
            Assert.That(privateObject.Target, Is.SameAs(testInstance));
        }

        [Test]
        public void CallPrivateMethod()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);

            int value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(7));

            privateObject.Invoke("DoubleProperty", m_BindingFlags);

            value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(49));
        }

        [Test]
        public void CallInexistentMethod()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);

            Assert.That(() => {
                privateObject.Invoke("XYZ", m_BindingFlags);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void CallPublicMethod()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);

            int value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(7));

            privateObject.Invoke("AddToProperty", 6);

            value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(13));
        }

        [Test]
        public void CallPublicMethodWithoutInstance()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);

            // Need to provide the BindingFlags.Instance for it to work.
            Assert.That(() => { privateObject.GetFieldOrProperty("m_Value", BindingFlags.NonPublic); },
                Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void CallPublicMethodWithFlags()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);

            int value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(7));

            privateObject.Invoke("AddToProperty", BindingFlags.Public | BindingFlags.Instance, 6);

            value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(13));
        }

        [Test]
        public void CallOverloadedMethod()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);

            int result = (int)privateObject.Invoke("Method", new Type[0], new object[0]);
            Assert.That(result, Is.EqualTo(2));

            result = (int)privateObject.Invoke("Method", m_BindingFlags, new Type[] { typeof(int) }, new object[] { 8 });
            Assert.That(result, Is.EqualTo(7));
        }

        [Test]
        public void CallOverloadedMethodWithFlags()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);

            int result = (int)privateObject.Invoke("Method", BindingFlags.Public | BindingFlags.Instance, new Type[0], new object[0]);
            Assert.That(result, Is.EqualTo(2));

            result = (int)privateObject.Invoke("Method", m_BindingFlags, new Type[] { typeof(int) }, new object[] { 8 });
            Assert.That(result, Is.EqualTo(7));
        }

        [Test]
        public void SetAndGetPublicField()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);

            int value = (int)privateObject.GetFieldOrProperty("PubField");
            Assert.That(value, Is.EqualTo(7));

            privateObject.SetFieldOrProperty("PubField", 9);

            value = (int)privateObject.GetFieldOrProperty("PubField");
            Assert.That(value, Is.EqualTo(9));
        }

        [Test]
        public void SetAndGetPrivateField()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);

            int value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(7));

            privateObject.SetFieldOrProperty("m_Value", m_BindingFlags, 9);

            value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(9));
        }

        [Test]
        public void SetAndGetPublicProperty()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);

            privateObject.SetFieldOrProperty("PubProp", 9);

            int value = (int)privateObject.GetFieldOrProperty("PubProp");
            Assert.That(value, Is.EqualTo(9));
        }

        [Test]
        public void SetAndGetPrivateProperty()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);

            privateObject.SetFieldOrProperty("Prop", m_BindingFlags, 9);

            int value = (int)privateObject.GetFieldOrProperty("Prop", m_BindingFlags);
            Assert.That(value, Is.EqualTo(9));
        }

        [Test]
        public void SetInexistentProperty()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.SetFieldOrProperty("InexistentProp", m_BindingFlags, 9);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void GetInexistentProperty()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.GetFieldOrProperty("InexistentProp", m_BindingFlags);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void SetInexistentField()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.SetFieldOrProperty("m_InexistentField", m_BindingFlags, 9);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void GetInexistentField()
        {
            PrivateObject privateObject = new PrivateObject(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.GetFieldOrProperty("m_InexistentField", m_BindingFlags);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void DifferentTypeAndArgCount()
        {
            Type genericType = typeof(ObjectGenericClassTest<object, string>);
            Assert.That(() => {
                _ = new PrivateObject("RJCP.CodeQualityTest", genericType.GetGenericTypeDefinition().FullName,
                    new[] { typeof(object), typeof(string) }, 9, "abc", 100);
            },
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void InvalidTypeName()
        {
            Assert.That(() => {
                _ = new PrivateObject("RJCP.CodeQualityTest", "abc_xyz", new[] { typeof(object), typeof(string) }, 9, "xyz");
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void ObjectWithNullType()
        {
            object obj = new object();
            Assert.That(() => {
                _ = new PrivateObject(obj, new PrivateType(null));
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NullObjectWithType()
        {
            object obj = null;
            Type t = typeof(int);
            Assert.That(() => {
                _ = new PrivateObject(obj, new PrivateType(t));
            }, Throws.Nothing);
        }
    }
}
