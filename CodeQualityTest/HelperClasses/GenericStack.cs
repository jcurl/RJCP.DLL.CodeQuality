﻿namespace RJCP.CodeQuality.HelperClasses
{
    using System.Collections.Generic;

    internal class GenericStack<T>
    {
        private readonly Stack<T> m_Stack = new();

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
