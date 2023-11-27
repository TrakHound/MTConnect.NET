// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that turns on or off an electric current or makes or breaks a circuit.
    /// </summary>
    public class SwitchCompositionComposition : Composition 
    {
        public const string TypeId = "SWITCH";
        public const string NameId = "switchComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that turns on or off an electric current or makes or breaks a circuit.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public SwitchCompositionComposition()  { Type = TypeId; }
    }
}