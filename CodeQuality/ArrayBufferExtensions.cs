namespace RJCP.CodeQuality
{
#if NETFRAMEWORK
    using System.Linq;
#endif

    /// <summary>
    /// Provides extensions for testing sections of arrays, common in testing .NET Framework.
    /// </summary>
    public static class ArrayBufferExtensions
    {
        /// <summary>
        /// Slices the array at the specified offset for the length given.
        /// </summary>
        /// <typeparam name="T">The array type</typeparam>
        /// <param name="array">The array to slice.</param>
        /// <param name="offset">The offset into the array to slice.</param>
        /// <param name="length">The length of the array section.</param>
        /// <returns>A new array</returns>
        /// <remarks>This should only be used for testing. On .NET Core, should use array ranges and Spans.</remarks>
        public static T[] Slice<T>(this T[] array, int offset, int length)
        {
#if NET6_0_OR_GREATER
            return array[offset..(offset + length)];
#else
            return array.Skip(offset).Take(length).ToArray();
#endif
        }
    }
}
