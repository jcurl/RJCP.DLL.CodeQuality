namespace NUnit.Framework.HelperClasses
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Used for testing the <see cref="AccessorBase"/> class functionality for a generic object.
    /// </summary>
    public class GenericClassTestAccessor : AccessorBase
    {
        public GenericClassTestAccessor(int capacity)
            : base("NUnitExtensionsTest", "NUnit.Framework.HelperClasses.GenericClassTest`1", new[] { typeof(int) }, new object[] { capacity })
        {
            BindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        }

        public event EventHandler<EventArgs> ItemAddedEvent
        {
            add
            {
                AddEventHandler("ItemAddedEvent", value);
            }
            remove
            {
                RemoveEventHandler("ItemAddedEvent", value);
            }
        }

        public event EventHandler<EventArgs> ItemRemovedEvent
        {
            add
            {
                AddEventHandler("ItemRemovedEvent", value);
            }
            remove
            {
                RemoveEventHandler("ItemRemovedEvent", value);
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
            Invoke("AddItem", parameterTypes:  new Type[] { typeof(int) }, args: new object[] { item } );
        }

        public void AddItem()
        {
            Invoke("AddItem", parameterTypes: new Type[0], args: new object[0]);
        }

        public int GetCount()
        {
            return (int)Invoke("GetCount");
        }

        public void InexistentMethod()
        {
            Invoke("InexistentMethod");
        }

        public int InexistentProperty
        {
            get
            {
                return (int)GetFieldOrProperty("InexistentProperty");
            }
            set
            {
                SetFieldOrProperty("InexistentProperty", value);
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
                SetFieldOrProperty("Value", null);
            }
        }

        public int Property
        {
            get
            {
                return (int)GetFieldOrProperty("Property");
            }
            set
            {
                SetFieldOrProperty("Property", value);
            }
        }

        public int Capacity { get { return (int)GetFieldOrProperty("Capacity"); } }

        public int ThreadsNumber
        {
            get
            {
                return (int)GetFieldOrProperty("ThreadsNumber");
            }
            set
            {
                SetFieldOrProperty("ThreadsNumber", value);
            }
        }
    }
}
