// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// An apparatus raising, driving, exhausting, or compressing fluids or gases by means of a piston, plunger, or set of rotating vanes.
    /// </summary>
    public class PumpComposition : Composition 
    {
        public const string TypeId = "PUMP";
        public const string NameId = "pump";
        public new const string DescriptionText = "An apparatus raising, driving, exhausting, or compressing fluids or gases by means of a piston, plunger, or set of rotating vanes.";

        public override string TypeDescription => DescriptionText;


        public PumpComposition()  { Type = TypeId; }
    }
}