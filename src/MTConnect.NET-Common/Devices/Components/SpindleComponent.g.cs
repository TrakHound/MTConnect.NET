// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_4_45f01b9_1643678227814_87818_1410

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component that provides an axis of rotation for the purpose of rapidly rotating a part or a tool to provide sufficient surface speed for cutting operations.Spindle was **DEPRECATED** in *MTConnect Version 1.1* and was replaced by RotaryMode.
    /// </summary>
    public class SpindleComponent : Component
    {
        public const string TypeId = "Spindle";
        public const string NameId = "spindleComponent";
        public new const string DescriptionText = "Component that provides an axis of rotation for the purpose of rapidly rotating a part or a tool to provide sufficient surface speed for cutting operations.Spindle was **DEPRECATED** in *MTConnect Version 1.1* and was replaced by RotaryMode.";

        public override string TypeDescription => DescriptionText;
        public override System.Version MaximumVersion => MTConnectVersions.Version11;
        public override System.Version MinimumVersion => MTConnectVersions.Version10; 


        public SpindleComponent() { Type = TypeId; }
    }
}