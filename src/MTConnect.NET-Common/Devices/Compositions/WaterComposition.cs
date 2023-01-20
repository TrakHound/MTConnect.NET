// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A fluid.
    /// </summary>
    public class WaterComposition : Composition 
    {
        public const string TypeId = "WATER";
        public const string NameId = "water";
        public new const string DescriptionText = "A fluid.";

        public override string TypeDescription => DescriptionText;


        public WaterComposition()  { Type = TypeId; }
    }
}