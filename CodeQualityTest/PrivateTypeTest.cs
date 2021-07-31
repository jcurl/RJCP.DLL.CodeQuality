namespace RJCP.CodeQuality
{
    using System;
    using System.Reflection;
    using HelperClasses;
    using NUnit.Framework;

    [TestFixture(Category = "RJCP.CodeQuality.PrivateType")]
    public class PrivateTypeTest
    {
        #region Validation Tests
        [Test]
        public void NullAssemblyName()
        {
            Assert.That(() => { _ = new PrivateType(null, "RJCP.CodeQuality.InternalClassTest"); }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void EmptyAssemblyName()
        {
            Assert.That(() => { _ = new PrivateType(string.Empty, "RJCP.CodeQuality.InternalClassTest"); }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void NullClassName()
        {
            Assert.That(() => { _ = new PrivateType("RJCP.CodeQualityTest", null); }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void EmptyClassName()
        {
            Assert.That(() => { _ = new PrivateType("RJCP.CodeQualityTest", string.Empty); }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void NullType()
        {
            Assert.That(() => { _ = new PrivateType(null); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void InvokeNullMethodName()
        {
            PrivateType privType = new PrivateType("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.InternalClassTest");
            Assert.That(() => { privType.InvokeStatic(null, 5); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void GetNullPropertyOrField()
        {
            PrivateType privType = new PrivateType("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.InternalClassTest");
            Assert.That(() => { privType.GetStaticFieldOrProperty(null); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SetNullPropertyOrField()
        {
            PrivateType privType = new PrivateType("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.InternalClassTest");
            Assert.That(() => { privType.SetStaticFieldOrProperty(null, 123); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void InvokeInexistentMethod()
        {
            PrivateType privType = new PrivateType("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.InternalClassTest");
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
            try {
                // Microsoft.VisualStudio.TestTools.UnitTesting v10.1.0.0 will fail (VS2015)
                // Microsoft.VisualStudio.TestTools.UnitTesting v14.0.0.0 will pass (VS2017)
                PrivateType privType = new PrivateType("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.InternalClassTest");
                Assert.That(() => { privType.InvokeStatic("ThrowEx", null); },
                    Throws.TypeOf<TargetInvocationException>().With.InnerException.TypeOf<InvalidOperationException>());
            } catch (AssertionException) {
                throw;
            }
        }
        #endregion

        #region Internal Class Tests
        [Test]
        public void InvokeStaticOnTypeFromAssembly()
        {
            PrivateType privType = new PrivateType("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.InternalClassTest");
            int result = (int)privType.InvokeStatic("TestStaticMethod", null);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void InvokeStaticOnTypeFromAssembly_WithArg()
        {
            PrivateType privType = new PrivateType("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.InternalClassTest");
            int result = (int)privType.InvokeStatic("TestIncArg", 5);

            Assert.That(result, Is.EqualTo(6));
        }

        [Test]
        public void InvokeStaticOnTypeFromAssembly_WithArgAndType()
        {
            PrivateType privType = new PrivateType("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.InternalClassTest");
            float result = (float)privType.InvokeStatic("TestIncArg", new Type[1] { typeof(float) }, new object[1] { 5.1f });

            Assert.That(result, Is.EqualTo(6.1f));
        }

        [Test]
        public void InvokeStaticOnTypeFromAssembly_GenericMethod()
        {
            PrivateType privType = new PrivateType("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.InternalClassTest");
            string result = (string)privType.InvokeStatic("GetDescription",
                new Type[1] { typeof(int) },
                new object[1] { 100 },
                new Type[1] { typeof(string) });

            Assert.That(result, Is.EqualTo("100"));
        }

        [Test]
        public void GetField_TypeFromAssembly()
        {
            PrivateType privType = new PrivateType("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.InternalClassTest");
            string result = (string)privType.GetStaticFieldOrProperty("s_MyStatic");

            Assert.That(result, Is.EqualTo("static"));
        }

        [Test]
        public void GetProperty_TypeFromAssembly()
        {
            PrivateType privType = new PrivateType("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.InternalClassTest");
            int result = (int)privType.GetStaticFieldOrProperty("MyStaticProperty");

            Assert.That(result, Is.EqualTo(111));
        }

        [Test]
        public void GetProperty_BindingFlags_TypeFromAssembly()
        {
            PrivateType privType = new PrivateType("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.InternalClassTest");
            int result = (int)privType.GetStaticFieldOrProperty("MyBaseStaticProp", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty);

            Assert.That(result, Is.EqualTo(99));
        }

        [Test]
        public void SetField_TypeFromAssembly()
        {
            PrivateType privType = new PrivateType("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.InternalClassTest");
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

            PrivateType privType = new PrivateType("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.InternalClassTest");
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
            PrivateType privType = new PrivateType("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.InternalClassTest");
            int result = (int)privType.InvokeStatic("MyBaseStaticMethod",
                BindingFlags.NonPublic | BindingFlags.Static,
                null);

            Assert.That(result, Is.EqualTo(111));
        }

        [Test]
        public void InvokeStatic_BaseTypeFromAssembly_ParamType()
        {
            PrivateType privType = new PrivateType("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.InternalClassTest");
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
            PrivateType privType = new PrivateType(typeof(PublicClassTest));
            Assert.That(privType.ReferencedType, Is.EqualTo(typeof(PublicClassTest)));
        }

        [Test]
        public void InvokeStaticFromPublicType()
        {
            PrivateType privType = new PrivateType(typeof(PublicClassTest));
            int result = (int)privType.InvokeStatic("TestPrivateStaticMethod", null);

            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void InvokeStaticFromPublicType_WithArg()
        {
            PrivateType privType = new PrivateType(typeof(PublicClassTest));
            int result = (int)privType.InvokeStatic("TestIncArg", 7);

            Assert.That(result, Is.EqualTo(8));
        }

        [Test]
        public void InvokeStaticFromPublicType_WithArgAndType()
        {
            PrivateType privType = new PrivateType(typeof(PublicClassTest));
            float result = (float)privType.InvokeStatic("TestIncArg", new Type[1] { typeof(float) }, new object[1] { 8.1f });

            Assert.That(result, Is.EqualTo(9.1f));
        }

        [Test]
        public void InvokeStaticFromPublicType_GenericMethod()
        {
            PrivateType privType = new PrivateType(typeof(PublicClassTest));
            string result = (string)privType.InvokeStatic("GetDescription",
                new Type[1] { typeof(int) },
                new object[1] { 100 },
                new Type[1] { typeof(string) });

            Assert.That(result, Is.EqualTo("100"));
        }

        [Test]
        public void GetField_FromPublicType()
        {
            PrivateType privType = new PrivateType(typeof(PublicClassTest));
            string result = (string)privType.GetStaticFieldOrProperty("s_MyStatic");

            Assert.That(result, Is.EqualTo("static"));
        }

        [Test]
        public void GetProperty_FromPublicType()
        {
            PrivateType privType = new PrivateType(typeof(PublicClassTest));
            int result = (int)privType.GetStaticFieldOrProperty("MyStaticProperty");

            Assert.That(result, Is.EqualTo(111));
        }
        #endregion
    }
}
