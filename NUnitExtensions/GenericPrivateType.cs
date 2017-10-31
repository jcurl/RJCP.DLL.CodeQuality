namespace NUnit.Framework
{
    using System;

    /// <summary>
    /// Represents the type of a private class that gives access to private generic static implementations.
    /// </summary>
    /// <seealso cref="NUnit.Framework.PrivateType" />
    public class GenericPrivateType : PrivateType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericPrivateType"/> class.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="typeArguments">The generic argument types.</param>
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
        /// This constructor is an extension of <see cref="PrivateType.PrivateType(string, string)"/> that gets a specific
        /// generic type. It can be used where ever a <see cref="PrivateType"/> can be used.
        /// </remarks>
        public GenericPrivateType(string assemblyName, string typeName, params Type[] typeArguments)
            : base(GetGenericObjectType(assemblyName, typeName, typeArguments)) { }

        private static Type GetGenericObjectType(string assemblyName, string typeName, Type[] typeArguments)
        {
            Type type = GetObjectType(assemblyName, typeName);
            if (type == null) throw new ArgumentNullException("typeName");

            return type.MakeGenericType(typeArguments);
        }
    }
}
