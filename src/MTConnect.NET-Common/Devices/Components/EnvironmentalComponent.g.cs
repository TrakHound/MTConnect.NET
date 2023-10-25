// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579572381990_149427_42240

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component that observes the surroundings of another Component.> Note: Environmental **SHOULD** be organized by Auxillaries, Systems or Parts depending on the relationship to the Component.
    /// </summary>
    public class EnvironmentalComponent : Component
    {
        public const string TypeId = "Environmental";
        public const string NameId = "environmentalComponent";
        public new const string DescriptionText = "Component that observes the surroundings of another Component.> Note: Environmental **SHOULD** be organized by Auxillaries, Systems or Parts depending on the relationship to the Component.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public EnvironmentalComponent() { Type = TypeId; }
    }
}