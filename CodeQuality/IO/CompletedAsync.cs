namespace RJCP.CodeQuality.IO
{
    using System;
    using System.Threading;

    internal class CompletedAsync<T> : CompletedAsync
    {
        private readonly T m_Result;

        public CompletedAsync(object state, T result) : base(state)
        {
            m_Result = result;
        }

        public T Result { get { return m_Result; } }
    }

    internal class CompletedAsync : IAsyncResult
    {
        private readonly object m_AsyncState;
        private ManualResetEvent m_AsyncWaitHandle;

        public CompletedAsync(object state)
        {
            m_AsyncState = state;
        }

        public bool IsCompleted { get { return true; } }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (m_AsyncWaitHandle == null) {
                    ManualResetEvent mre = new ManualResetEvent(true);
                    if (Interlocked.CompareExchange(ref m_AsyncWaitHandle, mre, null) != null) {
                        // Another thread created this object's event; dispose the event we just created
                        mre.Close();
                    }
                }
                return m_AsyncWaitHandle;
            }
        }

        public object AsyncState { get { return m_AsyncState; } }

        public bool CompletedSynchronously { get { return true; } }

        public static void End(IAsyncResult result)
        {
            if (!(result is CompletedAsync asyncResult))
                throw new ArgumentException("Invalid IAsyncResult on End", nameof(result));

            asyncResult.m_AsyncWaitHandle?.Close();
        }
    }
}
