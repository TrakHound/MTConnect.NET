// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1607345507474_877148_1773

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component that organize Interface types.
    /// </summary>
    public class InterfacesComponent : Component, IOrganizerComponent
    {
        public const string TypeId = "Interfaces";
        public const string NameId = "interfaces";
        public new const string DescriptionText = "Component that organize Interface types.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13; 


        public InterfacesComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}