namespace RJCP.CodeQuality.HelperClasses
{
    using System;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "Test case uses reflection")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1118:Utility classes should not have public constructors", Justification = "Test case uses reflection")]
    internal class NestedStaticGTypes1
    {
        internal class NestedStaticGType<T>
        {
            public static string Name()
            {
                return typeof(T).ToString();
            }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "Test case uses reflection")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1118:Utility classes should not have public constructors", Justification = "Test case uses reflection")]
    internal class NestedStaticGTypes2<T>
    {
        internal class NestedStaticGType
        {
            public static string Name()
            {
                return typeof(T).ToString();
            }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "Test case uses reflection")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1118:Utility classes should not have public constructors", Justification = "Test case uses reflection")]
    internal class NestedStaticGTypes3
    {
        internal class NestedStaticGType
        {
            public static string Name<T>()
            {
                return typeof(T).ToString();
            }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "Test case uses reflection")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1118:Utility classes should not have public constructors", Justification = "Test case uses reflection")]
    internal class NestedStaticGTypes4<T>
    {
        internal class NestedStaticGType<U>
        {
            public static string Name()
            {
                return String.Format("{0}+{1}",
                    typeof(T), typeof(U));
            }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "Test case uses reflection")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1118:Utility classes should not have public constructors", Justification = "Test case uses reflection")]
    internal class NestedStaticGTypes5<T>
    {
        internal class NestedStaticGType<U>
        {
            public static string Name<V>()
            {
                return String.Format("{0}+{1}+{2}",
                    typeof(T), typeof(U), typeof(V));
            }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "Test case uses reflection")]
    internal static class NestedStaticGTypes6<T>
    {
        internal static class NestedStaticGType<U>
        {
            public static string Name<V>()
            {
                return String.Format("{0}+{1}+{2}",
                    typeof(T), typeof(U), typeof(V));
            }
        }
    }

    internal class NestedGTypes1<T>
    {
        private T m_Value;

        public NestedGTypes1(T initialValue)
        {
            m_Value = initialValue;
        }

        public string Value() { return m_Value.ToString(); }

        internal class NestedGType<U>
        {
            private U m_Value;

            public NestedGType(U initialValue)
            {
                m_Value = initialValue;
            }

            public string ValueNested() { return m_Value.ToString(); }
        }
    }
}
