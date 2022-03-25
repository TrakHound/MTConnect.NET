// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
                    // Load Assemblies located in Base Directoy
                    var assemblyDir = AppDomain.CurrentDomain.BaseDirectory;
                    var dllFiles = Directory.GetFiles(assemblyDir, "*.dll");
                    if (!dllFiles.IsNullOrEmpty())
                    {
                        foreach (var dllFile in dllFiles)
                        {
                            try
                            {
                                // Load Assembly form DLL file
                                Assembly.LoadFrom(dllFile);
                            }
                            catch { }
                        }
                    }

                    _assemblies = AppDomain.CurrentDomain.GetAssemblies();
                }
                catch { }
            }            

            return _assemblies;
        }
    }
}
