// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738883_168484_44746

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that transforms electric energy from a source to a secondary circuit.
    /// </summary>
    public class TransformerComposition : Composition 
    {
        public const string TypeId = "TRANSFORMER";
        public const string NameId = "transformerComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that transforms electric energy from a source to a secondary circuit.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15; 


        public TransformerComposition()  { Type = TypeId; }
    }
}