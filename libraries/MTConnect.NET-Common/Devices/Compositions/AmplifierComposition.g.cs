// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738860_858943_44676

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of an electronic component or circuit that amplifies power, electric current, or voltage.
    /// </summary>
    public class AmplifierComposition : Composition 
    {
        public const string TypeId = "AMPLIFIER";
        public const string NameId = "amplifierComposition";
        public new const string DescriptionText = "Composition composed of an electronic component or circuit that amplifies power, electric current, or voltage.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public AmplifierComposition()  { Type = TypeId; }
    }
}