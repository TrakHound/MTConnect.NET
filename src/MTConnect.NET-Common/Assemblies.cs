// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Reflection;

#if NET5_0_OR_GREATER
    using System.Runtime.Loader;
#endif

namespace MTConnect
{
    internal class Assemblies
    {
        private static Assembly[] _assemblies;


        /// <summary>
        /// Load DLL Assembly files located in AppDomain BaseDirectory (used to load "plugin modules")
        /// </summary>
        public static IEnumerable<Assembly> Get()
        {
            if (_assemblies == null)
            {
                try
                {
                    var assemblies = new List<Assembly>();
                    var currentAssembly = Assembly.GetExecutingAssembly();

#if NET5_0_OR_GREATER
                    var currentAssemblyLoadContext = AssemblyLoadContext.GetLoadContext(currentAssembly);
#endif

                    var currentAssemblyPath = currentAssembly.Location;
                    var assemblyDir = Path.GetDirectoryName(currentAssemblyPath);

                    // Load Assemblies located in Base Directoy
                    var dllFiles = Directory.GetFiles(assemblyDir, "*.dll");
                    if (!dllFiles.IsNullOrEmpty())
                    {
                        foreach (var dllFile in dllFiles)
                        {
                            try
                            {
#if NET5_0_OR_GREATER
                                var assembly = currentAssemblyLoadContext.LoadFromAssemblyPath(dllFile);
#else
                                var assembly = Assembly.LoadFrom(dllFile);
#endif

                                if (assembly != null)
                                {
                                    assemblies.Add(assembly);
                                }
                            }
                            catch { }
                        }
                    }

                    _assemblies = assemblies.ToArray();
                }
                catch { }
            }

            return _assemblies;
        }
    }
}