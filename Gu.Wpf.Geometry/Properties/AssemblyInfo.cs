﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Gu.Wpf.Geometry")]
[assembly: AssemblyDescription("Geometries for WPF.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Johan Larsson")]
[assembly: AssemblyProduct("Gu.Wpf.Geometry")]
[assembly: AssemblyCopyright("Copyright ©  2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("a9cbadf5-65f8-4a81-858c-ea0cdfeb2e99")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.1")]
[assembly: AssemblyFileVersion("1.0.0.1")]
[assembly: InternalsVisibleTo("Gu.Wpf.Geometry.Tests", AllInternalsVisible = true)]
[assembly: InternalsVisibleTo("Gu.Wpf.Geometry.Demo", AllInternalsVisible = true)]
[assembly: InternalsVisibleTo("Gu.Wpf.Geometry.Benchmarks", AllInternalsVisible = true)]

[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]
[assembly: XmlnsDefinition("http://gu.se/Geometry", "Gu.Wpf.Geometry")]
[assembly: XmlnsPrefix("http://gu.se/Geometry", "geometry")]
