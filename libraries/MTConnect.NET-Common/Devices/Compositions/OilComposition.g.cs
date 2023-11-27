// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a viscous liquid.
    /// </summary>
    public class OilCompositionComposition : Composition 
    {
        public const string TypeId = "OIL";
        public const string NameId = "oilComposition";
        public new const string DescriptionText = "Composition composed of a viscous liquid.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public OilCompositionComposition()  { Type = TypeId; }
    }
}