// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1580312106462_657166_44381

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that breaks material into smaller pieces.
    /// </summary>
    public class ChopperComponent : Component
    {
        public const string TypeId = "Chopper";
        public const string NameId = "chopperComponent";
        public new const string DescriptionText = "Leaf Component that breaks material into smaller pieces.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public ChopperComponent() { Type = TypeId; }
    }
}