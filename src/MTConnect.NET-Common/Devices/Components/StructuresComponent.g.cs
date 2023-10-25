// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1607371600474_90853_450

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component that organize Structure types.
    /// </summary>
    public class StructuresComponent : Component, IOrganizerComponent
    {
        public const string TypeId = "Structures";
        public const string NameId = "structuresComponent";
        public new const string DescriptionText = "Component that organize Structure types.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public StructuresComponent() { Type = TypeId; }
    }
}