// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1580312106476_558459_44459

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that turns on or off an electric current or makes or breaks a circuit.
    /// </summary>
    public class SwitchComponent : Component
    {
        public const string TypeId = "Switch";
        public const string NameId = "switch";
        public new const string DescriptionText = "Leaf Component that turns on or off an electric current or makes or breaks a circuit.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public SwitchComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}