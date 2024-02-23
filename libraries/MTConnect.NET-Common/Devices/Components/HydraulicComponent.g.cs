// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1579572381993_131470_42246

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System that provides movement and distribution of pressurized liquid throughout the piece of equipment.
    /// </summary>
    public class HydraulicComponent : Component
    {
        public const string TypeId = "Hydraulic";
        public const string NameId = "hydraulic";
        public new const string DescriptionText = "System that provides movement and distribution of pressurized liquid throughout the piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11; 


        public HydraulicComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}