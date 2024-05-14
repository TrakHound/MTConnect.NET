// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738865_994591_44690

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of an inclined channel that conveys material.
    /// </summary>
    public class ChuteComposition : Composition 
    {
        public const string TypeId = "CHUTE";
        public const string NameId = "chuteComposition";
        public new const string DescriptionText = "Composition composed of an inclined channel that conveys material.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public ChuteComposition()  { Type = TypeId; }
    }
}