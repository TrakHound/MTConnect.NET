// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A pump or other mechanism for reducing volume and increasing pressure of gases
    /// in order to condense the gases to drive pneumatically powered pieces of equipment.
    /// </summary>
    public class CompressorComposition : Composition 
    {
        public const string TypeId = "COMPRESSOR";
        public const string NameId = "comp";
        public new const string DescriptionText = "A pump or other mechanism for reducing volume and increasing pressure of gases in order to condense the gases to drive pneumatically powered pieces of equipment.";

        public override string TypeDescription => DescriptionText;


        public CompressorComposition()  { Type = TypeId; }
    }
}