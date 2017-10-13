namespace NUnit.Framework
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Represents the type of a private class that gives access to private static implementations.
    /// </summary>
    public class PrivateType
    {
        private Type m_ObjectType;

        /// <summary>
        /// Initializes a new instance of the PrivateType class with the type information.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="objectType"/> may not be null.</exception>
        public PrivateType(Type objectType)
        {
            if (objectType == null) throw new ArgumentNullException("objectType");
            m_ObjectType = objectType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateType"/> class.
        /// </summary>
        /// <param name="assemblyName">The assembly name.</param>
        /// <param name="typeName">Fully qualified name of the type.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="assemblyName"/> or <paramref name="typeName"/> is <value>null</value> or empty.
        /// </exception>
        public PrivateType(string assemblyName, string typeName)
        {
            if (string.IsNullOrWhiteSpace(assemblyName)) throw new ArgumentException("Cannot be null or empty", "assemblyName");
            if (string.IsNullOrWhiteSpace(typeName)) throw new ArgumentException("Cannot be null or empty", "typeName");

            m_ObjectType = Assembly.Load(assemblyName).GetType(typeName);
        }

        /// <summary>
        /// Invokes static methods on the <see cref="PrivateType"/>.
        /// </summary>
        /// <param name="name">The name of the method to invoke.</param>
        /// <param name="args">Any array of arguments to pass.</param>
        /// <returns>An object that represents the invoked static method's return value, if any.</returns>
        /// <exception cref="ArgumentException">There is no method <paramref name="name"/> for this object.</exception>
        /// <remarks>
        /// Invokes the method provided for the object given using Reflection. This method is intended to be the same as
        /// https://msdn.microsoft.com/en-us/library/ms244026.aspx. This was reverse engineered from the assembly
        /// Microsoft.VisualStudio.QualityTools.UnitTestFramework and simplified for this particular use case.
        /// </remarks>
        public object InvokeStatic(string name, params object[] args)
        {
            BindingFlags bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
            return m_ObjectType.InvokeMember(name, bindingFlags, null, null, args);
        }
    }
}
