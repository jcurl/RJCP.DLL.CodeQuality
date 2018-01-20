namespace NUnit.Framework.HelperClasses
{
    using System;

    public class MyPublicEventArgs : EventArgs
    {
        public MyPublicEventArgs(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }
    }

    internal class EventClass
    {
        public event EventHandler<MyPublicEventArgs> MyPublicEvent;

        protected virtual void OnPublicEvent(MyPublicEventArgs args)
        {
            EventHandler<MyPublicEventArgs> handler = MyPublicEvent;
            if (handler != null) {
                MyPublicEvent(this, args);
            }
        }

        public void DoWork(int value)
        {
            OnPublicEvent(new MyPublicEventArgs(value));
        }
    }
}
