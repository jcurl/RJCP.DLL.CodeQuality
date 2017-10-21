namespace NUnit.Framework.HelperClasses
{
    using System;

    /// <summary>
    /// Used for testing the <see cref="PrivateObject"/> class functionality.
    /// </summary>
    internal class ObjectGenericClassTest<T, V>
        where V : class
    {
        private readonly T m_Item;
        private readonly V m_Element;

        public int Value { get; protected set; }

        private ObjectGenericClassTest(string name) { }

        public ObjectGenericClassTest(T item, V element)
        {
            m_Item = item;
            m_Element = element;
            Value = 5;
        }

        public ObjectGenericClassTest(T item, V element, int value)
        {
            m_Item = item;
            m_Element = element;
            Value = value;
        }
    }
}
