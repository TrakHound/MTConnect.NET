// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1607345849016_787585_1914

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component that organize Adapter types.
    /// </summary>
    public class AdaptersComponent : Component, IOrganizerComponent
    {
        public const string TypeId = "Adapters";
        public const string NameId = "adapters";
        public new const string DescriptionText = "Component that organize Adapter types.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public AdaptersComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}