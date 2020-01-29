namespace RJCP.CodeQuality.HelperClasses
{
    using System;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1118:Utility classes should not have public constructors", Justification = "Access to match original class being accessed")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3218:Inner class members should not shadow outer class \"static\" or type members", Justification = "Not an issue for an accessor")]
    public class NestedStaticGTypes1Accessor
    {
        private const string AssemblyName = "RJCP.CodeQualityTest";
        private const string TypeName = "RJCP.CodeQuality.HelperClasses.NestedStaticGTypes1";
        private static readonly PrivateType AccType = new PrivateType(AssemblyName, TypeName);

        public class NestedStaticGTypeAccessor<T>
        {
            private static readonly PrivateType AccType =
                NestedStaticGTypes1Accessor.AccType.GetNestedType("NestedStaticGType`1", new Type[] { typeof(T) });

            public static string Name()
            {
                return (string)AccessorBase.InvokeStatic(AccType, nameof(Name));
            }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1118:Utility classes should not have public constructors", Justification = "Access to match original class being accessed")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3218:Inner class members should not shadow outer class \"static\" or type members", Justification = "Not an issue for an accessor")]
    public class NestedStaticGTypes2Accessor<T>
    {
        private const string AssemblyName = "RJCP.CodeQualityTest";
        private const string TypeName = "RJCP.CodeQuality.HelperClasses.NestedStaticGTypes2`1";
        private static readonly PrivateType AccType = new PrivateType(AssemblyName, TypeName, new Type[] { typeof(T) });

        public class NestedStaticGTypeAccessor
        {
            private static readonly PrivateType AccType =
                NestedStaticGTypes2Accessor<T>.AccType.GetNestedType("NestedStaticGType", new Type[] { typeof(T) });

            public static string Name()
            {
                return (string)AccessorBase.InvokeStatic(AccType, nameof(Name));
            }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1118:Utility classes should not have public constructors", Justification = "Access to match original class being accessed")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3218:Inner class members should not shadow outer class \"static\" or type members", Justification = "Not an issue for an accessor")]
    public class NestedStaticGTypes3Accessor
    {
        private const string AssemblyName = "RJCP.CodeQualityTest";
        private const string TypeName = "RJCP.CodeQuality.HelperClasses.NestedStaticGTypes3";
        private static readonly PrivateType AccType = new PrivateType(AssemblyName, TypeName);

        public class NestedStaticGTypeAccessor
        {
            private static readonly PrivateType AccType =
                NestedStaticGTypes3Accessor.AccType.GetNestedType("NestedStaticGType");

            public static string Name<T>()
            {
                return (string)AccessorBase.InvokeStatic(AccType, nameof(Name), new Type[] { }, new object[] { }, new Type[] { typeof(T) } );
            }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1118:Utility classes should not have public constructors", Justification = "Access to match original class being accessed")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3218:Inner class members should not shadow outer class \"static\" or type members", Justification = "Not an issue for an accessor")]
    public class NestedStaticGTypes4Accessor<T>
    {
        private const string AssemblyName = "RJCP.CodeQualityTest";
        private const string TypeName = "RJCP.CodeQuality.HelperClasses.NestedStaticGTypes4`1";
        private static readonly PrivateType AccType = new PrivateType(AssemblyName, TypeName, new Type[] { typeof(T) });

        public class NestedStaticGTypeAccessor<U>
        {
            private static readonly PrivateType AccType =
                NestedStaticGTypes4Accessor<T>.AccType.GetNestedType("NestedStaticGType`1", new Type[] { typeof(T), typeof(U) });

            public static string Name()
            {
                return (string)AccessorBase.InvokeStatic(AccType, nameof(Name));
            }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1118:Utility classes should not have public constructors", Justification = "Access to match original class being accessed")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3218:Inner class members should not shadow outer class \"static\" or type members", Justification = "Not an issue for an accessor")]
    public class NestedStaticGTypes5Accessor<T>
    {
        private const string AssemblyName = "RJCP.CodeQualityTest";
        private const string TypeName = "RJCP.CodeQuality.HelperClasses.NestedStaticGTypes5`1";
        private static readonly PrivateType AccType = new PrivateType(AssemblyName, TypeName, new Type[] { typeof(T) });

        public class NestedStaticGTypeAccessor<U>
        {
            private static readonly PrivateType AccType =
                NestedStaticGTypes5Accessor<T>.AccType.GetNestedType("NestedStaticGType`1", new Type[] { typeof(T), typeof(U) });

            public static string Name<V>()
            {
                return (string)AccessorBase.InvokeStatic(AccType, nameof(Name),
                    new Type[] { }, new object[] { }, new Type[] { typeof(V) });
            }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3218:Inner class members should not shadow outer class \"static\" or type members", Justification = "Not an issue for an accessor")]
    public static class NestedStaticGTypes6Accessor<T>
    {
        private const string AssemblyName = "RJCP.CodeQualityTest";
        private const string TypeName = "RJCP.CodeQuality.HelperClasses.NestedStaticGTypes6`1";
        private static readonly PrivateType AccType = new PrivateType(AssemblyName, TypeName, new Type[] { typeof(T) });

        public static class NestedStaticGTypeAccessor<U>
        {
            private static readonly PrivateType AccType =
                NestedStaticGTypes6Accessor<T>.AccType.GetNestedType("NestedStaticGType`1", new Type[] { typeof(T), typeof(U) });

            public static string Name<V>()
            {
                return (string)AccessorBase.InvokeStatic(AccType, nameof(Name),
                    new Type[] { }, new object[] { }, new Type[] { typeof(V) });
            }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3218:Inner class members should not shadow outer class \"static\" or type members", Justification = "Not an issue for an accessor")]
    public class NestedGTypes1Accessor<T> : AccessorBase
    {
        private const string AssemblyName = "RJCP.CodeQualityTest";
        private const string TypeName = "RJCP.CodeQuality.HelperClasses.NestedGTypes1`1";
        private static readonly PrivateType AccType = new PrivateType(AssemblyName, TypeName, new Type[] { typeof(T) });

        public NestedGTypes1Accessor(T initialValue)
            : base(AccType, new Type[] { typeof(T) }, new object[] { initialValue }) { }

        public string Value() {
            return (string)Invoke(nameof(Value));
        }

        public class NestedGTypeAccessor<U> : AccessorBase
        {
            private static readonly PrivateType AccType =
                NestedGTypes1Accessor<T>.AccType.GetNestedType("NestedGType`1", new Type[] { typeof(T), typeof(U) });

            public NestedGTypeAccessor(U initialValue)
                : base(AccType, new Type[] { typeof(U) }, new object[] { initialValue }) { }

            public string ValueNested() {
                return (string)Invoke(nameof(ValueNested));
            }
        }
    }
}
