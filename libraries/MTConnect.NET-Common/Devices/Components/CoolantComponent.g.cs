// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579572381979_456626_42219

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System that provides distribution and management of fluids that remove heat from a piece of equipment.
    /// </summary>
    public class CoolantComponent : Component
    {
        public const string TypeId = "Coolant";
        public const string NameId = "coolantComponent";
        public new const string DescriptionText = "System that provides distribution and management of fluids that remove heat from a piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version12; 


        public CoolantComponent() { Type = TypeId; }
    }
}