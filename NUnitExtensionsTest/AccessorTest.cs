﻿namespace NUnit.Framework
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
    }
}