namespace RJCP.CodeQuality
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Resources;

    /// <summary>
    /// Class for managing test cases based on resources.
    /// </summary>
    public static class Resources
    {
        /// <summary>
        /// Prints all resources for the given resource base name.
        /// </summary>
        /// <param name="baseName">The resource base name.</param>
        /// <param name="assembly">A type existing in the assembly where the resource can be found.</param>
        /// <remarks>
        /// To get the base name of the resource, look at the applications <c><i>Resource</i>.Designer.cs</c> file, where
        /// there's often code such as:
        /// <code language="csharp"><![CDATA[
        /// internal static global::System.Resources.ResourceManager ResourceManager {
        ///   get {
        ///     if (object.ReferenceEquals(resourceMan, null)) {
        ///       global::System.Resources.ResourceManager
        ///         temp = new global::System.Resources.ResourceManager(
        ///           "RJCP.App.DltDump.Resources.ApplicationResources",
        ///           typeof(ApplicationResources).Assembly);
        ///       resourceMan = temp;
        ///     }
        ///     return resourceMan;
        ///   }
        /// }
        /// ]]></code>
        /// where the example above the base name is <c>RJCP.App.DltDump.Resources.ApplicationResources</c>.
        /// <para>The resources are printed using the current UI culture.</para>
        /// </remarks>
        public static void Print(string baseName, Type assembly)
        {
            Print(baseName, assembly.Assembly, CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// Prints all resources for the given resource base name.
        /// </summary>
        /// <param name="baseName">The resource base name.</param>
        /// <param name="assembly">A type existing in the assembly where the resource can be found.</param>
        /// <param name="culture">The culture to print the resources for.</param>
        /// <remarks>
        /// To get the base name of the resource, look at the applications <c><i>Resource</i>.Designer.cs</c> file, where
        /// there's often code such as:
        /// <code language="csharp"><![CDATA[
        /// internal static global::System.Resources.ResourceManager ResourceManager {
        ///   get {
        ///     if (object.ReferenceEquals(resourceMan, null)) {
        ///       global::System.Resources.ResourceManager
        ///         temp = new global::System.Resources.ResourceManager(
        ///           "RJCP.App.DltDump.Resources.ApplicationResources",
        ///           typeof(ApplicationResources).Assembly);
        ///       resourceMan = temp;
        ///     }
        ///     return resourceMan;
        ///   }
        /// }
        /// ]]></code>
        /// where the example above the base name is <c>RJCP.App.DltDump.Resources.ApplicationResources</c>.
        /// <para>The resources are printed using the specified culture.</para>
        /// </remarks>
        public static void Print(string baseName, Type assembly, CultureInfo culture)
        {
            Print(baseName, assembly.Assembly, culture);
        }

        /// <summary>
        /// Prints all resources for the given resource base name.
        /// </summary>
        /// <param name="baseName">The resource base name.</param>
        /// <param name="assembly">The assembly where the resource can be found.</param>
        /// <remarks>
        /// To get the base name of the resource, look at the applications <c><i>Resource</i>.Designer.cs</c> file, where
        /// there's often code such as:
        /// <code language="csharp"><![CDATA[
        /// internal static global::System.Resources.ResourceManager ResourceManager {
        ///   get {
        ///     if (object.ReferenceEquals(resourceMan, null)) {
        ///       global::System.Resources.ResourceManager
        ///         temp = new global::System.Resources.ResourceManager(
        ///           "RJCP.App.DltDump.Resources.ApplicationResources",
        ///           typeof(ApplicationResources).Assembly);
        ///       resourceMan = temp;
        ///     }
        ///     return resourceMan;
        ///   }
        /// }
        /// ]]></code>
        /// where the example above the base name is <c>RJCP.App.DltDump.Resources.ApplicationResources</c>.
        /// <para>The resources are printed using the current UI culture.</para>
        /// </remarks>
        public static void Print(string baseName, Assembly assembly)
        {
            Print(baseName, assembly, CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// Prints all resources for the given resource base name.
        /// </summary>
        /// <param name="baseName">The resource base name.</param>
        /// <param name="assembly">The assembly where the resource can be found.</param>
        /// <param name="culture">The culture to print the resources for.</param>
        /// <remarks>
        /// To get the base name of the resource, look at the applications <c><i>Resource</i>.Designer.cs</c> file, where
        /// there's often code such as:
        /// <code language="csharp"><![CDATA[
        /// internal static global::System.Resources.ResourceManager ResourceManager {
        ///   get {
        ///     if (object.ReferenceEquals(resourceMan, null)) {
        ///       global::System.Resources.ResourceManager
        ///         temp = new global::System.Resources.ResourceManager(
        ///           "RJCP.App.DltDump.Resources.ApplicationResources",
        ///           typeof(ApplicationResources).Assembly);
        ///       resourceMan = temp;
        ///     }
        ///     return resourceMan;
        ///   }
        /// }
        /// ]]></code>
        /// where the example above the base name is <c>RJCP.App.DltDump.Resources.ApplicationResources</c>.
        /// <para>The resources are printed using the specified culture.</para>
        /// </remarks>
        public static void Print(string baseName, Assembly assembly, CultureInfo culture)
        {
            ResourceManager rsrc = new ResourceManager(baseName, assembly);
            ResourceSet set = rsrc.GetResourceSet(culture, true, true);
            SortedDictionary<string, string> sorted = new SortedDictionary<string, string>();
            foreach (DictionaryEntry entry in set) {
                string key = entry.Key.ToString();

                if (entry.Value is string resource)
                    sorted.Add(key, resource);
            }

            Console.WriteLine("---------1---------2---------3---------4---------5---------6---------7---------8");
            Console.WriteLine("12345678901234567890123456789012345678901234567890123456789012345678901234567890");
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine("Resource: {0}; Assembly: {1}; Culture: {2}", baseName, assembly.ToString(), culture.ToString());
            foreach (KeyValuePair<string, string> entry in sorted) {
                Console.WriteLine("Key: {0}", entry.Key);
                Console.WriteLine("{0}", entry.Value);
            }
        }
    }
}
