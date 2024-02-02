// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a string like piece or filament of relatively rigid or flexible material provided in a variety of diameters.
    /// </summary>
    public class WireComposition : Composition 
    {
        public const string TypeId = "WIRE";
        public const string NameId = "wireComposition";
        public new const string DescriptionText = "Composition composed of a string like piece or filament of relatively rigid or flexible material provided in a variety of diameters.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public WireComposition()  { Type = TypeId; }
    }
}