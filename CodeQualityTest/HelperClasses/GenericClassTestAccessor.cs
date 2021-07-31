namespace RJCP.CodeQuality.HelperClasses
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Used for testing the <see cref="AccessorBase"/> class functionality for a generic object.
    /// </summary>
    public class GenericClassTestAccessor : AccessorBase
    {
        public GenericClassTestAccessor(int capacity)
            : base("RJCP.CodeQualityTest", "RJCP.CodeQuality.HelperClasses.GenericClassTest`1",
                  new[] { typeof(int) },      // Constructor signature to use
                  new object[] { capacity },  // Values to the constructor
                  new[] { typeof(int) })      // Class type arguments
        {
            BindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        }

        public event EventHandler<EventArgs> ItemAddedEvent
        {
            add
            {
                AddEventHandler(nameof(ItemAddedEvent), value);
            }
            remove
            {
                RemoveEventHandler(nameof(ItemAddedEvent), value);
            }
        }

        public event EventHandler<EventArgs> ItemRemovedEvent
        {
            add
            {
                AddEventHandler(nameof(ItemRemovedEvent), value);
            }
            remove
            {
                RemoveEventHandler(nameof(ItemRemovedEvent), value);
            }
        }

        public event EventHandler<EventArgs> RaiseCustomEvent
        {
            add
            {
                AddEventHandler(null, value);
            }
            remove
            {
                RemoveEventHandler(null, value);
            }
        }

        public void AddItem(int item)
        {
            Invoke(nameof(AddItem), parameterTypes: new Type[] { typeof(int) }, args: new object[] { item });
        }

        public void AddItem()
        {
            Invoke(nameof(AddItem), parameterTypes: new Type[0], args: new object[0]);
        }

        public int GetCount()
        {
            return (int)Invoke(nameof(GetCount));
        }

        public void InexistentMethod()
        {
            Invoke(nameof(InexistentMethod));
        }

        public int InexistentProperty
        {
            get
            {
                return (int)GetFieldOrProperty(nameof(InexistentProperty));
            }
            set
            {
                SetFieldOrProperty(nameof(InexistentProperty), value);
            }
        }

        public int Element
        {
            get
            {
                return (int)GetFieldOrProperty(null);
            }
            set
            {
                SetFieldOrProperty(null, "123");
            }
        }

        public int Value
        {
            get
            {
                return 0;
            }
            set
            {
                SetFieldOrProperty(nameof(Value), null);
            }
        }

        public int Property
        {
            get
            {
                return (int)GetFieldOrProperty(nameof(Property));
            }
            set
            {
                SetFieldOrProperty(nameof(Property), value);
            }
        }

        public int Capacity { get { return (int)GetFieldOrProperty(nameof(Capacity)); } }

        public int ThreadsNumber
        {
            get
            {
                return (int)GetFieldOrProperty(nameof(ThreadsNumber));
            }
            set
            {
                SetFieldOrProperty(nameof(ThreadsNumber), value);
            }
        }
    }
}
