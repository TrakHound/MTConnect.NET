// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A viscous liquid.
    /// </summary>
    public class OilComposition : Composition 
    {
        public const string TypeId = "OIL";
        public const string NameId = "oil";

        public OilComposition()  { Type = TypeId; }
    }
}
