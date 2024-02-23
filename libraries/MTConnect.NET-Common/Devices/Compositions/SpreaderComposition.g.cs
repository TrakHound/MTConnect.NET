// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738879_13707_44734

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that flattens or spreads materials.
    /// </summary>
    public class SpreaderComposition : Composition 
    {
        public const string TypeId = "SPREADER";
        public const string NameId = "spreaderComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that flattens or spreads materials.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15; 


        public SpreaderComposition()  { Type = TypeId; }
    }
}