namespace NUnit.Framework
{
    using System;

    /// <summary>
    /// Used for testing the <see cref="PrivateObject"/> class functionality.
    /// </summary>
    public class ObjectClassTest
    {
        private int m_Value;

        private object m_Count;

        private string m_Name;

        private int Prop { get; set; }

        public int PubProp { get; set; }

        public int PubField;

        private ObjectClassTest() { }

        private ObjectClassTest(string name) { }

        public ObjectClassTest(int i)
        {
            m_Value = i;
            PubField = i;
        }

        private void DoubleProperty()
        {
            m_Value = m_Value * m_Value;
        }

        public void AddToProperty(int value)
        {
            m_Value += value;
        }

        public void AddCount<T>(T value, string name)
        {
            m_Count = value;
            m_Name = name;
        }

        public int Method()
        {
            return 2;
        }

        protected int Method(int value)
        {
            return 7;
        }
    }
}
