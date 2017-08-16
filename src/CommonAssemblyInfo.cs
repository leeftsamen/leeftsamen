// <copyright file="CommonAssemblyInfo.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System.Reflection;

[assembly: AssemblyProduct("LeeftSamen")]
[assembly: AssemblyCompany("LeeftSamen B.V.")]
[assembly: AssemblyCopyright("Copyright © 2015-2016 LeeftSamen B.V.")]
[assembly: AssemblyTrademark("")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

// Will be overridden by Build Server
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0")]
