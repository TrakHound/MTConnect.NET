// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism for emitting a type of radiation.
    /// </summary>
    public class ExposureUnitComposition : Composition 
    {
        public const string TypeId = "EXPOSURE_UNIT";
        public const string NameId = "expunit";
        public new const string DescriptionText = "A mechanism for emitting a type of radiation.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version15;


        public ExposureUnitComposition()  { Type = TypeId; }
    }
}