namespace NUnit.Framework
{
    using System;

    /// <summary>
    /// Used for testing the <see cref="PrivateObject"/> class functionality.
    /// </summary>
    public class ObjectClassTest
    {
        private int m_Value;

        private int Prop { get; set; }

        public ObjectClassTest(int i)
        {
            m_Value = i;
        }

        private void DoubleProperty()
        {
            m_Value = m_Value * m_Value;
        }

        public void AddToProperty(int value)
        {
            m_Value += value;
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
