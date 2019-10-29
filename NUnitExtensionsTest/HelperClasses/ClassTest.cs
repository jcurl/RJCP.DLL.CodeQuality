namespace NUnit.Framework.HelperClasses
{
    using System.Collections.Generic;

    /// <summary>
    /// Used for testing the <see cref="AccessorBase"/> class functionality.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "Test case uses reflection")]
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
