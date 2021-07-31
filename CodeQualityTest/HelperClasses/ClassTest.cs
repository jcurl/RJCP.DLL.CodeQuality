namespace RJCP.CodeQuality.HelperClasses
{
    using System.Collections.Generic;

    /// <summary>
    /// Used for testing the <see cref="AccessorBase"/> class functionality.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Test case uses reflection")]
    internal class ClassTest
    {
        private readonly List<int> m_List;

        private int Capacity { get { return m_List.Capacity; } }

        public ClassTest(int length)
        {
            m_List = new List<int>(length);
        }
    }
}
