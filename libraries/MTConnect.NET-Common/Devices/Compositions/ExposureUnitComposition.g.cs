// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that emits a type of radiation.
    /// </summary>
    public class ExposureUnitComposition : Composition 
    {
        public const string TypeId = "EXPOSURE_UNIT";
        public const string NameId = "exposureUnitComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that emits a type of radiation.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15; 


        public ExposureUnitComposition()  { Type = TypeId; }
    }
}