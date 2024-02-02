// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1648551529939_657918_1127

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component that organize Controller entities.
    /// </summary>
    public class ControllersComponent : Component, IOrganizerComponent
    {
        public const string TypeId = "Controllers";
        public const string NameId = "controllers";
        public new const string DescriptionText = "Component that organize Controller entities.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version10; 


        public ControllersComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}