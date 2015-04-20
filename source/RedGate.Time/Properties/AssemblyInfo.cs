using System.Reflection;
using System.Runtime.InteropServices;
using RedGate.Time;

[assembly: AssemblyTitle("RedGate.Time")]
[assembly: AssemblyDescription("BCL time facility abstractions")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("Red Gate Software Limited")]
[assembly: AssemblyProduct("RedGate.Time")]
[assembly: AssemblyCopyright("Copyright © Red Gate Software Limited 2015")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("7d986fe1-648e-412b-bcb8-09cd23e59940")]


[assembly: AssemblyVersion(VersionInfo.AssemblyVersion)]
[assembly: AssemblyFileVersion(VersionInfo.AssemblyFileVersion)]
