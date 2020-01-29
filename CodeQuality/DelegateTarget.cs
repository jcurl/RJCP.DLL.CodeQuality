namespace RJCP.CodeQuality
{
    using System;

    internal class DelegateTarget
    {
        private int m_Counter = 0;

        public DelegateTarget(Delegate target)
        {
            Target = target;
        }

        public void AddRef()
        {
            m_Counter++;
        }

        public bool RemoveRef()
        {
            if (m_Counter == 0) throw new InvalidOperationException("Reference count already zero");
            m_Counter--;
            return m_Counter == 0;
        }

        public Delegate Target { get; private set; }
    }
}
