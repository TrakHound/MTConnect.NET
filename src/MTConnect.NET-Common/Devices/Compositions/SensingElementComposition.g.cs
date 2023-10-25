// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that provides a signal or measured value.
    /// </summary>
    public class SensingElementCompositionComposition : Composition 
    {
        public const string TypeId = "SENSING_ELEMENT";
        public const string NameId = "sensingElementComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that provides a signal or measured value.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public SensingElementCompositionComposition()  { Type = TypeId; }
    }
}