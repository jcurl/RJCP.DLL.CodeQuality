﻿// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE0056:Use index operator", Justification = ".NET Core only feature")]
[assembly: SuppressMessage("Style", "IDE0057:Use range operator", Justification = ".NET Core only feature")]
[assembly: SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Test Methods used for reflection")]
[assembly: SuppressMessage("Style", "IDE0230:Use UTF-8 string literal", Justification = "Testing byte streams, not UTF8 strings")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Test Methods used for reflection")]
[assembly: SuppressMessage("Performance", "CA1825:Avoid zero-length array allocations", Justification = ".NET Core only feature")]
[assembly: SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments", Justification = "Test Case Only")]
