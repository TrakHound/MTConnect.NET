// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1580312106460_808314_44366

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component composed of an electronic component or circuit that amplifies power, electric current, or voltage.
    /// </summary>
    public class AmplifierComponent : Component
    {
        public const string TypeId = "Amplifier";
        public const string NameId = "amplifierComponent";
        public new const string DescriptionText = "Leaf Component composed of an electronic component or circuit that amplifies power, electric current, or voltage.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public AmplifierComponent() { Type = TypeId; }
    }
}