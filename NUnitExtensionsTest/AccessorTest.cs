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
            AccessorGenericClassTest accessor = new AccessorGenericClassTest(5);
            Assert.That(accessor.Capacity, Is.EqualTo(5));
        }

        [Test]
        public void RaiseEvent()
        {
            AccessorGenericClassTest accessor = new AccessorGenericClassTest(5);
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
            AccessorGenericClassTest accessor = new AccessorGenericClassTest(5);
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
            AccessorGenericClassTest accessor = new AccessorGenericClassTest(5);
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
            AccessorGenericClassTest accessor = new AccessorGenericClassTest(5);
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
            AccessorGenericClassTest accessor = new AccessorGenericClassTest(5);
            accessor.AddItem(5);

            Assert.That(accessor.GetCount(), Is.EqualTo(1));
        }

        [Test]
        public void OverloadedMethod()
        {
            AccessorGenericClassTest accessor = new AccessorGenericClassTest(5);

            Assert.That(accessor.GetCount(), Is.EqualTo(0));
            accessor.AddItem();

            Assert.That(accessor.GetCount(), Is.EqualTo(1));
            accessor.AddItem(4);

            Assert.That(accessor.GetCount(), Is.EqualTo(2));
        }

        [Test]
        public void InexistentMethod()
        {
            AccessorGenericClassTest accessor = new AccessorGenericClassTest(5);

            Assert.That(() => {
                accessor.InexistentMethod();
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void SetPropertyValue()
        {
            AccessorGenericClassTest accessor = new AccessorGenericClassTest(5);
            accessor.ThreadsNumber = 30;

            Assert.That(accessor.ThreadsNumber, Is.EqualTo(30));
        }

        [Test]
        public void GetInexistentProperty()
        {
            AccessorGenericClassTest accessor = new AccessorGenericClassTest(5);
            Assert.That(() => {
                int itemValue = accessor.InexistentProperty;
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void SetInexistentProperty()
        {
            AccessorGenericClassTest accessor = new AccessorGenericClassTest(5);
            Assert.That(() => {
                accessor.InexistentProperty = 5;
            }, Throws.TypeOf<MissingMethodException>());
        }

        [Test]
        public void GetNullProperty()
        {
            AccessorGenericClassTest accessor = new AccessorGenericClassTest(5);
            Assert.That(() => {
                int element = accessor.Element;
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SetNullProperty()
        {
            AccessorGenericClassTest accessor = new AccessorGenericClassTest(5);
            Assert.That(() => {
                accessor.Element = 4;
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SetNullPropertyValue()
        {
            AccessorGenericClassTest accessor = new AccessorGenericClassTest(5);
            Assert.That(() => {
                accessor.Value = 4;
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void InitializeAccessorNonGeneric()
        {
            AccessorClassTest accessor = new AccessorClassTest(5);
            Assert.That(accessor.Capacity, Is.EqualTo(5));
        }
    }
}
