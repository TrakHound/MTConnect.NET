// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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