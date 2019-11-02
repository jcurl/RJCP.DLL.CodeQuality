namespace NUnit.Framework.HelperClasses
{
    using System;

    /// <summary>
    /// Used for testing the <see cref="PrivateObject"/> class functionality.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "Test case uses reflection")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Test case uses reflection")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0052:Remove unread private members", Justification = "Test case uses reflection")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Test case uses reflection")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S4487:Unread \"private\" fields should be removed", Justification = "Test case uses reflection")]
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
