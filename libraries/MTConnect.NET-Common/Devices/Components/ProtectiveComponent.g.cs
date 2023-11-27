// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579572382013_685011_42282

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System that provides functions used to detect or prevent harm or damage to equipment or personnel.
    /// </summary>
    public class ProtectiveComponent : Component
    {
        public const string TypeId = "Protective";
        public const string NameId = "protectiveComponent";
        public new const string DescriptionText = "System that provides functions used to detect or prevent harm or damage to equipment or personnel.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public ProtectiveComponent() { Type = TypeId; }
    }
}