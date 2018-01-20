namespace NUnit.Framework.HelperClasses
{
    using System;

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

        public void DoWork(int value)
        {
            Invoke(nameof(DoWork), value);
        }
    }
}
