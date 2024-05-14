// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738864_974727_44688

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that holds a part, stock material, or any other item in place.
    /// </summary>
    public class ChuckComposition : Composition 
    {
        public const string TypeId = "CHUCK";
        public const string NameId = "chuckComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that holds a part, stock material, or any other item in place.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public ChuckComposition()  { Type = TypeId; }
    }
}