// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579572382010_841174_42276

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Power was **DEPRECATED** in *MTConnect Version 1.1* and was replaced by Availability data item type.
    /// </summary>
    public class PowerComponent : Component
    {
        public const string TypeId = "Power";
        public const string NameId = "powerComponent";
        public new const string DescriptionText = "Power was **DEPRECATED** in *MTConnect Version 1.1* and was replaced by Availability data item type.";

        public override string TypeDescription => DescriptionText;
        public override System.Version MaximumVersion => MTConnectVersions.Version11;
        public override System.Version MinimumVersion => MTConnectVersions.Version10; 


        public PowerComponent() { Type = TypeId; }
    }
}