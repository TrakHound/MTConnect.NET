// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Cached set of the string values of an Enum, used by the generated DataItem validation
    /// so each Observation is checked with a single hash lookup instead of enumerating the Enum.
    /// </summary>
    internal static class EnumValues<TEnum> where TEnum : struct
    {
        private static readonly HashSet<string> _values = CreateValues();


        /// <summary>
        /// Returns true when <paramref name="value"/> matches (case-sensitive) one of the Enum values.
        /// </summary>
        public static bool Contains(string value) => value != null && _values.Contains(value);


        private static HashSet<string> CreateValues()
        {
            var values = new HashSet<string>(StringComparer.Ordinal);
            foreach (var value in Enum.GetValues(typeof(TEnum))) values.Add(value.ToString());
            return values;
        }
    }
}
