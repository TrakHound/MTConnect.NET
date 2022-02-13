// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// An inclined channel for conveying material.
    /// </summary>
    public class ChuteComposition : Composition 
    {
        public const string TypeId = "CHUTE";
        public const string NameId = "chute";
        public new const string DescriptionText = "An inclined channel for conveying material.";

        public override string TypeDescription => DescriptionText;


        public ChuteComposition()  { Type = TypeId; }
    }
}
