// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312106461_720908_44375

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that slows or stops a moving object by the absorption or transfer of the energy of momentum, usually by means of friction, electrical force, or magnetic force.
    /// </summary>
    public class BrakeComponent : Component
    {
        public const string TypeId = "Brake";
        public const string NameId = "brake";
        public new const string DescriptionText = "Leaf Component that slows or stops a moving object by the absorption or transfer of the energy of momentum, usually by means of friction, electrical force, or magnetic force.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public BrakeComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}