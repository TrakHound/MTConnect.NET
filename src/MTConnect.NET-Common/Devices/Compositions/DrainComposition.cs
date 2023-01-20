// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism that allows material to flow for the purpose of drainage from, for example, a vessel or tank.
    /// </summary>
    public class DrainComposition : Composition 
    {
        public const string TypeId = "DRAIN";
        public const string NameId = "drain";
        public new const string DescriptionText = "A mechanism that allows material to flow for the purpose of drainage from, for example, a vessel or tank.";

        public override string TypeDescription => DescriptionText;


        public DrainComposition()  { Type = TypeId; }
    }
}
