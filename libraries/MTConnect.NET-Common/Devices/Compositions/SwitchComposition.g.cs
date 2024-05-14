// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738880_730730_44738

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that turns on or off an electric current or makes or breaks a circuit.
    /// </summary>
    public class SwitchComposition : Composition 
    {
        public const string TypeId = "SWITCH";
        public const string NameId = "switchComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that turns on or off an electric current or makes or breaks a circuit.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public SwitchComposition()  { Type = TypeId; }
    }
}