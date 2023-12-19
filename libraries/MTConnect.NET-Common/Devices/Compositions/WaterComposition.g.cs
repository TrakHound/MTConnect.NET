// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a fluid.
    /// </summary>
    public class WaterComposition : Composition 
    {
        public const string TypeId = "WATER";
        public const string NameId = "waterComposition";
        public new const string DescriptionText = "Composition composed of a fluid.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public WaterComposition()  { Type = TypeId; }
    }
}