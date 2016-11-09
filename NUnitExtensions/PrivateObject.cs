namespace NUnit.Framework
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Allows test code to call methods and properties on the code under test that would be inaccessible because they are not public.
    /// </summary>
    public class PrivateObject
    {
        private Type m_ObjectType;
        private object m_Instance;

        /// <summary>
        /// Initialize the new instance of PrivateObject class using the type.
        /// </summary>
        /// <param name="objectType">The type of the object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="objectType"/> may not be null.</exception>
        public PrivateObject(Type objectType)
        {
            if (objectType == null) throw new ArgumentNullException("objectType");
            m_ObjectType = objectType;
            m_Instance = Activator.CreateInstance(m_ObjectType);
        }

        /// <summary>
        /// Initialize the new instance of PrivateObject class using the type.
        /// </summary>
        /// <param name="objectType">The type of the object.</param>
        /// <param name="args">The parameters to pass to the object types constructor.</param>
        /// <exception cref="ArgumentNullException"><paramref name="objectType"/> and <paramref name="args"/> may not be null.</exception>
        public PrivateObject(Type objectType, params object[] args)
        {
            if (objectType == null) throw new ArgumentNullException("objectType");
            if (args == null) throw new ArgumentNullException("args");
            m_ObjectType = objectType;
            m_Instance = Activator.CreateInstance(m_ObjectType, args);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateObject"/> class. This creates the object of the specified type and wraps it in the private object.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly that contains the type.</param>
        /// <param name="typeName">Fully qualified name of the type.</param>
        /// <param name="args">Arguments to pass to the constructor of the object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblyName"/> or <paramref name="typeName"/> is <value>null</value>.
        /// </exception>
        /// <remarks>
        /// An object is created by using the <paramref name="typeName"/> and <paramref name="args"/>.
        /// </remarks>
        public PrivateObject(string assemblyName, string typeName, params object[] args)
        {
            if (assemblyName == null) throw new ArgumentNullException("assemblyName");
            if (typeName == null) throw new ArgumentNullException("typeName");

            m_ObjectType = Assembly.Load(assemblyName).GetType(typeName);
            m_Instance = Activator.CreateInstance(m_ObjectType, args);
        }

        /// <summary>
        /// Gets or sets the wrapped object.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Try to set this property to <value>null</value>.
        /// </exception>
        /// <value>
        /// The wrapped object.
        /// </value>
        public object Target
        {
            get
            {
                return m_Instance;
            }

            set
            {
                if (value == null) throw new ArgumentNullException("value");
                m_Instance = value;
            }
        }

        /// <summary>
        /// Invoke private methods on a <see cref="PrivateObject"/> object.
        /// </summary>
        /// <param name="name">The name of the method to be invoked.</param>
        /// <param name="args">The arguments required by the method that is to be invoked.</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more <see cref="BindingFlags"/> that specifies how the search for the method is conducted.</param>
        /// <returns>An object that represents the return value of a private member.</returns>
        /// <remarks>
        /// This method is intended to be a simplified version for the existing method:
        /// https://msdn.microsoft.com/en-us/library/ms243710.aspx.
        /// </remarks>
        public object Invoke(string name, BindingFlags bindingFlags, params object[] args)
        {
            MethodInfo methodInfo = m_ObjectType.GetMethod(name, bindingFlags);
            return methodInfo.Invoke(m_Instance, args);
        }

        /// <summary>
        /// Sets a value for the field or property of the wrapped object, identified by name.
        /// </summary>
        /// <param name="name">The name of the private field or property to set.</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more <see cref="BindingFlags"/> that specifies how the search for the field or property is conducted.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentException">Thrown if the field or property given by name, is not found.</exception>
        /// <remarks>
        /// This method is intended to be a simplified version of the existing method:
        /// https://msdn.microsoft.com/en-us/library/ms243964.aspx
        /// </remarks>
        public void SetFieldOrProperty(string name, BindingFlags bindingFlags, object value)
        {
            FieldInfo fieldInfo = m_ObjectType.GetField(name, bindingFlags);
            if (fieldInfo != null) {
                fieldInfo.SetValue(m_Instance, value);
                return;
            }

            PropertyInfo propertyInfo = m_ObjectType.GetProperty(name, bindingFlags);
            if (propertyInfo != null) {
                propertyInfo.SetValue(m_Instance, value, null);
                return;
            }

            throw new ArgumentException("Could not find provided field or property name", "name");
        }

        /// <summary>
        /// Gets a value of a wrapped field or property based on the name.
        /// </summary>
        /// <param name="name">The name of the private field or property to get.</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more <see cref="BindingFlags"/> that specifies how the search for the field or property is conducted. The type of lookup need not be specified.</param>
        /// <returns>The value set for the name field or property.</returns>
        /// <remarks>
        /// This method is intended to be a simplified version of the existing method:
        /// https://msdn.microsoft.com/en-us/library/ms243787.aspx
        /// </remarks>
        public object GetFieldOrProperty(string name, BindingFlags bindingFlags)
        {
            FieldInfo fieldInfo = m_ObjectType.GetField(name, bindingFlags);
            if (fieldInfo != null) return fieldInfo.GetValue(m_Instance);

            PropertyInfo propertyInfo = m_ObjectType.GetProperty(name, bindingFlags);
            if (propertyInfo != null) return propertyInfo.GetValue(m_Instance, null);

            throw new ArgumentException("Could not find provided field or property name", "name");
        }
    }
}
