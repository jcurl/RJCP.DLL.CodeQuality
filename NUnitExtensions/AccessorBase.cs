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
        /// Initializes a new instance of the <see cref="AccessorBase"/> class from an existing object.
        /// </summary>
        /// <param name="pObject">The object wrapped in a <see cref="PrivateObject"/>.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="pObject"/> may not be <see langword="null"/>.</exception>
        protected AccessorBase(PrivateObject pObject)
        {
            if (pObject == null) throw new ArgumentNullException(nameof(pObject));
            m_PrivateObject = pObject;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessorBase" /> class from a <see cref="PrivateType" />.
        /// </summary>
        /// <param name="type">The <see cref="PrivateType" /> describing the object.</param>
        /// <param name="args">Arguments to pass to the constructor of the object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> may not be <see langword="null"/>.</exception>
        protected AccessorBase(PrivateType type, params object[] args)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            try {
                m_PrivateObject = new PrivateObject(type.ReferencedType, args);
            } catch (TargetInvocationException ex) {
                if (ex.InnerException == null) {
                    throw;
                } else {
                    throw ex.InnerException;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessorBase" /> class from a <see cref="PrivateType" />.
        /// </summary>
        /// <param name="type">The <see cref="PrivateType" /> describing the object.</param>
        /// <param name="parameterTypes">An array of <see cref="Type" /> objects representing the number,
        /// order, and type of the parameters for constructing the object.</param>
        /// <param name="args">Arguments to pass to the constructor of the object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> may not be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><para><paramref name="parameterTypes"/> is multidimensional</para>
        /// - or -
        /// <para>constructor cannot be found to match the parameters specified in <see cref="PrivateObject"/>.</para></exception>
        protected AccessorBase(PrivateType type, Type[] parameterTypes, object[] args)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            try {
                m_PrivateObject = new PrivateObject(type.ReferencedType, parameterTypes, args);
            } catch (TargetInvocationException ex) {
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
        /// <para>constructor cannot be found to match the parameters specified in <see cref="PrivateObject"/>.</para></exception>
        protected AccessorBase(string assemblyName, string typeName, Type[] parameterTypes, object[] args)
        {
            try {
                m_PrivateObject = new PrivateObject(assemblyName, typeName, parameterTypes, args);
            } catch (TargetInvocationException ex) {
                if (ex.InnerException == null) {
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
                m_PrivateObject = new PrivateObject(assemblyName, typeName, parameterTypes, args, typeArguments);
            } catch (TargetInvocationException ex) {
                if (ex.InnerException == null) {
                    throw;
                } else {
                    throw ex.InnerException;
                }
            }
        }

        /// <summary>
        /// Gets the underlying object maintained by the accessor base.
        /// </summary>
        /// <value>The target object wrapped by <see cref="AccessorBase"/>.</value>
        public object PrivateTargetObject { get { return m_PrivateObject.Target; } }

        /// <summary>
        /// Gets a value of a wrapped field or property identified by name.
        /// </summary>
        /// <param name="propertyName">Name of the field or property.</param>
        /// <returns>The value of the field or property.</returns>
        /// <exception cref="ArgumentNullException">The given <paramref name="propertyName"/> is <see langword="null"/>.</exception>
        /// <exception cref="MissingMethodException">The given <paramref name="propertyName"/> doesn't exist.</exception>
        protected object GetFieldOrProperty(string propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
            try {
                return m_PrivateObject.GetFieldOrProperty(propertyName, BindingFlags);
            } catch (TargetInvocationException ex) {
                if (ex.InnerException == null) {
                    throw;
                } else {
                    throw ex.InnerException;
                }
            }
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
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
            if (value == null) throw new ArgumentNullException(nameof(value));

            try {
                m_PrivateObject.SetFieldOrProperty(propertyName, BindingFlags, value);
            } catch (TargetInvocationException ex) {
                if (ex.InnerException == null) {
                    throw;
                } else {
                    throw ex.InnerException;
                }
            }
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
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));

            try {
                return m_PrivateObject.Invoke(methodName, BindingFlags, args);
            } catch (TargetInvocationException ex) {
                if (ex.InnerException == null) {
                    throw;
                } else {
                    throw ex.InnerException;
                }
            }
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
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));

            try {
                return m_PrivateObject.Invoke(methodName, BindingFlags, parameterTypes, args);
            } catch (TargetInvocationException ex) {
                if (ex.InnerException == null) {
                    throw;
                } else {
                    throw ex.InnerException;
                }
            }
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
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));

            try {
                return m_PrivateObject.Invoke(methodName, BindingFlags, parameterTypes, args, typeArguments);
            } catch (TargetInvocationException ex) {
                if (ex.InnerException == null) {
                    throw;
                } else {
                    throw ex.InnerException;
                }
            }
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
        protected void AddEventHandler(string eventName, Delegate handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            if (eventName == null) throw new ArgumentNullException(nameof(eventName));

            Type objectType = m_PrivateObject.Target.GetType();
            EventInfo eventInfo = objectType.GetEvent(eventName);
            if (eventInfo == null) throw new MissingMemberException(objectType.ToString(), eventName);

            Delegate delegateEventHandler = Delegate.CreateDelegate(eventInfo.EventHandlerType, handler.Target, handler.Method);
            try {
                eventInfo.AddEventHandler(m_PrivateObject.Target, delegateEventHandler);
            } catch (TargetInvocationException ex) {
                if (ex.InnerException == null) {
                    throw;
                } else {
                    throw ex.InnerException;
                }
            }
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
        protected void RemoveEventHandler(string eventName, Delegate handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            if (eventName == null) throw new ArgumentNullException(nameof(handler));

            Type objectType = m_PrivateObject.Target.GetType();
            EventInfo eventInfo = objectType.GetEvent(eventName);
            if (eventInfo == null) throw new MissingMemberException(objectType.ToString(), eventName);

            Delegate delegateEventHandler = Delegate.CreateDelegate(eventInfo.EventHandlerType, handler.Target, handler.Method);
            try {
                eventInfo.RemoveEventHandler(m_PrivateObject.Target, delegateEventHandler);
            } catch (TargetInvocationException ex) {
                if (ex.InnerException == null) {
                    throw;
                } else {
                    throw ex.InnerException;
                }
            }
        }

        /// <summary>
        /// A delegate that can be used as a template for trampolines normal <see cref="EventHandler"/>s.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs"/> arguments.</param>
        public delegate void AccessorEventHandler(object sender, object args);

        private DelegateTargets m_EventTargets = new DelegateTargets();

        /// <summary>
        /// Adds the event handler to the event specified by name.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="userSource">The user provided handler (used for look ups only).</param>
        /// <param name="handler">The handler to be registered to the event.</param>
        /// <exception cref="ArgumentNullException"><paramref name="eventName"/>, <paramref name="handler"/>, <paramref name="userSource"/>
        /// may not be <see langword="null"/>.</exception>
        /// <exception cref="MissingMemberException">The given <paramref name="eventName"/> doesn't exist.</exception>
        /// <exception cref="ArgumentException">The delegate <paramref name="handler"/> may not have a non-void return type.</exception>
        /// <remarks>
        /// This variant allows to register a <paramref name="handler"/> and remember it via a
        /// reference to the <paramref name="userSource"/>. This is critical when removing the handler later.
        /// This method is extremely useful for situations where the handler being registered is dynamically
        /// created.
        /// <para>Adding an event handler does not use the bit mask of <see cref="System.Reflection.BindingFlags"/>.</para>
        /// </remarks>
        protected void AddIndirectEventHandler(string eventName, Delegate userSource, Delegate handler)
        {
            if (userSource == null) throw new ArgumentNullException(nameof(userSource));
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            if (handler.Method.ReturnType != typeof(void)) throw new ArgumentException("handler has non-void return type", nameof(handler));
            AddEventHandler(eventName, handler);

            // Remember the handler, so we can remove it later
            m_EventTargets.AddTarget(eventName, userSource, handler);
        }

        /// <summary>
        /// Removes the indirect event handler from the event specified by name.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="userSource">The user provided handler.</param>
        /// <exception cref="ArgumentNullException"><paramref name="eventName"/> or <paramref name="userSource"/> may not be <see langword="null"/>.</exception>
        /// <exception cref="MissingMemberException">The given <paramref name="eventName"/> doesn't exist.</exception>
        /// <remarks>
        /// This variant is to be used in conjunction with <see cref="AddIndirectEventHandler(string, Delegate, Delegate)"/>.
        /// Provide the handler <paramref name="userSource"/> from the user, and the handler that was actually registered
        /// is removed.
        /// <para>Removing an event handler does not use the bit mask of <see cref="System.Reflection.BindingFlags"/>.</para>
        /// </remarks>
        protected void RemoveIndirectEventHandler(string eventName, Delegate userSource)
        {
            if (userSource == null) throw new ArgumentNullException(nameof(userSource));
            if (eventName == null) throw new ArgumentNullException(nameof(eventName));

            // Look up the dynamic handler we registered internally and unregister it
            Delegate handler = m_EventTargets.RemoveTarget(eventName, userSource);
            RemoveEventHandler(eventName, handler);
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
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));
            try {
                return type.InvokeStatic(methodName, args);
            } catch (TargetInvocationException ex) {
                if (ex.InnerException == null) {
                    throw;
                } else {
                    throw ex.InnerException;
                }
            }
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
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));
            try {
                return type.InvokeStatic(methodName, parameterTypes, args);
            } catch (TargetInvocationException ex) {
                if (ex.InnerException == null) {
                    throw;
                } else {
                    throw ex.InnerException;
                }
            }
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
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));
            try {
                return type.InvokeStatic(methodName, parameterTypes, args, typeArguments);
            } catch (TargetInvocationException ex) {
                if (ex.InnerException == null) {
                    throw;
                } else {
                    throw ex.InnerException;
                }
            }
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
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (name == null) throw new ArgumentNullException(nameof(name));
            try {
                return type.GetStaticFieldOrProperty(name);
            } catch (TargetInvocationException ex) {
                if (ex.InnerException == null) {
                    throw;
                } else {
                    throw ex.InnerException;
                }
            }
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
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (name == null) throw new ArgumentNullException(nameof(name));
            try {
                type.SetStaticFieldOrProperty(name, value);
            } catch (TargetInvocationException ex) {
                if (ex.InnerException == null) {
                    throw;
                } else {
                    throw ex.InnerException;
                }
            }
        }
    }
}
