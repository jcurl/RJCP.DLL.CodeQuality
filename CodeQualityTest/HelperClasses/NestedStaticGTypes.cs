namespace RJCP.CodeQuality.HelperClasses
{
    using System;

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
        private readonly T m_Value;

        public NestedGTypes1(T initialValue)
        {
            m_Value = initialValue;
        }

        public string Value() { return m_Value.ToString(); }

        internal class NestedGType<U>
        {
            private readonly U m_Value;

            public NestedGType(U initialValue)
            {
                m_Value = initialValue;
            }

            public string ValueNested() { return m_Value.ToString(); }
        }
    }
}
