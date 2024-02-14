namespace RJCP.CodeQuality.HelperClasses
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

    internal class MyPrivateEventArgs : EventArgs
    {
        public MyPrivateEventArgs(int value)
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
            if (handler is not null) {
                MyPublicEvent(this, args);
            }
        }

        public event EventHandler<MyPrivateEventArgs> MyPrivateEvent;

        protected virtual void OnPrivateEvent(MyPrivateEventArgs args)
        {
            EventHandler<MyPrivateEventArgs> handler = MyPrivateEvent;
            if (handler is not null) {
                handler(this, args);
            }
        }

        public void DoWork(int value)
        {
            OnPublicEvent(new MyPublicEventArgs(value));
            OnPrivateEvent(new MyPrivateEventArgs(value));
        }
    }
}
