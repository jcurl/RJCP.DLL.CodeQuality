namespace NUnit.Framework.HelperClasses
{
    using System;

    public class MyPrivateEventArgsAccessor : EventArgs
    {
        private class Accessor : AccessorBase
        {
            public Accessor(PrivateObject obj) : base(obj) { }

            public int Value
            {
                get
                {
                    return (int)GetFieldOrProperty(nameof(Value));
                }
            }
        }

        private Accessor m_Accessor;

        public MyPrivateEventArgsAccessor(PrivateObject obj)
        {
            m_Accessor = new Accessor(obj);
        }

        public int Value { get { return m_Accessor.Value; } }
    }

    public class EventClassAccessor : AccessorBase
    {
        private const string AssemblyName = "NUnitExtensionsTest";
        private const string TypeName = "NUnit.Framework.HelperClasses.EventClass";
        public static readonly PrivateType AccType = new PrivateType(AssemblyName, TypeName);

        public EventClassAccessor() : base(AccType) { }

        public event EventHandler<MyPublicEventArgs> MyPublicEvent
        {
            add
            {
                AddEventHandler(nameof(MyPublicEvent), value);
            }
            remove
            {
                RemoveEventHandler(nameof(MyPublicEvent), value);
            }
        }

        public event EventHandler<MyPrivateEventArgsAccessor> MyPrivateEvent
        {
            add
            {
                EventHandler<MyPrivateEventArgsAccessor> handler = value;
                AccessorEventHandler ieh = (s, a) => {
                    handler(s, new MyPrivateEventArgsAccessor(new PrivateObject(a)));
                };
                AddIndirectEventHandler(nameof(MyPrivateEvent), value, ieh);
            }
            remove
            {
                RemoveIndirectEventHandler(nameof(MyPrivateEvent), value);
            }
        }

        public void DoWork(int value)
        {
            Invoke(nameof(DoWork), value);
        }
    }
}
