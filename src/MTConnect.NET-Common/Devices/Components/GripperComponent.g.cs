// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1580312106469_530686_44423

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that holds a part, stock material, or any other item in place.
    /// </summary>
    public class GripperComponent : Component
    {
        public const string TypeId = "Gripper";
        public const string NameId = "gripperComponent";
        public new const string DescriptionText = "Leaf Component that holds a part, stock material, or any other item in place.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public GripperComponent() { Type = TypeId; }
    }
}