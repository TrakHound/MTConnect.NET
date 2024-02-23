// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1579572381986_185851_42231

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System composed of the main power supply for the piece of equipment that provides distribution of that power throughout the equipment.
    /// </summary>
    public class ElectricComponent : Component
    {
        public const string TypeId = "Electric";
        public const string NameId = "electric";
        public new const string DescriptionText = "System composed of the main power supply for the piece of equipment that provides distribution of that power throughout the equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11; 


        public ElectricComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}