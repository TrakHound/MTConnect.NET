// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1580312106465_576708_44396

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component composed of a pump or other mechanism that reduces volume and increases pressure of gases in order to condense the gases to drive pneumatically powered pieces of equipment.
    /// </summary>
    public class CompressorComponent : Component
    {
        public const string TypeId = "Compressor";
        public const string NameId = "compressorComponent";
        public new const string DescriptionText = "Leaf Component composed of a pump or other mechanism that reduces volume and increases pressure of gases in order to condense the gases to drive pneumatically powered pieces of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public CompressorComponent() { Type = TypeId; }
    }
}