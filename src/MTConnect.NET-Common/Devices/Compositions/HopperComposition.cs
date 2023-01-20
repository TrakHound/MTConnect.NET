// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A chamber or bin in which materials are stored temporarily, typically being filled through the top and dispensed through the bottom.
    /// </summary>
    public class HopperComposition : Composition 
    {
        public const string TypeId = "HOPPER";
        public const string NameId = "hop";
        public new const string DescriptionText = "A chamber or bin in which materials are stored temporarily, typically being filled through the top and dispensed through the bottom.";

        public override string TypeDescription => DescriptionText;


        public HopperComposition()  { Type = TypeId; }
    }
}
