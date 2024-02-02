// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of an interconnected series of objects that band together and transmit motion for a piece of equipment or to convey materials and objects.
    /// </summary>
    public class ChainComposition : Composition 
    {
        public const string TypeId = "CHAIN";
        public const string NameId = "chainComposition";
        public new const string DescriptionText = "Composition composed of an interconnected series of objects that band together and transmit motion for a piece of equipment or to convey materials and objects.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public ChainComposition()  { Type = TypeId; }
    }
}