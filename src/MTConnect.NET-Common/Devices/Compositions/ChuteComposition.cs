// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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