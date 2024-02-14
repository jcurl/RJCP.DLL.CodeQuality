namespace RJCP.CodeQuality.HelperClasses
{
    using System.Collections.Generic;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Test case uses reflection")]
    internal class IndexerClass
    {
        private readonly HashSet<int> m_HashSet = new();

        private bool this[int index]
        {
            get { return m_HashSet.Contains(index); }
            set
            {
                if (value) {
                    m_HashSet.Add(index);
                } else {
                    m_HashSet.Remove(index);
                }
            }
        }

        private int Prop { get; set; }
    }
}
