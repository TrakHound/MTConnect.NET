// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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