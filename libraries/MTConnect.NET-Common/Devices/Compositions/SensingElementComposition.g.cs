// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738878_359798_44732

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that provides a signal or measured value.
    /// </summary>
    public class SensingElementComposition : Composition 
    {
        public const string TypeId = "SENSING_ELEMENT";
        public const string NameId = "sensingElementComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that provides a signal or measured value.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public SensingElementComposition()  { Type = TypeId; }
    }
}