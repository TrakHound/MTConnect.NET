// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Door is a Component that represents the information for a mechanical mechanism or closure that can cover, for example, a physical access portal into a piece of equipment.
    /// The closure can be opened or closed to allow or restrict access to other parts of the equipment.
    /// </summary>
    public class DoorComponent : Component 
    {
        public const string TypeId = "Door";
        public const string NameId = "door";
        public new const string DescriptionText = "Door is a Component that represents the information for a mechanical mechanism or closure that can cover, for example, a physical access portal into a piece of equipment. The closure can be opened or closed to allow or restrict access to other parts of the equipment.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version11;


        public DoorComponent()  { Type = TypeId; }
    }
}