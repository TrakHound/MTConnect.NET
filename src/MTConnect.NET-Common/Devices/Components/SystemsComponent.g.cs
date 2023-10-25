// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1607344852741_562899_1488

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component that organize System types.
    /// </summary>
    public class SystemsComponent : Component, IOrganizerComponent
    {
        public const string TypeId = "Systems";
        public const string NameId = "systemsComponent";
        public new const string DescriptionText = "Component that organize System types.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11; 


        public SystemsComponent() { Type = TypeId; }
    }
}