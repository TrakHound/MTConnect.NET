// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism or wheel that turns in a frame or block and serves to change the direction of or to transmit force.
    /// </summary>
    public class PulleyComposition : Composition 
    {
        public const string TypeId = "PULLEY";
        public const string NameId = "pulleyComposition";
        public new const string DescriptionText = "Composition composed of a mechanism or wheel that turns in a frame or block and serves to change the direction of or to transmit force.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public PulleyComposition()  { Type = TypeId; }
    }
}