// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1622457074108_581195_524

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component that organize Part types.
    /// </summary>
    public class PartsComponent : Component, IOrganizerComponent
    {
        public const string TypeId = "Parts";
        public const string NameId = "partsComponent";
        public new const string DescriptionText = "Component that organize Part types.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version18; 


        public PartsComponent() { Type = TypeId; }
    }
}