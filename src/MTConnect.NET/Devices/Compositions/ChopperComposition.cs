// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism used to break material into smaller pieces.
    /// </summary>
    public class ChopperComposition : Composition 
    {
        public const string TypeId = "CHOPPER";
        public const string NameId = "chop";
        public new const string DescriptionText = "A mechanism used to break material into smaller pieces.";

        public override string TypeDescription => DescriptionText;


        public ChopperComposition()  { Type = TypeId; }
    }
}
