// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738868_160141_44696

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a pump or other mechanism that reduces volume and increases pressure of gases in order to condense the gases to drive pneumatically powered pieces of equipment.
    /// </summary>
    public class CompressorComposition : Composition 
    {
        public const string TypeId = "COMPRESSOR";
        public const string NameId = "compressorComposition";
        public new const string DescriptionText = "Composition composed of a pump or other mechanism that reduces volume and increases pressure of gases in order to condense the gases to drive pneumatically powered pieces of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public CompressorComposition()  { Type = TypeId; }
    }
}