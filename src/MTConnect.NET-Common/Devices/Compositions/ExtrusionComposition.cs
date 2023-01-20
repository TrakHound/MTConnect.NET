// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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