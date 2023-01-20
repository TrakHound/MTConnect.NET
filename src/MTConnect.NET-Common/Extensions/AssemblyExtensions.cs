// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MTConnect.Extensions
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Get the types within the assembly that match the predicate.
        /// <para>for example, to get all types within a namespace</para>
        /// <para>    typeof(SomeClassInAssemblyYouWant).Assembly.GetMatchingTypesInAssembly(item => "MyNamespace".Equals(item.Namespace))</para>
        /// </summary>
        /// <param name="assembly">The assembly to search</param>
        /// <param name="predicate">The predicate query to match against</param>
        /// <returns>The collection of types within the assembly that match the predicate</returns>
        /// <remarks>
        /// Code taken from https://stackoverflow.com/questions/7889228/how-to-prevent-reflectiontypeloadexception-when-calling-assembly-gettypes
        /// </remarks>
        public static IReadOnlyCollection<Type> GetMatchingTypesInAssembly(
            this Assembly assembly, 
            Predicate<Type> predicate)
        {
            var types = new List<Type>();
            try
            {
                var assemblyTypes = assembly.GetTypes();
                types = assemblyTypes.Where(i => predicate(i)).ToList();
                //types = assembly.GetTypes().Where(i => predicate(i) && i.Assembly == assembly).ToList();
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (var theType in ex.Types)
                {
                    try
                    {
                        if (theType != null && predicate(theType) && theType.Assembly == assembly)
                            types.Add(theType);
                    }
                    // This exception list is not exhaustive, modify to suit any reasons
                    // you find for failure to parse a single assembly
                    catch (BadImageFormatException)
                    {
                        // Type not in this assembly - reference to elsewhere ignored
                    }
                }
            }
            return types;
        }
    }
}