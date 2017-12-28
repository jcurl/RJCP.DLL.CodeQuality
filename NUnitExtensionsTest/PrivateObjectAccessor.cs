namespace NUnit.Framework
{
    using System;
    using System.Reflection;
#if MSTEST
    using VsPrivateObject = Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject;
    using VsPrivateType = Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType;
#endif

    // This file allows to compare test results against a Microsoft implementation and our
    // own implementation, to ensure that all test cases written ensure the same behavior
    // for both.
    //
    // when extending the functionality of NUnit.Framework.PrivateObject, ensure to add
    // the extension to the IPrivateObjectAccessor and update the classes under test.
    //
    // To test the MS implementation, ensure you have VS2012 test tools installed, and that
    // you test in debug mode (that defines MSTEST).

    public interface IPrivateObjectAccessor
    {
        object Target { get; set; }
        object Invoke(string name, params object[] args);
        object Invoke(string name, Type[] parameterTypes, object[] args);
        object Invoke(string name, Type[] parameterTypes, object[] args, Type[] typeArguments);
        object Invoke(string name, BindingFlags bindingFlags, params object[] args);
        object Invoke(string name, BindingFlags bindingFlags, Type[] parameterTypes, object[] args);
        void SetFieldOrProperty(string name, object value);
        void SetFieldOrProperty(string name, BindingFlags bindingFlags, object value);
        object GetFieldOrProperty(string name);
        object GetFieldOrProperty(string name, BindingFlags bindingFlags);
    }

    public class PrivateObjectAccessor : IPrivateObjectAccessor
    {
        private readonly PrivateObject m_PrivateObject;

        public PrivateObjectAccessor(object obj)
        {
            m_PrivateObject = new PrivateObject(obj);
        }

        public PrivateObjectAccessor(object obj, string memberToAccess)
        {
            m_PrivateObject = new PrivateObject(obj, memberToAccess);
        }

        public PrivateObjectAccessor(Type objectType, params object[] args)
        {
            m_PrivateObject = new PrivateObject(objectType, args);
        }

        public PrivateObjectAccessor(object obj, PrivateType type)
        {
            m_PrivateObject = new PrivateObject(obj, type);
        }

        public PrivateObjectAccessor(string assemblyName, string typeName, params object[] args)
        {
            m_PrivateObject = new PrivateObject(assemblyName, typeName, args);
        }

        public PrivateObjectAccessor(Type type, Type[] parameterTypes, object[] args)
        {
            m_PrivateObject = new PrivateObject(type, parameterTypes, args);
        }

        public PrivateObjectAccessor(string assemblyName, string typeName, Type[] parameterTypes, object[] args)
        {
            m_PrivateObject = new PrivateObject(assemblyName, typeName, parameterTypes, args);
        }

        public object Target
        {
            get { return m_PrivateObject.Target; }
            set { m_PrivateObject.Target = value; }
        }

        public object GetFieldOrProperty(string name)
        {
            return m_PrivateObject.GetFieldOrProperty(name);
        }

        public object GetFieldOrProperty(string name, BindingFlags bindingFlags)
        {
            return m_PrivateObject.GetFieldOrProperty(name, bindingFlags);
        }

        public object Invoke(string name, params object[] args)
        {
            return m_PrivateObject.Invoke(name, args);
        }

        public object Invoke(string name, BindingFlags bindingFlags, params object[] args)
        {
            return m_PrivateObject.Invoke(name, bindingFlags, args);
        }

        public object Invoke(string name, Type[] parameterTypes, object[] args)
        {
            return m_PrivateObject.Invoke(name, parameterTypes, args);
        }

        public object Invoke(string name, Type[] parameterTypes, object[] args, Type[] typeArguments)
        {
            return m_PrivateObject.Invoke(name, parameterTypes, args, typeArguments);
        }

        public object Invoke(string name, BindingFlags bindingFlags, Type[] parameterTypes, object[] args)
        {
            return m_PrivateObject.Invoke(name, bindingFlags, parameterTypes, args);
        }

        public object Invoke(string name, BindingFlags bindingFlags, Type[] parameterTypes, object[] args, Type[] typeArguments)
        {
            return m_PrivateObject.Invoke(name, bindingFlags, parameterTypes, args, typeArguments);
        }

        public void SetFieldOrProperty(string name, object value)
        {
            m_PrivateObject.SetFieldOrProperty(name, value);
        }

        public void SetFieldOrProperty(string name, BindingFlags bindingFlags, object value)
        {
            m_PrivateObject.SetFieldOrProperty(name, bindingFlags, value);
        }
    }

#if MSTEST
    public class PrivateObjectVsAccessor : IPrivateObjectAccessor
    {
        private readonly VsPrivateObject m_PrivateObject;

        public PrivateObjectVsAccessor(object obj)
        {
            m_PrivateObject = new VsPrivateObject(obj);
        }

        public PrivateObjectVsAccessor(object obj, string memberToAccess)
        {
            m_PrivateObject = new VsPrivateObject(obj, memberToAccess);
        }

        public PrivateObjectVsAccessor(object obj, VsPrivateType type)
        {
            m_PrivateObject = new VsPrivateObject(obj, type);
        }

        public PrivateObjectVsAccessor(string assemblyName, string typeName, params object[] args)
        {
            m_PrivateObject = new VsPrivateObject(assemblyName, typeName, args);
        }

        public PrivateObjectVsAccessor(Type objectType, params object[] args)
        {
            m_PrivateObject = new VsPrivateObject(objectType, args);
        }

        public PrivateObjectVsAccessor(Type type, Type[] parameterTypes, object[] args)
        {
            m_PrivateObject = new VsPrivateObject(type, parameterTypes, args);
        }

        public PrivateObjectVsAccessor(string assemblyName, string typeName, Type[] parameterTypes, object[] args)
        {
            m_PrivateObject = new VsPrivateObject(assemblyName, typeName, parameterTypes, args);
        }

        public object Target
        {
            get { return m_PrivateObject.Target; }
            set { m_PrivateObject.Target = value; }
        }

        public object GetFieldOrProperty(string name)
        {
            return m_PrivateObject.GetFieldOrProperty(name);
        }

        public object GetFieldOrProperty(string name, BindingFlags bindingFlags)
        {
            return m_PrivateObject.GetFieldOrProperty(name, bindingFlags);
        }

        public object Invoke(string name, params object[] args)
        {
            return m_PrivateObject.Invoke(name, args);
        }

        public object Invoke(string name, BindingFlags bindingFlags, params object[] args)
        {
            return m_PrivateObject.Invoke(name, bindingFlags, args);
        }

        public object Invoke(string name, Type[] parameterTypes, object[] args)
        {
            return m_PrivateObject.Invoke(name, parameterTypes, args);
        }

        public object Invoke(string name, BindingFlags bindingFlags, Type[] parameterTypes, object[] args)
        {
            return m_PrivateObject.Invoke(name, bindingFlags, parameterTypes, args);
        }

        public void SetFieldOrProperty(string name, object value)
        {
            m_PrivateObject.SetFieldOrProperty(name, value);
        }

        public void SetFieldOrProperty(string name, BindingFlags bindingFlags, object value)
        {
            m_PrivateObject.SetFieldOrProperty(name, bindingFlags, value);
        }

        public object Invoke(string name, Type[] parameterTypes, object[] args, Type[] typeArguments)
        {
            return m_PrivateObject.Invoke(name, parameterTypes, args, typeArguments);
        }
    }
#endif
}
