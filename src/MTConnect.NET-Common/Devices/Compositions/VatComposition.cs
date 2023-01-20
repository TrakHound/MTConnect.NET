// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A container for liquid or powdered materials.
    /// </summary>
    public class VatComposition : Composition 
    {
        public const string TypeId = "VAT";
        public const string NameId = "vat";
        public new const string DescriptionText = "A container for liquid or powdered materials.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version15;


        public VatComposition()  { Type = TypeId; }
    }
}