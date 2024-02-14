namespace RJCP.CodeQuality
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Run-time Type Helper.
    /// </summary>
    /// <remarks>
    /// This code is Copyright Microsoft, and shouldn't be deployed to the public,
    /// decompiled from v10.1.0.0 of Microsoft.VisualStudio.QualityTools.UnitTestFramework.
    /// </remarks>
    internal static class RuntimeTypeHelper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Decompiled from MS Sources")]
        internal static MethodBase SelectMethod(BindingFlags bindingAttr, MethodBase[] match, Type[] types, ParameterModifier[] modifiers)
        {
            ThrowHelper.ThrowIfNull(match);
            Type[] typeArray = new Type[types.Length];

            for (int i = 0; i < types.Length; i++) {
                typeArray[i] = types[i].UnderlyingSystemType;
            }
            types = typeArray;

            if (match.Length == 0) return null;

            int num = 0;
            for (int i = 0; i < match.Length; i++) {
                ParameterInfo[] parameters = match[i].GetParameters();
                if (parameters.Length == types.Length) {
                    int j;
                    for (j = 0; j < types.Length; j++) {
                        Type parameterType = parameters[j].ParameterType;
                        if (parameterType.ContainsGenericParameters) {
                            if (parameterType.IsArray != types[j].IsArray) {
                                break;
                            }
                        } else if (parameterType != types[j] && parameterType != typeof(object) && !parameterType.IsAssignableFrom(types[j])) {
                            break;
                        }
                    }
                    if (j == types.Length) {
                        match[num++] = match[i];
                    }
                }
            }

            if (num == 0) return null;
            if (num == 1) return match[0];

            int k = 0;
            bool flag = false;
            int[] numArray = new int[types.Length];
            for (int i = 0; i < types.Length; i++) {
                numArray[i] = i;
            }
            for (int i = 1; i < num; i++) {
                switch (FindMostSpecificMethod(match[k], numArray, null, match[i], numArray, null, types, null)) {
                case 0:
                    flag = true;
                    break;
                case 2:
                    flag = false;
                    k = i;
                    break;
                }
            }
            if (flag) {
                throw new AmbiguousMatchException();
            }
            return match[k];
        }

        private static int FindMostSpecificMethod(MethodBase m1, int[] paramOrder1, Type paramArrayType1, MethodBase m2, int[] paramOrder2, Type paramArrayType2, Type[] types, object[] args)
        {
            int mostSpecific = FindMostSpecific(m1.GetParameters(), paramOrder1, paramArrayType1, m2.GetParameters(), paramOrder2, paramArrayType2, types, args);

            if (mostSpecific != 0) {
                return mostSpecific;
            }

            if (!CompareMethodSigAndName(m1, m2)) {
                return 0;
            }

            int hierarchyDepth1 = GetHierarchyDepth(m1.DeclaringType);
            int hierarchyDepth2 = GetHierarchyDepth(m2.DeclaringType);
            if (hierarchyDepth1 == hierarchyDepth2) {
                return 0;
            }

            return hierarchyDepth1 < hierarchyDepth2 ? 2 : 1;
        }

        private static int FindMostSpecific(ParameterInfo[] p1, int[] paramOrder1, Type paramArrayType1, ParameterInfo[] p2, int[] paramOrder2, Type paramArrayType2, Type[] types, object[] args)
        {
            if (paramArrayType1 is not null && paramArrayType2 is null) {
                return 2;
            }

            if (paramArrayType2 is not null && paramArrayType1 is null) {
                return 1;
            }

            bool flag1 = false;
            bool flag2 = false;
            for (int index = 0; index < types.Length; ++index) {
                if (args is null || args[index] != Type.Missing) {
                    Type c1 = paramArrayType1 is null || paramOrder1[index] < p1.Length - 1 ? p1[paramOrder1[index]].ParameterType : paramArrayType1;
                    Type c2 = paramArrayType2 is null || paramOrder2[index] < p2.Length - 1 ? p2[paramOrder2[index]].ParameterType : paramArrayType2;
                    if (c1 != c2 && !c1.ContainsGenericParameters && !c2.ContainsGenericParameters) {
                        switch (FindMostSpecificType(c1, c2, types[index])) {
                        case 0:
                            return 0;
                        case 1:
                            flag1 = true;
                            continue;
                        case 2:
                            flag2 = true;
                            continue;
                        default:
                            continue;
                        }
                    }
                }
            }
            if (flag1 == flag2) {
                if (!flag1 && p1.Length != p2.Length && args is not null) {
                    if (p1.Length == args.Length)
                        return 1;
                    if (p2.Length == args.Length)
                        return 2;
                }
                return 0;
            }
            return !flag1 ? 2 : 1;
        }

        private static int FindMostSpecificType(Type c1, Type c2, Type t)
        {
            if (c1 == c2) {
                return 0;
            }

            if (c1 == t) {
                return 1;
            }

            if (c2 == t) {
                return 2;
            }

            if (c1.IsByRef || c2.IsByRef) {
                if (c1.IsByRef && c2.IsByRef) {
                    c1 = c1.GetElementType();
                    c2 = c2.GetElementType();
                } else if (c1.IsByRef) {
                    if (c1.GetElementType() == c2)
                        return 2;
                    c1 = c1.GetElementType();
                } else {
                    if (c2.GetElementType() == c1)
                        return 1;
                    c2 = c2.GetElementType();
                }
            }

            bool flag1;
            bool flag2;
            if (c1.IsPrimitive && c2.IsPrimitive) {
                flag1 = true;
                flag2 = true;
            } else {
                flag1 = c1.IsAssignableFrom(c2);
                flag2 = c2.IsAssignableFrom(c1);
            }

            if (flag1 == flag2) {
                return 0;
            }
            return flag1 ? 2 : 1;
        }

        internal static bool CompareMethodSigAndName(MethodBase m1, MethodBase m2)
        {
            ParameterInfo[] parameters1 = m1.GetParameters();
            ParameterInfo[] parameters2 = m2.GetParameters();

            if (parameters1.Length != parameters2.Length) {
                return false;
            }

            int length = parameters1.Length;
            for (int index = 0; index < length; ++index) {
                if (parameters1[index].ParameterType != parameters2[index].ParameterType) {
                    return false;
                }
            }
            return true;
        }

        private static int GetHierarchyDepth(Type t)
        {
            int num = 0;
            Type type = t;
            do {
                ++num;
                type = type.BaseType;
            }
            while (type is not null);
            return num;
        }

        internal static MethodBase FindMostDerivedNewSlotMeth(MethodBase[] match, int cMatches)
        {
            int num = 0;
            MethodBase methodBase = null;
            for (int index = 0; index < cMatches; ++index) {
                int hierarchyDepth = RuntimeTypeHelper.GetHierarchyDepth(match[index].DeclaringType);
                if (hierarchyDepth == num) {
                    throw new AmbiguousMatchException();
                }
                if (hierarchyDepth > num) {
                    num = hierarchyDepth;
                    methodBase = match[index];
                }
            }
            return methodBase;
        }
    }
}
