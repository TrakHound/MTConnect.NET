// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that allows material to flow for the purpose of drainage from, for example, a vessel or tank.
    /// </summary>
    public class DrainComposition : Composition 
    {
        public const string TypeId = "DRAIN";
        public const string NameId = "drainComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that allows material to flow for the purpose of drainage from, for example, a vessel or tank.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public DrainComposition()  { Type = TypeId; }
    }
}