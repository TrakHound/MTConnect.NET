// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that dispenses liquid or powered materials.
    /// </summary>
    public class ExtrusionUnitComposition : Composition 
    {
        public const string TypeId = "EXTRUSION_UNIT";
        public const string NameId = "extrusionUnitComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that dispenses liquid or powered materials.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15; 


        public ExtrusionUnitComposition()  { Type = TypeId; }
    }
}