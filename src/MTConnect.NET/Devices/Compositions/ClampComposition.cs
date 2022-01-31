// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism used to strengthen, support, or fasten objects in place.
    /// </summary>
    public class ClampComposition : Composition 
    {
        public const string TypeId = "CLAMP";
        public const string NameId = "clamp";

        public ClampComposition()  { Type = TypeId; }
    }
}
