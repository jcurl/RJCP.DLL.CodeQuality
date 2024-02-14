namespace RJCP.CodeQuality
{
    using System;
    using System.Globalization;
    using System.Reflection;

    /// <summary>
    /// Allows test code to call methods and properties on the code under test that would
    /// normally be inaccessible because they are not public.
    /// </summary>
    /// <remarks>
    /// Portions of this code is Copyright Microsoft, and shouldn't be deployed to the public,
    /// decompiled from v14.0.0.0 of Microsoft.VisualStudio.QualityTools.UnitTestFramework.
    /// </remarks>
    public class PrivateObject
    {
        private Type m_ObjectType;
        private object m_Instance;
        private GenericMethodCache m_MethodCache;

        private const BindingFlags DefaultBindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        #region Construction
        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateObject"/> class that creates the wrapper for the specified object.
        /// </summary>
        /// <param name="obj">The object to wrap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method is intended to provide the same functionality as
        /// https://msdn.microsoft.com/en-us/library/ms243350.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public PrivateObject(object obj)
        {
            ThrowHelper.ThrowIfNull(obj);
            ConstructFrom(obj);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateObject"/> class that creates the wrapper for the specified object.
        /// </summary>
        /// <param name="obj">The object to wrap. This serves as starting point to reach the private members.</param>
        /// <param name="memberToAccess">The dereferencing string that points to the object to be retrieved.
        /// This takes the form of "objectX.objectY.objectZ".</param>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="memberToAccess"/> contains a zero length member,
        /// or is an invalid format.</exception>
        /// <exception cref="TargetInvocationException">The <paramref name="memberToAccess"/> raised an exception in that property or field.</exception>
        /// <remarks>
        /// This method is intended to provide the same functionality as
        /// https://msdn.microsoft.com/en-us/library/ms243385.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public PrivateObject(object obj, string memberToAccess)
        {
            ThrowHelper.ThrowIfNull(obj);
            ValidateAccessString(memberToAccess);

            if (!(obj is PrivateObject privateObject))
                privateObject = new PrivateObject(obj);

            string[] members = memberToAccess.Split('.');
            foreach (string name in members) {
                privateObject = new PrivateObject(privateObject.InvokeHelper(name, DefaultBindingFlags | BindingFlags.GetField | BindingFlags.GetProperty, null));
            }
            ConstructFrom(privateObject.m_Instance);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateObject"/> class.
        /// </summary>
        /// <param name="obj">The object to wrap. This serves as starting point to reach the private members.</param>
        /// <param name="type">The type of the object as given by a <see cref="PrivateType"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> or <paramref name="type"/> may not
        /// be <see langword="null"/>.</exception>
        /// <remarks>
        /// This method is intended to provide the same functionality as
        /// https://msdn.microsoft.com/en-us/library/ms243390.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// <para>This method will raise an exception if <paramref name="obj"/> is <see langword="null"/>, which is not done
        /// in the MS implementation.</para>
        /// </remarks>
        public PrivateObject(object obj, PrivateType type)
        {
            ThrowHelper.ThrowIfNull(type);

            m_Instance = obj;
            m_ObjectType = type.ReferencedType;
            m_MethodCache = new GenericMethodCache(m_ObjectType);
        }

        /// <summary>
        /// Initialize the new instance of <see cref="PrivateObject"/> class using the type.
        /// </summary>
        /// <param name="type">The wrapped object type.</param>
        /// <param name="args">The parameters to pass to the object types constructor.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Constructor not found.</exception>
        /// <exception cref="TargetInvocationException">The constructor being called throws an exception.</exception>
        /// <exception cref="TypeLoadException"><paramref name="type"/> is not a valid type.</exception>
        /// <exception cref="MissingMethodException">No matching public constructor was found to match the
        /// <paramref name="type"/> and <paramref name="args"/>.</exception>
        /// <remarks>
        /// This method is intended to provide the same functionality as
        /// https://msdn.microsoft.com/en-us/library/ms243316.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public PrivateObject(Type type, params object[] args)
            : this(type, null, args) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateObject"/> class.
        /// This creates the object of the specified type and wraps it in the private object.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly that contains the type.</param>
        /// <param name="typeName">Fully qualified name of the type.</param>
        /// <param name="args">Arguments to pass to the constructor of the object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblyName"/> or <paramref name="typeName"/> is <value>null</value>.
        /// </exception>
        /// <exception cref="TargetInvocationException">The constructor being called throws an exception.</exception>
        /// <remarks>
        /// This method is intended to provide the same functionality as
        /// https://msdn.microsoft.com/en-us/library/ms243386.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public PrivateObject(string assemblyName, string typeName, params object[] args)
            : this(GetObjectType(assemblyName, typeName), null, args) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateObject"/> class.
        /// This creates the object of the specified type and wraps it in the private object.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly that contains the type.</param>
        /// <param name="typeName">Fully qualified name of the type.</param>
        /// <param name="parameterTypes">An array of <see cref="Type"/> objects representing the number,
        /// order, and type of the parameters for constructing the object.</param>
        /// <param name="args">Arguments to pass to the constructor of the object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblyName"/> or <paramref name="typeName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException"><para><paramref name="parameterTypes"/> is multidimensional</para>
        /// - or -
        /// <para> constructor cannot be found to match the parameters specified in PrivateObject.</para></exception>
        /// <exception cref="TargetInvocationException">The constructor being called throws an exception.</exception>
        /// <remarks>
        /// This method is intended to provide the same functionality as
        /// https://msdn.microsoft.com/en-us/library/ms243375.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public PrivateObject(string assemblyName, string typeName, Type[] parameterTypes, object[] args)
            : this(GetObjectType(assemblyName, typeName), parameterTypes, args) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateObject"/> class.
        /// </summary>
        /// <param name="type">The wrapped object type.</param>
        /// <param name="parameterTypes">An array of <see cref="Type"/> objects representing the number,
        /// order, and type of the parameters for constructing the object.</param>
        /// <param name="args">The arguments to define the object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Constructor not found.</exception>
        /// <exception cref="TargetInvocationException">The constructor being called throws an exception.</exception>
        /// <exception cref="TypeLoadException"><paramref name="type"/> is not a valid type.</exception>
        /// <exception cref="MissingMethodException">No matching public constructor was found to match the
        /// <paramref name="type"/> and <paramref name="args"/>.</exception>
        /// <remarks>
        /// This method is intended to provide the same functionality as
        /// https://msdn.microsoft.com/en-us/library/ms243297.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public PrivateObject(Type type, Type[] parameterTypes, object[] args)
        {
            ThrowHelper.ThrowIfNull(type);

            object obj;
            if (parameterTypes != null) {
                ConstructorInfo constructor = type.GetConstructor(DefaultBindingFlags, null, parameterTypes, null)
                    ?? throw new ArgumentException("Constructor not found");
                obj = constructor.Invoke(args);
            } else {
                obj = Activator.CreateInstance(type, DefaultBindingFlags | BindingFlags.CreateInstance, null, args, null);
            }
            ConstructFrom(obj);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateObject"/> class.
        /// This creates the object of the specified type and wraps it in a generic private object.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly that contains the type.</param>
        /// <param name="typeName">Fully qualified name of the type.</param>
        /// <param name="parameterTypes">An array of <see cref="Type"/> objects representing the number,
        /// order, and type of the parameters for constructing the object.</param>
        /// <param name="args">Arguments to pass to the constructor of the object.</param>
        /// <param name="typeArguments">The generic types for the arguments used in creating the object.</param>
        /// <exception cref="ArgumentNullException">
        /// <para>
        /// <paramref name="assemblyName"/> is <see langword="null"/>.
        /// </para>
        /// <para>-or-</para>
        /// <para>
        /// <paramref name="typeName"/> is <see langword="null"/>.
        /// </para>
        /// <para>-or-</para>
        /// <para>
        /// <paramref name="assemblyName"/> and <paramref name="typeName"/> is not found (<see cref="Assembly.GetType(string)"/>).
        /// </para>
        /// </exception>
        /// <exception cref="InvalidOperationException"><paramref name="typeName"/> it's not a generic type.</exception>
        /// <exception cref="ArgumentException">
        /// <para>
        /// One of the <paramref name="typeArguments"/> violates the constraints of <paramref name="typeName"/>.
        /// </para>
        /// <para>-or-</para>
        /// <para>
        /// <paramref name="typeName"/> was not found.
        /// </para>
        /// </exception>
        /// <exception cref="TargetInvocationException">The constructor being called throws an exception.</exception>
        /// <exception cref="TypeLoadException"><paramref name="typeName"/> is not a valid type.</exception>
        /// <exception cref="MissingMethodException">No matching public constructor was found to match the
        /// <paramref name="typeName"/> and <paramref name="args"/>.</exception>
        /// <remarks>
        /// An object is created by using the <paramref name="typeName"/> and <paramref name="args"/> of types <paramref name="typeArguments"/>.
        /// </remarks>
        public PrivateObject(string assemblyName, string typeName, Type[] parameterTypes, object[] args, Type[] typeArguments)
            : this(GetGenericObjectType(assemblyName, typeName, typeArguments), parameterTypes, args) { }

        // decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.
        private void ConstructFrom(object obj)
        {
            ThrowHelper.ThrowIfNull(obj);
            m_Instance = obj;
            m_ObjectType = obj.GetType();
            m_MethodCache = new GenericMethodCache(m_ObjectType);
        }

        private static readonly char[] InvalidWhiteSpaceAccessString = new char[3] { ' ', '\t', '\n' };

        // decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.
        private static void ValidateAccessString(string access)
        {
            ThrowHelper.ThrowIfNull(access);
            if (access.Length == 0)
                throw new ArgumentException("Invalid access member syntax");
            foreach (string member in access.Split('.')) {
                if (member.Length != 0) {
                    if (member.IndexOfAny(InvalidWhiteSpaceAccessString) == -1) {
                        continue;
                    }
                }
                throw new ArgumentException("Invalid access member syntax");
            }
        }

        private static Type GetObjectType(string assemblyName, string typeName)
        {
            ThrowHelper.ThrowIfNull(assemblyName);
            ThrowHelper.ThrowIfNull(typeName);

            Assembly assembly;
            try {
                assembly = Assembly.Load(assemblyName);
            } catch (ArgumentException) {
                throw new ArgumentException("Assembly cannot be loaded", nameof(assemblyName));
            }

            try {
                return assembly.GetType(typeName);
            } catch (ArgumentException) {
                throw new ArgumentException("typeName is invalid", nameof(typeName));
            }
        }

        private static Type GetGenericObjectType(string assemblyName, string typeName, Type[] genericTypes)
        {
            Type type = GetObjectType(assemblyName, typeName)
                ?? throw new ArgumentException("typeName is invalid", nameof(typeName));
            return type.MakeGenericType(genericTypes);
        }
        #endregion

        /// <summary>
        /// Gets or sets the wrapped object.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Try to set this property to <see langword="null"/>.
        /// </exception>
        /// <value>
        /// The wrapped object.
        /// </value>
        public object Target
        {
            get { return m_Instance; }
            set
            {
                ThrowHelper.ThrowIfNull(value);
                m_Instance = value;
                m_ObjectType = value.GetType();
            }
        }

        /// <summary>
        /// Gets the type of the underlying object.
        /// </summary>
        /// <value>This represents the Type of the wrapped object.</value>
        public Type RealType
        {
            get { return m_ObjectType; }
        }

        // decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.
        private object InvokeHelper(string name, BindingFlags bindingFlags, object[] args)
        {
            return m_ObjectType.InvokeMember(name, bindingFlags, null, m_Instance, args, CultureInfo.InvariantCulture);
        }

        #region Invoke
        /// <summary>
        /// Invoke private methods on a <see cref="PrivateObject"/> object.
        /// </summary>
        /// <param name="name">The name of the method to be invoked.</param>
        /// <param name="args">Any arguments that the method requires.</param>
        /// <returns>An object that represents the return value of a private member.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Member <paramref name="name"/> not found.</exception>
        /// <exception cref="TargetInvocationException">Invoking method resulted in an exception in that method.</exception>
        /// <remarks>
        /// This method is intended to provide the same functionality as
        /// https://msdn.microsoft.com/en-us/library/ms243741.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public object Invoke(string name, params object[] args)
        {
            return Invoke(name, DefaultBindingFlags, null, args, null);
        }

        /// <summary>
        /// Invoke private methods on a <see cref="PrivateObject"/> object.
        /// </summary>
        /// <param name="name">The name of the method to be invoked.</param>
        /// <param name="parameterTypes"><para>An array of Type objects that represents the number, order,
        /// and type of the parameters for the method to access.</para>
        /// - or -
        /// <para>An empty array of the type Type (that is, Type[] types = new Type[0]) to get a method that
        /// takes no parameters</para></param>
        /// <param name="args">Any arguments that the member requires.</param>
        /// <returns>An object that represents the return value of the invoked method.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Member <paramref name="name"/> not found.</exception>
        /// <exception cref="TargetInvocationException">Invoking method resulted in an exception in that method.</exception>
        /// <remarks>
        /// This method is intended to provide the same functionality as
        /// https://msdn.microsoft.com/en-us/library/ms243743.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public object Invoke(string name, Type[] parameterTypes, object[] args)
        {
            return Invoke(name, DefaultBindingFlags, parameterTypes, args, null);
        }

        /// <summary>
        /// Used to access generic members of a <see cref="PrivateObject"/>.
        /// </summary>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="parameterTypes"><para>An array of Type objects that represents the number, order,
        /// and type of the parameters for the method to access.</para>
        /// - or -
        /// <para>An empty array of the type Type (that is, Type[] types = new Type[0]) to get a method
        /// that takes no parameters.</para>
        /// </param>
        /// <param name="args">Any arguments that the member requires.</param>
        /// <param name="typeArguments">An array of type arguments to use when invoking a generic method.</param>
        /// <returns>
        /// An object that represents the return value of a private member.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Member <paramref name="name"/> not found.</exception>
        /// <exception cref="TargetInvocationException">Invoking method resulted in an exception in that method.</exception>
        /// <remarks>
        /// This method is intended to provide the same functionality as
        /// https://msdn.microsoft.com/en-us/library/bb546556.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public object Invoke(string name, Type[] parameterTypes, object[] args, Type[] typeArguments)
        {
            return Invoke(name, DefaultBindingFlags, parameterTypes, args, typeArguments);
        }

        /// <summary>
        /// Invoke private methods on a <see cref="PrivateObject"/> object.
        /// </summary>
        /// <param name="name">The name of the method to be invoked.</param>
        /// <param name="args">The arguments required by the method that is to be invoked.</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more <see cref="BindingFlags"/> that specifies how the search for the method is conducted.</param>
        /// <returns>An object that represents the return value of a private member.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Member <paramref name="name"/> not found.</exception>
        /// <exception cref="TargetInvocationException">Invoking method resulted in an exception in that method.</exception>
        /// <remarks>
        /// This method is intended to provide the same functionality as
        /// https://msdn.microsoft.com/en-us/library/ms243710.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public object Invoke(string name, BindingFlags bindingFlags, params object[] args)
        {
            return Invoke(name, bindingFlags, null, args, null);
        }

        /// <summary>
        /// Invoke private methods on a <see cref="PrivateObject" /> object.
        /// </summary>
        /// <param name="name">The name of the method to be invoked.</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more <see cref="BindingFlags" /> that specifies how the search for the method is conducted.</param>
        /// <param name="parameterTypes">The parameter types required by the method that is to be invoked.</param>
        /// <param name="args">The arguments required by the method that is to be invoked.</param>
        /// <returns>
        /// An object that represents the return value of a private member.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Member <paramref name="name"/> not found.</exception>
        /// <exception cref="TargetInvocationException">Invoking method resulted in an exception in that method.</exception>
        /// <remarks>
        /// This method is intended to provide the same functionality as
        /// https://msdn.microsoft.com/en-us/library/ms243755.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public object Invoke(string name, BindingFlags bindingFlags, Type[] parameterTypes, object[] args)
        {
            return Invoke(name, bindingFlags, parameterTypes, args, null);
        }

        /// <summary>
        /// Used to access generic members of a <see cref="PrivateObject"/>.
        /// </summary>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more BindingFlags that
        /// specifies how the search for the field or property is conducted. The type of lookup need not be specified.</param>
        /// <param name="parameterTypes"><para>An array of Type objects that represents the number, order,
        /// and type of the parameters for the method to access.</para>
        /// - or -
        /// <para>An empty array of the type Type(that is, Type[] types = new Type[0]) to get a method that takes no parameters.</para>
        /// </param>
        /// <param name="args">Any arguments that the member requires.</param>
        /// <param name="typeArguments">An array of type arguments to use when invoking a generic method.</param>
        /// <returns>
        /// An object that represents the return value of a private member.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/></exception>
        /// <exception cref="MissingMethodException">Member <paramref name="name"/> not found.</exception>
        /// <exception cref="TargetInvocationException">Invoking method resulted in an exception in that method.</exception>
        /// <remarks>
        /// This method is a culture invariant form of the MS implementation
        /// https://msdn.microsoft.com/en-us/library/bb546207.aspx. It wasn't in
        /// the original implementation by Microsoft.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public object Invoke(string name, BindingFlags bindingFlags, Type[] parameterTypes, object[] args, Type[] typeArguments)
        {
            ThrowHelper.ThrowIfNull(name);
            if (parameterTypes == null) {
                return InvokeHelper(name, bindingFlags | BindingFlags.InvokeMethod, args);
            }

            MethodInfo methodInfo = m_ObjectType.GetMethod(name, bindingFlags, null, parameterTypes, null);
            if (methodInfo == null && typeArguments != null) {
                methodInfo = m_MethodCache.GetGenericMethodFromCache(name, parameterTypes, typeArguments, bindingFlags, null);
            }

            if (methodInfo == null) {
                string msg = string.Format("Member {0} not found", name);
                throw new MissingMethodException(msg);
            }

            if (methodInfo.IsGenericMethodDefinition) {
                return methodInfo.MakeGenericMethod(typeArguments).Invoke(m_Instance, bindingFlags, null, args, CultureInfo.InvariantCulture);
            }
            return methodInfo.Invoke(m_Instance, bindingFlags, null, args, CultureInfo.InvariantCulture);
        }
        #endregion

        #region Fields and Properties
        /// <summary>
        /// Sets a value for the field or property of the wrapped object, identified by name.
        /// </summary>
        /// <param name="name">The name of the private field or property to set.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="MissingMethodException">The field or property <paramref name="name"/> doesn't exist.</exception>
        /// <exception cref="TargetInvocationException">Invoking method resulted in an exception in that method.</exception>
        /// <remarks>
        /// This method is intended to provide the same functionality as
        /// https://msdn.microsoft.com/en-us/library/ms244054.aspx
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public void SetFieldOrProperty(string name, object value)
        {
            SetFieldOrProperty(name, DefaultBindingFlags, value);
        }

        /// <summary>
        /// Sets a value for the field or property of the wrapped object, identified by name.
        /// </summary>
        /// <param name="name">The name of the private field or property to set.</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more <see cref="BindingFlags"/> that specifies how the search for the field or property is conducted.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="MissingMethodException">The field or property <paramref name="name"/> doesn't exist.</exception>
        /// <exception cref="TargetInvocationException">Invoking method resulted in an exception in that method.</exception>
        /// <remarks>
        /// This method is intended to provide the same functionality as
        /// https://msdn.microsoft.com/en-us/library/ms243964.aspx
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public void SetFieldOrProperty(string name, BindingFlags bindingFlags, object value)
        {
            ThrowHelper.ThrowIfNull(name);
            InvokeHelper(name, bindingFlags | BindingFlags.SetField | BindingFlags.SetProperty, new object[1] { value });
        }

        /// <summary>
        /// Gets a value of a wrapped field or property based on the name.
        /// </summary>
        /// <param name="name">The name of the private field or property to get.</param>
        /// <returns>The value set for the name field or property.</returns>
        /// <exception cref="MissingMethodException">The field or property <paramref name="name"/> doesn't exist.</exception>
        /// <exception cref="TargetInvocationException">Invoking method resulted in an exception in that method.</exception>
        /// <remarks>
        /// This method is intended to provide the same functionality as
        /// https://msdn.microsoft.com/en-us/library/ms243729.aspx.
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public object GetFieldOrProperty(string name)
        {
            return GetFieldOrProperty(name, DefaultBindingFlags);
        }

        /// <summary>
        /// Gets a value of a wrapped field or property based on the name.
        /// </summary>
        /// <param name="name">The name of the private field or property to get.</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more <see cref="BindingFlags"/> that specifies how the search for the field or property is conducted. The type of lookup need not be specified.</param>
        /// <returns>The value set for the name field or property.</returns>
        /// <exception cref="MissingMethodException">The field or property <paramref name="name"/> doesn't exist.</exception>
        /// <exception cref="TargetInvocationException">Invoking method resulted in an exception in that method.</exception>
        /// <remarks>
        /// This method is intended to provide the same functionality as
        /// https://msdn.microsoft.com/en-us/library/ms243787.aspx
        /// <para>It has been decompiled from v14.0.0.0 of Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.</para>
        /// </remarks>
        public object GetFieldOrProperty(string name, BindingFlags bindingFlags)
        {
            ThrowHelper.ThrowIfNull(name);
            return InvokeHelper(name, bindingFlags | BindingFlags.GetField | BindingFlags.GetProperty, null);
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <typeparam name="T">The return type of the property.</typeparam>
        /// <param name="name">The name of the property. Use <c>Item</c> for the indexer.</param>
        /// <returns>The value of the property.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="MissingMethodException">Property <paramref name="name"/> not found.</exception>
        /// <exception cref="TargetInvocationException">Invoking method resulted in an exception in that method.</exception>
        public T GetProperty<T>(string name)
        {
            return GetProperty<T>(name, DefaultBindingFlags, null, null);
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <typeparam name="T">The return type of the property.</typeparam>
        /// <param name="name">The name of the property. Use <c>Item</c> for the indexer.</param>
        /// <param name="bindingFlags">
        /// A bitmask comprised of one or more <see cref="BindingFlags"/> that specifies how the search for the field or
        /// property is conducted. The type of lookup need not be specified.
        /// </param>
        /// <returns>The value of the property.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="MissingMethodException">Property <paramref name="name"/> not found.</exception>
        /// <exception cref="TargetInvocationException">Invoking method resulted in an exception in that method.</exception>
        public T GetProperty<T>(string name, BindingFlags bindingFlags)
        {
            return GetProperty<T>(name, bindingFlags, null, null);
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <typeparam name="T">The return type of the property.</typeparam>
        /// <param name="name">The name of the property. Use <c>Item</c> for the indexer.</param>
        /// <param name="args">The arguments for indexing the property.</param>
        /// <returns>The value of the property.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="MissingMethodException">Property <paramref name="name"/> not found.</exception>
        /// <exception cref="TargetInvocationException">Invoking method resulted in an exception in that method.</exception>
        public T GetProperty<T>(string name, params object[] args)
        {
            return GetProperty<T>(name, DefaultBindingFlags, null, args);
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <typeparam name="T">The return type of the property.</typeparam>
        /// <param name="name">The name of the property. Use <c>Item</c> for the indexer.</param>
        /// <param name="parameterTypes">The parameter types for the arguments.</param>
        /// <param name="args">The arguments for indexing the property.</param>
        /// <returns>The value of the property.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="MissingMethodException">Property <paramref name="name"/> not found.</exception>
        /// <exception cref="TargetInvocationException">Invoking method resulted in an exception in that method.</exception>
        public T GetProperty<T>(string name, Type[] parameterTypes, params object[] args)
        {
            return GetProperty<T>(name, DefaultBindingFlags, parameterTypes, args);
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <typeparam name="T">The return type of the property.</typeparam>
        /// <param name="name">The name of the property. Use <c>Item</c> for the indexer.</param>
        /// <param name="bindingFlags">
        /// A bitmask comprised of one or more <see cref="BindingFlags"/> that specifies how the search for the field or
        /// property is conducted. The type of lookup need not be specified.
        /// </param>
        /// <param name="args">The arguments for indexing the property.</param>
        /// <returns>The value of the property.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="MissingMethodException">Property <paramref name="name"/> not found.</exception>
        /// <exception cref="TargetInvocationException">Invoking method resulted in an exception in that method.</exception>
        public T GetProperty<T>(string name, BindingFlags bindingFlags, params object[] args)
        {
            return GetProperty<T>(name, bindingFlags, null, args);
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <typeparam name="T">The return type of the property.</typeparam>
        /// <param name="name">The name of the property. Use <c>Item</c> for the indexer.</param>
        /// <param name="bindingFlags">
        /// A bitmask comprised of one or more <see cref="BindingFlags"/> that specifies how the search for the field or
        /// property is conducted. The type of lookup need not be specified.
        /// </param>
        /// <param name="parameterTypes">The parameter types for the arguments.</param>
        /// <param name="args">The arguments for indexing the property.</param>
        /// <returns>The value of the property.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="MissingMethodException">Property <paramref name="name"/> not found.</exception>
        /// <exception cref="TargetInvocationException">Invoking method resulted in an exception in that method.</exception>
        public T GetProperty<T>(string name, BindingFlags bindingFlags, Type[] parameterTypes, object[] args)
        {
            ThrowHelper.ThrowIfNull(name);
            if (parameterTypes == null)
                return (T)InvokeHelper(name, bindingFlags | BindingFlags.GetProperty, args);

            PropertyInfo propInfo = m_ObjectType.GetProperty(name, bindingFlags, null, typeof(T), parameterTypes, null);
            if (propInfo == null) {
                string msg = string.Format("Property {0} not found", name);
                throw new MissingMethodException(msg);
            }
            return (T)propInfo.GetValue(m_Instance, args);
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <typeparam name="T">The return type of the property.</typeparam>
        /// <param name="name">The name of the property. Use <c>Item</c> for the indexer.</param>
        /// <param name="value">The value to set the property to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="MissingMethodException">Property <paramref name="name"/> not found.</exception>
        /// <exception cref="TargetInvocationException">Invoking method resulted in an exception in that method.</exception>
        public void SetProperty<T>(string name, T value)
        {
            SetProperty(name, DefaultBindingFlags, null, value, null);
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <typeparam name="T">The return type of the property.</typeparam>
        /// <param name="name">The name of the property. Use <c>Item</c> for the indexer.</param>
        /// <param name="bindingFlags">
        /// A bitmask comprised of one or more <see cref="BindingFlags"/> that specifies how the search for the field or
        /// property is conducted. The type of lookup need not be specified.
        /// </param>
        /// <param name="value">The value to set the property to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="MissingMethodException">Property <paramref name="name"/> not found.</exception>
        /// <exception cref="TargetInvocationException">Invoking method resulted in an exception in that method.</exception>
        public void SetProperty<T>(string name, BindingFlags bindingFlags, T value)
        {
            SetProperty(name, bindingFlags, null, value, null);
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <typeparam name="T">The return type of the property.</typeparam>
        /// <param name="name">The name of the property. Use <c>Item</c> for the indexer.</param>
        /// <param name="value">The value to set the property to.</param>
        /// <param name="args">The arguments for indexing the property.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="MissingMethodException">Property <paramref name="name"/> not found.</exception>
        /// <exception cref="TargetInvocationException">Invoking method resulted in an exception in that method.</exception>
        public void SetProperty<T>(string name, T value, params object[] args)
        {
            SetProperty(name, DefaultBindingFlags, null, value, args);
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <typeparam name="T">The return type of the property.</typeparam>
        /// <param name="name">The name of the property. Use <c>Item</c> for the indexer.</param>
        /// <param name="bindingFlags">
        /// A bitmask comprised of one or more <see cref="BindingFlags"/> that specifies how the search for the field or
        /// property is conducted. The type of lookup need not be specified.
        /// </param>
        /// <param name="value">The value to set the property to.</param>
        /// <param name="args">The arguments for indexing the property.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="MissingMethodException">Property <paramref name="name"/> not found.</exception>
        /// <exception cref="TargetInvocationException">Invoking method resulted in an exception in that method.</exception>
        public void SetProperty<T>(string name, BindingFlags bindingFlags, T value, params object[] args)
        {
            SetProperty(name, bindingFlags, null, value, args);
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <typeparam name="T">The return type of the property.</typeparam>
        /// <param name="name">The name of the property. Use <c>Item</c> for the indexer.</param>
        /// <param name="parameterTypes">The parameter types for the arguments.</param>
        /// <param name="value">The value to set the property to.</param>
        /// <param name="args">The arguments for indexing the property.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="MissingMethodException">Property <paramref name="name"/> not found.</exception>
        /// <exception cref="TargetInvocationException">Invoking method resulted in an exception in that method.</exception>
        public void SetProperty<T>(string name, Type[] parameterTypes, T value, params object[] args)
        {
            SetProperty(name, DefaultBindingFlags, parameterTypes, value, args);
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <typeparam name="T">The return type of the property.</typeparam>
        /// <param name="name">The name of the property. Use <c>Item</c> for the indexer.</param>
        /// <param name="bindingFlags">
        /// A bitmask comprised of one or more <see cref="BindingFlags"/> that specifies how the search for the field or
        /// property is conducted. The type of lookup need not be specified.
        /// </param>
        /// <param name="parameterTypes">The parameter types for the arguments.</param>
        /// <param name="value">The value to set the property to.</param>
        /// <param name="args">The arguments for indexing the property.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="MissingMethodException">Property <paramref name="name"/> not found.</exception>
        /// <exception cref="TargetInvocationException">Invoking method resulted in an exception in that method.</exception>
        public void SetProperty<T>(string name, BindingFlags bindingFlags, Type[] parameterTypes, T value, object[] args)
        {
            ThrowHelper.ThrowIfNull(name);
            if (parameterTypes == null) {
                object[] invokeArgs;
                if (args == null) {
                    invokeArgs = new object[1] { value };
                } else {
                    invokeArgs = new object[args.Length + 1];
                    args.CopyTo(invokeArgs, 0);
                    invokeArgs[args.Length] = value;
                }
                InvokeHelper(name, bindingFlags | BindingFlags.SetProperty, invokeArgs);
                return;
            }

            PropertyInfo propInfo = m_ObjectType.GetProperty(name, bindingFlags, null, typeof(T), parameterTypes, null);
            if (propInfo == null) {
                string msg = string.Format("Property {0} not found", name);
                throw new MissingMethodException(msg);
            }
            propInfo.SetValue(m_Instance, value, args);
        }
        #endregion
    }
}
