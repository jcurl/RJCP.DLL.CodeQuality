namespace RJCP.CodeQuality
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal class GenericMethodCache
    {
        private readonly Type m_ObjectType;

        public GenericMethodCache(Type type)
        {
            m_ObjectType = type;
        }

        // decompiled from v10.1.0.0 of Microsoft.VisualStudio.QualityTools.UnitTestFramework.
        public MethodInfo GetGenericMethodFromCache(string methodName, Type[] parameterTypes, Type[] typeArguments, BindingFlags bindingFlags, ParameterModifier[] modifiers)
        {
            LinkedList<MethodInfo> candidates = GetMethodCandidates(methodName, parameterTypes, typeArguments, bindingFlags);
            MethodInfo[] methodInfoArray = new MethodInfo[candidates.Count];
            candidates.CopyTo(methodInfoArray, 0);

            if (parameterTypes == null || parameterTypes.Length != 0) {
                return RuntimeTypeHelper.SelectMethod(bindingFlags, methodInfoArray, parameterTypes, modifiers) as MethodInfo;
            }

            for (int i = 0; i < methodInfoArray.Length; ++i) {
                if (!RuntimeTypeHelper.CompareMethodSigAndName(methodInfoArray[i], methodInfoArray[0])) {
                    throw new AmbiguousMatchException();
                }
            }
            return RuntimeTypeHelper.FindMostDerivedNewSlotMeth(methodInfoArray, methodInfoArray.Length) as MethodInfo;
        }

        private Dictionary<string, LinkedList<MethodInfo>> m_MethodCache;

        // decompiled from v10.1.0.0 of Microsoft.VisualStudio.QualityTools.UnitTestFramework.
        private Dictionary<string, LinkedList<MethodInfo>> MethodCache
        {
            get
            {
                if (m_MethodCache == null) {
                    BuildGenericMethodCacheForType(m_ObjectType);
                }
                return m_MethodCache;
            }
        }

        // decompiled from v10.1.0.0 of Microsoft.VisualStudio.QualityTools.UnitTestFramework.
        private void BuildGenericMethodCacheForType(Type type)
        {
            m_MethodCache = new Dictionary<string, LinkedList<MethodInfo>>();
            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (MethodInfo method in methods) {
                if (method.IsGenericMethod || method.IsGenericMethodDefinition) {
                    if (!m_MethodCache.TryGetValue(method.Name, out LinkedList<MethodInfo> linkedList)) {
                        linkedList = new LinkedList<MethodInfo>();
                        m_MethodCache.Add(method.Name, linkedList);
                    }
                    linkedList.AddLast(method);
                }
            }
        }

        // decompiled from v10.1.0.0 of Microsoft.VisualStudio.QualityTools.UnitTestFramework.
        private LinkedList<MethodInfo> GetMethodCandidates(string methodName, Type[] parameterTypes, Type[] typeArguments, BindingFlags bindingFlags)
        {
            LinkedList<MethodInfo> methodCandidates = new LinkedList<MethodInfo>();
            if (!MethodCache.TryGetValue(methodName, out LinkedList<MethodInfo> cachedCandidates)) {
                return methodCandidates;
            }

            foreach (MethodInfo methodInfo in cachedCandidates) {
                bool candidate = true;
                if (methodInfo.GetGenericArguments().Length == typeArguments.Length) {
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    if (parameters.Length == parameterTypes.Length) {
                        if ((bindingFlags & BindingFlags.ExactBinding) != BindingFlags.Default) {
                            int num = 0;
                            foreach (ParameterInfo parameterInfo in parameters) {
                                Type parameterType = parameterTypes[num++];
                                if (parameterInfo.ParameterType.ContainsGenericParameters) {
                                    if (parameterInfo.ParameterType.IsArray != parameterType.IsArray) {
                                        candidate = false;
                                        break;
                                    }
                                } else if (parameterInfo.ParameterType != parameterType) {
                                    candidate = false;
                                    break;
                                }
                            }
                            if (candidate) {
                                methodCandidates.AddLast(methodInfo);
                            }
                        } else {
                            methodCandidates.AddLast(methodInfo);
                        }
                    }
                }
            }
            return methodCandidates;
        }
    }
}
