// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism that provides a signal or measured value.
    /// </summary>
    public class SensingElementComposition : Composition 
    {
        public const string TypeId = "SENSING_ELEMENT";
        public const string NameId = "senelement";

        public SensingElementComposition()  { Type = TypeId; }
    }
}
