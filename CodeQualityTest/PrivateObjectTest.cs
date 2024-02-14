namespace RJCP.CodeQuality
{
    using System;
    using System.Reflection;
    using HelperClasses;
    using NUnit.Framework;

    [TestFixture]

    public class PrivateObjectTest
    {
        private readonly BindingFlags m_BindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

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
            object obj = new();
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
            PrivateObject privateObj = new(new ClassTest(7), "Capacity");
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
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);
            Assert.That(privateObject.Target, Is.Not.Null);
        }

        [Test]
        public void PrivateCtorFromType()
        {
            object[] args = new object[] { "ObjectName" };
            PrivateObject privateObject = new(typeof(ObjectClassTest), args);

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
            object obj = new();
            Type type = null;

            Assert.That(() => {
                _ = new PrivateObject(obj, new PrivateType(type));
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PrivateObject_PrivateType()
        {
            object obj = new();
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
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void NullClassName()
        {
            Assert.That(() => { _ = new PrivateObject(AccessorTest.AssemblyName, null, new object[0]); },
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void EmptyClassName()
        {
            Assert.That(() => { _ = new PrivateObject(AccessorTest.AssemblyName, string.Empty, new object[0]); },
                Throws.TypeOf<ArgumentException>());
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
            PrivateObject privateObject = new(AccessorTest.AssemblyName, "RJCP.CodeQuality.HelperClasses.ObjectClassTest", 7);
            Assert.That(privateObject.Target, Is.Not.Null);
        }

        [Test]
        public void PrivateCtorFromAssembly_WithType()
        {
            PrivateObject privateObject = new(AccessorTest.AssemblyName, "RJCP.CodeQuality.HelperClasses.ObjectClassTest", "ObjectName");
            Assert.That(privateObject.Target, Is.Not.Null);
        }

        [Test]
        public void PrivateObjectFromAssembly_WithParameterTypes()
        {
            Type[] parameterTypes = new Type[] { typeof(int) };
            object[] args = new object[] { 7 };

            Assert.That(
                new PrivateObject(AccessorTest.AssemblyName, "RJCP.CodeQuality.HelperClasses.ObjectClassTest", parameterTypes, args),
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
                _ = new PrivateObject(AccessorTest.AssemblyName, null, parameterTypes, args);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PrivateObjectFromAssembly_WithParameterTypes_CtorNotFound()
        {
            Type[] parameterTypes = new Type[] { typeof(double) };
            object[] args = new object[] { 7 };

            Assert.That(() => {
                _ = new PrivateObject(AccessorTest.AssemblyName, "RJCP.CodeQuality.HelperClasses.ObjectClassTest", parameterTypes, args);
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
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.Invoke(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Invoke_Types()
        {
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);
            privateObject.Invoke("AddToProperty", new Type[] { typeof(int) }, new object[] { 3 });

            int value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(10));
        }

        [Test]
        public void Invoke_Types_Generic()
        {
            PrivateObject privateObject = new(typeof(ObjectClassTest), 3);
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
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);
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
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);
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
            PrivateObject privateObject = new(AccessorTest.AssemblyName, "RJCP.CodeQuality.HelperClasses.ObjectClassTest", 7);
            Assert.That(privateObject.Target, Is.Not.Null);
            Assert.That(() => { privateObject.Target = null; }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SetAnotherTarget()
        {
            PrivateObject privateObject = new(AccessorTest.AssemblyName, "RJCP.CodeQuality.HelperClasses.ObjectClassTest", 7);
            Assert.That(privateObject.Target, Is.Not.Null);
            ObjectClassTest testInstance = new(33);
            privateObject.Target = testInstance;
            Assert.That(privateObject.Target, Is.SameAs(testInstance));
        }

        [Test]
        public void CallPrivateMethod()
        {
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);

            int value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(7));

            privateObject.Invoke("DoubleProperty", m_BindingFlags);

            value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(49));
        }

        [Test]
        public void CallInexistentMethod()
        {
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);

            Assert.That(() => {
                privateObject.Invoke("XYZ", m_BindingFlags);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void CallInexistentMethod_WithParameterTyeps()
        {
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);

            Assert.That(() => {
                privateObject.Invoke("XYZ", m_BindingFlags, new Type[] { typeof(int) }, new object[] { 42 });
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void CallPublicMethod()
        {
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);

            int value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(7));

            privateObject.Invoke("AddToProperty", 6);

            value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(13));
        }

        [Test]
        public void CallPublicMethodWithoutInstance()
        {
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);

            // Need to provide the BindingFlags.Instance for it to work.
            Assert.That(() => { privateObject.GetFieldOrProperty("m_Value", BindingFlags.NonPublic); },
                Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void CallPublicMethodWithFlags()
        {
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);

            int value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(7));

            privateObject.Invoke("AddToProperty", BindingFlags.Public | BindingFlags.Instance, 6);

            value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(13));
        }

        [Test]
        public void CallOverloadedMethod()
        {
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);

            int result = (int)privateObject.Invoke("Method", new Type[0], new object[0]);
            Assert.That(result, Is.EqualTo(2));

            result = (int)privateObject.Invoke("Method", m_BindingFlags, new Type[] { typeof(int) }, new object[] { 8 });
            Assert.That(result, Is.EqualTo(7));
        }

        [Test]
        public void CallOverloadedMethodWithFlags()
        {
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);

            int result = (int)privateObject.Invoke("Method", BindingFlags.Public | BindingFlags.Instance, new Type[0], new object[0]);
            Assert.That(result, Is.EqualTo(2));

            result = (int)privateObject.Invoke("Method", m_BindingFlags, new Type[] { typeof(int) }, new object[] { 8 });
            Assert.That(result, Is.EqualTo(7));
        }

        [Test]
        public void SetAndGetPublicField()
        {
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);

            int value = (int)privateObject.GetFieldOrProperty("PubField");
            Assert.That(value, Is.EqualTo(7));

            privateObject.SetFieldOrProperty("PubField", 9);

            value = (int)privateObject.GetFieldOrProperty("PubField");
            Assert.That(value, Is.EqualTo(9));
        }

        [Test]
        public void SetAndGetPrivateField()
        {
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);

            int value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(7));

            privateObject.SetFieldOrProperty("m_Value", m_BindingFlags, 9);

            value = (int)privateObject.GetFieldOrProperty("m_Value", m_BindingFlags);
            Assert.That(value, Is.EqualTo(9));
        }

        [Test]
        public void SetAndGetPublicProperty()
        {
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);

            privateObject.SetFieldOrProperty("PubProp", 9);

            int value = (int)privateObject.GetFieldOrProperty("PubProp");
            Assert.That(value, Is.EqualTo(9));
        }

        [Test]
        public void SetAndGetPrivateProperty()
        {
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);

            privateObject.SetFieldOrProperty("Prop", m_BindingFlags, 9);

            int value = (int)privateObject.GetFieldOrProperty("Prop", m_BindingFlags);
            Assert.That(value, Is.EqualTo(9));
        }

        [Test]
        public void SetReadOnlyProperty()
        {
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.SetFieldOrProperty("PropReadOnly", 9);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void GetWriteOnlyProperty()
        {
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                _ = privateObject.GetFieldOrProperty("PropWriteOnly");
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void SetAndGetPropertyNullName()
        {
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                _ = privateObject.GetFieldOrProperty(null);
            }, Throws.TypeOf<ArgumentNullException>());

            Assert.That(() => {
                privateObject.SetFieldOrProperty(null, 42);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SetAndGetPropertyNullName_Bindings()
        {
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                _ = privateObject.GetFieldOrProperty(null, m_BindingFlags);
            }, Throws.TypeOf<ArgumentNullException>());

            Assert.That(() => {
                privateObject.SetFieldOrProperty(null, m_BindingFlags, 42);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SetInexistentProperty()
        {
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.SetFieldOrProperty("InexistentProp", m_BindingFlags, 9);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void GetInexistentProperty()
        {
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.GetFieldOrProperty("InexistentProp", m_BindingFlags);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void SetInexistentField()
        {
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.SetFieldOrProperty("m_InexistentField", m_BindingFlags, 9);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void GetInexistentField()
        {
            PrivateObject privateObject = new(typeof(ObjectClassTest), 7);
            Assert.That(() => {
                privateObject.GetFieldOrProperty("m_InexistentField", m_BindingFlags);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void DifferentTypeAndArgCount()
        {
            Type genericType = typeof(ObjectGenericClassTest<object, string>);
            Assert.That(() => {
                _ = new PrivateObject(AccessorTest.AssemblyName, genericType.GetGenericTypeDefinition().FullName,
                    new[] { typeof(object), typeof(string) }, 9, "abc", 100);
            },
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void InvalidTypeName()
        {
            Assert.That(() => {
                _ = new PrivateObject(AccessorTest.AssemblyName, "abc_xyz", new[] { typeof(object), typeof(string) }, 9, "xyz");
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void ObjectWithNullType()
        {
            object obj = new();
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

        [Test]
        public void PropertyGetSet()
        {
            PrivateObject privateObject = new(typeof(IndexerClass));
            int value = privateObject.GetProperty<int>("Prop");
            Assert.That(value, Is.EqualTo(0));

            privateObject.SetProperty("Prop", 2);
            value = privateObject.GetProperty<int>("Prop");
            Assert.That(value, Is.EqualTo(2));
        }

        [Test]
        public void PropertyGetSetNullName()
        {
            PrivateObject privateObject = new(typeof(IndexerClass));
            Assert.That(() => {
                _ = privateObject.GetProperty<int>(null);
            }, Throws.TypeOf<ArgumentNullException>());

            Assert.That(() => {
                privateObject.SetProperty(null, 42);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void ProeprtyGetSet_BindingFlags()
        {
            PrivateObject privateObject = new(typeof(IndexerClass));
            int value = privateObject.GetProperty<int>("Prop", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(value, Is.EqualTo(0));

            privateObject.SetProperty("Prop", BindingFlags.NonPublic | BindingFlags.Instance, 2);
            value = privateObject.GetProperty<int>("Prop", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(value, Is.EqualTo(2));
        }

        [Test]
        public void PropertyGetSetNullName_BindingFlags()
        {
            PrivateObject privateObject = new(typeof(IndexerClass));
            Assert.That(() => {
                _ = privateObject.GetProperty<int>(null, BindingFlags.NonPublic | BindingFlags.Instance);
            }, Throws.TypeOf<ArgumentNullException>());

            Assert.That(() => {
                privateObject.SetProperty(null, BindingFlags.NonPublic | BindingFlags.Instance, 2);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PropertyGetSet_BindingFlags_Public()
        {
            PrivateObject privateObject = new(typeof(IndexerClass));
            Assert.That(() => {
                _ = privateObject.GetProperty<int>("Prop", BindingFlags.Public);
            }, Throws.TypeOf<MissingMethodException>());

            Assert.That(() => {
                privateObject.SetProperty<int>("Prop", BindingFlags.Public, 42);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void PropertyGetSetIndexed()
        {
            PrivateObject privateObject = new(typeof(IndexerClass));
            bool set = privateObject.GetProperty<bool>("Item", 1);
            Assert.That(set, Is.False);

            privateObject.SetProperty("Item", true, 2);
            set = privateObject.GetProperty<bool>("Item", 2);
            Assert.That(set, Is.True);
        }

        [Test]
        public void PropertyGetSetIndexedNullName()
        {
            PrivateObject privateObject = new(typeof(IndexerClass));
            Assert.That(() => {
                _ = privateObject.GetProperty<bool>(null, 1);
            }, Throws.TypeOf<ArgumentNullException>());

            Assert.That(() => {
                privateObject.SetProperty(null, true, 2);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PropertyGetSetIndexed_BindingFlags()
        {
            PrivateObject privateObject = new(typeof(IndexerClass));
            bool set = privateObject.GetProperty<bool>("Item", BindingFlags.NonPublic | BindingFlags.Instance, 1);
            Assert.That(set, Is.False);

            privateObject.SetProperty("Item", BindingFlags.NonPublic | BindingFlags.Instance, true, 2);
            set = privateObject.GetProperty<bool>("Item", BindingFlags.NonPublic | BindingFlags.Instance, 2);
            Assert.That(set, Is.True);
        }

        [Test]
        public void PropertyGetSetIndexedNullName_BindingFlags()
        {
            PrivateObject privateObject = new(typeof(IndexerClass));
            Assert.That(() => {
                _ = privateObject.GetProperty<bool>(null, BindingFlags.NonPublic | BindingFlags.Instance, 1);
            }, Throws.TypeOf<ArgumentNullException>());

            Assert.That(() => {
                privateObject.SetProperty(null, BindingFlags.NonPublic | BindingFlags.Instance, true, 2);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void ProeprtyGetSetIndexed_BindingFlags_Public()
        {
            PrivateObject privateObject = new(typeof(IndexerClass));
            Assert.That(() => {
                _ = privateObject.GetProperty<int>("Item", BindingFlags.Public, 2);
            }, Throws.TypeOf<MissingMethodException>());

            Assert.That(() => {
                privateObject.SetProperty("Item", BindingFlags.Public, 42, 2);
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void PropertyGetSetIndexed_WithTypes()
        {
            PrivateObject privateObject = new(typeof(IndexerClass));
            bool set = privateObject.GetProperty<bool>("Item", new Type[] { typeof(int) }, 1);
            Assert.That(set, Is.False);

            privateObject.SetProperty("Item", new Type[] { typeof(int) }, true, 2);
            set = privateObject.GetProperty<bool>("Item", new Type[] { typeof(int) }, 2);
            Assert.That(set, Is.True);
        }

        [Test]
        public void PropertyGetSetIndexedNullName_WithTypes()
        {
            PrivateObject privateObject = new(typeof(IndexerClass));
            Assert.That(() => {
                _ = privateObject.GetProperty<bool>(null, new Type[] { typeof(int) }, 1);
            }, Throws.TypeOf<ArgumentNullException>());

            Assert.That(() => {
                privateObject.SetProperty(null, new Type[] { typeof(int) }, true, 2);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PropertyGetSetIndexed_WithTypes_BindingFlags()
        {
            PrivateObject privateObject = new(typeof(IndexerClass));
            bool set = privateObject.GetProperty<bool>("Item", BindingFlags.NonPublic | BindingFlags.Instance, new Type[] { typeof(int) }, new object[] { 1 });
            Assert.That(set, Is.False);

            privateObject.SetProperty("Item", BindingFlags.NonPublic | BindingFlags.Instance, new Type[] { typeof(int) }, true, new object[] { 2 });
            set = privateObject.GetProperty<bool>("Item", BindingFlags.NonPublic | BindingFlags.Instance, new Type[] { typeof(int) }, new object[] { 2 });
            Assert.That(set, Is.True);
        }

        [Test]
        public void PropertyGetSetIndexedNullName_WithTypes_BindingFlags()
        {
            PrivateObject privateObject = new(typeof(IndexerClass));
            Assert.That(() => {
                _ = privateObject.GetProperty<bool>(null, BindingFlags.NonPublic | BindingFlags.Instance, new Type[] { typeof(int) }, new object[] { 1 });
            }, Throws.TypeOf<ArgumentNullException>());

            Assert.That(() => {
                privateObject.SetProperty(null, BindingFlags.NonPublic | BindingFlags.Instance, new Type[] { typeof(int) }, true, new object[] { 2 });
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PropertyGetSetIndexed_WithTypes_BindingFlags_Public()
        {
            PrivateObject privateObject = new(typeof(IndexerClass));
            Assert.That(() => {
                _ = privateObject.GetProperty<bool>("Item", BindingFlags.Public, new Type[] { typeof(int) }, new object[] { 1 });
            }, Throws.TypeOf<MissingMethodException>());

            Assert.That(() => {
                privateObject.SetProperty("Item", BindingFlags.Public, new Type[] { typeof(int) }, true, new object[] { 2 });
            }, Throws.TypeOf<MissingMethodException>());
        }
    }
}
