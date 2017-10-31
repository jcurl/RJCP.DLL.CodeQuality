namespace NUnit.Framework
{
    using System;
    using System.Reflection;

    [TestFixture(typeof(PrivateTypeAccessor), Category = "NUnitExtensions.PrivateType")]
    [TestFixture(typeof(PrivateTypeVsAccessor), Category = "VisualStudio.PrivateType")]
    public class PrivateTypeTest<T> where T : class, IPrivateTypeAccessor
    {
        #region Dynamic Creation of Correct PrivateObject
        public static T CreatePrivateType(Type type)
        {
            if (typeof(T) == typeof(PrivateTypeAccessor)) return new PrivateTypeAccessor(type) as T;
            if (typeof(T) == typeof(PrivateTypeVsAccessor)) return new PrivateTypeVsAccessor(type) as T;
            return null;
        }

        public static T CreatePrivateType(string assemblyName, string typeName)
        {
            if (typeof(T) == typeof(PrivateTypeAccessor)) return new PrivateTypeAccessor(assemblyName, typeName) as T;
            if (typeof(T) == typeof(PrivateTypeVsAccessor)) return new PrivateTypeVsAccessor(assemblyName, typeName) as T;
            return null;
        }
        #endregion

        #region Validation Tests
        [Test]
        public void NullAssemblyName()
        {
            Assert.That(() => { CreatePrivateType(null, "NUnit.Framework.InternalClassTest"); }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void EmptyAssemblyName()
        {
            Assert.That(() => { CreatePrivateType(string.Empty, "NUnit.Framework.InternalClassTest"); }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void NullClassName()
        {
            Assert.That(() => { CreatePrivateType("NUnitExtensionsTest", null); }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void EmptyClassName()
        {
            Assert.That(() => { CreatePrivateType("NUnitExtensionsTest", string.Empty); }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void NullType()
        {
            Assert.That(() => { CreatePrivateType(null); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void InvokeNullMethodName()
        {
            T privType = CreatePrivateType("NUnitExtensionsTest", "NUnit.Framework.InternalClassTest");
            Assert.That(() => { privType.InvokeStatic(null, 5); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void GetNullPropertyOrField()
        {
            T privType = CreatePrivateType("NUnitExtensionsTest", "NUnit.Framework.InternalClassTest");
            Assert.That(() => { privType.GetStaticFieldOrProperty(null); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SetNullPropertyOrField()
        {
            T privType = CreatePrivateType("NUnitExtensionsTest", "NUnit.Framework.InternalClassTest");
            Assert.That(() => { privType.SetStaticFieldOrProperty(null, 123); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void InvokeInexistentMethod()
        {
            T privType = CreatePrivateType("NUnitExtensionsTest", "NUnit.Framework.InternalClassTest");
            Assert.That(() => {
                privType.InvokeStatic("IDoNotExist",
                BindingFlags.NonPublic | BindingFlags.Static,
                new Type[1] { typeof(int) },
                new object[1] { 5 });
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ThrowExWhenInvoke()
        {
            T privType = CreatePrivateType("NUnitExtensionsTest", "NUnit.Framework.InternalClassTest");
            Assert.That(() => { privType.InvokeStatic("ThrowEx", null); }, Throws.TypeOf<InvalidOperationException>());
        }
        #endregion

        #region Internal Class Tests
        [Test]
        public void InvokeStaticOnTypeFromAssembly()
        {
            T privType = CreatePrivateType("NUnitExtensionsTest", "NUnit.Framework.InternalClassTest");
            int result = (int)privType.InvokeStatic("TestStaticMethod", null);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void InvokeStaticOnTypeFromAssembly_WithArg()
        {
            T privType = CreatePrivateType("NUnitExtensionsTest", "NUnit.Framework.InternalClassTest");
            int result = (int)privType.InvokeStatic("TestIncArg", 5);

            Assert.That(result, Is.EqualTo(6));
        }

        [Test]
        public void InvokeStaticOnTypeFromAssembly_WithArgAndType()
        {
            T privType = CreatePrivateType("NUnitExtensionsTest", "NUnit.Framework.InternalClassTest");
            float result = (float)privType.InvokeStatic("TestIncArg", new Type[1] { typeof(float) }, new object[1] { 5.1f });

            Assert.That(result, Is.EqualTo(6.1f));
        }

        [Test]
        public void InvokeStaticOnTypeFromAssembly_GenericMethod()
        {
            T privType = CreatePrivateType("NUnitExtensionsTest", "NUnit.Framework.InternalClassTest");
            string result = (string)privType.InvokeStatic("GetDescription",
                new Type[1] { typeof(int) },
                new object[1] { 100 },
                new Type[1] { typeof(string) });

            Assert.That(result, Is.EqualTo("100"));
        }

        [Test]
        public void GetField_TypeFromAssembly()
        {
            T privType = CreatePrivateType("NUnitExtensionsTest", "NUnit.Framework.InternalClassTest");
            string result = (string)privType.GetStaticFieldOrProperty("s_MyStatic");

            Assert.That(result, Is.EqualTo("static"));
        }

        [Test]
        public void GetProperty_TypeFromAssembly()
        {
            T privType = CreatePrivateType("NUnitExtensionsTest", "NUnit.Framework.InternalClassTest");
            int result = (int)privType.GetStaticFieldOrProperty("MyStaticProperty");

            Assert.That(result, Is.EqualTo(111));
        }

        [Test]
        public void GetProperty_BindingFlags_TypeFromAssembly()
        {
            T privType = CreatePrivateType("NUnitExtensionsTest", "NUnit.Framework.InternalClassTest");
            int result = (int)privType.GetStaticFieldOrProperty("MyBaseStaticProp", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty);

            Assert.That(result, Is.EqualTo(99));
        }

        [Test]
        public void SetField_TypeFromAssembly()
        {
            T privType = CreatePrivateType("NUnitExtensionsTest", "NUnit.Framework.InternalClassTest");
            string result = (string)privType.GetStaticFieldOrProperty("s_MyStatic");
            Assert.That(result, Is.EqualTo("static"));

            try {
                privType.SetStaticFieldOrProperty("s_MyStatic", "new_value");
                result = (string)privType.GetStaticFieldOrProperty("s_MyStatic");
                Assert.That(result, Is.EqualTo("new_value"));
            } finally {
                privType.SetStaticFieldOrProperty("s_MyStatic", "static");
            }
        }

        [Test]
        public void SetField_BindingFlags_TypeFromAssembly()
        {
            BindingFlags getPropflags = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty;
            BindingFlags setPropflags = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.SetProperty;

            T privType = CreatePrivateType("NUnitExtensionsTest", "NUnit.Framework.InternalClassTest");
            int result = (int)privType.GetStaticFieldOrProperty("MyBaseStaticProp", getPropflags);
            Assert.That(result, Is.EqualTo(99));

            try {
                privType.SetStaticFieldOrProperty("MyBaseStaticProp", setPropflags, 33);
                result = (int)privType.GetStaticFieldOrProperty("MyBaseStaticProp", getPropflags);
                Assert.That(result, Is.EqualTo(33));
            } finally {
                privType.SetStaticFieldOrProperty("MyBaseStaticProp", setPropflags, 99);
            }

        }

        [Test]
        public void InvokeStatic_BaseTypeFromAssembly()
        {
            T privType = CreatePrivateType("NUnitExtensionsTest", "NUnit.Framework.InternalClassTest");
            int result = (int)privType.InvokeStatic("MyBaseStaticMethod",
                BindingFlags.NonPublic | BindingFlags.Static,
                null);

            Assert.That(result, Is.EqualTo(111));
        }

        [Test]
        public void InvokeStatic_BaseTypeFromAssembly_ParamType()
        {
            T privType = CreatePrivateType("NUnitExtensionsTest", "NUnit.Framework.InternalClassTest");
            string result = (string)privType.InvokeStatic("MyBaseStaticIntMethod",
                BindingFlags.NonPublic | BindingFlags.Static,
                new Type[1] { typeof(int) },
                new object[1] { 123 });

            Assert.That(result, Is.EqualTo("123"));
        }
        #endregion

        #region Public Class Tests
        [Test]
        public void ReferencedType_PublicType()
        {
            T privType = CreatePrivateType(typeof(PublicClassTest));
            Assert.That(privType.ReferencedType, Is.EqualTo(typeof(PublicClassTest)));
        }

        [Test]
        public void InvokeStaticFromPublicType()
        {
            T privType = CreatePrivateType(typeof(PublicClassTest));
            int result = (int)privType.InvokeStatic("TestPrivateStaticMethod", null);

            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void InvokeStaticFromPublicType_WithArg()
        {
            T privType = CreatePrivateType(typeof(PublicClassTest));
            int result = (int)privType.InvokeStatic("TestIncArg", 7);

            Assert.That(result, Is.EqualTo(8));
        }

        [Test]
        public void InvokeStaticFromPublicType_WithArgAndType()
        {
            T privType = CreatePrivateType(typeof(PublicClassTest));
            float result = (float)privType.InvokeStatic("TestIncArg", new Type[1] { typeof(float) }, new object[1] { 8.1f });

            Assert.That(result, Is.EqualTo(9.1f));
        }

        [Test]
        public void InvokeStaticFromPublicType_GenericMethod()
        {
            T privType = CreatePrivateType(typeof(PublicClassTest));
            string result = (string)privType.InvokeStatic("GetDescription",
                new Type[1] { typeof(int) },
                new object[1] { 100 },
                new Type[1] { typeof(string) });

            Assert.That(result, Is.EqualTo("100"));
        }

        [Test]
        public void GetField_FromPublicType()
        {
            T privType = CreatePrivateType(typeof(PublicClassTest));
            string result = (string)privType.GetStaticFieldOrProperty("s_MyStatic");

            Assert.That(result, Is.EqualTo("static"));
        }

        [Test]
        public void GetProperty_FromPublicType()
        {
            T privType = CreatePrivateType(typeof(PublicClassTest));
            int result = (int)privType.GetStaticFieldOrProperty("MyStaticProperty");

            Assert.That(result, Is.EqualTo(111));
        }
        #endregion
    }
}
