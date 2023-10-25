// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579572381988_757487_42237

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System composed of functions that form the last link segment of a piece of equipment.
    /// </summary>
    public class EndEffectorComponent : Component
    {
        public const string TypeId = "EndEffector";
        public const string NameId = "endEffectorComponent";
        public new const string DescriptionText = "System composed of functions that form the last link segment of a piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15; 


        public EndEffectorComponent() { Type = TypeId; }
    }
}