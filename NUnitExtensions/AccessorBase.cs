namespace NUnit.Framework
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Base class for private accessors that provide access to public and non-public members of a class.
    /// </summary>
    /// <remarks>
    /// Use .NET reflection to access non-public types from an assembly, typically for the purpose of
    /// testing that non-public type. The control binding that specifies how the search for
    /// properties is conducted can be set in the derived class by modifying the <see cref="BindingFlags"/> field.
    /// <include file="maml/AccessorBase.xml" path="Comments/AccessorBase/Remarks[@id='AccessorBase']/*"/>
    /// </remarks>
    public abstract class AccessorBase
    {
        /// <summary>
        /// The private object used for accessing the members of the class.
        /// </summary>
        private readonly PrivateObject m_PrivateObject;

        private BindingFlags m_BindingFlags = BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        /// A bit mask comprised of one or more <see cref="System.Reflection.BindingFlags"/> that specifies how the search for
        /// the properties or methods is conducted.
        /// </summary>
        /// <remarks>
        /// Your test case should generally set this only in the constructor. You should avoid changing this value during
        /// your tests to provide a consistent environment and maintainability for your test cases. Alternatively, your own
        /// methods should restore the original value to ensure expected behavior.
        /// <para>By default, only public methods of your class are available. To also exercise non-public methods
        /// (those that are internal to another assembly, or protected or private), add the flag <see cref="BindingFlags.NonPublic"/>
        /// to this bit mask.</para>
        /// <para>Please note, that this property naturally can only apply to non-static methods in this class.</para>
        /// </remarks>
        protected BindingFlags BindingFlags
        {
            get { return m_BindingFlags; }
            set { m_BindingFlags = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessorBase"/> class used for providing access
        /// to members of a non-generic class.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly that contains the type.</param>
        /// <param name="typeName">Fully qualified name of the type.</param>
        /// <param name="args">Arguments to pass to the constructor of the object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblyName"/> or <paramref name="typeName"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        /// An instance of <see cref="PrivateObject"/> is created by using the <paramref name="assemblyName"/>, <paramref name="typeName"/>
        /// and <paramref name="args"/>.
        /// </remarks>
        protected AccessorBase(string assemblyName, string typeName, params object[] args)
        {
            try {
                m_PrivateObject = new PrivateObject(assemblyName, typeName, args);
            } catch (TargetInvocationException ex) {
                // As parameterTypes == null, PrivateObject raises TargetInvocationException through
                // the Activator.CreateInstance
                if (ex.InnerException == null) {
                    throw;
                } else {
                    throw ex.InnerException;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessorBase"/> class used for providing access
        /// to members of a non-generic class.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly that contains the type.</param>
        /// <param name="typeName">Fully qualified name of the type.</param>
        /// <param name="parameterTypes">An array of <see cref="Type"/> objects representing the number,
        /// order, and type of the parameters for constructing the object.</param>
        /// <param name="args">Arguments to pass to the constructor of the object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblyName"/> or <paramref name="typeName"/> is <value>null</value>.
        /// </exception>
        /// <exception cref="ArgumentException"><para><paramref name="parameterTypes"/> is multidimensional</para>
        /// - or -
        /// <para> constructor cannot be found to match the parameters specified in PrivateObject.</para></exception>
        protected AccessorBase(string assemblyName, string typeName, Type[] parameterTypes, object[] args)
        {
            try {
                m_PrivateObject = new PrivateObject(assemblyName, typeName, parameterTypes, args);
            } catch (TargetInvocationException ex) {
                // If parameterTypes == null, PrivateObject raises TargetInvocationException through
                // the Activator.CreateInstance
                if (parameterTypes != null || ex.InnerException == null) {
                    throw;
                } else {
                    throw ex.InnerException;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessorBase"/> class used for providing access to
        /// members of a generic class.
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
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="typeName"/> it's not a generic type.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para>
        /// One of the <paramref name="typeArguments"/> violates the constraints of <paramref name="typeName"/>.
        /// </para>
        /// <para>-or-</para>
        /// <para>
        /// <paramref name="typeName"/> was not found.
        /// </para>
        /// </exception>
        /// <remarks>
        /// An instance of <see cref="PrivateObject"/> is created by using the <paramref name="assemblyName"/>, <paramref name="typeName"/>
        /// and <paramref name="args"/> of types <paramref name="typeArguments"/>.
        /// </remarks>
        protected AccessorBase(string assemblyName, string typeName, Type[] parameterTypes, object[] args, Type[] typeArguments)
        {
            try {
                m_PrivateObject = new GenericPrivateObject(assemblyName, typeName, parameterTypes, args, typeArguments);
            } catch (TargetInvocationException ex) {
                // If parameterTypes == null, PrivateObject raises TargetInvocationException through
                // the Activator.CreateInstance
                if (parameterTypes != null || ex.InnerException == null) {
                    throw;
                } else {
                    throw ex.InnerException;
                }
            }
        }

        /// <summary>
        /// Gets a value of a wrapped field or property identified by name.
        /// </summary>
        /// <param name="propertyName">Name of the field or property.</param>
        /// <returns>The value of the field or property.</returns>
        /// <exception cref="ArgumentNullException">The given <paramref name="propertyName"/> is <see langword="null"/>.</exception>
        /// <exception cref="MissingMethodException">The given <paramref name="propertyName"/> doesn't exist.</exception>
        protected object GetFieldOrProperty(string propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            return m_PrivateObject.GetFieldOrProperty(propertyName, BindingFlags);
        }

        /// <summary>
        /// Sets a value for the field or property of the wrapped object, identified by name.
        /// </summary>
        /// <param name="propertyName">Name of the field or property.</param>
        /// <param name="value">The value to be set.</param>
        /// <exception cref="ArgumentNullException">The given <paramref name="propertyName"/> is <see langword="null"/>.</exception>
        /// <exception cref="MissingMethodException">The given <paramref name="propertyName"/> doesn't exist.</exception>
        protected void SetFieldOrProperty(string propertyName, object value)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");
            if (value == null) throw new ArgumentNullException("value");

            m_PrivateObject.SetFieldOrProperty(propertyName, BindingFlags, value);
        }

        /// <summary>
        /// Invokes the method on the <see cref="PrivateObject"/>.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="args">The parameters required by the method.</param>
        /// <returns>An object that represents the return value of a private member.</returns>
        /// <exception cref="ArgumentNullException">The given <paramref name="methodName"/> is <see langword="null"/>.</exception>
        /// <exception cref="MissingMethodException">The given <paramref name="methodName"/> doesn't exist.</exception>
        protected object Invoke(string methodName, params object[] args)
        {
            if (methodName == null) throw new ArgumentNullException("methodName");

            return m_PrivateObject.Invoke(methodName, BindingFlags, args);
        }

        /// <summary>
        /// Invokes the method on the <see cref="PrivateObject"/>.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameterTypes">An array of <see cref="Type"/> objects that represent the number, order and type of the parameters for the method to access.</param>
        /// <param name="args">The parameters required by the method.</param>
        /// <returns>An object that represents the return value of a private member.</returns>
        /// <exception cref="ArgumentNullException">The given <paramref name="methodName"/> is <see langword="null"/>.</exception>
        /// <exception cref="MissingMethodException">The given <paramref name="methodName"/> doesn't exist.</exception>
        protected object Invoke(string methodName, Type[] parameterTypes, object[] args)
        {
            if (methodName == null) throw new ArgumentNullException("methodName");

            return m_PrivateObject.Invoke(methodName, BindingFlags, parameterTypes, args);
        }

        /// <summary>
        /// Invokes the method on the <see cref="PrivateObject"/>.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameterTypes">An array of <see cref="Type"/> objects that represent the number, order and type of the parameters for the method to access.</param>
        /// <param name="args">The parameters required by the method.</param>
        /// <param name="typeArguments">The type arguments.</param>
        /// <returns>An object that represents the return value of a private member.</returns>
        /// <exception cref="ArgumentNullException">The given <paramref name="methodName"/> is <see langword="null"/>.</exception>
        /// <exception cref="MissingMethodException">The given <paramref name="methodName"/> doesn't exist.</exception>
        protected object Invoke(string methodName, Type[] parameterTypes, object[] args, Type[] typeArguments)
        {
            if (methodName == null) throw new ArgumentNullException("methodName");

            return m_PrivateObject.Invoke(methodName, BindingFlags, parameterTypes, args, typeArguments);
        }

        /// <summary>
        /// Adds the event handler to the event specified by name.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="handler">The handler.</param>
        /// <exception cref="ArgumentNullException"><paramref name="eventName"/> or <paramref name="handler"/> may not be <see langword="null"/>.</exception>
        /// <exception cref="MissingMemberException">The given <paramref name="eventName"/> doesn't exist.</exception>
        /// <remarks>
        /// Adding an event handler does not use the bit mask of <see cref="System.Reflection.BindingFlags"/>.
        /// </remarks>
        protected void AddEventHandler(string eventName, EventHandler<EventArgs> handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");
            if (eventName == null) throw new ArgumentNullException("eventName");

            Type objectType = m_PrivateObject.Target.GetType();
            EventInfo eventInfo = objectType.GetEvent(eventName);
            if (eventInfo == null) throw new MissingMemberException(objectType.ToString(), eventName);

            Delegate delegateEventHandler = Delegate.CreateDelegate(eventInfo.EventHandlerType, handler.Target, handler.Method);
            eventInfo.AddEventHandler(m_PrivateObject.Target, delegateEventHandler);
        }

        /// <summary>
        /// Removes the event handler from the event specified by name.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="handler">The handler.</param>
        /// <exception cref="ArgumentNullException"><paramref name="eventName"/> or <paramref name="handler"/> may not be <see langword="null"/>.</exception>
        /// <exception cref="MissingMemberException">The given <paramref name="eventName"/> doesn't exist.</exception>
        /// <remarks>
        /// Removing an event handler does not use the bit mask of <see cref="System.Reflection.BindingFlags"/>.
        /// </remarks>
        protected void RemoveEventHandler(string eventName, EventHandler<EventArgs> handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");
            if (eventName == null) throw new ArgumentNullException("eventName");

            Type objectType = m_PrivateObject.Target.GetType();
            EventInfo eventInfo = objectType.GetEvent(eventName);
            if (eventInfo == null) throw new MissingMemberException(objectType.ToString(), eventName);

            Delegate delegateEventHandler = Delegate.CreateDelegate(eventInfo.EventHandlerType, handler.Target, handler.Method);
            eventInfo.RemoveEventHandler(m_PrivateObject.Target, delegateEventHandler);
        }

        /// <summary>
        /// Invokes the static method of the class given by the <see cref="PrivateType"/>.
        /// </summary>
        /// <param name="type">The type to execute the static method for.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="args">An array of arguments to pass.</param>
        /// <returns>An object that represents the invoked static method's return value, if any.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> or <paramref name="methodName"/> may not be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Private accessor <paramref name="methodName"/> can't be found.</exception>
        public static object InvokeStatic(PrivateType type, string methodName, params object[] args)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (methodName == null) throw new ArgumentNullException("methodName");
            return type.InvokeStatic(methodName, args);
        }

        /// <summary>
        /// Invokes the static method of the class given by the <see cref="PrivateType"/>.
        /// </summary>
        /// <param name="type">The type to execute the static method for.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameterTypes"><para>An array of Type objects that represents the number,
        /// order, and type of the parameters for the method.</para>
        /// - or -
        /// <para>An empty array of the type Type, that is, Type[] types = new Type[0] to get a
        /// method that takes no parameters.</para></param>
        /// <param name="args">An array of arguments to pass.</param>
        /// <returns>An object that represents the invoked static method's return value, if any.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> or <paramref name="methodName"/> may not be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Private accessor <paramref name="methodName"/> can't be found.</exception>
        public static object InvokeStatic(PrivateType type, string methodName, Type[] parameterTypes, object[] args)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (methodName == null) throw new ArgumentNullException("methodName");
            return type.InvokeStatic(methodName, parameterTypes, args);
        }

        /// <summary>
        /// Invokes the static method of the class given by the <see cref="PrivateType" />.
        /// </summary>
        /// <param name="type">The type to execute the static method for.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameterTypes"><para>An array of Type objects that represents the number,
        /// order, and type of the parameters for the method.</para>
        /// - or -
        /// <para>An empty array of the type Type, that is, Type[] types = new Type[0] to get a
        /// method that takes no parameters.</para></param>
        /// <param name="args">An array of arguments to pass.</param>
        /// <param name="typeArguments">An array of type arguments to use when invoking a generic method.</param>
        /// <returns>An object that represents the invoked static method's return value, if any.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type" /> or <paramref name="methodName" /> may not be <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">Private accessor <paramref name="methodName"/> can't be found.</exception>
        public static object InvokeStatic(PrivateType type, string methodName, Type[] parameterTypes, object[] args, Type[] typeArguments)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (methodName == null) throw new ArgumentNullException("methodName");
            return type.InvokeStatic(methodName, parameterTypes, args, typeArguments);
        }

        /// <summary>
        /// Gets a value of a static field or property in a wrapped type based on the name.
        /// </summary>
        /// <param name="type">The type to execute the static method for.</param>
        /// <param name="name">The name of the static field or property to get.</param>
        /// <returns>The value set for the <paramref name="name"/> field or property.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type" /> or <paramref name="name" /> may not be <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">Private accessor <paramref name="name"/> can't be found.</exception>
        public static object GetStaticFieldOrProperty(PrivateType type, string name)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (name == null) throw new ArgumentNullException("name");
            return type.GetStaticFieldOrProperty(name);
        }

        /// <summary>
        /// Sets a static field or property contained in the wrapped type.
        /// </summary>
        /// <param name="type">The type to execute the static method for.</param>
        /// <param name="name">The name of the static field or property to get.</param>
        /// <param name="value">The value to set to the field or property.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type" /> or <paramref name="name" /> may not be <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">Private accessor <paramref name="name"/> can't be found.</exception>
        public static void SetStaticFieldOrProperty(PrivateType type, string name, object value)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (name == null) throw new ArgumentNullException("name");
            type.SetStaticFieldOrProperty(name, value);
        }
    }
}
