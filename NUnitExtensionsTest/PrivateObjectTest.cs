namespace NUnit.Framework
{
    using System;
    using System.Reflection;
    using HelperClasses;

    [TestFixture(typeof(PrivateObjectAccessor), Category = "NUnitExtensions.PrivateObject")]
    [TestFixture(typeof(PrivateObjectVsAccessor), Category = "VisualStudio.PrivateObject")]
    public class PrivateObjectTest<T> where T : class, IPrivateObjectAccessor
    {
        private readonly BindingFlags m_BindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;

        #region Dynamic Creation of Correct PrivateObject
        public static T CreatePrivateObject(object obj)
        {
            if (typeof(T) == typeof(PrivateObjectAccessor)) return new PrivateObjectAccessor(obj) as T;
            if (typeof(T) == typeof(PrivateObjectVsAccessor)) return new PrivateObjectVsAccessor(obj) as T;
            return null;
        }

        public static T CreatePrivateObject(object obj, string memberToAccess)
        {
            if (typeof(T) == typeof(PrivateObjectAccessor)) return new PrivateObjectAccessor(obj, memberToAccess) as T;
            if (typeof(T) == typeof(PrivateObjectVsAccessor)) return new PrivateObjectVsAccessor(obj, memberToAccess) as T;
            return null;
        }

        public static T CreatePrivateObject(Type objectType, params object[] args)
        {
            if (typeof(T) == typeof(PrivateObjectAccessor)) return new PrivateObjectAccessor(objectType, args) as T;
            if (typeof(T) == typeof(PrivateObjectVsAccessor)) return new PrivateObjectVsAccessor(objectType, args) as T;
            return null;
        }

        public static T CreatePrivateObject(object obj, Type type)
        {
            if (typeof(T) == typeof(PrivateObjectAccessor)) return new PrivateObjectAccessor(obj, new PrivateType(type)) as T;
            if (typeof(T) == typeof(PrivateObjectVsAccessor)) return new PrivateObjectVsAccessor(obj, new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(type)) as T;
            return null;
        }

        public static T CreatePrivateObject(string assemblyName, string typeName, params object[] args)
        {
            if (typeof(T) == typeof(PrivateObjectAccessor)) return new PrivateObjectAccessor(assemblyName, typeName, args) as T;
            if (typeof(T) == typeof(PrivateObjectVsAccessor)) return new PrivateObjectVsAccessor(assemblyName, typeName, args) as T;
            return null;
        }

        public static T CreatePrivateObject(Type type, Type[] parameterTypes, object[] args)
        {
            if (typeof(T) == typeof(PrivateObjectAccessor)) return new PrivateObjectAccessor(type, parameterTypes, args) as T;
            if (typeof(T) == typeof(PrivateObjectVsAccessor)) return new PrivateObjectVsAccessor(type, parameterTypes, args) as T;
            return null;
        }

        public static T CreatePrivateObject(string assemblyName, string typeName, Type[] parameterTypes, object[] args)
        {
            if (typeof(T) == typeof(PrivateObjectAccessor)) return new PrivateObjectAccessor(assemblyName, typeName, parameterTypes, args) as T;
            if (typeof(T) == typeof(PrivateObjectVsAccessor)) return new PrivateObjectVsAccessor(assemblyName, typeName, parameterTypes, args) as T;
            return null;
        }
        #endregion

        #region Constructor
        [Test]
        public void NullObject()
        {
            object obj = null;
            Assert.That(() => {
                CreatePrivateObject(obj);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PrivateObject_Object()
        {
            Assert.NotNull(CreatePrivateObject(new object()));
        }

        [Test]
        public void PrivateObject_MemberToAccess_NullObject()
        {
            object obj = null;

            Assert.That(() => {
                CreatePrivateObject(obj, "test");
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PrivateObject_MemberToAccess_NullMember()
        {
            object obj = new object();
            string member = null;

            Assert.That(() => {
                CreatePrivateObject(obj, member);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PrivateObject_MemberToAccess_EmptyMember()
        {
            Assert.That(() => {
                CreatePrivateObject(new object(), string.Empty);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void PrivateObject_MemberToAccess()
        {
            T privateObj = CreatePrivateObject(new ClassTest(7), "Capacity");
            Assert.That(privateObj.Target, Is.EqualTo(7));
        }

        [Test]
        public void NullArgs()
        {
            Type objectType = typeof(ObjectClassTest);
            object[] args = null;

            Assert.NotNull(CreatePrivateObject(objectType, args));
        }

        [Test]
        public void InstanceFromType()
        {
            T privateObject = CreatePrivateObject(typeof(ObjectClassTest), 7);
            Assert.NotNull(privateObject.Target);
        }

        [Test]
        public void PrivateCtorFromType()
        {
            object[] args = new object[] { "ObjectName" };
            T privateObject = CreatePrivateObject(typeof(ObjectClassTest), args);

            Assert.NotNull(privateObject.Target);
        }

        [Test]
        public void PrivateObject_NullType()
        {
            Assert.That(() => {
                CreatePrivateObject(null, new object[] { });
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PrivateObject_PrivateType_NullObject()
        {
            object obj = null;
            Type type = typeof(ObjectClassTest);

            if (typeof(T) == typeof(PrivateObjectAccessor)) {
                Assert.That(() => {
                    CreatePrivateObject(obj, type);
                }, Throws.TypeOf<ArgumentNullException>());
            }

            if (typeof(T) == typeof(PrivateObjectVsAccessor)) {
                Assert.NotNull(CreatePrivateObject(obj, type));
            }
        }

        [Test]
        public void PrivateObject_PrivateType_NullType()
        {
            object obj = new object();
            Type type = null;

            Assert.That(() => {
                CreatePrivateObject(obj, type);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PrivateObject_PrivateType()
        {
            object obj = new object();
            Type type = typeof(ObjectClassTest);

            Assert.NotNull(CreatePrivateObject(obj, type));
        }

        [Test]
        public void NullAssemblyName()
        {
            Assert.That(() => { CreatePrivateObject(null, "NUnit.Framework.InternalClassTest", new object[0]); },
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void EmptyAssemblyName()
        {
            Assert.That(() => { CreatePrivateObject(string.Empty, "NUnit.Framework.InternalClassTest", new object[0]); },
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NullClassName()
        {
            Assert.That(() => { CreatePrivateObject("NUnitExtensionsTest", null, new object[0]); },
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void EmptyClassName()
        {
            Assert.That(() => { CreatePrivateObject("NUnitExtensionsTest", string.Empty, new object[0]); },
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NullTypeWithArgs()
        {
            Assert.That(() => { CreatePrivateObject(null, 0, 1); },
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void InstanceFromAssembly_WithTypeName()
        {
            T privateObject = CreatePrivateObject("NUnitExtensionsTest", "NUnit.Framework.ObjectClassTest", 7);
            Assert.NotNull(privateObject.Target);
        }

        [Test]
        public void PrivateCtorFromAssembly_WithType()
        {
            T privateObject = CreatePrivateObject("NUnitExtensionsTest", "NUnit.Framework.ObjectClassTest", "ObjectName");
            Assert.NotNull(privateObject.Target);
        }

        [Test]
        public void PrivateObjectFromAssembly_WithParameterTypes()
        {
            Type[] parameterTypes = new Type[] { typeof(int) };
            object[] args = new object[] { 7 };

            Assert.NotNull(CreatePrivateObject("NUnitExtensionsTest", "NUnit.Framework.ObjectClassTest", parameterTypes, args));
        }

        [Test]
        public void PrivateObjectFromAssembly_WithParameterTypes_NullAssembly()
        {
            Type[] parameterTypes = new Type[] { typeof(double) };
            object[] args = new object[] { 7 };

            Assert.That(() => {
                CreatePrivateObject(null, "NUnit.Framework.ObjectClassTest", parameterTypes, args);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PrivateObjectFromAssembly_WithParameterTypes_NullType()
        {
            Type[] parameterTypes = new Type[] { typeof(double) };
            object[] args = new object[] { 7 };

            Assert.That(() => {
                CreatePrivateObject("NUnitExtensionsTest", null, parameterTypes, args);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PrivateObjectFromAssembly_WithParameterTypes_CtorNotFound()
        {
            Type[] parameterTypes = new Type[] { typeof(double) };
            object[] args = new object[] { 7 };

            Assert.That(() => {
                CreatePrivateObject("NUnitExtensionsTest", "NUnit.Framework.ObjectClassTest", parameterTypes, args);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void PrivateObject_ParameterTypes()
        {
            Type[] parameterTypes = new Type[] { typeof(double) };
            object[] args = new object[] { 7 };

            Assert.That(() => {
                CreatePrivateObject(typeof(ObjectClassTest), parameterTypes, args);
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void PrivateObject_ParameterTypes_NullType()
        {
            Type[] parameterTypes = new Type[] { typeof(double) };
            object[] args = new object[] { 7 };

            Assert.That(() => {
                CreatePrivateObject(null, parameterTypes, args);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PrivateObject_ParameterTypes_CtorNotFound()
        {
            Type type = typeof(ObjectClassTest);
            Type[] parameterTypes = new Type[] { typeof(int) };
            object[] args = new object[] { 7 };

            Assert.NotNull(CreatePrivateObject(type, parameterTypes, args));
        }
        #endregion

        #region Invoke
        [Test]
        public void InvokeNullName()
        {
            T privateObject = CreatePrivateObject(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.Invoke(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void InvokeNullName_Types()
        {
            T privateObject = CreatePrivateObject(typeof(ObjectClassTest), 7);
            privateObject.Invoke("AddToProperty", new Type[] { typeof(int) }, new object[] { 3 });

            int value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(10));
        }

        [Test]
        public void Invoke_Types_Generic()
        {
            T privateObject = CreatePrivateObject(typeof(ObjectClassTest), 3);
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
            T privateObject = CreatePrivateObject(typeof(ObjectClassTest), 7);
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
            PrivateObjectAccessor privateObject = new PrivateObjectAccessor(typeof(ObjectClassTest), 7);
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
            T privateObject = CreatePrivateObject("NUnitExtensionsTest", "NUnit.Framework.ObjectClassTest", 7);
            Assert.NotNull(privateObject.Target);
            Assert.That(() => { privateObject.Target = null; }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SetAnotherTarget()
        {
            T privateObject = CreatePrivateObject("NUnitExtensionsTest", "NUnit.Framework.ObjectClassTest", 7);
            Assert.NotNull(privateObject.Target);
            ObjectClassTest testInstance = new ObjectClassTest(33);
            privateObject.Target = testInstance;
            Assert.That(privateObject.Target, Is.SameAs(testInstance));
        }

        [Test]
        public void CallPrivateMethod()
        {
            T privateObject = CreatePrivateObject(typeof(ObjectClassTest), 7);

            int value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(7));

            privateObject.Invoke("DoubleProperty", m_BindingFlags);

            value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(49));
        }

        [Test]
        public void CallInexistentMethod()
        {
            T privateObject = CreatePrivateObject(typeof(ObjectClassTest), 7);

            Assert.That(() => {
                privateObject.Invoke("XYZ", m_BindingFlags);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void CallPublicMethod()
        {
            T privateObject = CreatePrivateObject(typeof(ObjectClassTest), 7);

            int value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(7));

            privateObject.Invoke("AddToProperty", 6);

            value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(13));
        }

        [Test]
        public void CallPublicMethodWithoutInstance()
        {
            T privateObject = CreatePrivateObject(typeof(ObjectClassTest), 7);

            // Need to provide the BindingFlags.Instance for it to work.
            Assert.That(() => { privateObject.GetFieldOrProperty("m_Value", BindingFlags.NonPublic); },
                Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void CallPublicMethodWithFlags()
        {
            T privateObject = CreatePrivateObject(typeof(ObjectClassTest), 7);

            int value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(7));

            privateObject.Invoke("AddToProperty", BindingFlags.Public | BindingFlags.Instance, 6);

            value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(13));
        }

        [Test]
        public void CallOverloadedMethod()
        {
            T privateObject = CreatePrivateObject(typeof(ObjectClassTest), 7);

            int result = (int)privateObject.Invoke("Method", new Type[0], new object[0]);
            Assert.That(result, Is.EqualTo(2));

            result = (int)privateObject.Invoke("Method", m_BindingFlags, new Type[] { typeof(int) }, new object[] { 8 });
            Assert.That(result, Is.EqualTo(7));
        }

        [Test]
        public void CallOverloadedMethodWithFlags()
        {
            T privateObject = CreatePrivateObject(typeof(ObjectClassTest), 7);

            int result = (int)privateObject.Invoke("Method", BindingFlags.Public | BindingFlags.Instance, new Type[0], new object[0]);
            Assert.That(result, Is.EqualTo(2));

            result = (int)privateObject.Invoke("Method", m_BindingFlags, new Type[] { typeof(int) }, new object[] { 8 });
            Assert.That(result, Is.EqualTo(7));
        }

        [Test]
        public void SetAndGetPublicField()
        {
            T privateObject = CreatePrivateObject(typeof(ObjectClassTest), 7);

            int value = (int)privateObject.GetFieldOrProperty("PubField");
            Assert.That(value, Is.EqualTo(7));

            privateObject.SetFieldOrProperty("PubField", 9);

            value = (int)privateObject.GetFieldOrProperty("PubField");
            Assert.That(value, Is.EqualTo(9));
        }

        [Test]
        public void SetAndGetPrivateField()
        {
            T privateObject = CreatePrivateObject(typeof(ObjectClassTest), 7);

            int value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(7));

            privateObject.SetFieldOrProperty("m_Value", m_BindingFlags, 9);

            value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(9));
        }

        [Test]
        public void SetAndGetPublicProperty()
        {
            T privateObject = CreatePrivateObject(typeof(ObjectClassTest), 7);

            privateObject.SetFieldOrProperty("PubProp", 9);

            int value = (int)privateObject.GetFieldOrProperty("PubProp");
            Assert.That(value, Is.EqualTo(9));
        }

        [Test]
        public void SetAndGetPrivateProperty()
        {
            T privateObject = CreatePrivateObject(typeof(ObjectClassTest), 7);

            privateObject.SetFieldOrProperty("Prop", m_BindingFlags, 9);

            int value = (int)privateObject.GetFieldOrProperty("Prop", m_BindingFlags);
            Assert.That(value, Is.EqualTo(9));
        }

        [Test]
        public void SetInexistentProperty()
        {
            T privateObject = CreatePrivateObject(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.SetFieldOrProperty("InexistentProp", m_BindingFlags, 9);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void GetInexistentProperty()
        {
            T privateObject = CreatePrivateObject(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.GetFieldOrProperty("InexistentProp", m_BindingFlags);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void SetInexistentField()
        {
            T privateObject = CreatePrivateObject(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.SetFieldOrProperty("m_InexistentField", m_BindingFlags, 9);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void GetInexistentField()
        {
            T privateObject = CreatePrivateObject(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.GetFieldOrProperty("m_InexistentField", m_BindingFlags);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void DifferentTypeAndArgCount()
        {
            Type genericType = typeof(ObjectGenericClassTest<object, string>);
            Assert.That(() => { CreatePrivateObject("NUnitExtensionsTest", genericType.GetGenericTypeDefinition().FullName, new[] { typeof(object), typeof(string) }, 9, "abc", 100); },
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void InvalidTypeName()
        {
            Assert.That(() => {
                CreatePrivateObject("NUnitExtensionsTest", "abc_xyz", new[] { typeof(object), typeof(string) }, 9, "xyz");
            }, Throws.TypeOf<ArgumentNullException>());
        }
    }
}
