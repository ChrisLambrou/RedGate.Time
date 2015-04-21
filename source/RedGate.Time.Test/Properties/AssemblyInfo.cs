using System.Reflection;
using System.Runtime.InteropServices;
using RedGate.Time;

[assembly: AssemblyTitle("RedGate.Time.Test")]
[assembly: AssemblyDescription("Testing companion for RedGate.Time. Licensed under the MIT license.")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyCompany("Red Gate Software Limited")]
[assembly: AssemblyProduct("RedGate.Time.Test")]
[assembly: AssemblyCopyright("Copyright © Red Gate Software Limited 2015")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly: Guid("2538cd63-af18-4fab-96e9-883aeb1a46e7")]
[assembly: AssemblyVersion(VersionInfo.AssemblyVersion)]
[assembly: AssemblyFileVersion(VersionInfo.AssemblyFileVersion)]
