// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a container for liquid or powdered materials.
    /// </summary>
    public class VatCompositionComposition : Composition 
    {
        public const string TypeId = "VAT";
        public const string NameId = "vatComposition";
        public new const string DescriptionText = "Composition composed of a container for liquid or powdered materials.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15; 


        public VatCompositionComposition()  { Type = TypeId; }
    }
}