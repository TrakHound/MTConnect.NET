// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738861_157762_44680

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of an endless flexible band that transmits motion for a piece of equipment or conveys materials and objects.
    /// </summary>
    public class BeltComposition : Composition 
    {
        public const string TypeId = "BELT";
        public const string NameId = "beltComposition";
        public new const string DescriptionText = "Composition composed of an endless flexible band that transmits motion for a piece of equipment or conveys materials and objects.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public BeltComposition()  { Type = TypeId; }
    }
}