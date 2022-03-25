// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A rotary storage unit for material.
    /// </summary>
    public class ReelComposition : Composition 
    {
        public const string TypeId = "REEL";
        public const string NameId = "reel";
        public new const string DescriptionText = "A rotary storage unit for material.";

        public override string TypeDescription => DescriptionText;


        public ReelComposition()  { Type = TypeId; }
    }
}
