namespace NUnit.Framework.HelperClasses
{
    using System;
    using System.Collections.Generic;

    internal class GenericStack<T>
    {
        private Stack<T> m_Stack = new Stack<T>();

        public void Push(T item)
        {
            m_Stack.Push(item);
        }

        public T Pop()
        {
            return m_Stack.Pop();
        }
    }
}
