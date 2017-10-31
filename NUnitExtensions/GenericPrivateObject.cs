namespace NUnit.Framework
{
    using System;

    /// <summary>
    /// An object to create a <see cref="PrivateObject"/> with generic types.
    /// </summary>
    /// <remarks>
    /// The Microsoft version of <see cref="PrivateObject"/> doesn't let one create a type with generics.
    /// This class allows one to instantiate a generic type and wrap it around a <see cref="PrivateObject"/> .
    /// </remarks>
    /// <seealso cref="NUnit.Framework.PrivateObject" />
    public class GenericPrivateObject : PrivateObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericPrivateObject"/> class.
        /// This creates the object of the specified type and wraps it in a generic private object.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly that contains the type.</param>
        /// <param name="typeName">Fully qualified name of the type.</param>
        /// <param name="genericTypes">The generic types for the arguments used in creating the object.</param>
        /// <param name="args">Arguments to pass to the constructor of the object.</param>
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
        /// <paramref name="assemblyName"/> and <paramref name="typeName"/> is not found (<see cref="System.Reflection.Assembly.GetType(string)"/>).
        /// </para>
        /// </exception>
        /// <exception cref="InvalidOperationException"><paramref name="typeName"/> it's not a generic type.</exception>
        /// <exception cref="ArgumentException">
        /// <para>
        /// One of the <paramref name="genericTypes"/> violates the constraints of <paramref name="typeName"/>.
        /// </para>
        /// <para>-or-</para>
        /// <para>
        /// <paramref name="typeName"/> was not found.
        /// </para>
        /// </exception>
        /// <exception cref="System.Reflection.TargetInvocationException">The constructor being called throws an exception.</exception>
        /// <exception cref="TypeLoadException"><paramref name="typeName"/> is not a valid type.</exception>
        /// <exception cref="MissingMethodException">No matching public constructor was found to match the
        /// <paramref name="typeName"/> and <paramref name="args"/>.</exception>
        /// <remarks>
        /// An object is created by using the <paramref name="typeName"/> and <paramref name="args"/> of types <paramref name="genericTypes"/>.
        /// </remarks>
        public GenericPrivateObject(string assemblyName, string typeName, Type[] genericTypes, params object[] args)
            : base(GetGenericObjectType(assemblyName, typeName, genericTypes), args) { }

        private static Type GetGenericObjectType(string assemblyName, string typeName, Type[] genericTypes)
        {
            Type type = GetObjectType(assemblyName, typeName);
            if (type == null) throw new ArgumentNullException("typeName");

            return type.MakeGenericType(genericTypes);
        }
    }
}
