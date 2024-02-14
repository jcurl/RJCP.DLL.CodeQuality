﻿namespace RJCP.CodeQuality.Config
{
    using System;

    /// <summary>
    /// Represents a section in an INI file, containing key/value pairs.
    /// </summary>
    public class IniSection : IniKeyPair<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IniSection"/> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <exception cref="ArgumentNullException"><paramref name="header"/> is <see langword="null"/>.</exception>
        public IniSection(string header)
        {
            ThrowHelper.ThrowIfNull(header);
            Header = header;
        }

        /// <summary>
        /// Gets the name of the section.
        /// </summary>
        /// <value>The name of the section.</value>
        public string Header { get; private set; }
    }
}
