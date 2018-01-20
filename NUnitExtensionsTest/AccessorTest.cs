namespace NUnit.Framework
{
    using System;
    using HelperClasses;

    [TestFixture(Category = "NUnitExtensions.Accessor")]
    public class AccessorTest
    {
        [Test]
        public void InitializeAccessorGeneric()
        {
            GenericClassTestAccessor accessor = new GenericClassTestAccessor(5);
            Assert.That(accessor.Capacity, Is.EqualTo(5));
        }

        [Test]
        public void RaiseEvent()
        {
            GenericClassTestAccessor accessor = new GenericClassTestAccessor(5);
            int count = 0;
            EventHandler<EventArgs> handler = (s, e) => {
                count++;
            };

            accessor.ItemAddedEvent += handler;
            accessor.AddItem(5);
            Assert.That(count, Is.EqualTo(1));

            accessor.ItemAddedEvent -= handler;
            accessor.AddItem(6);
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void RaisePublicEvent()
        {
            EventClassAccessor accessor = new EventClassAccessor();

            int count = 0;
            EventHandler<MyPublicEventArgs> handler = (s, e) => {
                count += e.Value;
            };

            accessor.MyPublicEvent += handler;
            accessor.DoWork(45);
            Assert.That(count, Is.EqualTo(45));
            accessor.DoWork(10);
            Assert.That(count, Is.EqualTo(55));

            accessor.MyPublicEvent -= handler;
            accessor.DoWork(10);
            Assert.That(count, Is.EqualTo(55));
        }

        [Test]
        public void RaisePublicEvent2()
        {
            EventClassAccessor accessor = new EventClassAccessor();

            int count1 = 0;
            EventHandler<MyPublicEventArgs> handler1 = (s, e) => {
                count1 += e.Value;
            };
            int count2 = 0;
            EventHandler<MyPublicEventArgs> handler2 = (s, e) => {
                count2 += e.Value;
            };

            accessor.MyPublicEvent += handler1;
            accessor.DoWork(45);
            Assert.That(count1, Is.EqualTo(45));
            Assert.That(count2, Is.EqualTo(0));

            accessor.MyPublicEvent += handler2;
            accessor.DoWork(10);
            Assert.That(count1, Is.EqualTo(55));
            Assert.That(count2, Is.EqualTo(10));

            accessor.MyPublicEvent -= handler1;
            accessor.DoWork(10);
            Assert.That(count1, Is.EqualTo(55));
            Assert.That(count2, Is.EqualTo(20));

            accessor.MyPublicEvent -= handler2;
            accessor.DoWork(10);
            Assert.That(count1, Is.EqualTo(55));
            Assert.That(count2, Is.EqualTo(20));
        }

        [Test]
        public void AddAndRemoveInexistentEvent()
        {
            GenericClassTestAccessor accessor = new GenericClassTestAccessor(5);
            EventHandler<EventArgs> handler = (s, e) => { };

            Assert.That(() => {
                accessor.ItemRemovedEvent += handler;
            }, Throws.TypeOf<MissingMemberException>());

            Assert.That(() => {
                accessor.ItemRemovedEvent -= handler;
            }, Throws.TypeOf<MissingMemberException>());
        }

        [Test]
        public void NullEventHandler()
        {
            GenericClassTestAccessor accessor = new GenericClassTestAccessor(5);
            EventHandler<EventArgs> handler = null;

            Assert.That(() => {
                accessor.ItemAddedEvent += handler;
            }, Throws.TypeOf<ArgumentNullException>());

            Assert.That(() => {
                accessor.ItemAddedEvent -= handler;
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NullEventName()
        {
            GenericClassTestAccessor accessor = new GenericClassTestAccessor(5);
            EventHandler<EventArgs> handler = (s, e) => { };

            Assert.That(() => {
                accessor.RaiseCustomEvent += handler;
            }, Throws.TypeOf<ArgumentNullException>());

            Assert.That(() => {
                accessor.RaiseCustomEvent -= handler;
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void InvokeMethod()
        {
            GenericClassTestAccessor accessor = new GenericClassTestAccessor(5);
            accessor.AddItem(5);

            Assert.That(accessor.GetCount(), Is.EqualTo(1));
        }

        [Test]
        public void OverloadedMethod()
        {
            GenericClassTestAccessor accessor = new GenericClassTestAccessor(5);

            Assert.That(accessor.GetCount(), Is.EqualTo(0));
            accessor.AddItem();

            Assert.That(accessor.GetCount(), Is.EqualTo(1));
            accessor.AddItem(4);

            Assert.That(accessor.GetCount(), Is.EqualTo(2));
        }

        [Test]
        public void InexistentMethod()
        {
            GenericClassTestAccessor accessor = new GenericClassTestAccessor(5);

            Assert.That(() => {
                accessor.InexistentMethod();
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void SetPropertyValue()
        {
            GenericClassTestAccessor accessor = new GenericClassTestAccessor(5);
            accessor.ThreadsNumber = 30;

            Assert.That(accessor.ThreadsNumber, Is.EqualTo(30));
        }

        [Test]
        public void GetInexistentProperty()
        {
            GenericClassTestAccessor accessor = new GenericClassTestAccessor(5);
            Assert.That(() => {
                int itemValue = accessor.InexistentProperty;
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void SetInexistentProperty()
        {
            GenericClassTestAccessor accessor = new GenericClassTestAccessor(5);
            Assert.That(() => {
                accessor.InexistentProperty = 5;
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void GetNullProperty()
        {
            GenericClassTestAccessor accessor = new GenericClassTestAccessor(5);
            Assert.That(() => {
                int element = accessor.Element;
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SetNullProperty()
        {
            GenericClassTestAccessor accessor = new GenericClassTestAccessor(5);
            Assert.That(() => {
                accessor.Element = 4;
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SetNullPropertyValue()
        {
            GenericClassTestAccessor accessor = new GenericClassTestAccessor(5);
            Assert.That(() => {
                accessor.Value = 4;
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void InitializeAccessorNonGeneric()
        {
            ClassTestAccessor accessor = new ClassTestAccessor(5);
            Assert.That(accessor.Capacity, Is.EqualTo(5));
        }

        [Test]
        public void ConstructorWithException()
        {
            Assert.That(() => { new ClassExceptionCtorTestAccessor(); },
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void ConstructorWithExceptionsParamTypes()
        {
            Assert.That(() => { new ClassExceptionCtorTestAccessor(42); },
                Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void ConstructorWithExceptionTargetInvocation()
        {
            // What happens if the constructor throws itself a TargetInvocationException.
            // We shouldn't remove it. An failing test case might raise
            // InvalidOperationException instead, which would be wrong.
            //
            // Tests with parameterType != null
            Assert.That(() => { new ClassExceptionCtorTestAccessor(43); },
                Throws.TypeOf<System.Reflection.TargetInvocationException>());
        }

        [Test]
        public void ConstructorWithExceptionsParamTypesTargetInvocation()
        {
            // What happens if the constructor throws itself a TargetInvocationException.
            // We shouldn't remove it. An failing test case might raise
            // InvalidOperationException instead, which would be wrong.
            //
            // Tests with parameterType != null
            Assert.That(() => { new ClassExceptionCtorTestAccessor(false, 43); },
                Throws.TypeOf<System.Reflection.TargetInvocationException>());
        }

        [Test]
        public void ConstructorWithException2()
        {
            Assert.That(() => { new ClassExceptionCtorTestAccessor2(); },
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void ConstructorWithExceptionsParamTypes2()
        {
            Assert.That(() => { new ClassExceptionCtorTestAccessor2(42); },
                Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void ConstructorWithExceptionTargetInvocation2()
        {
            // What happens if the constructor throws itself a TargetInvocationException.
            // We shouldn't remove it. An failing test case might raise
            // InvalidOperationException instead, which would be wrong.
            //
            // Tests with parameterType != null
            Assert.That(() => { new ClassExceptionCtorTestAccessor2(43); },
                Throws.TypeOf<System.Reflection.TargetInvocationException>());
        }

        [Test]
        public void ConstructorWithExceptionGeneric()
        {
            Assert.That(() => { new ClassExceptionCtorTestAccessor<object>(); },
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void ConstructorWithExceptionGenericParamTypes()
        {
            Assert.That(() => { new ClassExceptionCtorTestAccessor<string>("foo", 42); },
                Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void ConstructorWithExceptionGenericTargetInvocation()
        {
            // What happens if the constructor throws itself a TargetInvocationException.
            // We shouldn't remove it. An failing test case might raise
            // InvalidOperationException instead, which would be wrong.
            //
            // Tests with parameterType != null
            Assert.That(() => { new ClassExceptionCtorTestAccessor<string>("foo", 43); },
                Throws.TypeOf<System.Reflection.TargetInvocationException>());
        }

        [Test]
        public void ConstructorWithExceptionsParamTypesTargetInvocation2()
        {
            // What happens if the constructor throws itself a TargetInvocationException.
            // We shouldn't remove it. An failing test case might raise
            // InvalidOperationException instead, which would be wrong.
            //
            // Tests with parameterType != null
            Assert.That(() => { new ClassExceptionCtorTestAccessor2(false, 43); },
                Throws.TypeOf<System.Reflection.TargetInvocationException>());
        }

        [Test]
        public void PropertyGetWithException()
        {
            ClassExceptionCtorTestAccessor c = new ClassExceptionCtorTestAccessor(0);
            Assert.That(() => { string x = c.Property; }, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void PropertySetWithException()
        {
            ClassExceptionCtorTestAccessor c = new ClassExceptionCtorTestAccessor(0);
            Assert.That(() => { c.Property = "foo"; }, Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void StaticPropertyGetWithException()
        {
            Assert.That(() => { int x = ClassExceptionCtorTestAccessor2.Property2; }, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void StaticPropertySetWithException()
        {
            Assert.That(() => { ClassExceptionCtorTestAccessor2.Property2 = 42; }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void StaticProperty()
        {
            StaticClassTestAccessor.Property = 42;
            Assert.That(StaticClassTestAccessor.Property, Is.EqualTo(42));
        }

        [Test]
        public void StaticMethod()
        {
            Assert.That(StaticClassTestAccessor.DoSomething, Is.EqualTo("0"));
            StaticClassTestAccessor.Property = 42;
            Assert.That(StaticClassTestAccessor.DoSomething, Is.EqualTo("42"));
        }

        [Test]
        public void DerivedAccessors()
        {
            InternalSimpleBase1Accessor b1 = new InternalSimpleBase1Accessor(42);
            Assert.That(b1.Value, Is.EqualTo(42));
            Assert.That(b1.DoSomething(), Is.EqualTo("42"));

            InternalSimpleDerived1Accessor d1 = new InternalSimpleDerived1Accessor(42, "Life");
            Assert.That(d1.Value, Is.EqualTo(42));
            Assert.That(d1.Description, Is.EqualTo("Life"));
            Assert.That(d1.DoSomething(), Is.EqualTo("Life: 42"));
        }

        [Test]
        public void GenericStackAccessor()
        {
            GenericStackAccessor<int> stack = new GenericStackAccessor<int>();
            stack.Push(1);
            Assert.That(stack.Pop(), Is.EqualTo(1));
        }

        [Test]
        public void PrivateReturnObject()
        {
            RelatedClassTestFactoryAccessor factory = new RelatedClassTestFactoryAccessor();
            RelatedClassTestAccessor item = factory.Create();
            Assert.That(item.Value, Is.EqualTo(42));
        }

        [Test]
        public void PrivateReturnObjectNullPrivateObject()
        {
            RelatedClassTestFactoryAccessor factory = new RelatedClassTestFactoryAccessor();

            // What happens if an Accessor gives a null PrivateObject
            Assert.That(() => { factory.Create_PrivateObjectNull(); }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void PrivateObjectInSignature()
        {
            RelatedItemClassAccessor item = new RelatedItemClassAccessor("test");
            Assert.That(item.Value, Is.EqualTo("test"));

            RelatedCollectionClassAccessor collection = new RelatedCollectionClassAccessor();
            collection.Add(item);
            Assert.That(collection.IsInCollection("test"), Is.True);
            Assert.That(collection.IsInCollection("foo"), Is.False);
        }
    }
}
