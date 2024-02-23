// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738871_896639_44706

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