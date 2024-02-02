// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that strengthens, supports, or fastens objects in place.
    /// </summary>
    public class ClampComposition : Composition 
    {
        public const string TypeId = "CLAMP";
        public const string NameId = "clampComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that strengthens, supports, or fastens objects in place.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public ClampComposition()  { Type = TypeId; }
    }
}