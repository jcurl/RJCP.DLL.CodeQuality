namespace RJCP.CodeQuality.HelperClasses
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Used for testing the <see cref="AccessorBase"/> class functionality.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Test case uses reflection")]
    internal class GenericClassTest<T>
    {
        private readonly List<T> m_List;
        private int Capacity { get { return m_List.Capacity; } }
        public int Property { get; set; }

        protected int ThreadsNumber = 0;

        public GenericClassTest(int capacity)
        {
            m_List = new List<T>(capacity);
        }

        public void AddItem(T item)
        {
            m_List.Add(item);
            OnItemAdded(new EventArgs());
        }

        protected void AddItem()
        {
            m_List.Add(default);
        }

        public event EventHandler<EventArgs> ItemAddedEvent;

        protected virtual void OnItemAdded(EventArgs args)
        {
            EventHandler<EventArgs> handler = ItemAddedEvent;
            if (handler is not null) handler(this, args);
        }

        protected int GetCount()
        {
            return m_List.Count;
        }

        public static string GenericTypeName()
        {
            return typeof(T).ToString();
        }
    }
}
