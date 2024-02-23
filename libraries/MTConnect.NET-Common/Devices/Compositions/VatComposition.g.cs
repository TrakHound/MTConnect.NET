// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738884_85965_44750

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a container for liquid or powdered materials.
    /// </summary>
    public class VatComposition : Composition 
    {
        public const string TypeId = "VAT";
        public const string NameId = "vatComposition";
        public new const string DescriptionText = "Composition composed of a container for liquid or powdered materials.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15; 


        public VatComposition()  { Type = TypeId; }
    }
}