// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738885_495151_44752

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a fluid.
    /// </summary>
    public class WaterComposition : Composition 
    {
        public const string TypeId = "WATER";
        public const string NameId = "waterComposition";
        public new const string DescriptionText = "Composition composed of a fluid.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public WaterComposition()  { Type = TypeId; }
    }
}