// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1607344360113_831146_1196

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component that organize Resource types.
    /// </summary>
    public class ResourcesComponent : Component, IOrganizerComponent
    {
        public const string TypeId = "Resources";
        public const string NameId = "resources";
        public new const string DescriptionText = "Component that organize Resource types.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public ResourcesComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}