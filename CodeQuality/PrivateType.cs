namespace RJCP.CodeQuality
{
    using System;
    using System.Globalization;
    using System.Reflection;

    /// <summary>
    /// Represents the type of a private class that gives access to private static implementations.
    /// </summary>
    /// <remarks>
    /// Portions of this code is Copyright Microsoft, and shouldn't be deployed to the public,
    /// decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.
    /// </remarks>
    public class PrivateType
    {
        private readonly Type m_ObjectType;

        private const BindingFlags MemberDefaultBinding = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

        /// <summary>
        /// Initializes a new instance of the PrivateType class with the type information.
        /// </summary>
        /// <param name="type">Type of the object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method is intended to provide the same functionality as
        /// https://msdn.microsoft.com/en-us/library/ms243359.aspx.
        /// </remarks>
        public PrivateType(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            m_ObjectType = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateType"/> class.
        /// </summary>
        /// <param name="assemblyName">The assembly name.</param>
        /// <param name="typeName">Fully qualified name of the type.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="assemblyName"/> or <paramref name="typeName"/> is <see langword="null"/> or empty.
        /// </exception>
        /// <exception cref="TypeLoadException">The type cannot be found.</exception>
        /// <exception cref="System.IO.FileNotFoundException"><paramref name="typeName"/> requires a dependent
        /// assembly that could not be found.</exception>
        /// <exception cref="System.IO.FileLoadException"><para><paramref name="typeName"/> requires a dependent
        /// assembly that was found, but could not be loaded.</para>
        /// - or -
        /// <para>The current assembly was loaded into the reflection-only context, and name requires a dependent
        /// assembly that was not preloaded.</para>
        /// </exception>
        /// <exception cref="BadImageFormatException"><para><paramref name="typeName"/> requires a dependent
        /// assembly, but the file is not a valid assembly.</para>
        /// - or -
        /// <para><paramref name="typeName"/> requires a dependent assembly which was compiled for a version
        /// of the runtime later than the currently loaded version.</para>
        /// </exception>
        /// <remarks>
        /// This method is intended to provide the same functionality as
        /// https://msdn.microsoft.com/en-us/library/ms243304.aspx.
        /// </remarks>
        public PrivateType(string assemblyName, string typeName)
        {
            if (string.IsNullOrEmpty(assemblyName)) throw new ArgumentException(nameof(assemblyName));
            if (string.IsNullOrEmpty(typeName)) throw new ArgumentException(nameof(typeName));

            m_ObjectType = Assembly.Load(assemblyName).GetType(typeName, true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateType"/> class providing type arguments for a generic type.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="typeArguments">The generic argument types.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="assemblyName"/> or <paramref name="typeName"/> is <see langword="null"/> or empty.
        /// - or -
        /// <para><paramref name=" typeName"/> not found.</para>
        /// - or -
        /// <para>The number of elements in <paramref name="typeArguments"/> is not the same as
        /// the number of type parameters in the current generic type definition.</para>
        /// - or -
        /// <para>Any element of <paramref name="typeArguments"/> does not satisfy the constraints
        /// specified for the corresponding type parameter of the current generic type.</para>
        /// - or -
        /// <para><paramref name="typeArguments"/> contains an element that is a pointer type,
        /// a by-ref type, or void.</para>
        /// </exception>
        /// <exception cref="TypeLoadException">The type cannot be found.</exception>
        /// <exception cref="System.IO.FileNotFoundException"><paramref name="typeName"/> requires a dependent
        /// assembly that could not be found.</exception>
        /// <exception cref="System.IO.FileLoadException"><para><paramref name="typeName"/> requires a dependent
        /// assembly that was found, but could not be loaded.</para>
        /// - or -
        /// <para>The current assembly was loaded into the reflection-only context, and name requires a dependent
        /// assembly that was not preloaded.</para>
        /// </exception>
        /// <exception cref="BadImageFormatException"><para><paramref name="typeName"/> requires a dependent
        /// assembly, but the file is not a valid assembly.</para>
        /// - or -
        /// <para><paramref name="typeName"/> requires a dependent assembly which was compiled for a version
        /// of the runtime later than the currently loaded version.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">The invoked method is not supported in the base class.
        /// Derived classes must provide an implementation.</exception>
        /// <remarks>
        /// This constructor is used to obtain a well defined type from a generic type.
        /// </remarks>
        public PrivateType(string assemblyName, string typeName, Type[] typeArguments)
        {
            if (string.IsNullOrEmpty(assemblyName)) throw new ArgumentException(nameof(assemblyName));
            if (string.IsNullOrEmpty(typeName)) throw new ArgumentException(nameof(typeName));

            Type type = Assembly.Load(assemblyName).GetType(typeName, true);
            if (type == null) throw new ArgumentNullException(nameof(typeName));

            m_ObjectType = type.MakeGenericType(typeArguments);
        }

        /// <summary>
        /// Gets the <see cref="Type"/>  representing the <see cref="PrivateType"/> .
        /// </summary>
        /// <value>The referenced type.</value>
        public Type ReferencedType { get { return m_ObjectType; } }

        /// <summary>
        /// Instantiates a new <see cref="PrivateType"/> from the nested type relative to this PrivateType.
        /// </summary>
        /// <param name="typeName">Name of the nested type.</param>
        /// <returns>A new <see cref="PrivateType"/> instance representing the nested type.</returns>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="typeName"/> is <see langword="null"/> or empty.</para>
        /// or
        /// <para><paramref name=" typeName"/> not found.</para>
        /// </exception>
        public PrivateType GetNestedType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) throw new ArgumentException(nameof(typeName));

            Type nestedType = m_ObjectType.GetNestedType(typeName, MemberDefaultBinding);
            if (nestedType == null) throw new ArgumentException("Type not found", nameof(typeName));
            return new PrivateType(nestedType);
        }

        /// <summary>
        /// Instantiates a new <see cref="PrivateType"/> from the nested type relative to this PrivateType.
        /// </summary>
        /// <param name="typeName">Name of the nested type.</param>
        /// <param name="typeArguments">The type arguments for creating the generic type.</param>
        /// <returns>A new <see cref="PrivateType"/> instance representing the nested type.</returns>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="typeName"/> is <see langword="null"/> or empty.</para>
        /// or
        /// <para><paramref name=" typeName"/> not found.</para>
        /// or
        /// <para>The number of elements in <paramref name="typeArguments"/> is not the same as
        /// the number of type parameters in the current generic type definition.</para>
        /// or
        /// <para>Any element of <paramref name="typeArguments"/> does not satisfy the constraints
        /// specified for the corresponding type parameter of the current generic type.</para>
        /// or
        /// <para><paramref name="typeArguments"/> contains an element that is a pointer type,
        /// a by-ref type, or void.</para>
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="typeArguments"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The type does not represent a generic type definition.</exception>
        /// <exception cref="NotSupportedException">The invoked method is not supported in the base class.
        /// Derived classes must provide an implementation.</exception>
        public PrivateType GetNestedType(string typeName, Type[] typeArguments)
        {
            if (string.IsNullOrEmpty(typeName)) throw new ArgumentException(nameof(typeName));

            Type nestedType = m_ObjectType.GetNestedType(typeName, MemberDefaultBinding);
            if (nestedType == null) throw new ArgumentException("Type not found", nameof(typeName));
            Type genericType = nestedType.MakeGenericType(typeArguments);
            return new PrivateType(genericType);
        }

        private object InvokeHelperStatic(string name, BindingFlags bindingFlags, object[] args)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return m_ObjectType.InvokeMember(name, bindingFlags | MemberDefaultBinding, null, null, args, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Invokes static methods on the <see cref="PrivateType"/>.
        /// </summary>
        /// <param name="name">The name of the method to invoke.</param>
        /// <param name="args">Any array of arguments to pass.</param>
        /// <returns>An object that represents the invoked static method's return value, if any.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">There is no method <paramref name="name"/> for this object.</exception>
        /// <exception cref="TargetInvocationException">The method being called throws an exception.</exception>
        /// <remarks>
        /// Invokes the method provided for the object given using Reflection. This method is intended to be the same as
        /// https://msdn.microsoft.com/en-us/library/ms244026.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public object InvokeStatic(string name, params object[] args)
        {
            return InvokeStatic(name, MemberDefaultBinding, null, args, null);
        }

        /// <summary>
        /// Invokes static methods on the <see cref="PrivateType"/>.
        /// </summary>
        /// <param name="name">The name of the method to invoke.</param>
        /// <param name="parameterTypes"><para>An array of Type objects that represents the number,
        /// order, and type of the parameters for the method.</para>
        /// - or -
        /// <para>An empty array of the type <see cref="Type"/>, that is, <c>Type[] types = new Type[0]</c> to get a
        /// method that takes no parameters.</para></param>
        /// <param name="args">An array of arguments to pass.</param>
        /// <returns>An object that represents the invoked static method's return value, if any.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Private accessor <paramref name="name"/> can't be found.</exception>
        /// <exception cref="TargetInvocationException">The method being called throws an exception.</exception>
        /// <remarks>
        /// Invokes the method provided for the object given using Reflection. This method is intended to be the same as
        /// https://msdn.microsoft.com/en-us/library/ms244010.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public object InvokeStatic(string name, Type[] parameterTypes, object[] args)
        {
            return InvokeStatic(name, MemberDefaultBinding, parameterTypes, args, null);
        }

        /// <summary>
        /// Invokes static methods on the <see cref="PrivateType"/>.
        /// </summary>
        /// <param name="name">The name of the method to invoke.</param>
        /// <param name="parameterTypes"><para>An array of Type objects that represents the number,
        /// order, and type of the parameters for the method.</para>
        /// - or -
        /// <para>An empty array of the type <see cref="Type"/>, that is, <c>Type[] types = new Type[0]</c> to get a
        /// method that takes no parameters.</para></param>
        /// <param name="args">An array of arguments to pass.</param>
        /// <param name="typeArguments">An array of type arguments to use when invoking a generic method.</param>
        /// <returns>An object that represents the invoked static method's return value, if any.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Private accessor <paramref name="name"/> can't be found.</exception>
        /// <exception cref="TargetInvocationException">The method being called throws an exception.</exception>
        /// <remarks>
        /// Invokes the method provided for the object given using Reflection. This method is intended to be the same as
        /// https://msdn.microsoft.com/en-us/library/bb546306.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public object InvokeStatic(string name, Type[] parameterTypes, object[] args, Type[] typeArguments)
        {
            return InvokeStatic(name, MemberDefaultBinding, parameterTypes, args, typeArguments);
        }

        /// <summary>
        /// Invokes static methods on the <see cref="PrivateType"/>.
        /// </summary>
        /// <param name="name">The name of the method to invoke.</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more BindingFlags that specifies how the search for the element is conducted.</param>
        /// <param name="args">An array of arguments to pass.</param>
        /// <returns>An object that represents the invoked static method's return value, if any.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Private accessor <paramref name="name"/> can't be found.</exception>
        /// <exception cref="TargetInvocationException">The method being called throws an exception.</exception>
        /// <remarks>
        /// Invokes the method provided for the object given using Reflection. This method is intended to be the same as
        /// https://msdn.microsoft.com/en-us/library/ms244042.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public object InvokeStatic(string name, BindingFlags bindingFlags, params object[] args)
        {
            return InvokeStatic(name, bindingFlags, null, args, null);
        }

        /// <summary>
        /// Invokes static methods on the <see cref="PrivateType"/>.
        /// </summary>
        /// <param name="name">The name of the method to invoke.</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more BindingFlags that specifies how the search for the element is conducted.</param>
        /// <param name="parameterTypes"><para>An array of Type objects that represents the number,
        /// order, and type of the parameters for the method.</para>
        /// - or -
        /// <para>An empty array of the type <see cref="Type"/>, that is, Type[] types = new Type[0] to get a
        /// method that takes no parameters.</para></param>
        /// <param name="args">An array of arguments to pass.</param>
        /// <returns>An object that represents the invoked static method's return value, if any.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Private accessor <paramref name="name"/> can't be found.</exception>
        /// <exception cref="TargetInvocationException">The method being called throws an exception.</exception>
        /// <remarks>
        /// Invokes the method provided for the object given using Reflection. This method is intended to be the same as
        /// https://msdn.microsoft.com/en-us/library/ms244012.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public object InvokeStatic(string name, BindingFlags bindingFlags, Type[] parameterTypes, object[] args)
        {
            return InvokeStatic(name, bindingFlags, parameterTypes, args, null);
        }

        /// <summary>
        /// Invokes static methods on the <see cref="PrivateType"/>.
        /// </summary>
        /// <param name="name">The name of the method to invoke.</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more BindingFlags that specifies how the search for the element is conducted.</param>
        /// <param name="parameterTypes"><para>An array of Type objects that represents the number,
        /// order, and type of the parameters for the method.</para>
        /// - or -
        /// <para>An empty array of the type <see cref="Type"/>, that is, Type[] types = new Type[0] to get a
        /// method that takes no parameters.</para></param>
        /// <param name="args">An array of arguments to pass.</param>
        /// <param name="typeArguments">An array of type arguments to use when invoking a generic method.</param>
        /// <returns>An object that represents the invoked static method's return value, if any.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Private accessor <paramref name="name"/> can't be found.</exception>
        /// <exception cref="TargetInvocationException">The method being called throws an exception.</exception>
        /// <remarks>
        /// Invokes the method provided for the object given using Reflection. This method is a CultureInvariant version
        /// as described by
        /// https://msdn.microsoft.com/en-us/library/bb546274.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public object InvokeStatic(string name, BindingFlags bindingFlags, Type[] parameterTypes, object[] args, Type[] typeArguments)
        {
            if (parameterTypes == null) {
                return InvokeHelperStatic(name, bindingFlags | BindingFlags.InvokeMethod, args);
            }

            if (name == null) throw new ArgumentNullException(nameof(name));
            MethodInfo method = m_ObjectType.GetMethod(name, bindingFlags | MemberDefaultBinding, null, parameterTypes, null);
            if (method == null) {
                string msg = string.Format("Private accessor member {0} not found", name);
                throw new ArgumentException(msg);
            }

            if (method.IsGenericMethodDefinition) {
                return method.MakeGenericMethod(typeArguments).Invoke(null, bindingFlags, null, args, CultureInfo.InvariantCulture);
            }
            return method.Invoke(null, bindingFlags, null, args, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets a value of a static field or property in a wrapped type based on the name.
        /// </summary>
        /// <param name="name">The name of the static field or property to get.</param>
        /// <returns>The value set for the <paramref name="name"/> field or property.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Private accessor <paramref name="name"/> can't be found.</exception>
        /// <exception cref="TargetInvocationException">The field or property being called throws an exception.</exception>
        /// <remarks>
        /// Invokes the method provided for the object given using Reflection. This method is intended to be the same as
        /// https://msdn.microsoft.com/en-us/library/ms244057.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public object GetStaticFieldOrProperty(string name)
        {
            return GetStaticFieldOrProperty(name, MemberDefaultBinding);
        }

        /// <summary>
        /// Gets a value of a static field or property in a wrapped type based on the name.
        /// </summary>
        /// <param name="name">The name of the static field or property to get.</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more BindingFlags that specifies how the search for the static
        /// field or property is conducted. The type of lookup need not be specified.
        /// <para>The default values are GetField, GetProperty, and Static.</para></param>
        /// <returns>The value set for the <paramref name="name"/> field or property.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Private accessor <paramref name="name"/> can't be found.</exception>
        /// <exception cref="TargetInvocationException">The field or property being called throws an exception.</exception>
        /// <remarks>
        /// Invokes the method provided for the object given using Reflection. This method is intended to be the same as
        /// https://msdn.microsoft.com/en-us/library/ms244124.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public object GetStaticFieldOrProperty(string name, BindingFlags bindingFlags)
        {
            return InvokeHelperStatic(name, bindingFlags | BindingFlags.Static | BindingFlags.GetField | BindingFlags.GetProperty, null);
        }

        /// <summary>
        /// Sets a static field or property contained in the wrapped type.
        /// </summary>
        /// <param name="name">The name of the static field or property to get.</param>
        /// <param name="value">The value to set to the static field or property.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Private accessor <paramref name="name"/> can't be found.</exception>
        /// <exception cref="TargetInvocationException">The field or property being called throws an exception.</exception>
        /// <remarks>
        /// Invokes the method provided for the object given using Reflection. This method is intended to be the same as
        /// https://msdn.microsoft.com/en-us/library/ms244118.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public void SetStaticFieldOrProperty(string name, object value)
        {
            SetStaticFieldOrProperty(name, MemberDefaultBinding, value);
        }

        /// <summary>
        /// Sets a static field or property contained in the wrapped type.
        /// </summary>
        /// <param name="name">The name of the static field or property to get.</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more BindingFlags that specifies how the search for the static
        /// field or property is conducted. The type of lookup need not be specified.
        /// <para>The default values are GetField, GetProperty, and Static.</para></param>
        /// <param name="value">The value to set to the static field or property.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Private accessor <paramref name="name"/> can't be found.</exception>
        /// <exception cref="TargetInvocationException">The field or property being called throws an exception.</exception>
        /// <remarks>
        /// Invokes the method provided for the object given using Reflection. This method is intended to be the same as
        /// https://msdn.microsoft.com/en-us/library/ms243923.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public void SetStaticFieldOrProperty(string name, BindingFlags bindingFlags, object value)
        {
            InvokeHelperStatic(name, bindingFlags | BindingFlags.Static | BindingFlags.SetField | BindingFlags.SetProperty, new object[1] { value });
        }
    }
}
