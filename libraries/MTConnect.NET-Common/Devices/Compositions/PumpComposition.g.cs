// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of an apparatus that raises, drives, exhausts, or compresses fluids or gases by means of a piston, plunger, or set of rotating vanes.
    /// </summary>
    public class PumpComposition : Composition 
    {
        public const string TypeId = "PUMP";
        public const string NameId = "pumpComposition";
        public new const string DescriptionText = "Composition composed of an apparatus that raises, drives, exhausts, or compresses fluids or gases by means of a piston, plunger, or set of rotating vanes.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public PumpComposition()  { Type = TypeId; }
    }
}