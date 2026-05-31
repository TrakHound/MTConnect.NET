// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// Controls how deep a Component or DataItem search descends into the Device hierarchy.
    /// </summary>
    public enum SearchType
    {
        /// <summary>Search only the immediate children of the starting container.</summary>
        Child,

        /// <summary>Search the entire subtree of the starting container at any depth.</summary>
        AnyLevel
    }
}
