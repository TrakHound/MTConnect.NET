// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A container for liquid or powdered materials.
    /// </summary>
    public class VatComposition : Composition 
    {
        public const string TypeId = "VAT";
        public const string NameId = "vat";

        public VatComposition()  { Type = TypeId; }
    }
}
