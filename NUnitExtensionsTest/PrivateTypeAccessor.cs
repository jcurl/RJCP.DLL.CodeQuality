namespace NUnit.Framework
{
    using System;
    using System.Reflection;
#if MSTEST
    using VsPrivateType = Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType;
#endif

    // This file allows to compare test results against a Microsoft implementation and our
    // own implementation, to ensure that all test cases written ensure the same behavior
    // for both.
    //
    // when extending the functionality of NUnit.Framework.PrivateObject, ensure to add
    // the extension to the IPrivateObjectAccessor and update the classes under test.

    public interface IPrivateTypeAccessor
    {
        Type ReferencedType { get; }

        object InvokeStatic(string name, BindingFlags bindingFlags, params object[] args);
        object InvokeStatic(string name, BindingFlags bindingFlags, Type[] parameterTypes, object[] args);
        object InvokeStatic(string name, params object[] args);
        object InvokeStatic(string name, Type[] parameterTypes, object[] args);
        object InvokeStatic(string name, Type[] parameterTypes, object[] args, Type[] typeArguments);
        object GetStaticFieldOrProperty(string name);
        object GetStaticFieldOrProperty(string name, BindingFlags bindingFlags);
        void SetStaticFieldOrProperty(string name, object value);
        void SetStaticFieldOrProperty(string name, BindingFlags bindingFlags, object value);
    }

    public class PrivateTypeAccessor : IPrivateTypeAccessor
    {
        private readonly PrivateType m_PrivateType;

        public PrivateTypeAccessor(Type type)
        {
            m_PrivateType = new PrivateType(type);
        }

        public PrivateTypeAccessor(string assemblyName, string typeName)
        {
            m_PrivateType = new PrivateType(assemblyName, typeName);
        }

        public Type ReferencedType
        {
            get { return m_PrivateType.ReferencedType; }
        }

        public object GetStaticFieldOrProperty(string name)
        {
            return m_PrivateType.GetStaticFieldOrProperty(name);
        }

        public object GetStaticFieldOrProperty(string name, BindingFlags bindingFlags)
        {
            return m_PrivateType.GetStaticFieldOrProperty(name, bindingFlags);
        }

        public object InvokeStatic(string name, params object[] args)
        {
            return m_PrivateType.InvokeStatic(name, args);
        }

        public object InvokeStatic(string name, Type[] parameterTypes, object[] args)
        {
            return m_PrivateType.InvokeStatic(name, parameterTypes, args);
        }

        public object InvokeStatic(string name, BindingFlags bindingFlags, params object[] args)
        {
            return m_PrivateType.InvokeStatic(name, bindingFlags, args);
        }

        public object InvokeStatic(string name, Type[] parameterTypes, object[] args, Type[] typeArguments)
        {
            return m_PrivateType.InvokeStatic(name, parameterTypes, args, typeArguments);
        }

        public object InvokeStatic(string name, BindingFlags bindingFlags, Type[] parameterTypes, object[] args)
        {
            return m_PrivateType.InvokeStatic(name, bindingFlags, parameterTypes, args);
        }

        public void SetStaticFieldOrProperty(string name, object value)
        {
            m_PrivateType.SetStaticFieldOrProperty(name, value);
        }

        public void SetStaticFieldOrProperty(string name, BindingFlags bindingFlags, object value)
        {
            m_PrivateType.SetStaticFieldOrProperty(name, bindingFlags, value);
        }
    }

#if MSTEST
    public class PrivateTypeVsAccessor : IPrivateTypeAccessor
    {
        private readonly VsPrivateType m_PrivateType;

        public PrivateTypeVsAccessor(Type type)
        {
            m_PrivateType = new VsPrivateType(type);
        }

        public PrivateTypeVsAccessor(string assemblyName, string typeName)
        {
            m_PrivateType = new VsPrivateType(assemblyName, typeName);
        }

        public Type ReferencedType
        {
            get { return m_PrivateType.ReferencedType; }
        }

        public object GetStaticFieldOrProperty(string name)
        {
            return m_PrivateType.GetStaticFieldOrProperty(name);
        }

        public object GetStaticFieldOrProperty(string name, BindingFlags bindingFlags)
        {
            return m_PrivateType.GetStaticFieldOrProperty(name, bindingFlags);
        }

        public object InvokeStatic(string name, params object[] args)
        {
            return m_PrivateType.InvokeStatic(name, args);
        }

        public object InvokeStatic(string name, Type[] parameterTypes, object[] args)
        {
            return m_PrivateType.InvokeStatic(name, parameterTypes, args);
        }

        public object InvokeStatic(string name, BindingFlags bindingFlags, params object[] args)
        {
            return m_PrivateType.InvokeStatic(name, bindingFlags, args);
        }

        public object InvokeStatic(string name, Type[] parameterTypes, object[] args, Type[] typeArguments)
        {
            return m_PrivateType.InvokeStatic(name, parameterTypes, args, typeArguments);
        }

        public object InvokeStatic(string name, BindingFlags bindingFlags, Type[] parameterTypes, object[] args)
        {
            return m_PrivateType.InvokeStatic(name, bindingFlags, parameterTypes, args);
        }

        public void SetStaticFieldOrProperty(string name, object value)
        {
            m_PrivateType.SetStaticFieldOrProperty(name, value);
        }

        public void SetStaticFieldOrProperty(string name, BindingFlags bindingFlags, object value)
        {
            m_PrivateType.SetStaticFieldOrProperty(name, bindingFlags, value);
        }
    }
#endif
}
