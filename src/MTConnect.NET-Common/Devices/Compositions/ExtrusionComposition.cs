// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism for dispensing liquid or powered materials.
    /// </summary>
    public class ExtrusionUnitComposition : Composition 
    {
        public const string TypeId = "EXTRUSION_UNIT";
        public const string NameId = "exunit";
        public new const string DescriptionText = "A mechanism for dispensing liquid or powered materials.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version15;


        public ExtrusionUnitComposition()  { Type = TypeId; }
    }
}
