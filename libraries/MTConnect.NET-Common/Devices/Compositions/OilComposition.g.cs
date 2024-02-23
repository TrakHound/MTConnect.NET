// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738875_818607_44722

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a viscous liquid.
    /// </summary>
    public class OilComposition : Composition 
    {
        public const string TypeId = "OIL";
        public const string NameId = "oilComposition";
        public new const string DescriptionText = "Composition composed of a viscous liquid.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public OilComposition()  { Type = TypeId; }
    }
}