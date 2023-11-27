// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579572381984_481596_42228

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component composed of a mechanical mechanism or closure that can cover a physical access portal into a piece of equipment allowing or restricting access to other parts of the equipment.
    /// </summary>
    public class DoorComponent : Component
    {
        public const string TypeId = "Door";
        public const string NameId = "doorComponent";
        public new const string DescriptionText = "Component composed of a mechanical mechanism or closure that can cover a physical access portal into a piece of equipment allowing or restricting access to other parts of the equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11; 


        public DoorComponent() { Type = TypeId; }
    }
}